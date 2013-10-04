#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.SharpZLib.Zip.Compression 
{
	
	/// <summary>
	/// This class stores the pending output of the Deflater.
	/// 
	/// author of the original java version : Jochen Hoenicke
	/// </summary>
	public class DeflaterPending : PendingBuffer
	{
		/// <summary>
		/// Construct instance with default buffer size
		/// </summary>
		public DeflaterPending() : base(DeflaterConstants.PENDING_BUF_SIZE)
		{
		}
	}
}
