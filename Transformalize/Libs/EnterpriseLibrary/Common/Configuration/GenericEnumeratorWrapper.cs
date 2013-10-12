//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Core
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections;
using System.Collections.Generic;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration
{
	/// <devdoc>
	/// Represents a genereic enumerator for the NamedElementCollection.
	/// </devdoc>	
	internal class GenericEnumeratorWrapper<T> : IEnumerator<T>
	{
		private IEnumerator wrappedEnumerator;

		internal GenericEnumeratorWrapper(IEnumerator wrappedEnumerator)
		{
			this.wrappedEnumerator = wrappedEnumerator;
		}

		T IEnumerator<T>.Current
		{
			get { return (T) this.wrappedEnumerator.Current; }
		}

		void IDisposable.Dispose()
		{
			this.wrappedEnumerator = null;
		}		

		object IEnumerator.Current
		{
			get { return this.wrappedEnumerator.Current; }
		}

		bool IEnumerator.MoveNext()
		{
			return this.wrappedEnumerator.MoveNext();
		}

		void IEnumerator.Reset()
		{
			this.wrappedEnumerator.Reset();
		}	
	}
}
