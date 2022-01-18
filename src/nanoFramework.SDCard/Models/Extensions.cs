//using GHIElectronics.TinyCLR.Devices.Gpio;
using System;
using System.Device.Gpio;
using System.Text;
using static TinyFatFS.FatFileSystem;

namespace TinyFatFS
{
    static public class Extensions
    {
        public static void ToHi(this GpioPin pin)
        {
            pin.Write(PinValue.High);
        }

        public static void ToLow(this GpioPin pin)
        {
            pin.Write(PinValue.Low);
        }

        public static bool IsHi(this GpioPin pin)
        {
            return pin.Read() == PinValue.High;
        }

        public static byte[] Slice(this byte[] arr, uint indexFrom, uint count)
        {
            uint length = count;
            var result = new byte[length];
            Array.Copy(arr, (int)indexFrom, result, 0, (int) length);

            return result;
        }

        public static byte[] SubArray(this byte[] arr, uint indexFrom)
        {
            int length = arr.Length - (int) indexFrom;
            var result = new byte[length];
            Array.Copy(arr, (int)indexFrom, result, 0, (int)length);

            return result;
        }

        public static byte[] ToNullTerminatedByteArray(this string str)
        {
            var arr = Encoding.UTF8.GetBytes(str);
            var result = new byte[arr.Length + 1];
            result[result.Length-1] = 0;
            Array.Copy(arr, result, arr.Length);
            return result;
        }

        public static byte[] ToByteArray(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        public static string ToStringNullTerminationRemoved(this byte[] buffer)
        {
            var value = Encoding.UTF8.GetString(buffer,0,buffer.Length);
            return value.TrimEnd('\0');
        }

        public static void ThrowIfError (this FileResult result)
        {
            string msg;       
            if (result != FileResult.Ok) {
                switch (result)
                {
                    case FileResult.Ok:
                        msg = "OK";
                        break;
                    case FileResult.DiskError:
                        msg = "Disk Error";
                        break;
                    case FileResult.InternalError:
                        msg = "Internal Error";
                        break;
                    case FileResult.NotReady:
                        msg = "Disk Not Ready";
                        break;
                    case FileResult.FileNotExist:
                        msg = "No File Exists";
                        break;
                    case FileResult.PathNotFound:
                        msg = "Path Not Found";
                        break;
                    case FileResult.InvalidPathName:
                        msg = "Invalid Path Name";
                        break;
                    case FileResult.AccessDenied:
                        msg = "Access Denied";
                        break;
                    case FileResult.Exists:
                        msg = "Already Exists";
                        break;
                    case FileResult.InvalidObject:
                        msg = "Invalid Object";
                        break;
                    case FileResult.WriteProtected:
                        msg = "Disk is Write Protected";
                        break;
                    case FileResult.InvalidDrive:
                        msg = "Invalid Drive";
                        break;
                    case FileResult.NotEnabled:
                        msg = "Not Enabled";
                        break;
                    case FileResult.NoFileSystem:
                        msg = "No File System";
                        break;
                    case FileResult.MKFSAborted:
                        msg = "MKFS Aborted";
                        break;
                    case FileResult.TimeOut:
                        msg = "Timeout";
                        break;
                    case FileResult.Locked:
                        msg = "Disk is Locked";
                        break;
                    case FileResult.NotEnoughCore:
                        msg = "Not Enough Core";
                        break;
                    case FileResult.TooManyOpenFiles:
                        msg = "Too Many Open Files";
                        break;
                    case FileResult.InvalidParameter:
                        msg = "Invalid Parameter";
                        break;
                    default:
                        msg = "Undefined";
                        break;
                }
                throw new ApplicationException($"Error: {msg}");
            }
        }

       
    }
}
