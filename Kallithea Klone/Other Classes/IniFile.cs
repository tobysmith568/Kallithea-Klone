﻿using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Kallithea_Klone.Other_Classes
{
    /// <summary>
    /// This class is taken from here:
    /// https://stackoverflow.com/a/14906422/3075190
    /// 
    /// With the statement:
    /// "I hereby release it into the public domain - you're free to use it commercially without attribution."
    /// As of 18/09/18
    /// </summary>
    public class IniFile
    {
        readonly string path;
        readonly string exe = Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        public IniFile(string IniPath = null)
        {
            path = new FileInfo(IniPath ?? exe + ".ini").FullName.ToString();
        }

        public string Read(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section ?? exe, Key, "", RetVal, 255, path);
            return RetVal.ToString();
        }

        public void Write(string Key, string Value, string Section = null)
        {
            WritePrivateProfileString(Section ?? exe, Key, Value, path);
        }

        public void DeleteKey(string Key, string Section = null)
        {
            Write(Key, null, Section ?? exe);
        }

        public void DeleteSection(string Section = null)
        {
            Write(null, null, Section ?? exe);
        }

        public bool KeyExists(string Key, string Section = null)
        {
            return Read(Key, Section).Length > 0;
        }
    }
}
