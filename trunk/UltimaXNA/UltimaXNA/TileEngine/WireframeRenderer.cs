/***************************************************************************
 *   WireframeRenderer.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace UltimaXNA.TileEngine
{
    class WireframeRenderer : DrawableGameComponent
    {
        // Effect and vertex declaration for drawing
        BasicEffect lineEffect;
        public Matrix ProjectionMatrix;

        List<VertexPositionColor[]> _polygonsToDraw;

        public WireframeRenderer(Game game)
            : base(game)
        {
            lineEffect = new BasicEffect(game.GraphicsDevice);
            lineEffect.VertexColorEnabled = true;

            _polygonsToDraw = new List<VertexPositionColor[]>();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            // Set line drawing renderstates. We disable backface culling
            // and turn off the depth buffer because we want to be able to
            // see the picked triangle outline regardless of which way it is
            // facing, and even if there is other geometry in front of it.
            Game.GraphicsDevice.RasterizerState.FillMode = FillMode.WireFrame;
            Game.GraphicsDevice.DepthStencilState = DepthStencilState.None;
                
            // Activate the line drawing BasicEffect.
            lineEffect.Projection = ProjectionMatrix;
            // lineEffect.View = camera.View;

            lineEffect.CurrentTechnique.Passes[0].Apply();

            // Draw the triangles.
            foreach (VertexPositionColor[] v in _polygonsToDraw)
            {
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, v, 0, 2);
            }

            // Reset renderstates to their default values.
            Game.GraphicsDevice.RasterizerState.FillMode = FillMode.Solid;
            Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            _polygonsToDraw.Clear();
        }

        public void AddMouseOverItem(MouseOverItem item)
        {
            VertexPositionColor[] polygon = new VertexPositionColor[item.Vertices.Length];
            for (int i = 0; i < item.Vertices.Length; i++)
            {
                polygon[i].Position = item.Vertices[i];
                polygon[i].Color = Color.Magenta;
            }
            _polygonsToDraw.Add(polygon);
        }
    }
}
