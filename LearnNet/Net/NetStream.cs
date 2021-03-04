using System;
using System.IO;

namespace LearnNet
{
    // Summary: 세션 스트림으로 사용하기 위한 스트림
    public class NetStream : Stream
    {
        protected byte[] buffer;  // bytes data
        protected long position;   // current stream position 

        public readonly long DefaultStreamSize = 128;
        public readonly long DefaultIncreaseSize = 128;

        // Summary:
        //      Creates a packet stream with initial size that increases on request
        public NetStream()
        {
            buffer = new byte[DefaultStreamSize];
            position = 0;
        }

        public override bool CanRead { get { return true; } }
        public override bool CanSeek { get { return true; } }
        public override bool CanWrite { get { return true; } }
        public override bool CanTimeout{ get { return false; } }

        // Summary:
        //     Gets or sets the number of bytes allocated for this stream.
        //
        // Returns:
        //     The length of the usable portion of the buffer for the stream.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     A capacity is set that is negative or less than the current length of the
        //     stream.
        //
        //   System.ObjectDisposedException:
        //     The current stream is closed.
        //
        //   System.NotSupportedException:
        //     set is invoked on a stream whose capacity cannot be modified.
        public virtual long Capacity { get { return buffer.Length; } }

        // Summary:
        //     Gets the length of the stream in bytes.
        //
        // Returns:
        //     The length of the stream in bytes.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The stream is closed.
        public override long Length { get {return buffer.Length; } }

        // Summary:
        //     Gets the current position within the stream.
        //
        // Returns:
        //     The current position within the stream.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     The position is set to a negative value or a value greater than System.Int32.MaxValue.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        public override long Position { get { return position; } set { position = value; }  }

        // Summary:
        //     Releases the unmanaged resources used by the System.IO.MemoryStream class
        //     and optionally releases the managed resources.
        //
        // Parameters:
        //   disposing:
        //     true to release both managed and unmanaged resources; false to release only
        //     unmanaged resources.
        protected override void Dispose(bool disposing)
        {
            buffer = null;
        }

        // Summary:
        //     Overrides the System.IO.Stream.Flush() method so that no action is performed.
        public override void Flush()
        {
            // empty
        }

        // Summary:
        //      Copy content to destination stream
        public void CopyTo(NetStream destination)
        {
            // 상대방이 버퍼가 있는 지 점검하고 없으면 확장
            destination.EnsureCapacity(Position);

            // BlockCopy checks arguments
            Buffer.BlockCopy(buffer, 0, destination.buffer, (int)destination.position, (int)Position );
            destination.Position += Position;
        }

        // Summary:
        //     Reads a block of bytes from the current stream and writes the data to a buffer.
        //
        // Parameters:
        //   buffer:
        //     When this method returns, contains the specified byte array with the values
        //     between offset and (offset + count - 1) replaced by the characters read from
        //     the current stream.
        //
        //   offset:
        //     The zero-based byte offset in buffer at which to begin storing data from
        //     the current stream.
        //
        //   count:
        //     The maximum number of bytes to read.
        //
        // Returns:
        //     The total number of bytes written into the buffer. This can be less than
        //     the number of bytes requested if that number of bytes are not currently available,
        //     or zero if the end of the stream is reached before any bytes are read.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     buffer is null.
        //
        //   System.ArgumentOutOfRangeException:
        //     offset or count is negative.
        //
        //   System.ArgumentException:
        //     offset subtracted from the buffer length is less than count.
        public override int Read(byte[] buf, int offset, int count)
        {
            int rcount = Math.Min(count, (int)(Capacity - position));
            // BlockCopy checks arguments
            System.Buffer.BlockCopy(this.buffer, (int)position, buf, offset, rcount);

            position += (long)rcount;

            return rcount;
        }

        // Summary:
        //     Reads a byte from the current stream.
        //
        // Returns:
        //     The byte cast to a System.Int32, or -1 if the end of the stream has been
        //     reached.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The current stream instance is closed.
        public override int ReadByte()
        {
            position += 1;

            return buffer[position - 1]; 
        }

