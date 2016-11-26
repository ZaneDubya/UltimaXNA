/***************************************************************************
 *   Resolutions.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Windows.Forms;
using UltimaXNA.Configuration.Properties;
#endregion

namespace UltimaXNA.Core
{
    /// <summary>
    /// Contains a list of all valid resolutions, and the code to change the size of the rendered window.
    /// </summary>
    class Resolutions
    {
        public static readonly List<ResolutionProperty> FullScreenResolutionsList;
        public static readonly List<ResolutionProperty> PlayWindowResolutionsList;
        public const int MAX_BUFFER_SIZE = 2056;

        public static void SetWindowSize(GameWindow window)
        {
            Rectangle game;
            System.Drawing.Rectangle screen;

            if (window != null)
            {
                game = window.ClientBounds;
                screen = Screen.GetWorkingArea(new System.Drawing.Rectangle(game.X, game.Y, game.Width, game.Height));
            }
            else
            {
                screen = Screen.GetWorkingArea(new System.Drawing.Point(0, 0));
            }

            foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                if (mode.Format != SurfaceFormat.Color)
                    continue;
                ResolutionProperty res = new ResolutionProperty(mode.Width, mode.Height);
                if (res.Width <= MAX_BUFFER_SIZE && res.Height <= MAX_BUFFER_SIZE)
                {
                    if (!FullScreenResolutionsList.Contains(res))
                    {
                        FullScreenResolutionsList.Add(res);
                    }
                }
            }

            foreach (ResolutionProperty res in FullScreenResolutionsList)
            {
                if (!PlayWindowResolutionsList.Contains(res) 
                    && res.Width <= screen.Width && res.Height <= screen.Height)
                {
                    PlayWindowResolutionsList.Add(res);
                }
            }
        }

        static Resolutions()
        {
            FullScreenResolutionsList = new List<ResolutionProperty>();
            PlayWindowResolutionsList = new List<ResolutionProperty>();

            SetWindowSize(null);
        }

        public static bool IsValidFullScreenResolution(ResolutionProperty resolution)
        {
            foreach (ResolutionProperty res in FullScreenResolutionsList)
                if (resolution.Width == res.Width && resolution.Height == res.Height)
                    return true;
            return false;
        }

        public static bool IsValidPlayWindowResolution(ResolutionProperty resolution)
        {
            foreach (ResolutionProperty res in PlayWindowResolutionsList)
                if (resolution.Width == res.Width && resolution.Height == res.Height)
                    return true;
            return false;
        }
    }
}
