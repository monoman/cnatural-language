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
using stab.lang;
using cnatural.helpers;

namespace cnatural.syntaxtree {

	public abstract class SyntaxNode : UserDataContainer {
		protected SyntaxNode() {
			this.StartPosition = -1;
		}
		
		public String Filename {
			get;
			set;
		}
		
		public int Line {
			get;
			set;
		}
		
		public int Column {
			get;
			set;
		}

        public int StartPosition {
            get;
			set;
        }

        public int EndPosition {
            get;
			set;
        }
		
		public IntIterable DisabledWarnings {
			get;
			set;
		}
	}

}
