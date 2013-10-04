#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using Transformalize.Libs.SharpZLib.Checksums;

namespace Transformalize.Libs.SharpZLib.Zip.Compression 
{
	
	/// <summary>
	/// Strategies for deflater
	/// </summary>
	public enum DeflateStrategy 
	{
		/// <summary>
		/// The default strategy
		/// </summary>
		Default  = 0,
		
		/// <summary>
		/// This strategy will only allow longer string repetitions.  It is
		/// useful for random data with a small character set.
		/// </summary>
		Filtered = 1,

			
		/// <summary>
		/// This strategy will not look for string repetitions at all.  It
		/// only encodes with Huffman trees (which means, that more common
		/// characters get a smaller encoding.
		/// </summary>
		HuffmanOnly = 2
	}

	// DEFLATE ALGORITHM:
	// 
	// The uncompressed stream is inserted into the window array.  When
	// the window array is full the first half is thrown away and the
	// second half is copied to the beginning.
	//
	// The head array is a hash table.  Three characters build a hash value
	// and they the value points to the corresponding index in window of 
	// the last string with this hash.  The prev array implements a
	// linked list of matches with the same hash: prev[index & WMASK] points
	// to the previous index with the same hash.
	// 

	
	/// <summary>
	/// Low level compression engine for deflate algorithm which uses a 32K sliding window
	/// with secondary compression from Huffman/Shannon-Fano codes.
	/// </summary>
	public class DeflaterEngine : DeflaterConstants 
	{
	    #region Constants
	    const int TooFar = 4096;
	    #endregion

	    #region Constructors
	    /// <summary>
	    /// Construct instance with pending buffer
	    /// </summary>
	    /// <param name="pending">
	    /// Pending buffer to use
	    /// </param>>
	    public DeflaterEngine(DeflaterPending pending) 
	    {
	        this.pending = pending;
	        huffman = new DeflaterHuffman(pending);
	        adler = new Adler32();
			
	        window = new byte[2 * WSIZE];
	        head   = new short[HASH_SIZE];
	        prev   = new short[WSIZE];
			
	        // We start at index 1, to avoid an implementation deficiency, that
	        // we cannot build a repeat pattern at index 0.
	        blockStart = strstart = 1;
	    }

	    #endregion

	    /// <summary>
	    /// Get current value of Adler checksum
	    /// </summary>		
	    public int Adler {
	        get {
	            return unchecked((int)adler.Value);
	        }
	    }

	    /// <summary>
	    /// Total data processed
	    /// </summary>		
	    public long TotalIn {
	        get {
	            return totalIn;
	        }
	    }

	    /// <summary>
	    /// Get/set the <see cref="DeflateStrategy">deflate strategy</see>
	    /// </summary>		
	    public DeflateStrategy Strategy {
	        get {
	            return strategy;
	        }
	        set {
	            strategy = value;
	        }
	    }

	    /// <summary>
		/// Deflate drives actual compression of data
		/// </summary>
		/// <param name="flush">True to flush input buffers</param>
		/// <param name="finish">Finish deflation with the current input.</param>
		/// <returns>Returns true if progress has been made.</returns>
		public bool Deflate(bool flush, bool finish)
		{
			bool progress;
			do 
			{
				FillWindow();
				var canFlush = flush && (inputOff == inputEnd);

#if DebugDeflation
				if (DeflaterConstants.DEBUGGING) {
					Console.WriteLine("window: [" + blockStart + "," + strstart + ","
								+ lookahead + "], " + compressionFunction + "," + canFlush);
				}
#endif
				switch (compressionFunction) 
				{
					case DEFLATE_STORED:
						progress = DeflateStored(canFlush, finish);
						break;
					case DEFLATE_FAST:
						progress = DeflateFast(canFlush, finish);
						break;
					case DEFLATE_SLOW:
						progress = DeflateSlow(canFlush, finish);
						break;
					default:
						throw new InvalidOperationException("unknown compressionFunction");
				}
			} while (pending.IsFlushed && progress); // repeat while we have no pending output and progress was made
			return progress;
		}