        // Summary:
        //     Sets the position within the current stream to the specified value.
        //
        // Parameters:
        //   offset:
        //     The new position within the stream. This is relative to the loc parameter,
        //     and can be positive or negative.
        //
        //   loc:
        //     A value of type System.IO.SeekOrigin, which acts as the seek reference point.
        //
        // Returns:
        //     The new position within the stream, calculated by combining the initial reference
        //     point and the offset.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     Seeking is attempted before the beginning of the stream.
        //
        //   System.ArgumentOutOfRangeException:
        //     offset is greater than System.Int32.MaxValue.
        //
        //   System.ArgumentException:
        //     There is an invalid System.IO.SeekOrigin. -or-offset caused an arithmetic
        //     overflow.
        //
        //   System.ObjectDisposedException:
        //     The current stream instance is closed.
        public override long Seek(long offset, SeekOrigin loc)
        {
            long savedPosition = position;

            switch (loc)
            {
                case SeekOrigin.Begin:
                    {
                        if (offset < 0)
                        {
                            throw new IOException("offset or count is negative");
                        }
                        position = offset;
                    }
                    break;

                case SeekOrigin.Current:
                    {
                        position += offset;
                    }
                    break;

                case SeekOrigin.End:
                    {
                        if (offset > 0)
                        {
                            throw new IOException("offset or count is negative");
                        }
                        position += offset;
                    }
                    break;
            }

            if (position >= Capacity)
            {
                position = savedPosition;

                throw new IOException("offset is larger thant capacity");
            }

            return position;
        }

        // Summary: Disable SetLength
        public override void SetLength(long value)
        {
            throw new NotImplementedException("SetLength cannot be used");
        }

        // Summary:
        //     Writes a block of bytes to the current stream using data read from a buffer.
        //
        // Parameters:
        //   buffer:
        //     The buffer to write data from.
        //
        //   offset:
        //     The zero-based byte offset in buffer at which to begin copying bytes to the
        //     current stream.
        //
        //   count:
        //     The maximum number of bytes to write.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     buffer is null.
        //
        //   System.NotSupportedException:
        //     The stream does not support writing. For additional information see System.IO.Stream.CanWrite.-or-
        //     The current position is closer than count bytes to the end of the stream,
        //     and the capacity cannot be modified.
        //
        //   System.ArgumentException:
        //     offset subtracted from the buffer length is less than count.
        //
        //   System.ArgumentOutOfRangeException:
        //     offset or count are negative.
        public override void Write(byte[] buf, int offset, int count)
        {
            // BlockCopy checks arguments

            EnsureCapacity(count);

            System.Buffer.BlockCopy(buf, offset, this.buffer, (int)position, count);

            position += (long)count;
        }

        // Summary:
        //    Writes a byte to the current stream at the current position.
        //
        // Parameters:
        //   value:
        //     The byte to write.
        //
        // Exceptions:
        //   System.NotSupportedException:
        //     The stream does not support writing. For additional information see System.IO.Stream.CanWrite.-or-
        //     The current position is at the end of the stream, and the capacity cannot
        //     be modified.
        //
        //   System.ObjectDisposedException:
        //     The current stream is closed.
        public override void WriteByte(byte value)
        {
            EnsureCapacity(1);

            buffer[position++] = value;
        }

        // Summary:
        //     Writes the entire contents of this memory stream to another stream.
        //
        // Parameters:
        //   stream:
        //     The stream to write this memory stream to.
        //
        // Exceptions:
        //   System.ArgumentNullException: stream is null.
        public virtual void WriteTo(Stream stream)
        {
            stream.Write(buffer, 0, (int)position + 1);
        }

        public void WriteUInt32(uint value)
        {
            WriteByte((byte)(value & 0xFF));
            WriteByte((byte)(value >> 8 & 0xFF));
            WriteByte((byte)(value >> 16 & 0xFF));
            WriteByte((byte)(value >> 24 & 0xFF));
        }

        public void ReadUInt32(out uint value)
        {
            int v1 = ReadByte();
            int v2 = ReadByte();
            int v3 = ReadByte();
            int v4 = ReadByte();

            value = (uint)(v4 << 24 | v3 << 16 | v2 << 8 | v1);
        }

        // Summary:
        //      Truncate bytes in buffer from front by moving the buffer content
        //
        public void TruncateFront(long bytes)
        {
            System.Buffer.BlockCopy(buffer, (int)bytes, buffer, 0, (int)(buffer.Length - bytes));

            position -= bytes;
        }

        // Summary: Get internal buffer
        public byte[] GetBuffer()
        {
            return buffer;
        }

        // Summary: 
        //  Check room for required bytes to write and resize if required
        private void EnsureCapacity(long bytes)
        {
            long requiredBytes = position + bytes;

            if (requiredBytes < buffer.Length)
            {
                return;
            }

            long newCapacity = buffer.Length;
            
            System.Diagnostics.Debug.Assert(newCapacity == Capacity);

            while (newCapacity <= requiredBytes)
            {
                newCapacity += DefaultIncreaseSize;
            }

            System.Diagnostics.Debug.Assert(newCapacity > requiredBytes);

            byte[] newBuffer = new byte[newCapacity];
            System.Buffer.BlockCopy(buffer, 0, newBuffer, 0, buffer.Length);

            buffer = newBuffer;
        } 
    }
}
