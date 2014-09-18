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
using Transformalize.Libs.NVelocity.Util.Introspection;

namespace Transformalize.Libs.NVelocity.Runtime.Parser.Node
{
    /// <summary>
	/// Returned the value of object property when executed.
	/// </summary>
	public class EnumValueExecutor : AbstractExecutor
	{
		protected Introspector introspector;

		public EnumValueExecutor(IRuntimeLogger r, Introspector i, Type type, String propertyName)
		{
			runtimeLogger = r;
			introspector = i;

			Discover(type, propertyName);
		}

		protected internal virtual void Discover(Type type, String propertyName)
		{
			try
			{
				value = Enum.Parse(type, propertyName, true);
			}
			catch (System.Exception ex)
			{
				runtimeLogger.Error(string.Format("EnumValueExecutor: {0}", ex));
			}
		}

		/// <summary>
		/// Execute property against context.
		/// </summary>
		public override Object Execute(Object o)
		{
			return value;
		}
	}
}
