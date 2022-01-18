using nanoFramework.SDCard.Helpers;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TinyFatFS;

namespace nanoFramework.SDCard {
    public interface IFileSystemEntryFinder {
        FileSystemEntry GetNext();
        void Close();
    }
    internal class NativeFileSystemEntryFinder : IFileSystemEntryFinder, IDisposable
    {
        public void Close()
        {
            FatFileSystem.Current.CloseDirectory(ref dir);
        }

        public void Dispose()
        {
            //do nothing
            //throw new NotImplementedException();
        }

        public FileSystemEntry GetNext()
        {
            if (CanSeek)
            {
                var typeObj = string.Empty;
                res = FatFileSystem.Current.ReadDirectoryEntry(ref dir, ref fno);           /* Read a directory item */
                if (res != FatFileSystem.FileResult.Ok || fno.fileName[0] == 0) return null;   /* Break on error or end of dir */
                if ((fno.fileAttribute & FatFileSystem.AM_DIR) > 0 && !((fno.fileAttribute & FatFileSystem.AM_SYS) > 0 || (fno.fileAttribute & FatFileSystem.AM_HID) > 0))
                {
                    typeObj = "dir";
                    /* It is a directory */
                    //var newpath = path + "/" + fno.fileName.ToStringNullTerminationRemoved();
                    //Debug.WriteLine($"Directory: {path}/{fno.fileName.ToStringNullTerminationRemoved()}");
                    //no recursion, so add comment
                    //res = ScanFiles(newpath);                    /* Enter the directory */
                    //if (res != FatFileSystem.FileResult.Ok) break;
                }
                else
                {
                    typeObj = "file";
                    /* It is a file. */
                    //Debug.WriteLine($"File: {path}/{fno.fileName.ToStringNullTerminationRemoved()}");
                }
                var fName = fno.fileName.ToStringNullTerminationRemoved();
                var fileEntry = new FileSystemEntry()
                {
                    CreationTime = new DateTime((int)(fno.fileDate >> 9) + 1980, (int)fno.fileDate >> 5 & 15, (int)fno.fileDate & 31,
                          (int)fno.fileTime >> 11, (int)fno.fileTime >> 5 & 63, 0),
                    FileName = fName,
                    LastAccessTime = DateTime.UtcNow,
                    LastWriteTime = DateTime.UtcNow,
                    Size = fno.fileSize,
                    Attributes = (System.IO.FileAttributes)fno.fileAttribute
                   
                };
                if(searchRegex.IsMatch(fName))
                   return fileEntry;
            }
            return null;
        }

        public bool CanSeek { get; set; } = false;
        FatFileSystem.FileResult res;
        FatFileSystem.FileInfo fno = null;
        FatFileSystem.DirectoryObject dir = null;
        string path;
        Wildcard searchRegex;
        public NativeFileSystemEntryFinder(string path, string searchPattern= "*.*")
        {
            fno = new FatFileSystem.FileInfo();
            dir = new FatFileSystem.DirectoryObject();
            byte[] buff = new byte[256];
            this.path = path;
            buff = path.ToNullTerminatedByteArray();

            res = FatFileSystem.Current.OpenDirectory(ref dir, buff);                      /* Open the directory */
            if (res == FatFileSystem.FileResult.Ok)
            {
                CanSeek = true;

                searchRegex = new Wildcard(searchPattern, RegexOptions.IgnoreCase);



            }
        }
    }
    /// <summary>
    /// Represents a wildcard running on the
    /// <see cref="System.Text.RegularExpressions"/> engine.
    /// </summary>
    public class Wildcard : Regex
    {
        /// <summary>
        /// Initializes a wildcard with the given search pattern.
        /// </summary>
        /// <param name="pattern">The wildcard pattern to match.</param>
        public Wildcard(string pattern)
         : base(WildcardToRegex(pattern))
        {
        }

        /// <summary>
        /// Initializes a wildcard with the given search pattern and options.
        /// </summary>
        /// <param name="pattern">The wildcard pattern to match.</param>
        /// <param name="options">A combination of one or more
        /// <see cref="System.Text.RegexOptions"/>.</param>
        public Wildcard(string pattern, RegexOptions options)
         : base(WildcardToRegex(pattern), options)
        {
        }

        /// <summary>
        /// Converts a wildcard to a regex.
        /// </summary>
        /// <param name="pattern">The wildcard pattern to convert.</param>
        /// <returns>A regex equivalent of the given wildcard.</returns>
        public static string WildcardToRegex(string pattern)
        {
            var tmp = "^" + Regex.Escape(pattern);
            tmp = Strings.Replace(tmp, "\\*", ".*");
            tmp = Strings.Replace(tmp, "\\?", ".") + "$";
            return tmp;
        }
    }
    /*
    internal class NativeFileSystemEntryFinder : IFileSystemEntryFinder, IDisposable {
#pragma warning disable CS0169
        IntPtr implPtr;

#pragma warning restore CS0169
        ~NativeFileSystemEntryFinder() => this.Dispose();

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern NativeFileSystemEntryFinder(string path, string searchPattern);

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern void Dispose();

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern void IFileSystemEntryFinder.Close();

        [MethodImpl(MethodImplOptions.InternalCall)]
        extern FileSystemEntry IFileSystemEntryFinder.GetNext();

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern FileSystemEntry GetFileInfo(string path);
    }*/
}