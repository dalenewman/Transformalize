#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using Transformalize.Libs.SharpZLib.Zip.Compression.Streams;

namespace Transformalize.Libs.SharpZLib.Checksums 
{
	
	/// <summary>
	/// Computes Adler32 checksum for a stream of data. An Adler32
	/// checksum is not as reliable as a CRC32 checksum, but a lot faster to
	/// compute.
	/// 
	/// The specification for Adler32 may be found in RFC 1950.
	/// ZLIB Compressed Data Format Specification version 3.3)
	/// 
	/// 
	/// From that document:
	/// 
	///      "ADLER32 (Adler-32 checksum)
	///       This contains a checksum value of the uncompressed data
	///       (excluding any dictionary data) computed according to Adler-32
	///       algorithm. This algorithm is a 32-bit extension and improvement
	///       of the Fletcher algorithm, used in the ITU-T X.224 / ISO 8073
	///       standard.
	/// 
	///       Adler-32 is composed of two sums accumulated per byte: s1 is
	///       the sum of all bytes, s2 is the sum of all s1 values. Both sums
	///       are done modulo 65521. s1 is initialized to 1, s2 to zero.  The
	///       Adler-32 checksum is stored as s2*65536 + s1 in most-
	///       significant-byte first (network) order."
	/// 
	///  "8.2. The Adler-32 algorithm
	/// 
	///    The Adler-32 algorithm is much faster than the CRC32 algorithm yet
	///    still provides an extremely low probability of undetected errors.
	/// 
	///    The modulo on unsigned long accumulators can be delayed for 5552
	///    bytes, so the modulo operation time is negligible.  If the bytes
	///    are a, b, c, the second sum is 3a + 2b + c + 3, and so is position
	///    and order sensitive, unlike the first sum, which is just a
	///    checksum.  That 65521 is prime is important to avoid a possible
	///    large class of two-byte errors that leave the check unchanged.
	///    (The Fletcher checksum uses 255, which is not prime and which also
	///    makes the Fletcher check insensitive to single byte changes 0 -
	///    255.)
	/// 
	///    The sum s1 is initialized to 1 instead of zero to make the length
	///    of the sequence part of s2, so that the length does not have to be
	///    checked separately. (Any sequence of zeroes has a Fletcher
	///    checksum of zero.)"
	/// </summary>
	/// <see cref="InflaterInputStream"/>
	/// <see cref="DeflaterOutputStream"/>
	public sealed class Adler32 : IChecksum
	{
		/// <summary>
		/// largest prime smaller than 65536
		/// </summary>
		const uint BASE = 65521;

	    /// <summary>
		/// Creates a new instance of the Adler32 class.
		/// The checksum starts off with a value of 1.
		/// </summary>
		public Adler32()
		{
			Reset();
		}

	    /// <summary>
	    /// Returns the Adler32 data checksum computed so far.
	    /// </summary>
	    public long Value {
	        get {
	            return checksum;
	        }
	    }

	    /// <summary>
		/// Resets the Adler32 checksum to the initial value.
		/// </summary>
		public void Reset()
		{
			checksum = 1;
		}
		
		/// <summary>
		/// Updates the checksum with a byte value.
		/// </summary>
		/// <param name="value">
		/// The data value to add. The high byte of the int is ignored.
		/// </param>
		public void Update(int value)
		{
			// We could make a length 1 byte array and call update again, but I
			// would rather not have that overhead
			var s1 = checksum & 0xFFFF;
			var s2 = checksum >> 16;
			
			s1 = (s1 + ((uint)value & 0xFF)) % BASE;
			s2 = (s1 + s2) % BASE;
			
			checksum = (s2 << 16) + s1;
		}
		
		/// <summary>
		/// Updates the checksum with an array of bytes.
		/// </summary>
		/// <param name="buffer">
		/// The source of the data to update with.
		/// </param>
		public void Update(byte[] buffer)
		{
			if ( buffer == null ) {
				throw new ArgumentNullException("buffer");
			}

			Update(buffer, 0, buffer.Length);
		}
		
		/// <summary>
		/// Updates the checksum with the bytes taken from the array.
		/// </summary>
		/// <param name="buffer">
		/// an array of bytes
		/// </param>
		/// <param name="offset">
		/// the start of the data used for this update
		/// </param>
		/// <param name="count">
		/// the number of bytes to use for this update
		/// </param>
		public void Update(byte[] buffer, int offset, int count)
		{
			if (buffer == null) {
				throw new ArgumentNullException("buffer");
			}
			
			if (offset < 0) {
#if NETCF_1_0
				throw new ArgumentOutOfRangeException("offset");
#else
				throw new ArgumentOutOfRangeException("offset", "cannot be negative");
#endif				
			}

			if ( count < 0 ) 
			{
#if NETCF_1_0
				throw new ArgumentOutOfRangeException("count");
#else
				throw new ArgumentOutOfRangeException("count", "cannot be negative");
#endif				
			}

			if (offset >= buffer.Length) 
			{
#if NETCF_1_0
				throw new ArgumentOutOfRangeException("offset");
#else
				throw new ArgumentOutOfRangeException("offset", "not a valid index into buffer");
#endif				
			}
			
			if (offset + count > buffer.Length) 
			{
#if NETCF_1_0
				throw new ArgumentOutOfRangeException("count");
#else
				throw new ArgumentOutOfRangeException("count", "exceeds buffer size");
#endif				
			}

			//(By Per Bothner)
			var s1 = checksum & 0xFFFF;
			var s2 = checksum >> 16;
			
			while (count > 0) {
				// We can defer the modulo operation:
				// s1 maximally grows from 65521 to 65521 + 255 * 3800
				// s2 maximally grows by 3800 * median(s1) = 2090079800 < 2^31
				var n = 3800;
				if (n > count) {
					n = count;
				}
				count -= n;
				while (--n >= 0) {
					s1 = s1 + (uint)(buffer[offset++] & 0xff);
					s2 = s2 + s1;
				}
				s1 %= BASE;
				s2 %= BASE;
			}
			
			checksum = (s2 << 16) | s1;
		}

	    #region Instance Fields
	    uint checksum;
	    #endregion
	}
}
