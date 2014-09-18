// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Reflection;

namespace Transformalize.Libs.NVelocity.Runtime.Parser.Node
{
    /// <summary>
	/// Abstract class that is used to execute an arbitrary
	/// method that is in introspected. This is the superclass
	/// for the GetExecutor and PropertyExecutor.
	/// </summary>
	public abstract class AbstractExecutor
	{
		protected internal IRuntimeLogger runtimeLogger = null;

		/// <summary>
		/// Method to be executed.
		/// </summary>
		protected internal MethodInfo method = null;

		protected internal PropertyInfo property = null;

		/// <summary>
		/// Value (probably from enum).
		/// </summary>
		protected internal object value;

		/// <summary>
		/// Execute method against context.
		/// </summary>
		public abstract Object Execute(Object o);

		public bool IsAlive
		{
			get { return (method != null || property != null || value != null); }
		}

		public MethodInfo Method
		{
			get { return method; }
		}

		public PropertyInfo Property
		{
			get { return property; }
		}

		public object Value
		{
			get { return value; }
		}
	}
}