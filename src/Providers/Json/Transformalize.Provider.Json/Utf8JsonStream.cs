using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Transformalize.Providers.Json {
   // Preserve StreamReader's BOM detection without retaining the entire document.
   // The underlying stream belongs to the caller.
   internal sealed class Utf8JsonStream : Stream {
      private readonly StreamReader _reader;
      private readonly Encoder _encoder = Encoding.UTF8.GetEncoder();
      private readonly char[] _characters = new char[1024];
      private readonly byte[] _bytes = new byte[4096];
      private int _offset, _count;
      private bool _finished;

      public Utf8JsonStream(Stream stream) {
         _reader = new StreamReader(stream, Encoding.UTF8, true, 1024, true);
      }

      public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken token) {
         token.ThrowIfCancellationRequested();
         if (count == 0) return 0;
         while (_count == 0 && !_finished) {
            var read = await _reader.ReadAsync(_characters, 0, _characters.Length).ConfigureAwait(false);
            token.ThrowIfCancellationRequested();
            _finished = read == 0;
            _offset = 0;
            _count = _encoder.GetBytes(_characters, 0, read, _bytes, 0, _finished);
         }
         var copied = Math.Min(count, _count);
         Buffer.BlockCopy(_bytes, _offset, buffer, offset, copied);
         _offset += copied;
         _count -= copied;
         return copied;
      }

      protected override void Dispose(bool disposing) {
         if (disposing) _reader.Dispose();
         base.Dispose(disposing);
      }

      public override bool CanRead => true;
      public override bool CanSeek => false;
      public override bool CanWrite => false;
      public override long Length => throw new NotSupportedException();
      public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
      public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
      public override void Flush() => throw new NotSupportedException();
      public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
      public override void SetLength(long value) => throw new NotSupportedException();
      public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
   }
}
