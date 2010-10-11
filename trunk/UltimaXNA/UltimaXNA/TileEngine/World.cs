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
using UltimaXNA.InputOld;
#endregion

namespace UltimaXNA.TileEngine
{
    public class World : IWorld
    {
        private IInputService _input;

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
        public int MaxItemAltitude { get; internal set; }
        public int MaxTerrainAltitude { get; internal set; }

        public World(Game game)
        {
            _input = game.Services.GetService<IInputService>();
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
                MapTile t = Map.GetMapTile(CenterPosition.Draw_TileX, CenterPosition.Draw_TileY, true);

                MaxItemAltitude = 255;
                MaxTerrainAltitude = 255;

                if (t != null)
                {
                    bool isUnderRoof, isUnderTerrain;
                    t.IsUnder(CenterPosition.Z, out isUnderRoof, out isUnderTerrain);
                    if (isUnderRoof)
                        MaxItemAltitude = CenterPosition.Z - (CenterPosition.Z % 20) + 20;
                    if (isUnderTerrain)
                        MaxTerrainAltitude = -255;
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

            MapObject mapObject;
            List<MapObject> mapObjects;
            Texture2D texture;

            VertexPositionNormalTextureHue[] vectorBuffer;
            Vector3 drawPosition = new Vector3();
            int width, height;
            int drawX, drawY;
            Vector2 hue; // x is the hue. y = 0, no hue. y = 1, total hue.  y = 2, partial hue.
            PickTypes pick;
            bool flip = false;

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
                        drawPosition.Z = SpriteBatch3D.Z;

                        mapObject = mapObjects[i];
                        if (mapObject is MapObjectGround)
                        {
                            if (mapObject.Z >= MaxTerrainAltitude)
                                break;

                            MapObjectGround groundTile = (MapObjectGround)mapObject;
                            if (groundTile.NoDraw)
                                continue;

                            if (groundTile.MustUpdateSurroundings)
                                Map.UpdateSurroundings(groundTile);

                            Data.LandData landData = Data.TileData.LandData[groundTile.ItemID & 0x3FFF];
                            if (landData.TextureID <= 0)
                            {
                                texture = Data.Art.GetLandTexture(groundTile.ItemID);
                                width = height = 44;
                                drawX = 0;
                                drawY = (groundTile.Z << 2);
                                hue = Vector2.Zero;
                                pick = PickTypes.PickGroundTiles;
                                flip = false;
                            }
                            else
                            {
                                // this is an isometric stretched tile and needs a specialized draw routine.
                                texture = Data.Texmaps.GetTexmapTexture(landData.TextureID);

                                _vertexBufferStretched[0].Position = drawPosition;
                                _vertexBufferStretched[0].Position.X += 22;
                                _vertexBufferStretched[0].Position.Y -= (groundTile.Z << 2);

                                _vertexBufferStretched[1].Position = drawPosition;
                                _vertexBufferStretched[1].Position.X += 44;
                                _vertexBufferStretched[1].Position.Y += 22 - (groundTile.Surroundings.East << 2);

                                _vertexBufferStretched[2].Position = drawPosition;
                                _vertexBufferStretched[2].Position.Y += 22 - (groundTile.Surroundings.South << 2);

                                _vertexBufferStretched[3].Position = drawPosition;
                                _vertexBufferStretched[3].Position.X += 22;
                                _vertexBufferStretched[3].Position.Y += 44 - (groundTile.Surroundings.Down << 2);

                                _vertexBufferStretched[0].Normal = groundTile.Normals[0];
                                _vertexBufferStretched[1].Normal = groundTile.Normals[1];
                                _vertexBufferStretched[2].Normal = groundTile.Normals[2];
                                _vertexBufferStretched[3].Normal = groundTile.Normals[3];

                                hue = Vector2.Zero;
                                if (_vertexBufferStretched[0].Hue != hue)
                                {
                                    _vertexBufferStretched[0].Hue =
                                    _vertexBufferStretched[1].Hue =
                                    _vertexBufferStretched[2].Hue =
                                    _vertexBufferStretched[3].Hue = hue;
                                }

                                if (!_spriteBatch.Draw(texture, _vertexBufferStretched))
                                    continue;

                                if (((PickType & PickTypes.PickGroundTiles) == PickTypes.PickGroundTiles) || ClientVars.DEBUG_HighlightMouseOverObjects)
                                    if (MouseOverList.IsPointInObject(_vertexBufferStretched, _input.MousePosition))
                                    {
                                        MouseOverItem item = new MouseOverItem(texture, _vertexBufferStretched[0].Position, mapObject);
                                        item.Vertices = new Vector3[4] { _vertexBufferStretched[0].Position, _vertexBufferStretched[1].Position, _vertexBufferStretched[2].Position, _vertexBufferStretched[3].Position };
                                        overList.Add2DItem(item);
                                    }

                                SpriteBatch3D.Z += 1000;
                                ObjectsRendered++;

                                continue;
                            }
                        }
                        else if (mapObject is MapObjectStatic) // StaticItem
                        {
                            if (mapObject.Z >= MaxItemAltitude)
                                continue;

                            MapObjectStatic item = (MapObjectStatic)mapObject;
                            if (item.NoDraw)
                                continue;
                            texture = Data.Art.GetStaticTexture(item.ItemID);
                            width = texture.Width;
                            height = texture.Height;
                            drawX = (width >> 1) - 22;
                            drawY = (item.Z << 2) + height - 44;
                            hue = Vector2.Zero;
                            pick = PickTypes.PickStatics;
                            flip = false;
                        }
                        else if (mapObject is MapObjectMobile)
                        {
                            if (mapObject.Z >= MaxItemAltitude)
                                continue;

                            MapObjectMobile item = (MapObjectMobile)mapObject;
                            Data.FrameXNA[] iFrames = Data.AnimationsXNA.GetAnimation(item.BodyID, item.Action, item.Facing, item.Hue);
                            if (iFrames == null)
                                continue;
                            int iFrame = item.Frame(iFrames.Length);
                            if (iFrames[iFrame].Texture == null)
                                continue;
                            texture = iFrames[iFrame].Texture;
                            width = texture.Width;
                            height = texture.Height;

                            flip = (item.Facing > 4) ? true : false;
                            if (flip)
                            {
                                drawX = iFrames[iFrame].Center.X - 22 + (int)((item.Position.Draw_Xoffset - item.Position.Draw_Yoffset) * 22);
                                drawY = iFrames[iFrame].Center.Y + (item.Z << 2) + height - 22 - (int)((item.Position.Draw_Xoffset + item.Position.Draw_Yoffset) * 22);
                            }
                            else
                            {
                                drawX = iFrames[iFrame].Center.X - 22 - (int)((item.Position.Draw_Xoffset - item.Position.Draw_Yoffset) * 22);
                                drawY = iFrames[iFrame].Center.Y + (item.Z << 2) + height - 22 - (int)((item.Position.Draw_Xoffset + item.Position.Draw_Yoffset) * 22);
                            }

                            hue = getHueVector(item.Hue);
                            if (ClientVars.LastTarget != null && ClientVars.LastTarget == item.OwnerSerial)
                                hue = new Vector2(((Entities.Mobile)item.OwnerEntity).NotorietyHue - 1, 1);
                            pick = PickTypes.PickObjects;
                        }
                        else if (mapObject is MapObjectCorpse)
                        {
                            if (mapObject.Z >= MaxItemAltitude)
                                continue;

                            MapObjectCorpse item = (MapObjectCorpse)mapObject;
                            Data.FrameXNA[] iFrames = Data.AnimationsXNA.GetAnimation(item.BodyID, Data.BodyConverter.DeathAnimationIndex(item.BodyID), item.Facing, item.Hue);
                            if (iFrames == null)
                                continue;
                            int iFrame = item.FrameIndex;
                            if (iFrames[iFrame].Texture == null)
                                continue;
                            texture = iFrames[iFrame].Texture;
                            width = texture.Width;
                            height = texture.Height;
                            drawX = iFrames[iFrame].Center.X - 22;
                            drawY = iFrames[iFrame].Center.Y + (item.Z << 2) + height - 22;
                            hue = getHueVector(item.Hue);
                            pick = PickTypes.PickObjects;
                            flip = false;
                        }
                        else if (mapObject is MapObjectItem)
                        {
                            if (mapObject.Z >= MaxItemAltitude)
                                continue;

                            MapObjectItem item = (MapObjectItem)mapObject;
                            texture = Data.Art.GetStaticTexture(item.ItemID);
                            width = texture.Width;
                            height = texture.Height;
                            drawX = (width >> 1) - 22;
                            drawY = (item.Z << 2) + height - 44;
                            hue = getHueVector(item.Hue);
                            pick = PickTypes.PickObjects;
                            flip = false;
                        }
                        else if (mapObject is MapObjectText)
                        {
                            if (mapObject.Z >= MaxItemAltitude)
                                continue;

                            MapObjectText item = (MapObjectText)mapObject;
                            texture = item.Texture;
                            width = texture.Width;
                            height = texture.Height;
                            drawX = (width >> 1) - 22 - (int)((item.Position.Draw_Xoffset - item.Position.Draw_Yoffset) * 22);
                            drawY = ((int)item.Position.Draw_Zoffset << 2) + height - 44 - (int)((item.Position.Draw_Xoffset + item.Position.Draw_Yoffset) * 22);
                            hue = getHueVector(item.Hue);
                            pick = PickTypes.PickObjects;
                            flip = false;
                        }
                        else
                        {
                            continue; // unknown object type, skip it.
                        }

                        if (flip)
                        {
                            vectorBuffer = VertexPositionNormalTextureHue.PolyBufferFlipped;
                            vectorBuffer[0].Position = drawPosition;
                            vectorBuffer[0].Position.X += drawX + 44;
                            vectorBuffer[0].Position.Y -= drawY;

                            vectorBuffer[1].Position = vectorBuffer[0].Position;
                            vectorBuffer[1].Position.Y += height;

                            vectorBuffer[2].Position = vectorBuffer[0].Position;
                            vectorBuffer[2].Position.X -= width;

                            vectorBuffer[3].Position = vectorBuffer[1].Position;
                            vectorBuffer[3].Position.X -= width;
                        }
                        else
                        {
                            vectorBuffer = VertexPositionNormalTextureHue.PolyBuffer;
                            vectorBuffer[0].Position = drawPosition;
                            vectorBuffer[0].Position.X -= drawX;
                            vectorBuffer[0].Position.Y -= drawY;

                            vectorBuffer[1].Position = vectorBuffer[0].Position;
                            vectorBuffer[1].Position.X += width;

                            vectorBuffer[2].Position = vectorBuffer[0].Position;
                            vectorBuffer[2].Position.Y += height;

                            vectorBuffer[3].Position = vectorBuffer[1].Position;
                            vectorBuffer[3].Position.Y += height;
                        }

                        if (vectorBuffer[0].Hue != hue)
                        {
                            vectorBuffer[0].Hue =
                            vectorBuffer[1].Hue =
                            vectorBuffer[2].Hue =
                            vectorBuffer[3].Hue = hue;
                        }

                        if (!_spriteBatch.Draw(texture, vectorBuffer))
                            continue;

                        if ((PickType & pick) == pick)
                        {
                            if (((!flip) && MouseOverList.IsPointInObject(vectorBuffer[0].Position, vectorBuffer[3].Position, _input.MousePosition)) ||
                                ((flip) && MouseOverList.IsPointInObject(vectorBuffer[2].Position, vectorBuffer[1].Position, _input.MousePosition)))
                            {
                                MouseOverItem item; 
                                if (!flip)
                                {
                                    item = new MouseOverItem(texture, vectorBuffer[0].Position, mapObject);
                                    item.Vertices = new Vector3[4] { vectorBuffer[0].Position, vectorBuffer[1].Position, vectorBuffer[2].Position, vectorBuffer[3].Position };
                                }
                                else
                                {
                                    item = new MouseOverItem(texture, vectorBuffer[2].Position, mapObject);
                                    item.Vertices = new Vector3[4] { vectorBuffer[2].Position, vectorBuffer[0].Position, vectorBuffer[3].Position, vectorBuffer[1].Position };
                                }
                                overList.Add2DItem(item);
                            }
                        }

                        SpriteBatch3D.Z += 1000;
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

        WorldTexture[] worldTextures;

        private void render_map()
        {
            if (worldTextures == null)
            {
                worldTextures = new WorldTexture[(int)Math.Ceiling((float)Map.Width / 64f) * (int)Math.Ceiling((float)Map.Height / 64f)];
            }

            bool loadedCell = false;
            int rendersize = 128;

            for (int iy = 0; iy < 1; iy++)
            {
                for (int ix = 0; ix < 1; ix++)
                {
                    int x = ix * 8 + (CenterPosition.Draw_TileX >> 6) * 8;
                    int y = iy * 8 + (CenterPosition.Draw_TileY >> 6) * 8;
                    int index = (y >> 3) * (int)Math.Ceiling((float)Map.Width / 64f) + (x >> 3);
                    WorldTexture t = worldTextures[index];
                    if (t == null || true)
                    {
                        if (loadedCell)
                            continue;
                        else
                        {
                            t = worldTextures[index] = new WorldTexture(_spriteBatch.Game.GraphicsDevice, Map, x, y);
                            loadedCell = true;
                        }
                    }

                    VertexPositionNormalTextureHue[] v = VertexPositionNormalTextureHue.PolyBuffer;

                    v[0].Position = new Vector3((ix + 1) * rendersize, iy * rendersize, SpriteBatch3D.Z);

                    v[1].Position = v[0].Position;
                    v[1].Position.X += rendersize;

                    v[2].Position = v[0].Position;
                    v[2].Position.Y += rendersize;

                    v[3].Position = v[1].Position;
                    v[3].Position.Y += rendersize;

                    _spriteBatch.Draw(t.Texture(), v);
                }
            }
        }

        private bool onScreen(ref float left, ref float top, ref float right, ref float bottom)
        {
            if (bottom < 0)
                return false;
            if (right < 0)
                return false;
            if (top > ClientVars.BackBufferHeight)
                return false;
            if (left > ClientVars.BackBufferWidth)
                return false;
            return true;
        }

        private Vector2 getHueVector(int hue)
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