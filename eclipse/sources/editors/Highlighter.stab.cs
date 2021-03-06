/*
   Licensed to the Apache Software Foundation (ASF) under one or more
   contributor license agreements.  See the NOTICE file distributed with
   this work for additional information regarding copyright ownership.
   The ASF licenses this file to You under the Apache License, Version 2.0
   (the "License"); you may not use this file except in compliance with
   the License.  You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */
#undef TRACE
 
using java.lang;
using java.util;
using org.eclipse.core.resources;
using org.eclipse.jface.text;
using org.eclipse.jface.text.source;
using org.eclipse.swt;
using org.eclipse.swt.custom;
using org.eclipse.swt.graphics;
using stab.query;
using stab.reflection;
using cnatural.helpers;
using cnatural.parser;
using cnatural.syntaxtree;

namespace cnatural.eclipse.editors {

	class StyledPosition : Position {
		private ISharedTextColors sharedTextColors;
	
		StyledPosition(int offset, int length, ISharedTextColors sharedTextColors)
				: super(offset, length) {
			this.sharedTextColors = sharedTextColors;
		}

		public SemanticStyle SemanticStyle;

		public Color Foreground {
			get {
				return sharedTextColors.getColor(this.SemanticStyle.Foreground);
			}
		}
		
		public Color Background {
			get {
				return sharedTextColors.getColor(this.SemanticStyle.Background);
			}
		}
		
		public int FontStyle {
			get {
				return this.SemanticStyle.FontStyle;
			}
		}
		
		public bool Underline {
			get {
				return this.SemanticStyle.Underline;
			}
		}
		
		public bool Strikethrough {
			get {
				return this.SemanticStyle.Strikethrough;
			}
		}
	}

	class Token : StyledPosition {
		Token(int offset, int length, ISharedTextColors sharedTextColors, LexicalUnit lexicalUnit, Keyword keyword)
				: super(offset, length, sharedTextColors) {
			this.LexicalUnit = lexicalUnit;
			this.Keyword = keyword;
		}
		
		public LexicalUnit LexicalUnit^;
		
		public Keyword Keyword^;
		
		public bool IsUpdated;
	}

	public class Highlighter {
		private final static Comparator<Token> tokenPositionComparator = new TokenPositionComparator();
		private class TokenPositionComparator : Comparator<Token> {
			public int compare(Token t1, Token t2) {
				return t1.offset - t2.offset;
			}
		}
	
		private IDocument document;
		private IFile file;
		private ISourceViewer sourceViewer;
		private TextListener textListener;
		private ISharedTextColors sharedTextColors;
		private CompilationUnitHighlighter compilationUnitHighlighter;

		private List<StyledPosition> preprocessorPositions;
		private List<Token> tokens;
		private List<Token> removedTokens;
	
		public Highlighter(ISourceViewer sourceViewer, IDocument document, IFile file, ISharedTextColors sharedTextColors) {
			this.sourceViewer = sourceViewer;
			this.document = document;
			this.sharedTextColors = sharedTextColors;
			this.compilationUnitHighlighter = new CompilationUnitHighlighter();
			this.textListener = new TextListener(this);
			
			initialize(document.get().toCharArray(), true);
		}

		//
		// Gets the index of the position in the list which contains the offset, or -1.
		// positions: a list of non-overlapping Position objects, sorted by offset
		// offset: an arbitrary integer
		//
		public static int getPositionIndex(List<? : StyledPosition> positions, int offset) {
			int start = 0;
			int end = positions.size() - 1;
			while (start <= end) {
				int index = start + (end - start + 1) / 2;
				var position = positions[index];
				int poffset = position.offset;
				if (poffset <= offset && poffset + position.length > offset) {
					return index;
				}
				if (poffset < offset) {
					start = index + 1;
				} else {
					end = index - 1;
				}
			}
			return -1;
		}

		//
		// Gets the index of the position in the list which contains the offset, or is just after the offset, or -1.
		// positions: a list of non-overlapping Position objects, sorted by offset
		// offset: an arbitrary integer
		//
		public static int getNextPositionIndex(List<? : StyledPosition> positions, int offset) {
			int start = 0;
			int end = positions.size() - 1;
			int prev = -1;
			while (start <= end) {
				int index = start + (end - start + 1) / 2;
				var position = positions[index];
				int poffset = position.offset;
				if (poffset <= offset && poffset + position.length > offset) {
					return index;
				}
				if (poffset < offset) {
					start = index + 1;
				} else {
					prev = index;
					end = index - 1;
				}
			}
			return prev;
		}
		