		/// <summary>
		/// Sets input data to be deflated.  Should only be called when <code>NeedsInput()</code>
		/// returns true
		/// </summary>
		/// <param name="buffer">The buffer containing input data.</param>
		/// <param name="offset">The offset of the first byte of data.</param>
		/// <param name="count">The number of bytes of data to use as input.</param>
		public void SetInput(byte[] buffer, int offset, int count)
		{
			if ( buffer == null ) 
			{
				throw new ArgumentNullException("buffer");
			}

			if ( offset < 0 ) 
			{
				throw new ArgumentOutOfRangeException("offset");
			}

			if ( count < 0 ) 
			{
				throw new ArgumentOutOfRangeException("count");
			}

			if (inputOff < inputEnd) 
			{
				throw new InvalidOperationException("Old input was not completely processed");
			}
			
			var end = offset + count;
			
			/* We want to throw an ArrayIndexOutOfBoundsException early.  The
			* check is very tricky: it also handles integer wrap around.
			*/
			if ((offset > end) || (end > buffer.Length) ) 
			{
				throw new ArgumentOutOfRangeException("count");
			}
			
			inputBuf = buffer;
			inputOff = offset;
			inputEnd = end;
		}

		/// <summary>
		/// Determines if more <see cref="SetInput">input</see> is needed.
		/// </summary>		
		/// <returns>Return true if input is needed via <see cref="SetInput">SetInput</see></returns>
		public bool NeedsInput()
		{
			return (inputEnd == inputOff);
		}

		/// <summary>
		/// Set compression dictionary
		/// </summary>
		/// <param name="buffer">The buffer containing the dictionary data</param>
		/// <param name="offset">The offset in the buffer for the first byte of data</param>
		/// <param name="length">The length of the dictionary data.</param>
		public void SetDictionary(byte[] buffer, int offset, int length) 
		{
#if DebugDeflation
			if (DeflaterConstants.DEBUGGING && (strstart != 1) ) 
			{
				throw new InvalidOperationException("strstart not 1");
			}
#endif
			adler.Update(buffer, offset, length);
			if (length < MIN_MATCH) 
			{
				return;
			}

			if (length > MAX_DIST) 
			{
				offset += length - MAX_DIST;
				length = MAX_DIST;
			}
			
			Array.Copy(buffer, offset, window, strstart, length);
			
			UpdateHash();
			--length;
			while (--length > 0) 
			{
				InsertString();
				strstart++;
			}
			strstart += 2;
			blockStart = strstart;
		}
		
		/// <summary>
		/// Reset internal state
		/// </summary>		
		public void Reset()
		{
			huffman.Reset();
			adler.Reset();
			blockStart = strstart = 1;
			lookahead = 0;
			totalIn   = 0;
			prevAvailable = false;
			matchLen = MIN_MATCH - 1;
			
			for (var i = 0; i < HASH_SIZE; i++) {
				head[i] = 0;
			}
			
			for (var i = 0; i < WSIZE; i++) {
				prev[i] = 0;
			}
		}

		/// <summary>
		/// Reset Adler checksum
		/// </summary>		
		public void ResetAdler()
		{
			adler.Reset();
		}

