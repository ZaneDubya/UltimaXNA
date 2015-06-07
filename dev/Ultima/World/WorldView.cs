/***************************************************************************
 *   WorldView.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using InterXLib.Patterns.MVC;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Ultima.World.Views;
using UltimaXNA.Ultima.Entities;
using UltimaXNA.Ultima.Entities.Mobiles;
using UltimaXNA.Ultima.EntityViews;
#endregion

namespace UltimaXNA.Ultima.World
{
    class WorldView : AView
    {
        public IsometricRenderer Isometric
        {
            get;
            private set;
        }

        public MiniMapTexture MiniMap
        {
            get;
            private set;
        }

        protected new WorldModel Model
        {
            get { return (WorldModel)base.Model; }
        }

        public WorldView(WorldModel model)
            : base(model)
        {
            Isometric = new IsometricRenderer();
            Isometric.Initialize();
            Isometric.LightDirection = -0.6f;

            MiniMap = new MiniMapTexture();
            MiniMap.Initialize();
        }

        public override void Draw(double frameTime)
        {
            AEntity player = EntityManager.GetPlayerObject();
            Position3D center = player.Position;
            AEntityView.s_Technique = (player as Mobile).IsAlive ? Techniques.Default : Techniques.Grayscale;

            Isometric.Update(Model.Map, center, Model.Input.MousePick);
            MiniMap.Update(Model.Map, center);
        }
    }
}