		public char[] Text^;
		
		public List<Token> Tokens {
			get {
				return tokens;
			}
		}
		
		public CompilationUnitNode CompilationUnit^;
		
		public Library TypeSystem^;
		
		public Library AnnotatedTypeSystem^;
		
		public void update(char[] text, CompilationUnitNode compilationUnit, Library typeSystem, Library annotatedTypeSystem) {
			if (text != this.Text) {
				return;
			}
			try {
				compilationUnitHighlighter.highlight(compilationUnit, tokens);
				sourceViewer.invalidateTextPresentation();
				foreach (var token in tokens) {
					token.IsUpdated = false;
				}
				removedTokens.clear();
				this.CompilationUnit = compilationUnit;
				this.TypeSystem = typeSystem;
				this.AnnotatedTypeSystem = annotatedTypeSystem;
			} catch (Exception e) {
				Environment.logException(e);
				initialize(text, false);
			}
		}
		
		void dispose() {
			if (textListener != null) {
				textListener.dispose();
				textListener = null;
			}
		}

		private void initialize(char[] text, bool invalidate) {
			Environment.trace(this, "initialization");
			var t0 = System.nanoTime();

			var newTokens = new ArrayList<Token>();
			var newRemovedTokens = new ArrayList<Token>();
			var newPreprocessorPositions = new ArrayList<StyledPosition>();
			try {
				var errorManager = new CodeErrorManager();
				var preprocessor = new Preprocessor(errorManager, text);
				var preprocessedText = preprocessor.preprocess();
				if (!errorManager.HasErrors) {
					var scanner = new PreprocessedTextScanner(errorManager, preprocessedText);
					LexicalUnit lu;
					while ((lu = scanner.nextLexicalUnit()) != LexicalUnit.EndOfStream) {
						int sp = scanner.StartPosition;
						newTokens.add(new Token(sp, scanner.EndPosition - sp, sharedTextColors, lu, scanner.Keyword));
					}
					if (!errorManager.HasErrors) {
						computePreprocessorStyles(preprocessedText.InputSectionParts, newPreprocessorPositions);
						foreach (var tk in newTokens) {
							tk.SemanticStyle = getSemanticStyle(tk.LexicalUnit);
						}
						this.Text = text;
						this.tokens = newTokens;
						this.removedTokens = newRemovedTokens;
						this.preprocessorPositions = newPreprocessorPositions;
						if (invalidate) {
							sourceViewer.invalidateTextPresentation();
						}
					} else {
						newTokens.clear();
					}
				}
			} catch (CodeErrorException e) {
				newTokens.clear();
			}
			this.Text = text;
			this.tokens = newTokens;
			this.removedTokens = newRemovedTokens;
			this.preprocessorPositions = newPreprocessorPositions;
			this.CompilationUnit = null;
			this.TypeSystem = null;
			this.AnnotatedTypeSystem = null;
			//checkPositionsConsistency();
			
			Environment.trace(this, "initialization done in " + ((System.nanoTime() - t0) / 1e6) + "ms");
		}
		
		/*
		private void checkPositionsConsistency() {
			int offset = 0;
			int tindex = 0;
			int ppindex = 0;
			while (tindex < tokens.size() || ppindex < preprocessorPositions.size()) {
				if (tindex < tokens.size() && tokens[tindex].offset == offset) {
					offset += tokens[tindex++].length;
				} else if (ppindex < preprocessorPositions.size() && preprocessorPositions[ppindex].offset == offset) {
					offset += preprocessorPositions[ppindex++].length;
				} else {
					throw new IllegalStateException("offset: " + offset);
				}
			}
		}
		*/

		private void addPreprocessorSymbols(Set<String> symbols) {
			var it = Query.empty<String>();
			if (file != null) {
				var projectManager = Environment.getProjectManager(file);
				if (projectManager != null) {
					it = projectManager.Properties.PreprocessorSymbols;
				}
			}
			foreach (var s in it) {
				symbols.add(s);
			}
		}
		
