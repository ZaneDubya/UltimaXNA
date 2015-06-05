/***************************************************************************
 *   CoreModule.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region Usings
using UltimaXNA.Core.Patterns;
#endregion

namespace UltimaXNA
{
    internal sealed class CoreModule : IModule
    {
        public string Name
        {
            get { return "UltimaXNA Core Module"; }
        }

        UltimaGame m_Engine;

        public void Load()
        {
            m_Engine = ServiceRegistry.Register<UltimaGame>(new UltimaGame());
        }

        public void Unload()
        {
            ServiceRegistry.Unregister<UltimaGame>();
        }
    }
}