	    /// <summary>
		/// Set the deflate level (0-9)
		/// </summary>
		/// <param name="level">The value to set the level to.</param>
		public void SetLevel(int level)
		{
			if ( (level < 0) || (level > 9) )
			{
				throw new ArgumentOutOfRangeException("level");
			}

			goodLength = GOOD_LENGTH[level];
			max_lazy   = MAX_LAZY[level];
			niceLength = NICE_LENGTH[level];
			max_chain  = MAX_CHAIN[level];
			
			if (COMPR_FUNC[level] != compressionFunction) {

#if DebugDeflation
				if (DeflaterConstants.DEBUGGING) {
				   Console.WriteLine("Change from " + compressionFunction + " to "
										  + DeflaterConstants.COMPR_FUNC[level]);
				}
#endif
				switch (compressionFunction) {
					case DEFLATE_STORED:
						if (strstart > blockStart) {
							huffman.FlushStoredBlock(window, blockStart,
								strstart - blockStart, false);
							blockStart = strstart;
						}
						UpdateHash();
						break;

					case DEFLATE_FAST:
						if (strstart > blockStart) {
							huffman.FlushBlock(window, blockStart, strstart - blockStart,
								false);
							blockStart = strstart;
						}
						break;

					case DEFLATE_SLOW:
						if (prevAvailable) {
							huffman.TallyLit(window[strstart-1] & 0xff);
						}
						if (strstart > blockStart) {
							huffman.FlushBlock(window, blockStart, strstart - blockStart, false);
							blockStart = strstart;
						}
						prevAvailable = false;
						matchLen = MIN_MATCH - 1;
						break;
				}
				compressionFunction = COMPR_FUNC[level];
			}
		}
		
		/// <summary>
		/// Fill the window
		/// </summary>
		public void FillWindow()
		{
			/* If the window is almost full and there is insufficient lookahead,
			 * move the upper half to the lower one to make room in the upper half.
			 */
			if (strstart >= WSIZE + MAX_DIST) 
			{
				SlideWindow();
			}
			
			/* If there is not enough lookahead, but still some input left,
			 * read in the input
			 */
			while (lookahead < MIN_LOOKAHEAD && inputOff < inputEnd) 
			{
				var more = 2 * WSIZE - lookahead - strstart;
				
				if (more > inputEnd - inputOff) 
				{
					more = inputEnd - inputOff;
				}
				
				Array.Copy(inputBuf, inputOff, window, strstart + lookahead, more);
				adler.Update(inputBuf, inputOff, more);
				
				inputOff += more;
				totalIn  += more;
				lookahead += more;
			}
			
			if (lookahead >= MIN_MATCH) 
			{
				UpdateHash();
			}
		}
		
		void UpdateHash() 
		{
/*
			if (DEBUGGING) {
				Console.WriteLine("updateHash: "+strstart);
			}
*/
			ins_h = (window[strstart] << HASH_SHIFT) ^ window[strstart + 1];
		}
		
		/// <summary>
		/// Inserts the current string in the head hash and returns the previous
		/// value for this hash.
		/// </summary>
		/// <returns>The previous hash value</returns>
		int InsertString() 
		{
			short match;
			var hash = ((ins_h << HASH_SHIFT) ^ window[strstart + (MIN_MATCH -1)]) & HASH_MASK;

#if DebugDeflation
			if (DeflaterConstants.DEBUGGING) 
			{
				if (hash != (((window[strstart] << (2*HASH_SHIFT)) ^ 
								  (window[strstart + 1] << HASH_SHIFT) ^ 
								  (window[strstart + 2])) & HASH_MASK)) {
						throw new SharpZipBaseException("hash inconsistent: " + hash + "/"
												+window[strstart] + ","
												+window[strstart + 1] + ","
												+window[strstart + 2] + "," + HASH_SHIFT);
					}
			}
#endif
			prev[strstart & WMASK] = match = head[hash];
			head[hash] = unchecked((short)strstart);
			ins_h = hash;
			return match & 0xffff;
		}
		
		void SlideWindow()
		{
			Array.Copy(window, WSIZE, window, 0, WSIZE);
			matchStart -= WSIZE;
			strstart   -= WSIZE;
			blockStart -= WSIZE;
			
			// Slide the hash table (could be avoided with 32 bit values
			// at the expense of memory usage).
			for (var i = 0; i < HASH_SIZE; ++i) {
				var m = head[i] & 0xffff;
				head[i] = (short)(m >= WSIZE ? (m - WSIZE) : 0);
			}
			
			// Slide the prev table.
			for (var i = 0; i < WSIZE; i++) {
				var m = prev[i] & 0xffff;
				prev[i] = (short)(m >= WSIZE ? (m - WSIZE) : 0);
			}
		}
		
