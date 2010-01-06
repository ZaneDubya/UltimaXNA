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
        VertexDeclaration lineVertexDeclaration;
        public Matrix ProjectionMatrix;

        List<VertexPositionColor[]> _polygonsToDraw;

        public WireframeRenderer(Game game)
            : base(game)
        {
            lineEffect = new BasicEffect(game.GraphicsDevice, null);
            lineEffect.VertexColorEnabled = true;
            lineVertexDeclaration = new VertexDeclaration(game.GraphicsDevice,
                VertexPositionColorTexture.VertexElements);

            _polygonsToDraw = new List<VertexPositionColor[]>();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            RenderState renderState = this.Game.GraphicsDevice.RenderState;

            // Set line drawing renderstates. We disable backface culling
            // and turn off the depth buffer because we want to be able to
            // see the picked triangle outline regardless of which way it is
            // facing, and even if there is other geometry in front of it.
            renderState.FillMode = FillMode.WireFrame;
            renderState.CullMode = CullMode.None;
            renderState.DepthBufferEnable = false;

            // Activate the line drawing BasicEffect.
            lineEffect.Projection = ProjectionMatrix;
            // lineEffect.View = camera.View;

            lineEffect.Begin();
            lineEffect.CurrentTechnique.Passes[0].Begin();

            // Draw the triangles.
            GraphicsDevice.VertexDeclaration = lineVertexDeclaration;

            foreach (VertexPositionColor[] v in _polygonsToDraw)
            {
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, v, 0, 2);
            }

            lineEffect.CurrentTechnique.Passes[0].End();
            lineEffect.End();

            // Reset renderstates to their default values.
            renderState.FillMode = FillMode.Solid;
            renderState.CullMode = CullMode.CullCounterClockwiseFace;
            renderState.DepthBufferEnable = true;

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
