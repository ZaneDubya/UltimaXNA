/***************************************************************************
 *   FileManager.cs
 *   Based on code from UltimaSDK: http://ultimasdk.codeplex.com/
 *   And on code from OpenUO: https://github.com/jeffboulanger/OpenUO
 *      Copyright (c) 2011 OpenUO Software Team.
 *      All Rights Reserved.
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Ultima.IO.UOP;
#endregion

namespace UltimaXNA.Ultima.IO
{
    class FileManager
    {
        static private bool m_isDataPresent = false;
        static public bool IsUODataPresent
        {
            get { return m_isDataPresent; }
        }

        static readonly string[] m_knownRegkeys = new string[] { 
                @"Origin Worlds Online\Ultima Online\KR Legacy Beta", 
                @"EA Games\Ultima Online: Mondain's Legacy\1.00.0000", 
                @"Origin Worlds Online\Ultima Online\1.0", 
                @"Origin Worlds Online\Ultima Online Third Dawn\1.0",
                @"EA GAMES\Ultima Online Samurai Empire", 
                @"EA Games\Ultima Online: Mondain's Legacy", 
                @"EA GAMES\Ultima Online Samurai Empire\1.0", 
                @"EA GAMES\Ultima Online Samurai Empire\1.00.0000", 
                @"EA GAMES\Ultima Online: Samurai Empire\1.0", 
                @"EA GAMES\Ultima Online: Samurai Empire\1.00.0000", 
                @"EA Games\Ultima Online: Mondain's Legacy\1.0", 
                @"EA Games\Ultima Online: Mondain's Legacy\1.00.0000", 
                @"Origin Worlds Online\Ultima Online Samurai Empire BETA\2d\1.0", 
                @"Origin Worlds Online\Ultima Online Samurai Empire BETA\3d\1.0", 
                @"Origin Worlds Online\Ultima Online Samurai Empire\2d\1.0", 
                @"Origin Worlds Online\Ultima Online Samurai Empire\3d\1.0",
                @"Electronic Arts\EA Games\Ultima Online Stygian Abyss Classic",
                @"Electronic Arts\EA Games\Ultima Online Classic",
                @"Electronic Arts\EA Games\"
            };

        private static readonly Version m_ConvertedToUOPVersion = new Version("7.0.24.0");
        private static string m_FileDirectory;
        private static Version m_Version;

        public static string DataPath
        {
            get { return m_FileDirectory; }
        }

        public static bool Is64Bit
        {
            get { return IntPtr.Size == 8; } 
        }

        public static Version Version
        {
            get
            {
                if (m_Version == null)
                {
                    var clientExe = GetPath("client.exe");

                    if (File.Exists(clientExe))
                    {
                        var fileVersionInfo = FileVersionInfo.GetVersionInfo(clientExe);
                        m_Version = new Version(
                            fileVersionInfo.FileMajorPart,
                            fileVersionInfo.FileMinorPart,
                            fileVersionInfo.FileBuildPart,
                            fileVersionInfo.FilePrivatePart);
                    }
                    else
                    {
                        m_Version = new Version("0.0.0.0");
                    }
                }

                return m_Version;
            }
        }

        public static bool IsUopFormat
        {
            get { return Version >= m_ConvertedToUOPVersion; }
        }

        static FileManager()
        {
            Tracer.Debug("Initializing UOData. Is64Bit = {0}", Is64Bit);
            Tracer.Debug("Looking for UO Installation:");

            if (Settings.UltimaOnline.DataDirectory != null && Directory.Exists(Settings.UltimaOnline.DataDirectory))
            {
                Tracer.Debug("Settings: {0}", Settings.UltimaOnline.DataDirectory);

                m_FileDirectory = Settings.UltimaOnline.DataDirectory;
                m_isDataPresent = true;
            }
            else
            {
                for (int i = 0; i < m_knownRegkeys.Length; i++)
                {
                    string exePath;

                    if (Is64Bit)
                    {
                        exePath = GetExePath(@"Wow6432Node\" + m_knownRegkeys[i]);
                    }
                    else
                    {
                        exePath = GetExePath(m_knownRegkeys[i]);
                    }

                    if (exePath != null && Directory.Exists(exePath))
                    {
                        if (InternalClientIsCompatible(exePath))
                        {
                            Tracer.Debug("Compatible: {0}", exePath);

                            Settings.UltimaOnline.DataDirectory = exePath;

                            m_FileDirectory = exePath;
                            m_isDataPresent = true;
                        }
                        else
                        {
                            Tracer.Debug("Incompatible: {0}", exePath);
                        }
                    }
                }
            }

            if (m_FileDirectory == null)
            {
                m_isDataPresent = false;
            }
            else
            {
                Tracer.Debug(string.Empty);
                Tracer.Debug("Selected: {0}", m_FileDirectory);
            }
        }

        private static bool InternalClientIsCompatible(string path)
        {
            IEnumerable<string> files = Directory.EnumerateFiles(path);
            foreach (string filepath in files)
            {
                string extension = Path.GetExtension(filepath).ToLower();
                if (extension == ".uop")
                {
                    return false;
                }
            }
            return true;
        }

        private static string GetExePath(string subName)
        {
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(string.Format(@"SOFTWARE\{0}", subName));

                if (key == null)
                {
                    key = Registry.CurrentUser.OpenSubKey(string.Format(@"SOFTWARE\{0}", subName));

                    if (key == null)
                    {
                        return null;
                    }
                }

                string path = key.GetValue("ExePath") as string;

                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                {
                    path = key.GetValue("Install Dir") as string;

                    if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                    {
                        path = key.GetValue("InstallDir") as string;

                        if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                        {
                            return null;
                        }
                    }
                }

                if (File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                }

                if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                {
                    return null;
                }

                return path;
            }
            catch
            {
                return null;
            }
        }

        public static string GetFilePath(string name)
        {
            if (m_FileDirectory != null)
            {
                name = Path.Combine(m_FileDirectory, name);
                // Fix for opening files which don't exist -Smjert
                if (File.Exists(name))
                    return name;
            }

            return null;
        }

        public static bool Exists(string name)
        {
            try
            {
                name = Path.Combine(m_FileDirectory, name);
                Tracer.Debug("Checking if file exists [{0}]", name);

                if (File.Exists(name))
                {
                    return true;
                }

                return false;
            }
            catch { return false; }
        }

        public static bool Exists(string name, int index)
        {
            return Exists(String.Format(name, index));
        }

        public static bool Exists(string name, int index, string type)
        {
            return Exists(String.Format("{0}{1}.{2}", name, index, type));
        }

        public static bool Exists(string name, string type)
        {
            return Exists(String.Format("{0}.{1}", name, type));
        }

        public static FileStream GetFile(string name)
        {
            try
            {
                name = Path.Combine(m_FileDirectory, name);

                return new FileStream(name, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch { return null; }
        }

        public static FileStream GetFile(string name, uint index)
        {
            return GetFile(String.Format(name, index));
        }

        public static FileStream GetFile(string name, uint index, string type)
        {
            return GetFile(String.Format("{0}{1}.{2}", name, index, type));
        }

        public static FileStream GetFile(string name, string type)
        {
            return GetFile(String.Format("{0}.{1}", name, type));
        }

        public static string GetPath(string name)
        {
            return Path.Combine(m_FileDirectory, name);
        }


        public static AFileIndex CreateFileIndex(string uopFile, int length, bool hasExtra, string extension)
        {
            uopFile = GetPath(uopFile);

            AFileIndex fileIndex = new UopFileIndex(uopFile, length, hasExtra, extension);


            fileIndex.Open();

            return fileIndex;
        }

        public static AFileIndex CreateFileIndex(string idxFile, string mulFile, int length, int patch_file)
        {
            idxFile = GetPath(idxFile);
            mulFile = GetPath(mulFile);

            AFileIndex fileIndex = new MulFileIndex(idxFile, mulFile, length, patch_file);
            fileIndex.Open();

            return fileIndex;
        }
    }
}