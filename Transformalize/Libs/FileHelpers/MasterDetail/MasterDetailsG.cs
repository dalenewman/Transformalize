#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion


#define GENERICS
#if NET_2_0

using System.ComponentModel;
using System.Diagnostics;
//using System.ComponentModel.TypeConverter;

namespace FileHelpers.MasterDetail
{
	/// <summary>
	/// <para>This class contains information of a Master record an their Details records.</para>
	/// <para>This class is used for the Read and Write operations of the <see cref="MasterDetailEngine"/>.</para>
	/// </summary>
#if ! GENERICS
#if NET_2_0
    [DebuggerDisplay("Master: {Master.ToString()} - Details: {Details.Length}")]
#endif
    [TypeConverter(typeof(ExpandableObjectConverter))]
	public class MasterDetails
	{

		/// <summary>Create an empty instance.</summary>
		public MasterDetails()
		{
			mDetails = mEmpty.mDetails;
		}

		/// <summary>Create a new instance with the specified values.</summary>
		/// <param name="master">The master record.</param>
		/// <param name="details">The details record.</param>
		public MasterDetails(object master, object[] details)
		{
			mMaster = master;
			mDetails = details;
		}
		
		#if NET_2_0
		[DebuggerBrowsable(DebuggerBrowsableState.Never)] 
		#endif
		private static MasterDetails mEmpty = new MasterDetails(null, new object[] {});

		/// <summary>Returns a canonical empty MasterDetail object.</summary>
		public static MasterDetails Empty
		{
			get { return mEmpty; }
		}

		#if NET_2_0
		[DebuggerBrowsable(DebuggerBrowsableState.Never)] 
		#endif
		internal object mMaster;

		/// <summary>The Master record.</summary>
		public object Master
		{
			get { return mMaster; }
			set { mMaster = value; }
		}

		#if NET_2_0
		[DebuggerBrowsable(DebuggerBrowsableState.Never)] 
		#endif
		internal object[] mDetails;

		/// <summary>An Array with the Detail records.</summary>
		[TypeConverter(typeof(ArrayConverter))]
		public object[] Details
		{
			get { return mDetails; }
			set { mDetails = value; }
		}

#else
	public class MasterDetails<M,D>
        where M : class
        where D : class
	{

		/// <summary>Create an empty instance.</summary>
		public MasterDetails()
		{
			mDetails = mEmpty.mDetails;
		}

		/// <summary>Create a new instance with the specified values.</summary>
		/// <param name="master">The master record.</param>
		/// <param name="details">The details record.</param>
		public MasterDetails(M master, D[] details)
		{
			mMaster = master;
			mDetails = details;
		}

		private static MasterDetails<M,D> mEmpty = new MasterDetails<M,D>(null, new D[] {});

		/// <summary>Returns a canonical empty MasterDetail object.</summary>
		public static MasterDetails<M,D> Empty
		{
			get { return mEmpty; }
		}

		internal M mMaster;

		/// <summary>The Master record.</summary>
		public M Master
		{
			get { return mMaster; }
			set { mMaster = value; }
		}

		internal D[] mDetails;

		/// <summary>An Array with the Detail records.</summary>
		public D[] Details
		{
			get { return mDetails; }
			set { mDetails = value; }
		}

#endif

	}
}

#endif