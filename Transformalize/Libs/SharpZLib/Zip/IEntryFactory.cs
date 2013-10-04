#region License
// /*
// See license included in this library folder.
// */
#endregion

using Transformalize.Libs.SharpZLib.Core;

namespace Transformalize.Libs.SharpZLib.Zip
{
	/// <summary>
	/// Defines factory methods for creating new <see cref="ZipEntry"></see> values.
	/// </summary>
	public interface IEntryFactory
	{
	    /// <summary>
	    /// Get/set the <see cref="INameTransform"></see> applicable.
	    /// </summary>
	    INameTransform NameTransform { get; set;  }

	    /// <summary>
		/// Create a <see cref="ZipEntry"/> for a file given its name
		/// </summary>
		/// <param name="fileName">The name of the file to create an entry for.</param>
		/// <returns>Returns a <see cref="ZipEntry">file entry</see> based on the <paramref name="fileName"/> passed.</returns>
		ZipEntry MakeFileEntry(string fileName);

		/// <summary>
		/// Create a <see cref="ZipEntry"/> for a file given its name
		/// </summary>
		/// <param name="fileName">The name of the file to create an entry for.</param>
		/// <param name="useFileSystem">If true get details from the file system if the file exists.</param>
		/// <returns>Returns a <see cref="ZipEntry">file entry</see> based on the <paramref name="fileName"/> passed.</returns>
		ZipEntry MakeFileEntry(string fileName, bool useFileSystem);

		/// <summary>
		/// Create a <see cref="ZipEntry"/> for a directory given its name
		/// </summary>
		/// <param name="directoryName">The name of the directory to create an entry for.</param>
		/// <returns>Returns a <see cref="ZipEntry">directory entry</see> based on the <paramref name="directoryName"/> passed.</returns>
		ZipEntry MakeDirectoryEntry(string directoryName);

		/// <summary>
		/// Create a <see cref="ZipEntry"/> for a directory given its name
		/// </summary>
		/// <param name="directoryName">The name of the directory to create an entry for.</param>
		/// <param name="useFileSystem">If true get details from the file system for this directory if it exists.</param>
		/// <returns>Returns a <see cref="ZipEntry">directory entry</see> based on the <paramref name="directoryName"/> passed.</returns>
		ZipEntry MakeDirectoryEntry(string directoryName, bool useFileSystem);
	}
}
