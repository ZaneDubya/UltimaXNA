/***************************************************************************
 *   UltimaUIResourceProvider.cs
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
using System;
using System.Collections.Generic;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.Resources;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Resources;
#endregion

namespace UltimaXNA.Ultima
{
    class ResourceProvider : IResourceProvider
    {
        private AnimationResource m_Anim;
        private ArtMulResource m_Art;
        private ClilocResource m_Cliloc;
        private EffectDataResource m_Effects;
        private FontsResource m_Fonts;
        private GumpMulResource m_Gumps;
        private TexmapResource m_Texmaps;
        private Dictionary<Type, object> m_Resources = new Dictionary<Type, object>();

        public ResourceProvider(Game game)
        {
            m_Anim = new AnimationResource(game.GraphicsDevice);
            m_Art = new ArtMulResource(game.GraphicsDevice);
            m_Cliloc = new ClilocResource("enu");
            m_Effects = new EffectDataResource();
            m_Fonts = new FontsResource(game.GraphicsDevice);
            m_Gumps = new GumpMulResource(game.GraphicsDevice);
            m_Texmaps = new TexmapResource(game.GraphicsDevice);
        }

        public IAnimationFrame[] GetAnimation(int body, int action, int direction, int hue)
        {
            return m_Anim.GetAnimation(body, action, direction, hue);
        }

        public Texture2D GetUITexture(int textureIndex, bool replaceMask080808 = false)
        {
            return m_Gumps.GetGumpXNA(textureIndex);
        }

        public Texture2D GetItemTexture(int itemIndex)
        {
            return m_Art.GetStaticTexture(itemIndex);
        }

        public Texture2D GetLandTexture(int landIndex)
        {
            return m_Art.GetLandTexture(landIndex);
        }

        public void GetItemDimensions(int itemIndex, out int width, out int height)
        {
            m_Art.GetStaticDimensions(itemIndex, out width, out height);
        }

        public Texture2D GetTexmapTexture(int textureIndex)
        {
            return m_Texmaps.GetTexmapTexture(textureIndex);
        }

        /// <summary>
        /// Returns a Ultima Online Hue index that approximates the passed color.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public ushort GetWebSafeHue(Color color)
        {
            return (ushort)HueData.GetWebSafeHue(color);
        }

        public IFont GetUnicodeFont(int fontIndex)
        {
            return m_Fonts.GetUniFont(fontIndex);
        }

        public IFont GetAsciiFont(int fontIndex)
        {
            return m_Fonts.GetAsciiFont(fontIndex);
        }

        public string GetString(int clilocIndex)
        {
            return m_Cliloc.GetString(clilocIndex);
        }

        

        public void RegisterResource<T>(IResource<T> resource)
        {
            Type type = typeof(T);

            if (m_Resources.ContainsKey(type))
            {
                Tracer.Critical(string.Format("Attempted to register resource provider of type {0} twice.", type));
                m_Resources.Remove(type);
            }

            m_Resources.Add(type, resource);
        }

        public T GetResource<T>(int resourceIndex)
        {
            Type type = typeof(T);

            if (m_Resources.ContainsKey(type))
            {
                IResource<T> resource = (IResource<T>)m_Resources[type];
                return (T)resource.GetResource(resourceIndex);
            }
            else
            {
                Tracer.Critical(string.Format("Attempted to get resource provider of type {0}, but no provider with this type is registered.", type));
                return default(T);
            }
        }
    }
}
