using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using UltimaXNA.Diagnostics;

namespace UltimaXNA.Data
{
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

	class Music
	{
		private static ILoggingService _log;
        private static int PlayingId = -1;

		#region Internals

		private const string _internalMusicName = "UltimaXNAMusic";

		[DllImport( "winmm.dll" )]
		private static extern int mciSendString(string lpCommand, StringBuilder lpReturn, int nReturnLength, IntPtr callBack);

		private static void InternalPlay(string path, bool repeat)
		{
			// open resource
            int error = mciSendString(string.Format("open {0} type MPEGVideo alias {1}", path, _internalMusicName), null, 0, IntPtr.Zero);
            if (error == 0)
            {
				// start playing
				string playCommand = string.Format ( "play {0} from 0", _internalMusicName );
				if (repeat)
				{
					playCommand += " repeat";
				}
				if (mciSendString ( playCommand, null, 0, IntPtr.Zero ) != 0)
				{
					_log.Error("Error playing mp3 file {0}", path);
				}
			} else {
				_log.Error("Error opening mp3 file {0}", path);
			}
		}

		private static void InternalStop()
		{
            if (PlayingId != -1)
            {
                // Stop playing
                if (mciSendString(string.Format("stop {0}", _internalMusicName), null, 0, IntPtr.Zero) == 0)
                {
                    // close resource
                    if (mciSendString(string.Format("close {0}", _internalMusicName), null, 0, IntPtr.Zero) != 0)
                    {
                        _log.Error("Error closing current mp3 file");
                    }
                }
                else
                {
                    _log.Error("Error stopping current mp3 file");
                }
            }
		}

		public static int InternalVolume
		{
			get
			{
				StringBuilder buffer = new StringBuilder ( 261 );
				int result = mciSendString ( string.Format("status {0} volume", _internalMusicName),
				   buffer, buffer.Capacity, IntPtr.Zero );
				if (result != 0)
				{
					_log.Error("Error reading volume");
					return 0;
				}
				return int.Parse ( buffer.ToString() );
			}

			set
			{
				int result = mciSendString ( string.Format("setaudio {0} volume to {1}", _internalMusicName, value), null, 0, IntPtr.Zero );
				if (result != 0)
				{
					_log.Error("Error setting volume");
				}
			}
		}

		#endregion Internals

		private static Hashtable _songList;

		public static int Volume
		{
			get { return InternalVolume; }
			set { InternalVolume = value; }
		}

		static Music()
		{
			_log = new Logger(typeof(Music));

			_songList = new Hashtable ();
			StreamReader reader = new StreamReader(FileManager.GetFile ( "Music\\Digital\\Config.txt" ));
			String line;
			while ((line = reader.ReadLine ()) != null)
			{
				UOMusic toAdd = ParseConfigFile ( line );
				if (toAdd != null)
				{
					_songList.Add ( toAdd.Id, toAdd );
				}
			}
		}

		private static char[] _configFileDelimiters = new char[] { ' ', ',', '\t' };
		private static UOMusic ParseConfigFile(string line)
		{
			string[] splits = line.Split ( _configFileDelimiters );
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
			if (_songList.Contains(id)) {
				return (UOMusic)_songList[id];
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
				_log.Error("Received unknown music id {0}", id);
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

	}
}
