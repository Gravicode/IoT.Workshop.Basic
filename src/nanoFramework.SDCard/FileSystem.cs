using nanoFramework.SDCard.Helpers;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using TinyFatFS;

namespace nanoFramework.SDCard {
    public static class FileSystem {
        private static readonly IDictionary mounted = new Hashtable();

        const int hdc = 1; //dummy hdc
        
        public static IDriveProvider Mount(int SpiBusId,int CsPin, int DummyCsPin)
        {
            if (FileSystem.mounted.Contains(hdc))
                throw new InvalidOperationException("Already mounted");

            FatFileSystem.SetSPIConfig(SpiBusId,CsPin, DummyCsPin);

            FatFileSystem.FatFS fs=null;
            FatFileSystem.FileResult res;

            if (fs == null) fs = new FatFileSystem.FatFS();

            res = FatFileSystem.Current.MountDrive(ref fs, "", 1);     /* Give a work area to the default drive */
            res.ThrowIfError();

            Debug.WriteLine("Drive successfully mounted");


            var drive = new NativeDriveProvider(ref fs);

            var provider = DriveInfo.RegisterDriveProvider(drive);

            mounted[hdc] = drive;

            return drive;
        }

        static string ReformatPath(string sPath)
        {
            if (mounted.Count > 0)
            {
                var driveName = ((NativeDriveProvider)mounted[hdc]).Name;
                return Strings.Replace(sPath, driveName, "/");
            }
            return sPath;
        }