		private void computePreprocessorStyles(Iterable<InputSectionPart> inputSectionParts, List<StyledPosition> preprocessorPositions) {
			foreach (var part in inputSectionParts) {
				if (part.InputSectionPartKind == InputSectionPartKind.SourceCode) {
					continue;
				}
				int line = document.getLineOfOffset(part.Position);
				var style = new StyledPosition(document.getLineOffset(line), document.getLineLength(line), sharedTextColors);
				style.SemanticStyle = SemanticStyle.Preprocessor;
				preprocessorPositions.add(style);
				switch (part.InputSectionPartKind) {
				case If:
				case Elif:
				case Else:
					if (part.SkippedSection) {
						InputSectionPart skippedPart = Query.firstOrDefault(part.InputSectionParts);
						if (skippedPart != null) {
							style = new StyledPosition(skippedPart.Position, skippedPart.Length, sharedTextColors);
							style.SemanticStyle = SemanticStyle.SkippedSource;
							preprocessorPositions.add(style);
						}
					} else {
						computePreprocessorStyles(part.InputSectionParts, preprocessorPositions);
					}
					break;
				case Region:
					computePreprocessorStyles(part.InputSectionParts, preprocessorPositions);
					line = document.getLineOfOffset(part.Position + part.Length - 1);
					style = new StyledPosition(document.getLineOffset(line), document.getLineLength(line), sharedTextColors);
					style.SemanticStyle = SemanticStyle.Preprocessor;
					preprocessorPositions.add(style);
					break;
				}
			}
		}

		private SemanticStyle getSemanticStyle(LexicalUnit lexicalUnit) {
			switch (lexicalUnit) {
			case SingleLineComment:
				return SemanticStyle.SingleLineComment;
			case DelimitedComment:
				return SemanticStyle.DelimitedComment;
			case Keyword:
				return SemanticStyle.Keyword;
			case StringLiteral:
			case VerbatimStringLiteral:
				return SemanticStyle.StringLiteral;
			case CharacterLiteral:
				return SemanticStyle.CharacterLiteral;
			case DecimalIntegerLiteral:
			case DoubleLiteral:
			case FloatLiteral:
			case HexadecimalIntegerLiteral:
			case HexadecimalLongLiteral:
			case RealLiteral:
				return SemanticStyle.NumberLiteral;
			}
			return null;
		}
		
		private void updateTextPresentation(TextPresentation textPresentation) {
			var extent = textPresentation.getExtent();
			Environment.trace(this, "applyTextPresentation " + extent);
		
			int startOffset = extent.getOffset();
			int endOffset = startOffset + extent.getLength();
			
			var styleRanges = createStyleRanges(this.tokens, startOffset, endOffset);
			if (styleRanges != null) {
				textPresentation.mergeStyleRanges(styleRanges);
			}
			styleRanges = createStyleRanges(this.preprocessorPositions, startOffset, endOffset);
			if (styleRanges != null) {
				textPresentation.mergeStyleRanges(styleRanges);
			}
		}
		
		private static StyleRange[] createStyleRanges(List<? : StyledPosition> positions, int startOffset, int endOffset) {
			if (positions == null || positions.size() == 0) {
				return null;
			}
		
			int index = getNextPositionIndex(positions, startOffset);
			if (index == -1) {
				return null;
			}
			var styleRanges = new ArrayList<StyleRange>();
			int size = positions.size();
			while (index < size) {
				var position = positions[index++];
				int offset = position.offset;
				int length = position.length;
				if (offset >= endOffset) {
					break;
				}
				if (offset < startOffset) {
					int delta = startOffset - offset;
					offset -= delta;
					length -= delta;
				}
				if (offset + length > endOffset) {
					length = endOffset - offset;
				}
				var styleRange = createStyleRange(position, offset, length);
				if (styleRange != null) {
					styleRanges.add(styleRange);
				}
			}
			size = styleRanges.size();
			if (size == 0) {
				return null;
			} else {
				return styleRanges.toArray(new StyleRange[size]);
			}
		}
		
		private static StyleRange createStyleRange(StyledPosition position, int offset, int length) {
			if (position.SemanticStyle == null) {
				return null;
			}
			var result = new StyleRange(offset, length, position.Foreground, position.Background, position.FontStyle);
			result.strikeout = position.Strikethrough;
			result.underline = position.Underline;
			return result;
		}

		private static void updateOffsetsAfter(int offset, int delta, List<? : StyledPosition> positions) {
			int index = getNextPositionIndex(positions, offset);
			if (index != -1) {
				while (index < positions.size()) {
					var position = positions[index++];
					position.offset += delta;
				}
			}
		}

