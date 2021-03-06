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
using org.eclipse.core.resources;
using FilePath = org.eclipse.core.runtime.Path;
using org.eclipse.jface.layout;
using org.eclipse.jface.viewers;
using org.eclipse.jface.window;
using org.eclipse.swt;
using org.eclipse.swt.events;
using org.eclipse.swt.graphics;
using org.eclipse.swt.layout;
using org.eclipse.swt.widgets;
using org.eclipse.ui.model;
using org.eclipse.ui.views.navigator;
using cnatural.eclipse.ui;
using cnatural.helpers;

namespace cnatural.eclipse.preferences {

	//
	// Manages the controls and events of the Libraries table
	//
	class LibrariesPreferences {
		private final static IContentProvider tableContentProvider = new TableContentProvider();
		private final static ICheckStateProvider tableCheckStateProvider = new TableCheckStateProvider();
		private final static ILabelProvider tableLabelProvider = new TableLabelProvider();
		private final static ICheckStateListener tableCheckStateListener = new TableCheckStateListener();
		
		private BuildSettingsPropertyPage propertyPage;
		private CheckboxTableViewer tableViewer;
		private Button upButton;
		private Button downButton;
		private Button topButton;
		private Button bottomButton;
		private Button removeButton;

		LibrariesPreferences(BuildSettingsPropertyPage propertyPage) {
			this.propertyPage = propertyPage;
		}

		Control createControl(Composite parent) {
			PixelConverter converter = new PixelConverter(parent);
			
			var composite = new Composite(parent, SWT.NONE);
			composite.setFont(parent.getFont());
			
			var layout = new GridLayout();
			layout.numColumns = 2;
			composite.setLayout(layout);

			//
			// Label
			//
			var label = new Label(composite, SWT.LEFT | SWT.WRAP);
			label.setFont(composite.getFont());
			label.setText(Messages.librariesPreferencesLabelText);
	
			var gd = new GridData(GridData.HORIZONTAL_ALIGN_FILL);
			gd.horizontalSpan = 2;
			gd.verticalAlignment = GridData.BEGINNING;
			label.setLayoutData(gd);

			//
			// Table
			//
			var tableComposite = new Composite(composite, SWT.NONE);
			tableComposite.setFont(composite.getFont());
			var tableColumnLayout = new TableColumnLayout();
			tableComposite.setLayout(tableColumnLayout);
			
			var table = new Table(tableComposite, SWT.BORDER | SWT.MULTI | SWT.H_SCROLL | SWT.V_SCROLL | SWT.CHECK);
			table.setFont(composite.getFont());
			table.setHeaderVisible(false);
			table.setLinesVisible(false);
			var column = new TableColumn(table, SWT.NONE);
			tableColumnLayout.setColumnData(column, new ColumnWeightData(100, false));
	
			// Table viewer
			tableViewer = new CheckboxTableViewer(table);
			tableViewer.setContentProvider(tableContentProvider);
			tableViewer.setCheckStateProvider(tableCheckStateProvider);
			tableViewer.setLabelProvider(tableLabelProvider);
			tableViewer.addCheckStateListener(tableCheckStateListener);
			tableViewer.addSelectionChangedListener(event => {
				if (event.getSelection().isEmpty()) {
					upButton.setEnabled(false);
					downButton.setEnabled(false);
					topButton.setEnabled(false);
					bottomButton.setEnabled(false);
					removeButton.setEnabled(false);
				} else {
					removeButton.setEnabled(true);
					var selection = (IStructuredSelection)event.getSelection();
					var libraries = propertyPage.getLibrariesWorkingCopy();
					if (selection.size() == libraries.size()) {
						upButton.setEnabled(false);
						downButton.setEnabled(false);
						topButton.setEnabled(false);
						bottomButton.setEnabled(false);
					} else {
						var it = selection.iterator();
						int min = libraries.size();
						int max = 0;
						while (it.hasNext()) {
							var lib = (ProjectLibrary)it.next();
							int index = libraries.indexOf(lib);
							if (index < min) {
								min = index;
							}
							if (index > max) {
								max = index;
							}
						}
						if (min > 0) {
							upButton.setEnabled(true);
							topButton.setEnabled(true);
						} else {
							upButton.setEnabled(false);
							topButton.setEnabled(false);
						}
						if (max < libraries.size() - 1) {
							downButton.setEnabled(true);
							bottomButton.setEnabled(true);
						} else {
							downButton.setEnabled(false);
							bottomButton.setEnabled(false);
						}
					}
				}
			});
			tableViewer.setInput(propertyPage.LibrariesWorkingCopy);
			
			gd = new GridData();
			gd.horizontalAlignment = GridData.FILL;
			gd.grabExcessHorizontalSpace = false;
			gd.verticalAlignment = GridData.FILL;
			gd.grabExcessVerticalSpace = true;
			gd.horizontalSpan = 1;
			gd.widthHint = converter.convertWidthInCharsToPixels(50);
			gd.heightHint = converter.convertHeightInCharsToPixels(6);
			tableComposite.setLayoutData(gd);

			//
			// Buttons
			//
			var buttons = new Composite(composite, SWT.NONE);
			buttons.setFont(composite.getFont());
			
			layout = new GridLayout();
			layout.marginWidth = 0;
			layout.marginHeight = 0;
			buttons.setLayout(layout);
	
			gd = new GridData();
			gd.horizontalAlignment = GridData.FILL;
			gd.grabExcessHorizontalSpace = true;
			gd.verticalAlignment = GridData.FILL;
			gd.grabExcessVerticalSpace = true;
			gd.horizontalSpan = 1;
			buttons.setLayoutData(gd);
			
			var button = createButton(buttons, Messages.librariesPreferencesAddJARsButtonText, true);
			button.addSelectionListener(new SelectionAdapter(e => openAddJARsDialog()));

			createSeparator(buttons);

			upButton = createButton(buttons, Messages.librariesPreferencesUpButtonText, false);
			upButton.addSelectionListener(new SelectionAdapter(e => moveSelectionUp()));
			downButton = createButton(buttons, Messages.librariesPreferencesDownButtonText, false);
			downButton.addSelectionListener(new SelectionAdapter(e => moveSelectionDown()));
			
			createSeparator(buttons);
	
			topButton = createButton(buttons, Messages.librariesPreferencesTopButtonText, false);
			topButton.addSelectionListener(new SelectionAdapter(e => moveSelectionToTop()));
			bottomButton = createButton(buttons, Messages.librariesPreferencesBottomButtonText, false);
			bottomButton.addSelectionListener(new SelectionAdapter(e => moveSelectionToBottom()));

			createSeparator(buttons);

			removeButton = createButton(buttons, Messages.librariesPreferencesRemoveButtonText, false);
			removeButton.addSelectionListener(new SelectionAdapter(e => removeSelection()));
			
			return composite;
		}

