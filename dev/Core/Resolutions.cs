using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using UltimaXNA.Configuration;
using Microsoft.Xna.Framework;
using System.Drawing;
using System.Windows.Forms;
using UltimaXNA.Configuration.Properties;

namespace UltimaXNA.Core
{
    internal class Resolutions
    {
        public static readonly List<ResolutionProperty> FullScreenResolutionsList;
        public static readonly List<ResolutionProperty> PlayWindowResolutionsList;

        public static void SetScreenSize(GameWindow window)
        {
            Microsoft.Xna.Framework.Rectangle game;
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
                if (!FullScreenResolutionsList.Contains(res))
                {
                    FullScreenResolutionsList.Add(res);
                }
            }

            foreach (ResolutionProperty res in FullScreenResolutionsList)
            {
                if (!PlayWindowResolutionsList.Contains(res) && res.Width <= screen.Width && res.Height <= screen.Height)
                {
                    PlayWindowResolutionsList.Add(res);
                }
            }
        }

        static Resolutions()
        {
            FullScreenResolutionsList = new List<ResolutionProperty>();
            PlayWindowResolutionsList = new List<ResolutionProperty>();

            SetScreenSize(null);
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
