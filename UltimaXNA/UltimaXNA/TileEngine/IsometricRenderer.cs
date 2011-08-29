/***************************************************************************
 *   IsometricRenderer.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   Based on code from ClintXNA's renderer: http://www.runuo.com/forums/xna/92023-hi.html
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
using UltimaXNA.Entities;
using UltimaXNA.Extensions;
using UltimaXNA.Graphics;
using UltimaXNA.Input;
#endregion

namespace UltimaXNA.TileEngine
{
    public class IsometricRenderer : IIsometricRenderer
    {
        private IInputState _input;

        #region RenderingVariables
        VectorRenderer _vectors;
        SpriteBatch3D _spriteBatch;
        VertexPositionNormalTextureHue[] _vertexBufferStretched;
        #endregion

        #region MousePickingVariables
        MouseOverItem _overObject, _overGround;
        public MapObject MouseOverObject { get { return (_overObject == null) ? null : _overObject.Object; } }
        public MapObject MouseOverGround { get { return (_overGround == null) ? null : _overGround.Object; } }
        public Vector2 MouseOverObjectPoint { get { return (_overObject == null) ? new Vector2(0, 0) : _overObject.InTexturePosition; } }
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

        private static bool _flag_HighlightMouseOver = false;
        public static bool Flag_HighlightMouseOver
        {
            get { return _flag_HighlightMouseOver; }
            set { _flag_HighlightMouseOver = value; }
        }

        private static Map _map;
        public Map Map
        {
            get { return _map; }
            set { _map = value; }
        }
        internal static Map InternalMap
        {
            get { return _map; }
        }
        public int ObjectsRendered { get; internal set; }
        public Position3D CenterPosition { get; set; }
        public static bool DrawTerrain = true;
        private int _maxItemAltitude;

        Vector2 _renderOffset;
        public Vector2 RenderOffset
        {
            get { return _renderOffset; }
        }

        public IsometricRenderer(Game game)
        {
            _input = game.Services.GetService<IInputState>();
            _spriteBatch = new SpriteBatch3D(game);
            _vectors = new VectorRenderer(game.GraphicsDevice, game.Content);

            PickType = PickTypes.PickNothing;
            _vertexBufferStretched = new VertexPositionNormalTextureHue[] {
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(0, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(1, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(0, 1, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(1, 1, 0))
            };
        }

        public void Update(GameTime gameTime)
        {
            if (ClientVars.EngineVars.Map != -1)
            {
                if ((Map == null) || (Map.Index != ClientVars.EngineVars.Map))
                    Map = new Map(ClientVars.EngineVars.Map);
                // Update the Map's position so it loads the tiles we're going to be drawing
                Map.Update(CenterPosition.X, CenterPosition.Y);

                // Are we inside (under a roof)? Do not draw tiles above our head.
                _maxItemAltitude = 255;

                MapTile t;
                if ((t = Map.GetMapTile(CenterPosition.X, CenterPosition.Y, true)) != null)
                {
                    MapObject underObject, underTerrain;
                    t.IsUnder(CenterPosition.Z, out underObject, out underTerrain);

                    // if we are under terrain, then do not draw any terrain at all.
                    DrawTerrain = !(underTerrain == null);

                    if (!(underObject == null))
                    {
                        // Roofing and new floors ALWAYS begin at intervals of 20.
                        // if we are under a ROOF, then get rid of everything above me.Z + 20
                        // (this accounts for A-frame roofs). Otherwise, get rid of everything
                        // at the object above us.Z.
                        if (((MapObjectStatic)underObject).ItemData.Roof)
                        {
                            _maxItemAltitude = CenterPosition.Z - (CenterPosition.Z % 20) + 20;
                        }
                        else
                        {
                            _maxItemAltitude = underObject.Z - (underObject.Z % 20);
                        }

                        // If we are under a roof tile, do not make roofs transparent if we are on an edge.
                        if (underObject is MapObjectStatic && ((MapObjectStatic)underObject).ItemData.Roof)
                        {
                            bool isRoofSouthEast = true;
                            if ((t = Map.GetMapTile(CenterPosition.X + 1, CenterPosition.Y + 1, true)) != null)
                            {
                                t.IsUnder(CenterPosition.Z, out underObject, out underTerrain);
                                isRoofSouthEast = !(underObject == null);
                            }

                            if (!isRoofSouthEast)
                                _maxItemAltitude = 255;
                        }
                    }
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (ClientVars.EngineVars.IsMinimized)
                return;

            if (ClientVars.EngineVars.Map < 0)
                return;

            
            render(out _renderOffset);
            renderVectors(_renderOffset);
        }

        private void render(out Vector2 renderOffset)
        {
            // Prerender objects
            _spriteBatch.Prepare(false, false);
            MapObjectPrerendered.RenderObjects(_spriteBatch);

            // ClientVars.EngineVars.RenderSize = 20;

            int RenderBeginX = CenterPosition.X - (ClientVars.EngineVars.RenderSize / 2);
            int RenderBeginY = CenterPosition.Y - (ClientVars.EngineVars.RenderSize / 2);
            int RenderEndX = RenderBeginX + ClientVars.EngineVars.RenderSize;
            int RenderEndY = RenderBeginY + ClientVars.EngineVars.RenderSize;

            renderOffset.X = (ClientVars.EngineVars.BackBufferWidth >> 1) - 22;
            renderOffset.X -= (int)((CenterPosition.X_offset - CenterPosition.Y_offset) * 22);
            renderOffset.X -= (RenderBeginX - RenderBeginY) * 22;

            renderOffset.Y = ((ClientVars.EngineVars.BackBufferHeight - (ClientVars.EngineVars.RenderSize * 44)) >> 1);
            renderOffset.Y += (CenterPosition.Z << 2) + (int)(CenterPosition.Z_offset * 4);
            renderOffset.Y -= (int)((CenterPosition.X_offset + CenterPosition.Y_offset) * 22);
            renderOffset.Y -= (RenderBeginX + RenderBeginY) * 22;

            ObjectsRendered = 0; // Count of objects rendered for statistics and debug
            MouseOverList overList = new MouseOverList(); // List of items for mouse over
            overList.MousePosition = _input.MousePosition;
            List<MapObject> mapObjects;
            Vector3 drawPosition = new Vector3();

            for (int ix = RenderBeginX; ix < RenderEndX; ix++)
            {
                drawPosition.X = (ix - RenderBeginY) * 22 + renderOffset.X;
                drawPosition.Y = (ix + RenderBeginY) * 22 + renderOffset.Y;

                DrawTerrain = true;

                for (int iy = RenderBeginY; iy < RenderEndY; iy++)
                {
                    MapTile tile = Map.GetMapTile(ix, iy, true);
                    if (tile == null)
                        continue;

                    mapObjects = tile.Items;
                    for (int i = 0; i < mapObjects.Count; i++)
                    {
                        if (mapObjects[i].Draw(_spriteBatch, drawPosition, overList, PickType, _maxItemAltitude))
                            ObjectsRendered++;
                    }
                    tile.ClearTemporaryObjects();

                    drawPosition.X -= 22f;
                    drawPosition.Y += 22f;
                }
            }

            // Update the MouseOver objects
            _overObject = overList.GetForemostMouseOverItem(_input.MousePosition);
            _overGround = overList.GetForemostMouseOverItem<MapObjectGround>(_input.MousePosition);

            // Draw the objects we just send to the spritebatch.
            _spriteBatch.Prepare(true, true);
            _spriteBatch.Flush();
        }

        private void renderVectors(Vector2 renderOffset)
        {
            if (Flag_HighlightMouseOver)
            {
                if (_overObject != null)
                {
                    _vectors.DrawLine(_overObject.Vertices[0], _overObject.Vertices[1], Color.LightBlue);
                    _vectors.DrawLine(_overObject.Vertices[1], _overObject.Vertices[3], Color.LightBlue);
                    _vectors.DrawLine(_overObject.Vertices[3], _overObject.Vertices[2], Color.LightBlue);
                    _vectors.DrawLine(_overObject.Vertices[2], _overObject.Vertices[0], Color.LightBlue);
                }
                if (_overGround != null)
                {
                    _vectors.DrawLine(_overGround.Vertices[0], _overGround.Vertices[1], Color.Teal);
                    _vectors.DrawLine(_overGround.Vertices[1], _overGround.Vertices[3], Color.Teal);
                    _vectors.DrawLine(_overGround.Vertices[3], _overGround.Vertices[2], Color.Teal);
                    _vectors.DrawLine(_overGround.Vertices[2], _overGround.Vertices[0], Color.Teal);
                }
            }

            _vectors.Render_ViewportSpace();
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
    }
}