		private void openAddJARsDialog() {
			var dialog = new FilteredElementTreeSelectionDialog(
					propertyPage.getShell(), new WorkbenchLabelProvider(), new WorkbenchContentProvider());
			dialog.setValidator(new TypedElementSelectionValidator(new Class<?>[] { typeof(IFile) }, true, null));
			dialog.setHelpAvailable(false);
			dialog.setTitle(Messages.librariesPreferencesOpenJARsDialogTitle);
			dialog.setMessage(Messages.librariesPreferencesOpenJARsDialogMessage);
			dialog.setInput(propertyPage.getProject());
			dialog.setComparator(new ResourceComparator(ResourceComparator.NAME));
	
			var excludedFiles = new ArrayList<IResource>();
			foreach (var lib in propertyPage.getLibrariesWorkingCopy()) {
				excludedFiles.add(propertyPage.getProject().getFile(FilePath.fromPortableString(lib.getPath())));
			}
			dialog.addFilter(new ArchiveFileFilter(excludedFiles, true, true));
			
			if (dialog.open() == Window.OK) {
				var elements = dialog.getResult();
				if (sizeof(elements) > 0) {
					for (int i = 0; i < sizeof(elements); i++) {
						var element = (IFile)elements[i];
						propertyPage.getLibrariesWorkingCopy().add(new ProjectLibrary(element.getProjectRelativePath().toPortableString()));
					}
					tableViewer.setInput(propertyPage.getLibrariesWorkingCopy());
				}
			}
		}

		private void moveSelectionUp() {
			var s = tableViewer.getSelection();
			if (s.isEmpty()) {
				return;
			}
			var selection = (IStructuredSelection)s;
			var libraries = propertyPage.getLibrariesWorkingCopy();
			var it = selection.iterator();
			int prev = -1;
			var contiguous = true;
			while (it.hasNext()) {
				var lib = (ProjectLibrary)it.next();
				int index = libraries.indexOf(lib);
				if (prev > -1 && index != prev + 1) {
					contiguous = false;
				}
				prev = index;
				if (index > 0) {
					libraries.remove(lib);
					libraries.add(index - 1, lib);
					if (index == 1) {
						upButton.setEnabled(false);
					}
				}
			}
			if (contiguous && !upButton.isEnabled()) {
				topButton.setEnabled(false);
			}
			downButton.setEnabled(true);
			bottomButton.setEnabled(true);
			tableViewer.setInput(libraries);
		}
	
		private void moveSelectionDown() {
			ISelection s = tableViewer.getSelection();
			if (s.isEmpty()) {
				return;
			}
			IStructuredSelection selection = (IStructuredSelection)s;
			var libraries = propertyPage.getLibrariesWorkingCopy();
			var inverted = new ProjectLibrary[selection.size()];
			var it = selection.iterator();
			int i = sizeof(inverted);
			while (it.hasNext()) {
				inverted[--i] = (ProjectLibrary)it.next();
			}
			int prev = -1;
			var contiguous = true;
			foreach (ProjectLibrary lib in inverted) {
				int index = libraries.indexOf(lib);
				if (prev > -1 && index != prev + 1) {
					contiguous = false;
				}
				prev = index;
				if (index < libraries.size() - 1) {
					libraries.remove(lib);
					libraries.add(index + 1, lib);
					if (index + 2 == libraries.size()) {
						downButton.setEnabled(false);
					}
				}
			}
			if (contiguous && !downButton.isEnabled()) {
				bottomButton.setEnabled(false);
			}
			topButton.setEnabled(true);
			upButton.setEnabled(true);
			tableViewer.setInput(libraries);
		}
	
