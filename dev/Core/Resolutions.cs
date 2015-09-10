using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using UltimaXNA.Configuration;

namespace UltimaXNA.Core
{
    internal class Resolutions
    {
        public static readonly List<ResolutionConfig> FullScreenResolutionsList;
        public static readonly List<ResolutionConfig> PlayWindowResolutionsList;

        static Resolutions()
        {
            FullScreenResolutionsList = new List<ResolutionConfig>();
            PlayWindowResolutionsList = new List<ResolutionConfig>();

            foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                if (mode.Format != SurfaceFormat.Color)
                    continue;
                ResolutionConfig res = new ResolutionConfig(mode.Width, mode.Height);
                if (!FullScreenResolutionsList.Contains(res))
                {
                    FullScreenResolutionsList.Add(res);
                }
            }

            foreach (ResolutionConfig res in FullScreenResolutionsList)
            {
                if (!PlayWindowResolutionsList.Contains(res) && res.Width < 2048 && res.Height < 2048)
                {
                    PlayWindowResolutionsList.Add(res);
                }
            }
        }

        public static bool IsValidFullScreenResolution(ResolutionConfig resolution)
        {
            foreach (ResolutionConfig res in FullScreenResolutionsList)
                if (resolution.Width == res.Width && resolution.Height == res.Height)
                    return true;
            return false;
        }

        public static bool IsValidPlayWindowResolution(ResolutionConfig resolution)
        {
            foreach (ResolutionConfig res in PlayWindowResolutionsList)
                if (resolution.Width == res.Width && resolution.Height == res.Height)
                    return true;
            return false;
        }
    }
}