		/// <summary>
		/// Find the best (longest) string in the window matching the 
		/// string starting at strstart.
		///
		/// Preconditions:
		/// <code>
		/// strstart + MAX_MATCH &lt;= window.length.</code>
		/// </summary>
		/// <param name="curMatch"></param>
		/// <returns>True if a match greater than the minimum length is found</returns>
		bool FindLongestMatch(int curMatch) 
		{
			var chainLength = max_chain;
			var niceLength  = this.niceLength;
			var prev    = this.prev;
			var scan        = strstart;
			int match;
			var best_end = strstart + matchLen;
			var best_len = Math.Max(matchLen, MIN_MATCH - 1);
			
			var limit = Math.Max(strstart - MAX_DIST, 0);
			
			var strend = strstart + MAX_MATCH - 1;
			var scan_end1 = window[best_end - 1];
			var scan_end  = window[best_end];
			
			// Do not waste too much time if we already have a good match:
			if (best_len >= goodLength) {
				chainLength >>= 2;
			}
			
			/* Do not look for matches beyond the end of the input. This is necessary
			* to make deflate deterministic.
			*/
			if (niceLength > lookahead) {
				niceLength = lookahead;
			}
			
#if DebugDeflation

			if (DeflaterConstants.DEBUGGING && (strstart > 2 * WSIZE - MIN_LOOKAHEAD))
			{
				throw new InvalidOperationException("need lookahead");
			}
#endif			
	
			do {

#if DebugDeflation

				if (DeflaterConstants.DEBUGGING && (curMatch >= strstart) )
				{
					throw new InvalidOperationException("no future");
				}
#endif            
				if (window[curMatch + best_len] != scan_end      || 
					window[curMatch + best_len - 1] != scan_end1 || 
					window[curMatch] != window[scan]             || 
					window[curMatch + 1] != window[scan + 1]) {
					continue;
				}
				
				match = curMatch + 2;
				scan += 2;
				
				/* We check for insufficient lookahead only every 8th comparison;
				* the 256th check will be made at strstart + 258.
				*/
				while (
					window[++scan] == window[++match] && 
					window[++scan] == window[++match] && 
					window[++scan] == window[++match] && 
					window[++scan] == window[++match] && 
					window[++scan] == window[++match] && 
					window[++scan] == window[++match] && 
					window[++scan] == window[++match] && 
					window[++scan] == window[++match] &&
					(scan < strend))
				{
					// Do nothing
				}
				
				if (scan > best_end) {
#if DebugDeflation
					if (DeflaterConstants.DEBUGGING && (ins_h == 0) )
						Console.Error.WriteLine("Found match: " + curMatch + "-" + (scan - strstart));
#endif
					matchStart = curMatch;
					best_end = scan;
					best_len = scan - strstart;
					
					if (best_len >= niceLength) {
						break;
					}
					
					scan_end1  = window[best_end - 1];
					scan_end   = window[best_end];
				}
				scan = strstart;
			} while ((curMatch = (prev[curMatch & WMASK] & 0xffff)) > limit && --chainLength != 0);
			
			matchLen = Math.Min(best_len, lookahead);
			return matchLen >= MIN_MATCH;
		}
		
		bool DeflateStored(bool flush, bool finish)
		{
			if (!flush && (lookahead == 0)) {
				return false;
			}
			
			strstart += lookahead;
			lookahead = 0;
			
			var storedLength = strstart - blockStart;
			
			if ((storedLength >= MAX_BLOCK_SIZE) || // Block is full
				(blockStart < WSIZE && storedLength >= MAX_DIST) ||   // Block may move out of window
				flush) {
				var lastBlock = finish;
				if (storedLength > MAX_BLOCK_SIZE) {
					storedLength = MAX_BLOCK_SIZE;
					lastBlock = false;
				}
				
#if DebugDeflation
				if (DeflaterConstants.DEBUGGING) 
				{
				   Console.WriteLine("storedBlock[" + storedLength + "," + lastBlock + "]");
				}
#endif

				huffman.FlushStoredBlock(window, blockStart, storedLength, lastBlock);
				blockStart += storedLength;
				return !lastBlock;
			}
			return true;
		}
		
