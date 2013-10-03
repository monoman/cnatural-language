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
using org.eclipse.osgi.util;

namespace cnatural.eclipse {

	//
	// Localized strings
	//
	public static class Messages : NLS {
		static Messages() {
			NLS.initializeMessages("cnatural.eclipse.messages", typeof(Messages));
		}
		
		public static String pluginStartup;
		
		public static String librariesTabItemText;
		public static String librariesPreferencesLabelText;
		public static String librariesPreferencesAddJARsButtonText;
		public static String librariesPreferencesUpButtonText;
		public static String librariesPreferencesDownButtonText;
		public static String librariesPreferencesTopButtonText;
		public static String librariesPreferencesBottomButtonText;
		public static String librariesPreferencesRemoveButtonText;
		public static String librariesPreferencesOpenJARsDialogTitle;
		public static String librariesPreferencesOpenJARsDialogMessage;
		
		public static String fullBuildTaskName;
		
		public static String emptyCompletionText;
	}	
}
