using System;
using System.Text;

namespace nanoFramework.Media
{
    public enum UnmanagedBufferLocation
    {
        ManagedMemory,
        UnmanagedMemory,
    }
    public class UnmanagedBuffer : IDisposable
    {
        //private IntPtr ptr;
        private byte[] mem;
        private bool disposed;

        public byte[] Bytes => this.mem;

        public UnmanagedBuffer(int length)
          : this(length, UnmanagedBufferLocation.UnmanagedMemory)
        {
        }

        public UnmanagedBuffer(int length, UnmanagedBufferLocation location)
        {
            if (location != UnmanagedBufferLocation.UnmanagedMemory)
                throw new ArgumentOutOfRangeException(nameof(location));
            //this.ptr = Memory.UnmanagedMemory.Allocate((long)length);
            this.mem = new byte[length]; //Memory.UnmanagedMemory.ToBytes(this.ptr, (long)length);
        }

        ~UnmanagedBuffer() => this.Dispose(false);

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize((object)this);
        }

        private void Dispose(bool fDisposing)
        {
            if (this.disposed)
                return;
            //Memory.UnmanagedMemory.Free(this.ptr);
            //this.ptr = IntPtr.Zero;
            this.mem = (byte[])null;
            this.disposed = true;
        }
    }
}