		bool DeflateFast(bool flush, bool finish)
		{
			if (lookahead < MIN_LOOKAHEAD && !flush) {
				return false;
			}
			
			while (lookahead >= MIN_LOOKAHEAD || flush) {
				if (lookahead == 0) {
					// We are flushing everything
					huffman.FlushBlock(window, blockStart, strstart - blockStart, finish);
					blockStart = strstart;
					return false;
				}
				
				if (strstart > 2 * WSIZE - MIN_LOOKAHEAD) {
					/* slide window, as FindLongestMatch needs this.
					 * This should only happen when flushing and the window
					 * is almost full.
					 */
					SlideWindow();
				}
				
				int hashHead;
				if (lookahead >= MIN_MATCH && 
					(hashHead = InsertString()) != 0 && 
					strategy != DeflateStrategy.HuffmanOnly &&
					strstart - hashHead <= MAX_DIST && 
					FindLongestMatch(hashHead)) {
					// longestMatch sets matchStart and matchLen
#if DebugDeflation
					if (DeflaterConstants.DEBUGGING) 
					{
						for (int i = 0 ; i < matchLen; i++) {
							if (window[strstart + i] != window[matchStart + i]) {
								throw new SharpZipBaseException("Match failure");
							}
						}
					}
#endif					

					var full = huffman.TallyDist(strstart - matchStart, matchLen);

					lookahead -= matchLen;
					if (matchLen <= max_lazy && lookahead >= MIN_MATCH) {
						while (--matchLen > 0) {
							++strstart;
							InsertString();
						}
						++strstart;
					} else {
						strstart += matchLen;
						if (lookahead >= MIN_MATCH - 1) {
							UpdateHash();
						}
					}
					matchLen = MIN_MATCH - 1;
					if (!full) {
						continue;
					}
				} else {
					// No match found
					huffman.TallyLit(window[strstart] & 0xff);
					++strstart;
					--lookahead;
				}
				
				if (huffman.IsFull()) {
					var lastBlock = finish && (lookahead == 0);
					huffman.FlushBlock(window, blockStart, strstart - blockStart, lastBlock);
					blockStart = strstart;
					return !lastBlock;
				}
			}
			return true;
		}
		
