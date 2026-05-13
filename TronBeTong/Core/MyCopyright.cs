using System;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace TronBeTongV3.Core
{
    public class MyCopyright
    {
        public static string? GetPCFootPrint()
        {
            string? driverLetter = Path.GetPathRoot(AppDomain.CurrentDomain.BaseDirectory)?.TrimEnd('\\');

            if (driverLetter != null)
            {
                string query = $"ASSOCIATORS OF {{Win32_LogicalDisk.DeviceID='{driverLetter}'}} WHERE AssocClass = Win32_LogicalDiskToPartition";

                using ManagementObjectSearcher searcher = new(query);
                foreach (var p in searcher.Get())
                {
                    string? partDeviceId = p["DeviceID"].ToString();
                    if (partDeviceId != null)
                    {
                        string diskQuery = $"ASSOCIATORS OF {{Win32_DiskPartition.DeviceID='{partDeviceId}'}} WHERE AssocClass = Win32_DiskDriveToDiskPartition";

                        using var diskSearcher = new ManagementObjectSearcher(diskQuery);
                        foreach (ManagementObject disk in diskSearcher.Get())
                        {
                            return disk["SerialNumber"]?.ToString();
                        }
                    }
                }
            }

            return null;
        }

        public static string ComputeSha256(string s)
        {
            StringBuilder sb = new();

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(s);
                byte[] hashBytes = sha256.ComputeHash(bytes);

                int n = 1;
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("X2"));
                    if (n < 16 && n % 4 == 0) sb.Append('-');
                    n++;
                }
            }

            return sb.ToString();
        }

        public static string ComputeMD5(string s)
        {
            StringBuilder sb = new();

            using (MD5 md5 = MD5.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(s);
                byte[] hashBytes = md5.ComputeHash(bytes);

                int n = 1;
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("X2"));
                    if (n < 16 && n % 4 == 0) sb.Append('-');
                    n++;
                }
            }

            return sb.ToString();
        }
    }
}