        public static bool Unmount()
        {
            try
            {
                if (!FileSystem.mounted.Contains(hdc))
                    throw new InvalidOperationException("Not mounted");

                var drive = (IDriveProvider)FileSystem.mounted[hdc];

                FileSystem.mounted.Remove(hdc);

                DriveInfo.DeregisterDriveProvider(drive);


                Debug.WriteLine("Drive successfully unmounted");
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        private class NativeDriveProvider : IDriveProvider
        {
            static FatFileSystem.FatFS fs;
            static FatFileSystem.FileResult res;
            public NativeDriveProvider(ref FatFileSystem.FatFS fileSystem)
            {
                fs = fileSystem;
            }
            public NativeDriveProvider()
            {
                if (fs == null) fs = new FatFileSystem.FatFS();
            }
            private bool initialized;

            public string Name { get; set; } = "SD";

            public DriveType DriveType => DriveType.Removable;

            public string DriveFormat
            {
                get
                {
                    switch (fs.fs_type)
                    {
                        case FatFileSystem.FS_EXFAT:
                            return "EXFAT";
                        case FatFileSystem.FS_FAT12:
                            return "FAT12";
                        case FatFileSystem.FS_FAT16:
                            return "FAT16";
                        case FatFileSystem.FS_FAT32:
                            return "FAT32";
                    }
                    return "";
                }
            }

            public bool IsReady => fs != null;

            public long AvailableFreeSpace {
                get
                {
                    uint fre_clust = 0;
                    uint fre_sect;

                    /* Get volume information and free clusters of drive 1 */
                    res = FatFileSystem.Current.GetFreeSpace("0:", ref fre_clust, ref fs);
                    if (res != TinyFatFS.FatFileSystem.FileResult.Ok)
                    {
                        Debug.WriteLine($"An error occured. {res.ToString()}");
                        return 0;
                    };

                    /* Get total sectors and free sectors */
                    //tot_sect = (fs.n_fatent - 2) * fs.csize;
                    fre_sect = fre_clust * fs.csize;
                    return fre_sect; //KB = fre_sect/2 
                }
            }

            public long TotalFreeSpace
            {
                get
                {
                    uint fre_clust = 0;
                    uint fre_sect;

                    /* Get volume information and free clusters of drive 1 */
                    res = FatFileSystem.Current.GetFreeSpace("0:", ref fre_clust, ref fs);
                    if (res != TinyFatFS.FatFileSystem.FileResult.Ok)
                    {
                        Debug.WriteLine($"An error occured. {res.ToString()}");
                        return 0;
                    };

                    /* Get total sectors and free sectors */
                    //tot_sect = (fs.n_fatent - 2) * fs.csize;
                    fre_sect = fre_clust * fs.csize;
                    return fre_sect; //KB = fre_sect/2 
                }
            }

            public long TotalSize
            {

                get
                {
                    uint fre_clust = 0;
                    uint  tot_sect;

                    /* Get volume information and free clusters of drive 1 */
                    res = FatFileSystem.Current.GetFreeSpace("0:", ref fre_clust, ref fs);
                    if (res != TinyFatFS.FatFileSystem.FileResult.Ok)
                    {
                        Debug.WriteLine($"An error occured. {res.ToString()}");
                        return 0;
                    };

                    /* Get total sectors and free sectors */
                    tot_sect = (fs.n_fatent - 2) * fs.csize;
                    //fre_sect = fre_clust * fs.csize;
                    return tot_sect; //KB = tot_sect/2 
                }

            }

            public string VolumeLabel
            {
                get
                {
                    
                    string path = "/";
                    FatFileSystem.FileInfo fno = new FatFileSystem.FileInfo();
                    FatFileSystem.DirectoryObject dir = new FatFileSystem.DirectoryObject();
                    byte[] buff = new byte[256];
                    buff = path.ToNullTerminatedByteArray();

                    res = FatFileSystem.Current.OpenDirectory(ref dir, buff);                      /* Open the directory */
                    if (res == FatFileSystem.FileResult.Ok)
                    {
                        res = FatFileSystem.Current.ReadDirectoryEntry(ref dir, ref fno);           /* Read a directory item */

                        //res = FatFileSystem.Current.ReadVolumeLabel(ref dir);
                        //return dir.fn.ToStringNullTerminationRemoved();
                        if (res == FatFileSystem.FileResult.Ok)
                        {
                            return fno.fileName.ToStringNullTerminationRemoved();
                        }
                    }
                    return string.Empty;
                }
            }

            

            public void CreateDirectory(string path)
            {
                path = ReformatPath(path);
                res = FatFileSystem.Current.CreateDirectory(path);
                if (res != TinyFatFS.FatFileSystem.FileResult.Exists) res.ThrowIfError();

            }

            public void Delete(string path)
            {
                path = ReformatPath(path);
                res = FatFileSystem.Current.DeleteFileOrDirectory(path);     /* Give a work area to the default drive */
                res.ThrowIfError();

            }

            public IFileSystemEntryFinder Find(string path, string searchPattern)=> new NativeFileSystemEntryFinder(ReformatPath(path), searchPattern);
            
            public FileAttributes GetAttributes(string path)
            {
                path = ReformatPath(path);

                FatFileSystem.FileInfo fno = new FatFileSystem.FileInfo();

                res = FatFileSystem.Current.GetFileAttributes(path, ref fno);
                switch (res)
                {

                    case FatFileSystem.FileResult.Ok:
                        Debug.WriteLine($"Size: {fno.fileSize}");
                        Debug.WriteLine(String.Format("Timestamp: {0}/{1}/{2}, {3}:{4}",
                               (fno.fileDate >> 9) + 1980, fno.fileDate >> 5 & 15, fno.fileDate & 31,
                               fno.fileTime >> 11, fno.fileTime >> 5 & 63));
                        Debug.WriteLine(String.Format("Attributes: {0}{1}{2}{3}{4}",
                               (fno.fileAttribute & FatFileSystem.AM_DIR) > 0 ? 'D' : '-',
                               (fno.fileAttribute & FatFileSystem.AM_RDO) > 0 ? 'R' : '-',
                               (fno.fileAttribute & FatFileSystem.AM_HID) > 0 ? 'H' : '-',
                               (fno.fileAttribute & FatFileSystem.AM_SYS) > 0 ? 'S' : '-',
                               (fno.fileAttribute & FatFileSystem.AM_ARC) > 0 ? 'A' : '-'));
                        if((fno.fileAttribute & FatFileSystem.AM_DIR) > 0)return FileAttributes.Directory;
                        if ((fno.fileAttribute & FatFileSystem.AM_RDO) > 0) return FileAttributes.ReadOnly;
                        if ((fno.fileAttribute & FatFileSystem.AM_HID) > 0) return FileAttributes.Hidden;
                        if ((fno.fileAttribute & FatFileSystem.AM_SYS) > 0) return FileAttributes.System;
                        if ((fno.fileAttribute & FatFileSystem.AM_ARC) > 0) return FileAttributes.Archive;

                        break;

                    case FatFileSystem.FileResult.FileNotExist:
                        Debug.WriteLine("File does not exist");
                        return FileAttributes.NotExists;
                        //throw new Exception("File does not exist");
                        break;

                    default:
                        Debug.WriteLine($"An error occured. {res.ToString()}");
                        throw new Exception("error:"+res);
                        break;
                }
                return FileAttributes.Normal;
            }

            public FileSystemEntry GetFileSystemEntry(string path)
            {
                path = ReformatPath(path);

                FatFileSystem.FileInfo fno = new FatFileSystem.FileInfo();

                res = FatFileSystem.Current.GetFileAttributes(path, ref fno);
                switch (res)
                {

                    case FatFileSystem.FileResult.Ok:
                        var fileEntry = new FileSystemEntry()
                        {
                            CreationTime = new DateTime((int)(fno.fileDate >> 9) + 1980, (int)fno.fileDate >> 5 & 15, (int)fno.fileDate & 31,
                               (int)fno.fileTime >> 11, (int)fno.fileTime >> 5 & 63, 0),
                            FileName = fno.fileName.ToStringNullTerminationRemoved(),
                            LastAccessTime = DateTime.UtcNow,
                            LastWriteTime = DateTime.UtcNow,
                            Size = fno.fileSize

                        };

                        return fileEntry;

                    case FatFileSystem.FileResult.FileNotExist:
                        Debug.WriteLine("File does not exist");
                        break;

                    default:
                        Debug.WriteLine($"An error occured. {res.ToString()}");
                        break;
                }
                return null;
            }

            public void Initialize(string name)
            {
                if (this.initialized) throw new InvalidOperationException();

                this.initialized = true;
                this.Name = name;
            }

            public bool Move(string source, string destination)
            {
                throw new NotImplementedException();
            }

            public IFileStream OpenFile(string path, int bufferSize)=>new NativeFileStream(ReformatPath(path), bufferSize);

            public void SetAttributes(string path, FileAttributes attributes)
            {
                //not available in library
                throw new NotImplementedException();
            }
        }
        private class NativeFileStream : IFileStream, IDisposable
        {
            MemoryStream ms;
            
            FatFileSystem.FileResult res;
            FatFileSystem.FileObject fileObject;
            string path;
            int bufferSize;
            public NativeFileStream(string path, int bufferSize)
            {
                path = ReformatPath(path);
                fileObject = new FatFileSystem.FileObject();
                this.path = path;
                this.bufferSize = bufferSize;
                FatFileSystem.FileInfo fno = new FatFileSystem.FileInfo();

                res = FatFileSystem.Current.GetFileAttributes(path, ref fno);
                //file is not exists
                if (res != FatFileSystem.FileResult.Ok)
                {
                    canWrite = true;
                    canRead = true;
                    canSeek = true;
                    res = FatFileSystem.Current.OpenFile(ref fileObject, path, FatFileSystem.FA_WRITE | FatFileSystem.FA_CREATE_ALWAYS);
                    ms = new MemoryStream();
                }
                else
                {
                    canWrite = true;
                    canRead = true;
                    canSeek = true;
                    res = FatFileSystem.Current.OpenFile(ref fileObject, path, FatFileSystem.FA_READ | FatFileSystem.FA_WRITE);
                    //put data on memory
                    var newPayload = new byte[fno.fileSize];
                    res = FatFileSystem.Current.ReadFile(ref fileObject, ref newPayload, fno.fileSize, ref bw);    /* Read data from file */
                    if (res == FatFileSystem.FileResult.Ok)
                    {
                        ms = new MemoryStream(newPayload);
                    }
                }
                res.ThrowIfError();

            }
            private bool canWrite;
            public bool CanWrite => canWrite;

            private bool canRead;
            public bool CanRead => canRead;

            private bool canSeek;
            public bool CanSeek => canSeek;

            private bool isClosed = false;
            public bool IsClosed => isClosed;

            public long Length { get => ms.Length; set => throw new NotImplementedException(); }

            public void Close()
            {
                if (!IsClosed)
                {
                    res = FatFileSystem.Current.CloseFile(ref fileObject);                              /* Close the file */
                    res.ThrowIfError();
                    isClosed = true;
                }
            }

            public void Dispose()
            {
                //do nothing
                
                fileObject = null;
                //throw new NotImplementedException();
            }

            public void Flush()
            {
                if (CanWrite)
                {
                    var size = bw;
                    bw = 0;
                    res = FatFileSystem.Current.WriteFile(ref fileObject, ms.ToArray(), size, ref bw);    /* Write data to the file */
                    res.ThrowIfError();

                    Close();
                    //res = FatFileSystem.Current.CloseFile(ref fileObject);   /* Close the file */
                    //res.ThrowIfError();
                }
                //throw new NotImplementedException();
            }
            //file pointer
            uint bw = 0;
            public int Read(byte[] buffer, int offset, int count, TimeSpan timeout)
            {
                var x = ms.Read(buffer, offset, count);
                bw = (uint)offset + (uint)x;
                if (bw < 0) bw = 0;
                if (bw >= ms.Length) bw =(uint) ms.Length - 1;
                return x;
                //var msg = Encoding.UTF8.GetString(buffer, 0, count);
                //Debug.WriteLine($"{msg}");
            }

            public long Seek(long offset, SeekOrigin origin)
            {
                switch (origin)
                {
                    case SeekOrigin.Begin:
                        bw = (uint)offset;
                        if (bw >= ms.Length) bw = (uint)ms.Length-1;
                        break;
                    case SeekOrigin.Current:
                        bw = bw + (uint)offset;
                        if (bw >= ms.Length) bw = (uint)ms.Length-1;
                        break;
                    case SeekOrigin.End:
                        bw = (uint)ms.Length - (uint)offset;
                        if (bw < 0) bw = 0;
                        break;
                }
                return bw;
            }

            public int Write(byte[] buffer, int offset, int count, TimeSpan timeout)
            {
                ms.Write(buffer, offset, count);
                bw = (uint)offset + (uint)count;
                return count - offset;
            }
        }
        /*
        public static IDriveProvider Mount(IntPtr hdc) {
            if (FileSystem.mounted.Contains(hdc))
                throw new InvalidOperationException("Already mounted");

            var drive = new NativeDriveProvider();

            var provider = DriveInfo.RegisterDriveProvider(drive);

            FileSystem.Initialize(hdc, provider.Name);

            mounted[hdc] = drive;

            return drive;
        }

        public static bool Unmount(IntPtr hdc) {
            if (!FileSystem.mounted.Contains(hdc))
                throw new InvalidOperationException("Not mounted");

            var drive = (IDriveProvider)FileSystem.mounted[hdc];

            FileSystem.mounted.Remove(hdc);

            DriveInfo.DeregisterDriveProvider(drive);

            return FileSystem.Uninitialize(hdc);
        }

        public static void Flush(IntPtr hdc) => FileSystem.FlushAll(hdc);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern static void FlushAll(IntPtr nativeProvider);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern static void Initialize(IntPtr nativeProvider, string name);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern static bool Uninitialize(IntPtr nativeProvider);

        private class NativeDriveProvider : IDriveProvider {
            private bool initialized;

            public extern DriveType DriveType { [MethodImpl(MethodImplOptions.InternalCall)] get; }

            public extern string DriveFormat { [MethodImpl(MethodImplOptions.InternalCall)] get; }

            public extern bool IsReady { [MethodImpl(MethodImplOptions.InternalCall)] get; }

            public extern long AvailableFreeSpace { [MethodImpl(MethodImplOptions.InternalCall)] get; }

            public extern long TotalFreeSpace { [MethodImpl(MethodImplOptions.InternalCall)] get; }

            public extern long TotalSize { [MethodImpl(MethodImplOptions.InternalCall)] get; }

            public extern string VolumeLabel { [MethodImpl(MethodImplOptions.InternalCall)] get; }

            public string Name { get; private set; }

            [MethodImpl(MethodImplOptions.InternalCall)]
            public extern void CreateDirectory(string path);

            [MethodImpl(MethodImplOptions.InternalCall)]
            public extern void Delete(string path);

            [MethodImpl(MethodImplOptions.InternalCall)]
            public extern FileAttributes GetAttributes(string path);

            [MethodImpl(MethodImplOptions.InternalCall)]
            public extern FileSystemEntry GetFileSystemEntry(string path);

            [MethodImpl(MethodImplOptions.InternalCall)]
            public extern bool Move(string source, string destination);

            [MethodImpl(MethodImplOptions.InternalCall)]
            public extern void SetAttributes(string path, FileAttributes attributes);

            public IFileSystemEntryFinder Find(string path, string searchPattern) => new NativeFileSystemEntryFinder(path, searchPattern);

            public IFileStream OpenFile(string path, int bufferSize) => new NativeFileStream(path, bufferSize);

            public void Initialize(string name) {
                if (this.initialized) throw new InvalidOperationException();

                this.initialized = true;
                this.Name = name;
            }
        }

        private class NativeFileStream : IFileStream, IDisposable {
#pragma warning disable CS0169
            private IntPtr impl;

#pragma warning restore CS0169
            ~NativeFileStream() => this.Dispose();

            [MethodImpl(MethodImplOptions.InternalCall)]
            public extern NativeFileStream(string path, int bufferSize);

            [MethodImpl(MethodImplOptions.InternalCall)]
            public extern void Dispose();

            public extern bool CanWrite { [MethodImpl(MethodImplOptions.InternalCall)] get; }

            public extern bool CanRead { [MethodImpl(MethodImplOptions.InternalCall)] get; }

            public extern bool CanSeek { [MethodImpl(MethodImplOptions.InternalCall)] get; }

            public extern long Length { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

            [MethodImpl(MethodImplOptions.InternalCall)]
            public extern void Close();

            [MethodImpl(MethodImplOptions.InternalCall)]
            public extern void Flush();

            [MethodImpl(MethodImplOptions.InternalCall)]
            public extern int Read(byte[] buffer, int offset, int count, TimeSpan timeout);

            [MethodImpl(MethodImplOptions.InternalCall)]
            public extern long Seek(long offset, SeekOrigin origin);

            [MethodImpl(MethodImplOptions.InternalCall)]
            public extern int Write(byte[] buffer, int offset, int count, TimeSpan timeout);
        }*/
    }
}
