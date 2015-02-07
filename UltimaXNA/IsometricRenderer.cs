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
using UltimaXNA.Entity;
using UltimaXNA.Graphics;
using UltimaXNA.Input;
using UltimaXNA.TileEngine;
#endregion

namespace UltimaXNA
{
    public class IsometricRenderer
    {
        #region RenderingVariables
        static VectorRenderer _vectors;
        static SpriteBatch3D _spriteBatch;
        static VertexPositionNormalTextureHue[] _vertexBufferStretched;
        #endregion

        #region MousePickingVariables
        private static MouseOverItem _overObject, _overGround;
        public static MapObject MouseOverObject { get { return (_overObject == null) ? null : _overObject.Object; } }
        public static MapObject MouseOverGround { get { return (_overGround == null) ? null : _overGround.Object; } }
        public static Vector2 MouseOverObjectPoint { get { return (_overObject == null) ? new Vector2(0, 0) : _overObject.InTexturePosition; } }
        public const PickTypes DefaultPickType = PickTypes.PickStatics | PickTypes.PickObjects;
        public static PickTypes PickType { get; set; }
        #endregion

        #region LightingVariables
        private static int _lightLevelPersonal = 9, _lightLevelOverall = 9;
        private static float _lightDirection = 4.12f, _lightHeight = -0.75f;
        public static int PersonalLightning
        {
            set { _lightLevelPersonal = value; recalculateLightning(); }
            get { return _lightLevelPersonal; }
        }
        public static int OverallLightning
        {
            set { _lightLevelOverall = value; recalculateLightning(); }
            get { return _lightLevelOverall; }
        }
        public static float LightDirection
        {
            set { _lightDirection = value; recalculateLightning(); }
            get { return _lightDirection; }
        }
        public static float LightHeight
        {
            set { _lightHeight = value; recalculateLightning(); }
            get { return _lightHeight; }
        }
        #endregion

        private static bool _flag_HighlightMouseOver = false;
        public static bool Flag_HighlightMouseOver
        {
            get { return _flag_HighlightMouseOver; }
            set { _flag_HighlightMouseOver = value; }
        }

        private static Map _map;
        public static Map Map
        {
            get { return _map; }
            set { _map = value; }
        }
        public static int ObjectsRendered { get; internal set; }
        public static Position3D CenterPosition { get; set; }
        public static bool DrawTerrain = true;
        private static int _maxItemAltitude;

        static Vector2 _renderOffset;
        public static Vector2 RenderOffset
        {
            get { return _renderOffset; }
        }

        public static void Initialize(Game game)
        {
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

        public static void Update(GameTime gameTime)
        {
            if (UltimaVars.EngineVars.Map != -1)
            {
                if ((_map == null) || (_map.Index != UltimaVars.EngineVars.Map))
                    _map = new Map(UltimaVars.EngineVars.Map);
                // Update the Map's position so it loads the tiles we're going to be drawing
                _map.Update(CenterPosition.X, CenterPosition.Y);

                // Are we inside (under a roof)? Do not draw tiles above our head.
                _maxItemAltitude = 255;

                MapTile t;
                if ((t = _map.GetMapTile(CenterPosition.X, CenterPosition.Y, true)) != null)
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
                            _maxItemAltitude = (int)(underObject.Z - (underObject.Z % 20));
                        }

                        // If we are under a roof tile, do not make roofs transparent if we are on an edge.
                        if (underObject is MapObjectStatic && ((MapObjectStatic)underObject).ItemData.Roof)
                        {
                            bool isRoofSouthEast = true;
                            if ((t = _map.GetMapTile(CenterPosition.X + 1, CenterPosition.Y + 1, true)) != null)
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

        public static void Draw(GameTime gameTime)
        {
            if (UltimaVars.EngineVars.Map < 0)
                return;
            
            render(out _renderOffset);
            renderVectors(_renderOffset);
        }

        private static void render(out Vector2 renderOffset)
        {
            if (CenterPosition == null)
            {
                renderOffset = new Vector2();
                return;
            }

            // Prerender objects
            _spriteBatch.Prepare(false, false);
            MapObjectPrerendered.RenderObjects(_spriteBatch);

            // UltimaVars.EngineVars.RenderSize = 20;

            int RenderBeginX = CenterPosition.X - (UltimaVars.EngineVars.RenderSize / 2);
            int RenderBeginY = CenterPosition.Y - (UltimaVars.EngineVars.RenderSize / 2);
            int RenderEndX = RenderBeginX + UltimaVars.EngineVars.RenderSize;
            int RenderEndY = RenderBeginY + UltimaVars.EngineVars.RenderSize;

            renderOffset.X = (UltimaVars.EngineVars.ScreenSize.X >> 1) - 22;
            renderOffset.X -= (int)((CenterPosition.X_offset - CenterPosition.Y_offset) * 22);
            renderOffset.X -= (RenderBeginX - RenderBeginY) * 22;

            renderOffset.Y = ((UltimaVars.EngineVars.ScreenSize.Y - (UltimaVars.EngineVars.RenderSize * 44)) >> 1);
            renderOffset.Y += (CenterPosition.Z * 4) + (int)(CenterPosition.Z_offset * 4);
            renderOffset.Y -= (int)((CenterPosition.X_offset + CenterPosition.Y_offset) * 22);
            renderOffset.Y -= (RenderBeginX + RenderBeginY) * 22;

            ObjectsRendered = 0; // Count of objects rendered for statistics and debug
            MouseOverList overList = new MouseOverList(); // List of items for mouse over
            overList.MousePosition = UltimaEngine.Input.MousePosition;
            List<MapObject> mapObjects;
            Vector3 drawPosition = new Vector3();

            for (int ix = RenderBeginX; ix < RenderEndX; ix++)
            {
                drawPosition.X = (ix - RenderBeginY) * 22 + renderOffset.X;
                drawPosition.Y = (ix + RenderBeginY) * 22 + renderOffset.Y;

                DrawTerrain = true;

                for (int iy = RenderBeginY; iy < RenderEndY; iy++)
                {
                    MapTile tile = _map.GetMapTile(ix, iy, true);
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
            _overObject = overList.GetForemostMouseOverItem(UltimaEngine.Input.MousePosition);
            _overGround = overList.GetForemostMouseOverItem<MapObjectGround>(UltimaEngine.Input.MousePosition);

            // Draw the objects we just send to the spritebatch.
            _spriteBatch.Prepare(true, true);
            _spriteBatch.Flush();
        }

        private static void renderVectors(Vector2 renderOffset)
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

        private static void recalculateLightning()
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
            Vector3 lightDirection = new Vector3((float)Math.Cos(_lightDirection), _lightHeight, (float)Math.Sin(_lightDirection));

            _spriteBatch.SetLightDirection(lightDirection);

            // again some guesstimated values, but to me it looks okay this way :) 
            _spriteBatch.SetAmbientLightIntensity(light * 0.8f);
            _spriteBatch.SetDirectionalLightIntensity(light);
        }
    }
}