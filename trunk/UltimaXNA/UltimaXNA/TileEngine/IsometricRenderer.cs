/***************************************************************************
 *   World.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   Based on code from ClintXNA's renderer: http://www.runuo.com/forums/xna/92023-hi.html
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
using UltimaXNA.Entities;
using UltimaXNA.Extensions;
using UltimaXNA.Input;
#endregion

namespace UltimaXNA.TileEngine
{
    public class IsometricRenderer : IIsometricRenderer
    {
        private IInputState _input;

        #region RenderingVariables
        WireframeRenderer _wireframe;
        SpriteBatch3D _spriteBatch;
        VertexPositionNormalTextureHue[] _vertexBufferStretched;
        #endregion

        #region MousePickingVariables
        MouseOverItem _overObject, _overGround;
        public MapObject MouseOverObject { get { return (_overObject == null) ? null : _overObject.Object; } }
        public MapObject MouseOverGround { get { return (_overGround == null) ? null : _overGround.Object; } }
        public Point MouseOverObjectPoint { get { return (_overObject == null) ? new Point(0, 0) : _overObject.InTexturePosition; } }
        public const PickTypes DefaultPickType = PickTypes.PickStatics | PickTypes.PickObjects;
        public PickTypes PickType { get; set; }
        #endregion

        #region LightingVariables
        private int _personalLightning = 0, _overallLightning = 0;
        private float _lightingDirection = 0f;
        public int PersonalLightning
        {
            get { return _personalLightning; }
            set
            {
                _personalLightning = value;
                recalculateLightning();
            }
        }
        public int OverallLightning
        {
            get { return _overallLightning; }
            set
            {
                _overallLightning = value;
                recalculateLightning();
            }
        }
        public float LightDirection
        {
            set { _lightingDirection = value; recalculateLightning(); }
            get { return _lightingDirection; }
        }
        #endregion

        public Map Map {get; set; }
        public int ObjectsRendered { get; internal set; }
        public Position3D CenterPosition { get; set; }
        private int _maxItemAltitude;
        public static bool DrawTerrain = true;

        public IsometricRenderer(Game game)
        {
            _input = game.Services.GetService<IInputState>();
            _spriteBatch = new SpriteBatch3D(game);
            _wireframe = new WireframeRenderer(game);
            _wireframe.Initialize();

            PickType = PickTypes.PickNothing;
            _vertexBufferStretched = new VertexPositionNormalTextureHue[] {
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(0, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(1, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(0, 1, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(1, 1, 0))
            };
        }

        private void recalculateLightning()
        {
            float light = Math.Min(30 - OverallLightning + PersonalLightning, 30f);
            light = Math.Max(light, 0);
            light /= 30; // bring it between 0-1

            // -0.3 corresponds pretty well to the darkest possible light in the original client
            // 0.5 is quite okay for the brightest light.
            // so we'll just interpolate between those two values

            light *= 0.8f;
            light -= 0.3f;

            // i'd use a fixed lightning direction for now - maybe enable this effect with a custom packet?
            Vector3 lightDirection = new Vector3(0f, (float)-Math.Cos(_lightingDirection), (float)Math.Sin(_lightingDirection));

            _spriteBatch.SetLightDirection(lightDirection);

            // again some guesstimated values, but to me it looks okay this way :) 
            _spriteBatch.SetAmbientLightIntensity(light * 0.8f);
            _spriteBatch.SetDirectionalLightIntensity(light);
        }

        public void Update(GameTime gameTime)
        {
            if (ClientVars.Map != -1)
            {
                if ((Map == null) || (Map.Index != ClientVars.Map))
                    Map = new Map(ClientVars.Map);
                // Update the Map's position so it loads the tiles we're going to be drawing
                Map.Update(CenterPosition.Draw_TileX, CenterPosition.Draw_TileY);

                // Are we inside (under a roof)? Do not draw tiles above our head.
                _maxItemAltitude = 255;

                MapTile t = Map.GetMapTile(CenterPosition.Draw_TileX, CenterPosition.Draw_TileY, true);
                if (t != null)
                {
                    bool isUnderRoof, isUnderTerrain;
                    t.IsUnder(CenterPosition.Z, out isUnderRoof, out isUnderTerrain);
                    if (isUnderRoof)
                        _maxItemAltitude = CenterPosition.Z - (CenterPosition.Z % 20) + 20;

                    if (isUnderTerrain)
                        DrawTerrain = false;
                    else
                        DrawTerrain  = true;

                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (ClientVars.Map != -1)
            {
                render();
                _spriteBatch.Flush(true);
                if (ClientVars.DEBUG_HighlightMouseOverObjects)
                {
                    if (_overObject != null)
                        _wireframe.AddMouseOverItem(_overObject);
                    if (_overGround != null)
                        _wireframe.AddMouseOverItem(_overGround);
                    _wireframe.ProjectionMatrix = _spriteBatch.WorldMatrix;
                    _wireframe.Draw(gameTime);
                }
            }
        }

        private void render()
        {
            if (ClientVars.IsMinimized)
                return;

            _spriteBatch.DrawWireframe = ClientVars.DEBUG_DrawWireframe;

            int RenderBeginX = CenterPosition.Draw_TileX - (ClientVars.RenderSize / 2);
            int RenderBeginY = CenterPosition.Draw_TileY - (ClientVars.RenderSize / 2);
            int RenderEndX = RenderBeginX + ClientVars.RenderSize;
            int RenderEndY = RenderBeginY + ClientVars.RenderSize;

            int renderOffsetX = (ClientVars.BackBufferWidth >> 1) - 22;
            renderOffsetX -= (int)((CenterPosition.Draw_Xoffset - CenterPosition.Draw_Yoffset) * 22);
            renderOffsetX -= (RenderBeginX - RenderBeginY) * 22;

            int renderOffsetY = ((ClientVars.BackBufferHeight - (ClientVars.RenderSize * 44)) >> 1);
            renderOffsetY += (CenterPosition.Z << 2) + (int)(CenterPosition.Draw_Zoffset * 4);
            renderOffsetY -= (int)((CenterPosition.Draw_Xoffset + CenterPosition.Draw_Yoffset) * 22);
            renderOffsetY -= (RenderBeginX + RenderBeginY) * 22;

            ObjectsRendered = 0; // Count of objects rendered for statistics and debug
            MouseOverList overList = new MouseOverList(); // List of items for mouse over
            overList.MousePosition = _input.MousePosition;
            List<MapObject> mapObjects;
            Vector3 drawPosition = new Vector3();

            for (int ix = RenderBeginX; ix < RenderEndX; ix++)
            {
                drawPosition.X = (ix - RenderBeginY) * 22 + renderOffsetX;
                drawPosition.Y = (ix + RenderBeginY) * 22 + renderOffsetY;

                for (int iy = RenderBeginY; iy < RenderEndY; iy++)
                {
                    MapTile tile = Map.GetMapTile(ix, iy, true);
                    if (tile == null)
                        continue;

                    mapObjects = tile.GetSortedObjects();
                    for (int i = 0; i < mapObjects.Count; i++)
                    {
                        if (mapObjects[i].Draw(_spriteBatch, drawPosition, overList, PickType, _maxItemAltitude))
                            ObjectsRendered++;
                    }

                    drawPosition.X -= 22f;
                    drawPosition.Y += 22f;
                }
            }
            // Update the Mouse Over Objects
            _overObject = overList.GetForemostMouseOverItem(_input.MousePosition);
            _overGround = overList.GetForemostMouseOverItem<MapObjectGround>(_input.MousePosition);
        }

        public static Vector2 GetHueVector(int hue)
        {
            if (hue == 0)
                return new Vector2(0);

            int hueType = 1;
            if ((hue & 0x8000) != 0) // partial hue
                hueType = 2;
            else if ((hue & 0x4000) != 0) // transparant
                hueType = 3;

            return new Vector2(hue & 0x3FFF - 1, hueType);
        }
    }
}