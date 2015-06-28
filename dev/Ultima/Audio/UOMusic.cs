/***************************************************************************
 *   UOMusic.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using Microsoft.Xna.Framework.Media;
using System.Reflection;
using UltimaXNA.Ultima.IO;

namespace UltimaXNA.Ultima.Audio
{
    class UOMusic
    {
        private Song m_Song;

        public readonly int Index;
        public readonly string Name;
        public readonly bool DoLoop;
        
        public Song Song
        {
            get { return m_Song; }
        }

        public SoundState Status = SoundState.Unloaded;

        public UOMusic(int index, string name, bool loop)
        {
            Index = index;
            Name = name;
            DoLoop = loop;
        }

        public string Path
        {
            get
            {
                string path = FileManager.GetPath(string.Format("Music\\Digital\\{0}.mp3", Name));
                return path;
            }
        }

        public void Load()
        {
            if (Status == SoundState.Unloaded)
            {
                Status = SoundState.Loading;
                // Static song ctor requires a URI, which is a pain in the butt,
                // so we're going to just reflect out the ctor.
                var ctor = typeof(Song).GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance, null,
                    new[] { typeof(string), typeof(string), typeof(int) }, null);
                m_Song = (Song)ctor.Invoke(new object[] { Name, Path, 0 });
                Status = SoundState.Loaded;
            }
        }
    }
}
