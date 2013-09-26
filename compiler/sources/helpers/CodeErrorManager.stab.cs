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
using java.lang;
using java.util;
using stab.lang;
using stab.query;

namespace stab.tools.helpers {

	public class CodeErrorException : RuntimeException {
	}

	public class CodeError {
		public CodeError(String filename, int id, int level, String message, int line, int column) {
			this.Filename = filename;
			this.Id = id;
			this.Level = level;
			this.Message = message;
			this.Line = line;
			this.Column = column;
		}

		public String Filename^;
		
		public int Id^;
		
		public int Level^;
		
		public String Message^;
		
		public int Line^;
		
		public int Column^;
	}


	public class CodeErrorManager {
		private List<CodeError> errors;
		private bool[] errorsByScope;
		private int scopes;
		private IntIterable disabledWarnings;
		private bool disableWarnings;
	
		public CodeErrorManager() {
			this.errors = new ArrayList<CodeError>();
			this.errorsByScope = new bool[16];
		}
	
		public Iterable<CodeError> Errors {
			get {
				return errors;
			}
		}
	
		public int ErrorCount {
			get {
				return this.errors.size();
			}
			set {
				while (value < this.errors.size()) {
					this.errors.remove(this.errors.size() - 1);
				}
			}
		}
	
		public bool HasErrors {
			get {
				if (this.scopes == 0) {
					return this.errors.any(p => p.Level == 0);
				} else {
					return this.errorsByScope[this.scopes];
				}
			}
		}

		public IntIterable DisabledWarnings {
			get {
				return disabledWarnings;
			}
			set {
				disabledWarnings = value;
				disableWarnings = false;
			}
		}
		
		public void disableAllWarnings() {
			disableWarnings = true;
		}
		
		public void disable() {
			this.scopes++;
			if (this.scopes == sizeof(this.errorsByScope)) {
				var t = new bool[sizeof(this.errorsByScope) * 2];
				System.arraycopy(this.errorsByScope, 0, t, 0, sizeof(this.errorsByScope));
				this.errorsByScope = t;
			}
			this.errorsByScope[this.scopes] = false;
		}
	
		public void restore() {
			this.scopes--;
		}
		
		public bool IsDisabled {
			get {
				return this.scopes > 0;
			}
		}
	
		public void addError(String filename, int id, int level, String message, int line, int column) {
			if (level == 0) {
				this.errorsByScope[this.scopes] = true;
			}
			if (this.scopes == 0) {
				if (level == 0 || (!disableWarnings && this.DisabledWarnings != null && !this.DisabledWarnings.contains(id))) {
					this.errors.add(new CodeError(filename, id, level, message, line, column));
				}
			}
		}
	}
}