		private void moveSelectionToTop() {
			var s = tableViewer.getSelection();
			if (s.isEmpty()) {
				return;
			}
			var selection = (IStructuredSelection)s;
			var libraries = propertyPage.getLibrariesWorkingCopy();
			var it = selection.iterator();
			int current = 0;
			while (it.hasNext()) {
				var lib = (ProjectLibrary)it.next();
				int index = libraries.indexOf(lib);
				if (index > current) {
					libraries.remove(lib);
					libraries.add(current++, lib);
				}
			}
			downButton.setEnabled(true);
			bottomButton.setEnabled(true);
			topButton.setEnabled(false);
			upButton.setEnabled(false);
			tableViewer.setInput(libraries);
		}
	
		private void moveSelectionToBottom() {
			var s = tableViewer.getSelection();
			if (s.isEmpty()) {
				return;
			}
			var selection = (IStructuredSelection)s;
			var libraries = propertyPage.getLibrariesWorkingCopy();
			var inverted = new ProjectLibrary[selection.size()];
			var it = selection.iterator();
			int i = sizeof(inverted);
			while (it.hasNext()) {
				inverted[--i] = (ProjectLibrary)it.next();
			}
			int offset = 0;
			int size = libraries.size();
			foreach (var lib in inverted) {
				int index = libraries.indexOf(lib);
				if (index < libraries.size() - offset - 1) {
					libraries.remove(lib);
					libraries.add(size - ++offset, lib);
				}
			}
			downButton.setEnabled(false);
			bottomButton.setEnabled(false);
			topButton.setEnabled(true);
			upButton.setEnabled(true);
			tableViewer.setInput(libraries);
		}
	
		private void removeSelection() {
			var s = tableViewer.getSelection();
			if (s.isEmpty()) {
				return;
			}
			var selection = (IStructuredSelection)s;
			var libraries = propertyPage.getLibrariesWorkingCopy();
			var it = selection.iterator();
			while (it.hasNext()) {
				var lib = (ProjectLibrary)it.next();
				libraries.remove(lib);
			}
			tableViewer.setInput(libraries);
		}

		private static Button createButton(Composite parent, String label, bool enabled) {
			var button = new Button(parent, SWT.PUSH);
			button.setFont(parent.getFont());
			button.setText(label);
			button.setEnabled(enabled);
			
			var gd = new GridData();
			gd.horizontalAlignment = GridData.FILL;
			gd.grabExcessHorizontalSpace = true;
			gd.verticalAlignment = GridData.BEGINNING;
			button.setLayoutData(gd);
			
			return button;
		}

		private static void createSeparator(Composite parent) {
			var separator = new Label(parent, SWT.NONE);
			separator.setFont(parent.getFont());
			separator.setVisible(false);
			
			var gd = new GridData();
			gd.horizontalAlignment = GridData.FILL;
			gd.verticalAlignment = GridData.BEGINNING;
			gd.heightHint = 4;
			separator.setLayoutData(gd);
		}

		private interface ISelectionHandler {
			void handle(SelectionEvent e);
		}
		
		private class SelectionAdapter : SelectionListener {
			private ISelectionHandler handler;
		
			SelectionAdapter(ISelectionHandler handler) {
				this.handler = handler;
			}
			public void widgetSelected(SelectionEvent e) {
				handler.handle(e);
			}
			public void widgetDefaultSelected(SelectionEvent e) {
				handler.handle(e);
			}
		}

		private class TableContentProvider : IStructuredContentProvider {
			public void inputChanged(Viewer viewer, Object oldInput, Object newInput) {
			}
			
			public void dispose() {
			}
			
			public Object[] getElements(Object inputElement) {
				return ((ArrayList<ProjectLibrary>)inputElement).toArray();
			}
		}
		
		private class TableCheckStateProvider : ICheckStateProvider {
			public bool isGrayed(Object element) {
				return false;
			}
			
			public bool isChecked(Object element) {
				return ((ProjectLibrary)element).Enabled;
			}
		}
		
		private class TableLabelProvider : ILabelProvider {
			public void removeListener(ILabelProviderListener listener) {
			}
			
			public bool isLabelProperty(Object element, String property) {
				return false;
			}
			
			public void dispose() {
			}
			
			public void addListener(ILabelProviderListener listener) {
			}
			
			public String getText(Object element) {
				return PathHelper.getFileName(((ProjectLibrary)element).getPath());
			}
			
			public Image getImage(Object element) {
				return Environment.getIcon(Icon.Jar);
			}
		}
		
		private class TableCheckStateListener : ICheckStateListener {
			public void checkStateChanged(CheckStateChangedEvent event) {
				var lib = (ProjectLibrary)event.getElement();
				lib.setEnabled(event.getChecked());
			}
		}
	}
}
