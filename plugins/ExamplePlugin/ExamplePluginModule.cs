/***************************************************************************
 *   ExamplePluginModule.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using UltimaXNA.Core.Patterns;

namespace ExamplePlugin
{
    sealed class ExamplePluginModule : IModule
    {
        public string Name
        {
            get { return "Example Plugin - Not for production!"; }
        }

        public void Load()
        {
            // CandleObjectDebugger candle = new CandleObjectDebugger();
            // candle.OutputAllCandleTextures();
            WebSafeHueCreator.CreateHues();
        }

        public void Unload()
        {

        }
    }
}