		private void update(int offset, String insertedText, int removedLength) {
			Environment.trace(this, "update(" + offset + ", \"" + insertedText + "\", " + removedLength + ")");
			var t0 = System.nanoTime();

			var text = document.get().toCharArray();
			int insertedLength = insertedText.length();
			int ntokens = this.tokens.size();

			if (ntokens == 0 || insertedText.indexOf('#') != -1) {
				Environment.trace(this, "No token or #");
				initialize(text, false);
				return;
			}

			var pppositions = this.preprocessorPositions;

			// Check if a preprocessor directive was altered
			if (removedLength == 0) {
				int index = getPositionIndex(pppositions, offset);
				if (index != -1) {
					var ppp = pppositions[index];
					if (ppp.SemanticStyle == SemanticStyle.Preprocessor) {
						Environment.trace(this, "Inside a preprocessor directive");
						initialize(text, false);
					} else {
						Environment.trace(this, "Skipped source expanded");
						
						// Inside a skipped source
						ppp.length += insertedLength;
						updateOffsetsAfter(ppp.offset + ppp.length, insertedLength, pppositions);
						updateOffsetsAfter(ppp.offset + ppp.length, insertedLength, tokens);
						this.Text = text;
						((ITextViewerExtension2)sourceViewer).invalidateTextPresentation(ppp.offset, ppp.length);
					}
					return;
				}
			} else {
				int index = getNextPositionIndex(pppositions, offset);
				if (index != -1) {
					var ppp = pppositions[index];
					if (ppp.overlapsWith(offset, removedLength)) {
						Environment.trace(this, "Overlaps a preprocessor directive");
						initialize(text, false);
						return;
					}
				}
			}

			// Get the start token
			int startIndex = getPositionIndex(this.tokens, offset);
			if (startIndex == -1) {
				// End of document just after a preprocessor directive
				Environment.trace(this, "End of file");
				var ppIndex = getPositionIndex(pppositions, offset - 1);
				if (ppIndex == -1) {
					startIndex = ntokens - 1;
				} else {
					// Just after a preprocessor directive
					var insertedTokens = scan(insertedText.toCharArray());
					if (insertedTokens == null) {
						// Something went wrong: document scan
						initialize(text, false);
						return;
					}
					var ppp = pppositions[ppIndex];
					int start = ppp.offset + ppp.length;
					var newTokens = new ArrayList<Token>(tokens);
					foreach (var t in insertedTokens) {
						t.offset += start;
						newTokens.add(t);
					}
					this.tokens = newTokens;
					this.Text = text;
					((ITextViewerExtension2)sourceViewer).invalidateTextPresentation(offset, insertedLength);
					return;
				}
			}
			
			var startToken = this.tokens[startIndex];
			if (offset > 0 && startIndex > 0 && startToken.offset == offset &&
					getPositionIndex(this.preprocessorPositions, offset - 1) == -1) {
					// if offset is the token start position: take the preceding token
					startToken = this.tokens[--startIndex];
			}

			// Get the end token
			int endIndex = getPositionIndex(this.tokens, offset + removedLength);
			if (endIndex == -1) {
				// End of document
				endIndex = ntokens - 1;
			} else {
				// Find the token at the end of line
				while (tokens[endIndex].LexicalUnit != LexicalUnit.NewLine && endIndex + 1 < ntokens) {
					endIndex++;
				}
			}
			var endToken = this.tokens[endIndex];
			int delta = insertedLength - removedLength;

			// Copy the text to scan into an array and scan it
			int index = startToken.offset;
			int subLength = endToken.offset + endToken.length + delta - index;
			var subText = new char[subLength];
			for (int i = 0; i < subLength; i++) {
				subText[i] = text[index++];
			}
			
			var insertedTokens = scan(subText);
			if (insertedTokens == null) {
				// Something went wrong: try a full scan
				initialize(text, false);
				return;
			}
			updateOffsetsAfter(0, startToken.offset, insertedTokens);
			
			// Update the offset of the trailing tokens and preprocessor parts
			int start = Math.max(startToken.offset + startToken.length, offset);
			updateOffsetsAfter(start, delta, pppositions);
			updateOffsetsAfter(start, delta, tokens);
			updateOffsetsAfter(start, delta, removedTokens);

			// Exclude the unmodified tokens
			var commonPartLength = 0;
			for (int i = 0; i < insertedTokens.size() && startIndex < endIndex; i++) {
				var oldToken = tokens[startIndex];
				var newToken = insertedTokens[i];
				if (sameToken(oldToken, newToken)) {
					commonPartLength++;
					startIndex++;
				} else {
					break;
				}
			}
			while (commonPartLength-- > 0) {
				insertedTokens.remove(0);
			}
			startToken = tokens[startIndex];

			commonPartLength = 0;
			for (int i = insertedTokens.size() - 1; i >= 0 && endIndex > startIndex; i--) {
				var oldToken = tokens[endIndex];
				var newToken = insertedTokens[i];
				if (sameToken(oldToken, newToken)) {
					commonPartLength++;
					endIndex--;
				} else {
					break;
				}
			}
			while (commonPartLength-- > 0) {
				insertedTokens.remove(insertedTokens.size() - 1);
			}
			endToken = tokens[endIndex];
			
			// Copy the removed tokens
			for (int i = startIndex; i <= endIndex; i++) {
				var token = tokens[i];
				if (!token.IsUpdated) {
					token.IsUpdated = true;
					removedTokens.add(token);
				}
			}
			Collections.sort(removedTokens, tokenPositionComparator);

			// Create the new list of tokens
			var newTokens = new ArrayList<Token>();
			for (int i = 0; i < startIndex; i++) {
				newTokens.add(tokens[i]);
			}
			newTokens.addAll(insertedTokens);
			for (int i = endIndex + 1; i < ntokens; i++) {
				newTokens.add(tokens[i]);
			}
			
			this.Text = text;
			this.tokens = newTokens;
			
			int off = startToken.offset;
			int len = Math.min(subLength + removedLength, sizeof(text) - off);
			((ITextViewerExtension2)sourceViewer).invalidateTextPresentation(off, len);

			//checkPositionsConsistency();
						
			Environment.trace(this, "update done in " + ((System.nanoTime() - t0) / 1e6) + "ms");
		}
		
