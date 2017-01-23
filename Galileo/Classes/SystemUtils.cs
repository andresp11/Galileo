using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;

namespace OMR.helpers
{
    public class SystemUtils
    {
        static bool is64BitProcess = (IntPtr.Size == 8);
        static bool is64BitOperatingSystem = is64BitProcess || InternalCheckIsWow64();

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process(
            [In] IntPtr hProcess,
            [Out] out bool wow64Process
        );
        public static bool checkGS()
        {
            string gsDll = "gsdll32.dll";
/*            if (SystemUtils.InternalCheckIsWow64())
                gsDll = "gsdll64.dll";

            if (!File.Exists(gsDll))
            {
                if (tryGSCopy()) return true;
                OMR.Forms.gsDownloader downloader = new Forms.gsDownloader();
                downloader.ShowDialog();
                if (downloader.DialogResult == DialogResult.Abort)
                {
                    if (InternalCheckIsWow64())
                    { File.Delete("redist\\ghostscript\\GPL Ghost Script 9.18 x64.exe"); }
                    else
                    { File.Delete("redist\\ghostscript\\GPL Ghost Script 9.18 x32.exe"); }
                }
                return File.Exists(gsDll);
            }
            else*/
                return true;
        }
        public static bool tryGSCopy()
        {
            string expectedFileAddress = "";
            try
            {
                expectedFileAddress = (string)RegistryAccess.read_GS_DLL_path();
                string target = "gsdll32.dll";
                if (InternalCheckIsWow64())
                {
                    target = "gsdll64.dll";
                }
                    File.Copy(expectedFileAddress, "gsdll64.dll"); 
            }
            catch { return false; }

            if (InternalCheckIsWow64())
            {
                return File.Exists("gsdll64.dll");
            }
            else
            {
                return File.Exists("gsdll32.dll");
            }
        }

        public static bool InternalCheckIsWow64()
        {
            if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) ||
                Environment.OSVersion.Version.Major >= 6)
            {
                using (Process p = Process.GetCurrentProcess())
                {
                    bool retVal;
                    if (!IsWow64Process(p.Handle, out retVal))
                    {
                        return false;
                    }
                    return retVal;
                }
            }
            else
            {
                return false;
            }
        }
    }
    public class RegistryAccess
    {
        public static void WriteRegStr(string AppName, string ValueName, object value)
        {
            RegistryKey MyKey = Registry.CurrentUser.CreateSubKey("Software\\" + AppName, RegistryKeyPermissionCheck.ReadWriteSubTree);
            try
            {
                MyKey.SetValue(ValueName, Convert.ToInt32(value), RegistryValueKind.DWord);
            }
            catch
            {
                MyKey.SetValue(ValueName, value.ToString(), RegistryValueKind.String);
            }
        }
        public static void DeleteReg(string AppName, string ValueName)
        {
            RegistryKey MyKey = Registry.CurrentUser.CreateSubKey("Software\\" + AppName, RegistryKeyPermissionCheck.ReadWriteSubTree);
            try
            {
                MyKey.DeleteValue(ValueName);
            }
            catch
            {
            }
        }

        public static string read_GS_DLL_path()
        {
            if (SystemUtils.InternalCheckIsWow64())
                return GetRegKey64(RegHive.HKEY_LOCAL_MACHINE, "SOFTWARE\\GPL Ghostscript\\9.18", "GS_DLL");
            else
                return (string)Registry.LocalMachine.OpenSubKey("Software\\GPL Ghostscript\\9.18", false).GetValue("GS_DLL");
        }

        public static object HKCU_Software_ReadReg(string AppName, string ValueName)
        {
            RegistryKey MyKey;
            try
            {
                MyKey = Registry.CurrentUser.OpenSubKey("Software\\" + AppName, RegistryKeyPermissionCheck.ReadWriteSubTree);
                if (MyKey == null)
                    throw new NullReferenceException();
            }
            catch
            {
                MyKey = Registry.CurrentUser.CreateSubKey("Software\\" + AppName, RegistryKeyPermissionCheck.ReadWriteSubTree);
            }
            try
            {
                object obj = MyKey.GetValue(ValueName) ;
                if (obj == null)
                    throw new Exception();
                return obj;
            }
            catch
            {
                WriteRegStr(AppName, ValueName, false);
                object obj = MyKey.GetValue(ValueName);
                return obj;
            }
        }
        #region Member Variables
        #region Read 64bit Reg from 32bit app
        [DllImport("Advapi32.dll")]
        static extern uint RegOpenKeyEx(
            UIntPtr hKey,
            string lpSubKey,
            uint ulOptions,
            int samDesired,
            out int phkResult);

        [DllImport("Advapi32.dll")]
        static extern uint RegCloseKey(int hKey);

        [DllImport("advapi32.dll", EntryPoint = "RegQueryValueEx")]
        public static extern int RegQueryValueEx(
            int hKey, string lpValueName,
            int lpReserved,
            ref uint lpType,
            System.Text.StringBuilder lpData,
            ref uint lpcbData);
        #endregion
        #endregion

        #region Functions
        static public string GetRegKey64(UIntPtr inHive, String inKeyName, String inPropertyName)
        {
            return GetRegKey64(inHive, inKeyName, RegSAM.WOW64_64Key, inPropertyName);
        }

        static public string GetRegKey32(UIntPtr inHive, String inKeyName, String inPropertyName)
        {
            return GetRegKey64(inHive, inKeyName, RegSAM.WOW64_32Key, inPropertyName);
        }

        static public string GetRegKey64(UIntPtr inHive, String inKeyName, RegSAM in32or64key, String inPropertyName)
        {
            //UIntPtr HKEY_LOCAL_MACHINE = (UIntPtr)0x80000002;
            int hkey = 0;

            try
            {
                uint lResult = RegOpenKeyEx(RegHive.HKEY_LOCAL_MACHINE, inKeyName, 0, (int)RegSAM.QueryValue | (int)in32or64key, out hkey);
                if (0 != lResult) return null;
                uint lpType = 0;
                uint lpcbData = 1024;
                StringBuilder AgeBuffer = new StringBuilder(1024);
                RegQueryValueEx(hkey, inPropertyName, 0, ref lpType, AgeBuffer, ref lpcbData);
                string Age = AgeBuffer.ToString();
                return Age;
            }
            finally
            {
                if (0 != hkey) RegCloseKey(hkey);
            }
        }
        #endregion

        #region Enums
        #endregion
    }

    public enum RegSAM
    {
        QueryValue = 0x0001,
        SetValue = 0x0002,
        CreateSubKey = 0x0004,
        EnumerateSubKeys = 0x0008,
        Notify = 0x0010,
        CreateLink = 0x0020,
        WOW64_32Key = 0x0200,
        WOW64_64Key = 0x0100,
        WOW64_Res = 0x0300,
        Read = 0x00020019,
        Write = 0x00020006,
        Execute = 0x00020019,
        AllAccess = 0x000f003f
    }

    public static class RegHive
    {
        public static UIntPtr HKEY_LOCAL_MACHINE = new UIntPtr(0x80000002u);
        public static UIntPtr HKEY_CURRENT_USER = new UIntPtr(0x80000001u);
    }
    

}