		bool DeflateSlow(bool flush, bool finish)
		{
			if (lookahead < MIN_LOOKAHEAD && !flush) {
				return false;
			}
			
			while (lookahead >= MIN_LOOKAHEAD || flush) {
				if (lookahead == 0) {
					if (prevAvailable) {
						huffman.TallyLit(window[strstart-1] & 0xff);
					}
					prevAvailable = false;
					
					// We are flushing everything
#if DebugDeflation
					if (DeflaterConstants.DEBUGGING && !flush) 
					{
						throw new SharpZipBaseException("Not flushing, but no lookahead");
					}
#endif               
					huffman.FlushBlock(window, blockStart, strstart - blockStart,
						finish);
					blockStart = strstart;
					return false;
				}
				
				if (strstart >= 2 * WSIZE - MIN_LOOKAHEAD) {
					/* slide window, as FindLongestMatch needs this.
					 * This should only happen when flushing and the window
					 * is almost full.
					 */
					SlideWindow();
				}
				
				var prevMatch = matchStart;
				var prevLen = matchLen;
				if (lookahead >= MIN_MATCH) {

					var hashHead = InsertString();

					if (strategy != DeflateStrategy.HuffmanOnly &&
						hashHead != 0 &&
						strstart - hashHead <= MAX_DIST &&
						FindLongestMatch(hashHead)) {
						
						// longestMatch sets matchStart and matchLen
							
						// Discard match if too small and too far away
						if (matchLen <= 5 && (strategy == DeflateStrategy.Filtered || (matchLen == MIN_MATCH && strstart - matchStart > TooFar))) {
							matchLen = MIN_MATCH - 1;
						}
					}
				}
				
				// previous match was better
				if ((prevLen >= MIN_MATCH) && (matchLen <= prevLen) ) {
#if DebugDeflation
					if (DeflaterConstants.DEBUGGING) 
					{
					   for (int i = 0 ; i < matchLen; i++) {
						  if (window[strstart-1+i] != window[prevMatch + i])
							 throw new SharpZipBaseException();
						}
					}
#endif
					huffman.TallyDist(strstart - 1 - prevMatch, prevLen);
					prevLen -= 2;
					do {
						strstart++;
						lookahead--;
						if (lookahead >= MIN_MATCH) {
							InsertString();
						}
					} while (--prevLen > 0);

					strstart ++;
					lookahead--;
					prevAvailable = false;
					matchLen = MIN_MATCH - 1;
				} else {
					if (prevAvailable) {
						huffman.TallyLit(window[strstart-1] & 0xff);
					}
					prevAvailable = true;
					strstart++;
					lookahead--;
				}
				
				if (huffman.IsFull()) {
					var len = strstart - blockStart;
					if (prevAvailable) {
						len--;
					}
					var lastBlock = (finish && (lookahead == 0) && !prevAvailable);
					huffman.FlushBlock(window, blockStart, len, lastBlock);
					blockStart += len;
					return !lastBlock;
				}
			}
			return true;
		}

	    #region Instance Fields

	    // Hash index of string to be inserted

	    /// <summary>
	    /// The adler checksum
	    /// </summary>
	    readonly Adler32 adler;

	    /// <summary>
	    /// Hashtable, hashing three characters to an index for window, so
	    /// that window[index]..window[index+2] have this hash code.  
	    /// Note that the array should really be unsigned short, so you need
	    /// to and the values with 0xffff.
	    /// </summary>
	    readonly short[] head;

	    readonly DeflaterHuffman huffman;
	    readonly DeflaterPending pending;

	    /// <summary>
	    /// <code>prev[index &amp; WMASK]</code> points to the previous index that has the
	    /// same hash code as the string starting at index.  This way 
	    /// entries with the same hash code are in a linked list.
	    /// Note that the array should really be unsigned short, so you need
	    /// to and the values with 0xffff.
	    /// </summary>
	    readonly short[] prev;

	    /// <summary>
	    /// This array contains the part of the uncompressed stream that 
	    /// is of relevance.  The current character is indexed by strstart.
	    /// </summary>
	    readonly byte[] window;

	    int    blockStart;

	    /// <summary>
	    /// The current compression function.
	    /// </summary>
	    int compressionFunction;

	    int goodLength;

	    /// <summary>
	    /// The input data for compression.
	    /// </summary>
	    byte[] inputBuf;

	    /// <summary>
	    /// The end offset of the input data.
	    /// </summary>
	    int inputEnd;

	    /// <summary>
	    /// The offset into inputBuf, where input data starts.
	    /// </summary>
	    int inputOff;

	    int ins_h;

	    /// <summary>
	    /// lookahead is the number of characters starting at strstart in
	    /// window that are valid.
	    /// So window[strstart] until window[strstart+lookahead-1] are valid
	    /// characters.
	    /// </summary>
	    int    lookahead;

	    int    matchLen;
	    int    matchStart;

	    int max_chain, max_lazy, niceLength;
	    bool   prevAvailable;
	    DeflateStrategy strategy;

	    /// <summary>
	    /// Points to the current character in the window.
	    /// </summary>
	    int    strstart;

	    /// <summary>
	    /// The total bytes of input read.
	    /// </summary>
	    long totalIn;

	    #endregion
	}
}