		private static bool sameToken(Token t1, Token t2) {
			if (t1.offset != t2.offset) {
				return false;
			}
			if (t1.length != t2.length) {
				return false;
			}
			if (t1.LexicalUnit != t2.LexicalUnit) {
				return false;
			}
			if (t1.LexicalUnit == LexicalUnit.Identifier) {
				return false;
			}
			if (t1.LexicalUnit == LexicalUnit.Keyword && t1.Keyword != t2.Keyword) {
				return false;
			}
			return true;
		}

		private List<Token> scan(char[] text) {
			Environment.trace(this, "Scanning [" + new String(text) + "]");
			var result = new ArrayList<Token>();
			try {
				var errorManager = new CodeErrorManager();
				var scanner = new SourceCodeScanner(errorManager, text);
				LexicalUnit lu;
				while ((lu = scanner.nextLexicalUnit()) != LexicalUnit.EndOfStream) {
					int sp = scanner.StartPosition;
					result.add(new Token(sp, scanner.EndPosition - sp, sharedTextColors, lu, scanner.Keyword) { IsUpdated = true });
				}
				if (errorManager.HasErrors) {
					return null;
				}
				foreach (var tk in result) {
					tk.SemanticStyle = getSemanticStyle(tk.LexicalUnit);
				}
			} catch (CodeErrorException e) {
				return null;
			}
			return result;
		}

		private class TextListener : IDocumentListener, ITextPresentationListener {
			private Highlighter highlighter;
			private bool suspendTextPresentationUpdate;
		
			TextListener(Highlighter highlighter) {
				this.highlighter = highlighter;
				((ITextViewerExtension4)highlighter.sourceViewer).addTextPresentationListener(this);
				highlighter.document.addDocumentListener(this);
			}
			
			void dispose() {
				if (highlighter != null) {
					((ITextViewerExtension4)highlighter.sourceViewer).removeTextPresentationListener(this);
					highlighter.document.removeDocumentListener(this);
					highlighter = null;
				}
			}
		
			public void documentAboutToBeChanged(DocumentEvent event) {
				suspendTextPresentationUpdate = true;
			}
			
			public void documentChanged(DocumentEvent event) {
				suspendTextPresentationUpdate = false;
				highlighter.update(event.getOffset(), event.getText(), event.getLength());
			}
			
			public void applyTextPresentation(TextPresentation textPresentation) {
				if (!suspendTextPresentationUpdate) {
					highlighter.updateTextPresentation(textPresentation);
				}
			}
		}	
	}
}
