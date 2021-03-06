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
using org.eclipse.core.runtime;
using org.eclipse.jface.viewers;
using stab.query;

namespace cnatural.eclipse.helpers {

	//
	// Utility methods to manipulate Eclipse APIs.
	//
	public class EclipseHelper {
	
		public static IResource getResourceFromSelection(ISelection selection) {
			IResource resource = null;
			if (selection instanceof IStructuredSelection) {
				var it = ((IStructuredSelection)selection).iterator();
				while (it.hasNext()) {
					var next = it.next();
					if (next instanceof IResource) {
						resource = (IResource)next;
					} else if (next instanceof IAdaptable) {
						resource = (IResource)((IAdaptable)next).getAdapter(typeof(IResource));
					}
					if (resource != null) {
						break;
					}
				}
			}
			return resource;
		}
		
		public static IProject getProjectFromSelection(ISelection selection) {
			var resource = getResourceFromSelection(selection);
			if (resource == null) {
				return null;
			}
			if (resource.getType() == IResource.PROJECT) {
				return (IProject)resource;
			} else {
				return resource.getProject();
			}
		}
	
		public static bool hasNature(IProject project, String natureId) {
			if (project.exists() && project.isOpen()) {
				try {
					return project.hasNature(natureId);
				} catch (CoreException e) {
					Environment.logException(e);
				}
			}
			return false;
		}
		
		public static void createFolders(IResource resource) {
			var parent = (IFolder)resource.getParent();
			if (!parent.exists()) {
				createFolders(parent);
				try {
					parent.create(IResource.FORCE, true, null);
				} catch (CoreException e) {
					Environment.logException(e);
				}
			}
		}
		
		public static Iterable<IFile> getModifiedFiles(IResourceDelta delta, Iterable<String> extensions, Iterable<String> excludedFolders) {
			var result = new HashSet<IFile>();
			try {
				delta.accept(p => {
					var resource = p.getResource();
					switch (resource.getType()) {
					case IResource.FOLDER:
						return !excludedFolders.contains(resource.getProjectRelativePath().toPortableString());
					case IResource.FILE:
						var file = (IFile)resource;
						if (extensions.contains(file.getFileExtension())) {
							//if ((p.getFlags() & IResourceDelta.CONTENT) != 0 || (p.getFlags() & IResourceDelta.MARKERS) == 0) {
							result.add(file);
							//}
						}
						return false;
					}
					return true;
				});
			} catch (CoreException e) {
				Environment.logException(e);
			}
			return result;
		}
	}
}
