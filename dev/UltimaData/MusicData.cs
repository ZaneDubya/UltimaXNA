/***************************************************************************
 *   Music.cs
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using UltimaXNA.Core.Diagnostics;

namespace UltimaXNA.UltimaData
{
	class MusicData
	{
        private static int PlayingId = -1;

		#region Internals

		private const string m_internalMusicName = "UltimaXNAMusic";

		[DllImport( "winmm.dll" )]
		private static extern int mciSendString(string lpCommand, StringBuilder lpReturn, int nReturnLength, IntPtr callBack);

		private static void InternalPlay(string path, bool repeat)
		{
			// open resource
            string mciCommand = string.Format("open \"{0}\" type MPEGVideo alias {1}", path, m_internalMusicName);
            int error = mciSendString(mciCommand, null, 0, IntPtr.Zero);
            if (error == 0)
            {
                PlayingId = 1;
				// start playing
				string playCommand = string.Format ( "play {0} from 0", m_internalMusicName );
				if (repeat)
				{
					playCommand += " repeat";
				}
				if (mciSendString ( playCommand, null, 0, IntPtr.Zero ) != 0)
				{
                    Logger.Error("Error playing mp3 file {0}", path);
				}
			} else {
                Logger.Error("Error opening mp3 file {0}", path);
			}
		}

		private static void InternalStop()
		{
            if (PlayingId != -1)
            {
                // Stop playing
                if (mciSendString(string.Format("stop {0}", m_internalMusicName), null, 0, IntPtr.Zero) == 0)
                {
                    PlayingId = -1;
                    // close resource
                    if (mciSendString(string.Format("close {0}", m_internalMusicName), null, 0, IntPtr.Zero) != 0)
                    {
                        Logger.Error("Error closing current mp3 file");
                    }
                }
                else
                {
                    Logger.Error("Error stopping current mp3 file");
                }
            }
		}

		public static int InternalVolume
		{
			get
			{
				StringBuilder buffer = new StringBuilder ( 261 );
				int result = mciSendString ( string.Format("status {0} volume", m_internalMusicName),
				   buffer, buffer.Capacity, IntPtr.Zero );
				if (result != 0)
				{
                    Logger.Error("Error reading volume");
					return 0;
				}
				return int.Parse ( buffer.ToString() );
			}

			set
			{
				int result = mciSendString ( string.Format("setaudio {0} volume to {1}", m_internalMusicName, value), null, 0, IntPtr.Zero );
				if (result != 0)
				{
                    Logger.Error("Error setting volume");
				}
			}
		}

		#endregion Internals

		private static Hashtable m_songList;
        private const string m_ConfigFilePath = @"Music\Digital\Config.txt";

		public static int Volume
		{
			get { return InternalVolume; }
			set { InternalVolume = value; }
		}

		static MusicData()
		{
			m_songList = new Hashtable ();
		    if (!FileManager.Exists(m_ConfigFilePath))
                return;
			StreamReader reader = new StreamReader(FileManager.GetFile(m_ConfigFilePath));
			String line;
			while ((line = reader.ReadLine ()) != null)
			{
				UOMusic toAdd = ParseConfigFile ( line );
				if (toAdd != null)
				{
					m_songList.Add ( toAdd.Id, toAdd );
				}
			}
		}

		private static char[] m_configFileDelimiters = new char[] { ' ', ',', '\t' };
		private static UOMusic ParseConfigFile(string line)
		{
			string[] splits = line.Split ( m_configFileDelimiters );
			if (splits.Length < 2 || splits.Length > 3) {
				return null;
			}

			UOMusic ret = new UOMusic();
			ret.Id = int.Parse ( splits[0] );
			ret.Name = splits[1];
			ret.DoLoop = splits.Length == 3 ? splits[2] == "loop" : false;

			return ret;
		}

		private static UOMusic GetMusicById(int id)
		{
			if (m_songList.Contains(id)) {
				return (UOMusic)m_songList[id];
			} else {
				return null;
			}
		}

		public static void PlayMusic(int id)
		{
			// stop the current song
			StopMusic();

			// just interrupting the music?
			if (id == -1)
			{
				return;
			}

			// check if we have that id
			UOMusic music = GetMusicById ( id );
			if (music == null)
			{
                Logger.Error("Received unknown music id {0}", id);
				return;
			}

			// Console.WriteLine ( "play mp3 file={0} loop={1}", music.Name, music.DoLoop );

			// check if the corresponding file exists
			string path = FileManager.GetFilePath(String.Format("Music\\Digital\\{0}.mp3", music.Name));
			if (path == null && !File.Exists(path)) {
				return;
			}

			// play the file
			InternalPlay ( path, music.DoLoop );
		}

		public static void StopMusic()
		{
			InternalStop ();
		}

        class UOMusic
        {
            public int Id;
            public string Name;
            public bool DoLoop;

            public UOMusic()
            {
                this.Id = -1;
                this.Name = "";
                this.DoLoop = false;
            }

            public UOMusic(int id, string name, bool doLoop)
            {
                this.Id = id;
                this.Name = name;
                this.DoLoop = doLoop;
            }
        }
	}
}
