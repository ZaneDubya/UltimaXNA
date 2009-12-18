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
    public class World : IWorld
    {
        private IInputService _input;

        #region RenderingVariables
        private SpriteBatch3D _spriteBatch;
        private VertexPositionNormalTextureHue[] _vertexBuffer;
        private VertexPositionNormalTextureHue[] _vertexBufferForStretchedTile;
        private bool _isFirstUpdate = true; // This variable is used to skip the first 'update' cycle.
        #endregion

        #region MousePickingVariables
        public MapObject MouseOverObject { get; internal set; }
        public MapObject MouseOverGroundTile { get; internal set; }
        public PickTypes PickType { get; set; }
        public bool DEBUG_DrawTileOver { get; set; }
        private RayPicker _rayPicker;
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
        public bool DEBUG_DrawDebug { get; set; }
        public DrawPosition CenterPosition { get; set; }
        private DrawPosition _lastCenterPosition = null;
        public int RenderBeginX { get; set; }
        public int RenderBeginY { get; set; }
        public int MaxRoofAltitude { get; internal set; }

        public World(Game game)
        {
            Map = new Map(0, 40, 0, 0);

            _input = game.Services.GetService<IInputService>();
            _spriteBatch = new SpriteBatch3D(game);
            _rayPicker = new RayPicker(game);

            PickType = PickTypes.PickNothing;
            DEBUG_DrawTileOver = false;
            const float iUVMin = .01f;
            const float iUVMax = .99f;
            _vertexBuffer = new VertexPositionNormalTextureHue[] {
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector3(0, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector3(1, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector3(0, 1, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector3(1, 1, 0))
            };
            _vertexBufferForStretchedTile = new VertexPositionNormalTextureHue[] {
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(), new Vector3(iUVMin, iUVMin, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(), new Vector3(iUVMax, iUVMin, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(), new Vector3(iUVMin, iUVMax, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(), new Vector3(iUVMax, iUVMax, 0))
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
            if (CenterPosition != _lastCenterPosition)
            {
                RenderBeginX = CenterPosition.TileX - Map.GameSize / 2;
                RenderBeginY = CenterPosition.TileY - Map.GameSize / 2;
                Map.Update(CenterPosition.TileX, CenterPosition.TileY);
                // Are we inside (under a roof)? Do not draw tiles above our head.
                if (Map.GetMapCell(CenterPosition.TileX, CenterPosition.TileY).UnderRoof(CenterPosition.TileZ))
                {
                    MaxRoofAltitude = CenterPosition.TileZ + 20;
                }
                else
                {
                    MaxRoofAltitude = 255;
                }
            }

            _lastCenterPosition = new DrawPosition(CenterPosition);
            updateRenderer();
        }

        public void Draw(GameTime gameTime)
        {
            _spriteBatch.FlushOld(true);
            if (MouseOverGroundTile != null && DEBUG_DrawTileOver)
                _rayPicker.DrawPickedTriangle(_spriteBatch.WorldMatrix);
        }

        private void updateRenderer()
        {
            if (_isFirstUpdate)
            {
                _isFirstUpdate = false;
                return;
            }

            Vector3 drawPosition = new Vector3();
            float drawX, drawY, drawZ = 0;
            int width, height;
            MapObject mapObject;
            List<MapObject> mapObjects;
            Texture2D texture;

            ObjectsRendered = 0; // Count of objects rendered for statistics and debug
            // List of items for mouse over
            MouseOverList iMouseOverList = new MouseOverList();

            float xOffset = (GameState.BackBufferWidth / 2) - 22;
            float yOffset = (GameState.BackBufferHeight / 2) - ((Map.GameSize * 44) / 2);
            yOffset += ((float)CenterPosition.TileZ + CenterPosition.OffsetZ) * 4;
            xOffset -= (int)((CenterPosition.OffsetX - CenterPosition.OffsetY) * 22);
            yOffset -= (int)((CenterPosition.OffsetX + CenterPosition.OffsetY) * 22);

            // If we are going to be checking for GroundTiles, Clear the RayPicker.
            // If we're not, then make sure we clear the last picked ground tile.
            if ((PickType & PickTypes.PickGroundTiles) == PickTypes.PickGroundTiles)
                _rayPicker.FlushObjects();
            else
                MouseOverGroundTile = null;

            foreach (MapCell mapCell in Map.m_MapCells.Values)
            {
                int x = mapCell.X - RenderBeginX;
                int y = mapCell.Y - RenderBeginY;
                drawPosition.X = (x - y) * 22 + xOffset;
                drawPosition.Y = (x + y) * 22 + yOffset;
                mapObjects = mapCell.GetSortedObjects();

                for (int i = 0; i < mapObjects.Count; i++)
                {
                    mapObject = mapObjects[i];
                    if (mapObject.Z >= MaxRoofAltitude)
                        continue;

                    if (mapObject is MapObjectGround) // GroundTile
                    {
                        MapObjectGround groundTile = (MapObjectGround)mapObject;

                        if (groundTile.Ignored)
                        {
                            continue;
                        }

                        drawY = groundTile.Z << 2;

                        Data.LandData landData = Data.TileData.LandData[groundTile.ItemID & 0x3FFF];

                        if (landData.TextureID <= 0 || landData.Wet) // Not Stretched
                        {
                            texture = Data.Art.GetLandTexture(groundTile.ItemID);

                            _vertexBuffer[0].Position = drawPosition;
                            _vertexBuffer[0].Position.Y -= drawY;
                            _vertexBuffer[0].Position.Z = drawZ;

                            _vertexBuffer[1].Position = drawPosition;
                            _vertexBuffer[1].Position.X += 44;
                            _vertexBuffer[1].Position.Y -= drawY;
                            _vertexBuffer[1].Position.Z = drawZ;

                            _vertexBuffer[2].Position = drawPosition;
                            _vertexBuffer[2].Position.Y += 44 - drawY;
                            _vertexBuffer[2].Position.Z = drawZ;

                            _vertexBuffer[3].Position = drawPosition;
                            _vertexBuffer[3].Position.X += 44;
                            _vertexBuffer[3].Position.Y += 44 - drawY;
                            _vertexBuffer[3].Position.Z = drawZ;

                            _vertexBuffer[0].Hue = Vector2.Zero;
                            _vertexBuffer[1].Hue = Vector2.Zero;
                            _vertexBuffer[2].Hue = Vector2.Zero;
                            _vertexBuffer[3].Hue = Vector2.Zero;

                            if (!_spriteBatch.Draw(texture, _vertexBuffer))
                                continue;
                        }
                        else // Stretched
                        {
                            texture = Data.Texmaps.GetTexmapTexture(landData.TextureID);

                            _vertexBufferForStretchedTile[0].Position = drawPosition;
                            _vertexBufferForStretchedTile[0].Position.X += 22;

                            _vertexBufferForStretchedTile[1].Position = drawPosition;
                            _vertexBufferForStretchedTile[1].Position.X += 44;
                            _vertexBufferForStretchedTile[1].Position.Y += 22;

                            _vertexBufferForStretchedTile[2].Position = drawPosition;
                            _vertexBufferForStretchedTile[2].Position.Y += 22;

                            _vertexBufferForStretchedTile[3].Position = drawPosition;
                            _vertexBufferForStretchedTile[3].Position.X += 22;
                            _vertexBufferForStretchedTile[3].Position.Y += 44;

                            _vertexBufferForStretchedTile[0].Normal = groundTile.Normals[0];
                            _vertexBufferForStretchedTile[1].Normal = groundTile.Normals[1];
                            _vertexBufferForStretchedTile[2].Normal = groundTile.Normals[2];
                            _vertexBufferForStretchedTile[3].Normal = groundTile.Normals[3];

                            _vertexBufferForStretchedTile[0].Position.Y -= drawY;
                            _vertexBufferForStretchedTile[1].Position.Y -= (groundTile.Surroundings.East << 2);
                            _vertexBufferForStretchedTile[2].Position.Y -= (groundTile.Surroundings.South << 2);
                            _vertexBufferForStretchedTile[3].Position.Y -= (groundTile.Surroundings.Down << 2);

                            _vertexBufferForStretchedTile[0].Position.Z = drawZ;
                            _vertexBufferForStretchedTile[1].Position.Z = drawZ;
                            _vertexBufferForStretchedTile[2].Position.Z = drawZ;
                            _vertexBufferForStretchedTile[3].Position.Z = drawZ;

                            _vertexBufferForStretchedTile[0].Hue =
                                _vertexBufferForStretchedTile[1].Hue =
                                _vertexBufferForStretchedTile[2].Hue =
                                _vertexBufferForStretchedTile[3].Hue = Vector2.Zero;

                            if (!_spriteBatch.Draw(texture, _vertexBufferForStretchedTile))
                                continue;

                            if ((PickType & PickTypes.PickGroundTiles) == PickTypes.PickGroundTiles)
                                _rayPicker.AddObject(mapObject, _vertexBufferForStretchedTile);
                        }
                    }
                    else if (mapObject is MapObjectStatic) // StaticItem
                    {
                        MapObjectStatic staticItem = (MapObjectStatic)mapObject;

                        if (staticItem.Ignored)
                            continue;

                        Data.Art.GetStaticDimensions(staticItem.ItemID, out width, out height);

                        texture = Data.Art.GetStaticTexture(staticItem.ItemID);

                        drawX = (width >> 1) - 22;
                        drawY = (staticItem.Z << 2) + height - 44;

                        float drawLeft = drawPosition.X - drawX;
                        float drawTop = drawPosition.Y - drawY;
                        float drawRight = drawLeft + width;
                        float drawBottom = drawTop + height;

                        _vertexBuffer[0].Position.X = drawLeft;
                        _vertexBuffer[0].Position.Y = drawTop;
                        _vertexBuffer[0].Position.Z = drawZ;

                        _vertexBuffer[1].Position.X = drawRight;
                        _vertexBuffer[1].Position.Y = drawTop;
                        _vertexBuffer[1].Position.Z = drawZ;

                        _vertexBuffer[2].Position.X = drawLeft;
                        _vertexBuffer[2].Position.Y = drawBottom;
                        _vertexBuffer[2].Position.Z = drawZ;

                        _vertexBuffer[3].Position.X = drawRight;
                        _vertexBuffer[3].Position.Y = drawBottom;
                        _vertexBuffer[3].Position.Z = drawZ;

                        _vertexBuffer[0].Hue =
                            _vertexBuffer[1].Hue =
                            _vertexBuffer[2].Hue =
                            _vertexBuffer[3].Hue = Vector2.Zero;

                        if (!_spriteBatch.Draw(texture, _vertexBuffer))
                            continue;

                        if ((PickType & PickTypes.PickStatics) == PickTypes.PickStatics)
                            if (isMouseOverObject(_vertexBuffer[0].Position, _vertexBuffer[3].Position))
                                iMouseOverList.AddItem(new MouseOverItem(texture, _vertexBuffer[0].Position, mapObject));
                    }
                    else if (mapObject is MapObjectMobile) // Mobile
                    {
                        MapObjectMobile iMobile = (MapObjectMobile)mapObject;

                        Data.FrameXNA[] iFrames = Data.AnimationsXNA.GetAnimation(iMobile.BodyID, iMobile.Action, iMobile.Facing, iMobile.Hue, false);
                        if (iFrames == null)
                            continue;
                        int iFrame = iMobile.Frame(iFrames.Length);
                        if (iFrames[iFrame].Texture == null)
                            continue;

                        width = iFrames[iFrame].Texture.Width;
                        height = iFrames[iFrame].Texture.Height;
                        drawX = iFrames[iFrame].Center.X - 22 - (int)((iMobile.Offset.X - iMobile.Offset.Y) * 22);
                        drawY = iFrames[iFrame].Center.Y + (iMobile.Z << 2) + height - 22 - (int)((iMobile.Offset.X + iMobile.Offset.Y) * 22);

                        _vertexBuffer[0].Position = drawPosition;
                        _vertexBuffer[0].Position.X -= drawX;
                        _vertexBuffer[0].Position.Y -= drawY;
                        _vertexBuffer[0].Position.Z = drawZ;

                        _vertexBuffer[1].Position = drawPosition;
                        _vertexBuffer[1].Position.X += width - drawX;
                        _vertexBuffer[1].Position.Y -= drawY;
                        _vertexBuffer[1].Position.Z = drawZ;

                        _vertexBuffer[2].Position = drawPosition;
                        _vertexBuffer[2].Position.X -= drawX;
                        _vertexBuffer[2].Position.Y += height - drawY;
                        _vertexBuffer[2].Position.Z = drawZ;

                        _vertexBuffer[3].Position = drawPosition;
                        _vertexBuffer[3].Position.X += width - drawX;
                        _vertexBuffer[3].Position.Y += height - drawY;
                        _vertexBuffer[3].Position.Z = drawZ;

                        // hueVector: x is the hue, y sets whether or not to use it.
                        // y = 1, total hue.
                        // y = 2, partial hue.
                        Vector2 hueVector = getHueVector(iMobile.Hue);
                        if (GameState.LastTarget != null && GameState.LastTarget == iMobile.OwnerSerial)
                            hueVector = new Vector2(((Entities.Mobile)iMobile.OwnerEntity).NotorietyHue - 1, 1);

                        _vertexBuffer[0].Hue =
                            _vertexBuffer[1].Hue =
                            _vertexBuffer[2].Hue =
                            _vertexBuffer[3].Hue = hueVector;

                        if (!_spriteBatch.Draw(iFrames[iFrame].Texture, _vertexBuffer))
                            continue;

                        if ((PickType & PickTypes.PickObjects) == PickTypes.PickObjects)
                            if (isMouseOverObject(_vertexBuffer[0].Position, _vertexBuffer[3].Position))
                                iMouseOverList.AddItem(new MouseOverItem(iFrames[iFrame].Texture, _vertexBuffer[0].Position, mapObject));
                    }
                    else if (mapObject is MapObjectCorpse)
                    {
                        MapObjectCorpse iObject = (MapObjectCorpse)mapObject;

                        Data.FrameXNA[] iFrames = Data.AnimationsXNA.GetAnimation(iObject.BodyID, Data.BodyConverter.DeathAnimationIndex(iObject.BodyID), iObject.Facing, iObject.Hue, false);
                        // GetAnimation fails so it returns null, temporary fix - Smjert
                        if (iFrames == null)
                            continue;
                        int iFrame = iObject.FrameIndex;
                        // If the frame data is corrupt, then the texture will not load. Fix for broken cleaver data, maybe others. --Poplicola 6/15/2009
                        if (iFrames[iFrame].Texture == null)
                            continue;
                        width = iFrames[iFrame].Texture.Width;
                        height = iFrames[iFrame].Texture.Height;
                        drawX = iFrames[iFrame].Center.X - 22;
                        drawY = iFrames[iFrame].Center.Y + (iObject.Z << 2) + height - 22;
                        texture = iFrames[iFrame].Texture;

                        _vertexBuffer[0].Position = drawPosition;
                        _vertexBuffer[0].Position.X -= drawX;
                        _vertexBuffer[0].Position.Y -= drawY;
                        _vertexBuffer[0].Position.Z = drawZ;

                        _vertexBuffer[1].Position = drawPosition;
                        _vertexBuffer[1].Position.X += width - drawX;
                        _vertexBuffer[1].Position.Y -= drawY;
                        _vertexBuffer[1].Position.Z = drawZ;

                        _vertexBuffer[2].Position = drawPosition;
                        _vertexBuffer[2].Position.X -= drawX;
                        _vertexBuffer[2].Position.Y += height - drawY;
                        _vertexBuffer[2].Position.Z = drawZ;

                        _vertexBuffer[3].Position = drawPosition;
                        _vertexBuffer[3].Position.X += width - drawX;
                        _vertexBuffer[3].Position.Y += height - drawY;
                        _vertexBuffer[3].Position.Z = drawZ;

                        // hueVector: x is the hue, y sets whether or not to use it.
                        // y = 1, total hue.
                        // y = 2, partial hue.
                        _vertexBuffer[0].Hue =
                            _vertexBuffer[1].Hue =
                            _vertexBuffer[2].Hue =
                            _vertexBuffer[3].Hue = getHueVector(iObject.Hue);

                        if (!_spriteBatch.Draw(texture, _vertexBuffer))
                            continue;

                        if ((PickType & PickTypes.PickStatics) == PickTypes.PickStatics)
                            if (isMouseOverObject(_vertexBuffer[0].Position, _vertexBuffer[3].Position))
                                iMouseOverList.AddItem(new MouseOverItem(texture, _vertexBuffer[0].Position, mapObject));
                    }
                    else if (mapObject is MapObjectItem)
                    {
                        MapObjectItem iObject = (MapObjectItem)mapObject;

                        Data.Art.GetStaticDimensions(iObject.ItemID, out width, out height);
                        texture = Data.Art.GetStaticTexture(iObject.ItemID);
                        drawX = (width >> 1) - 22;
                        drawY = (iObject.Z << 2) + height - 44;

                        _vertexBuffer[0].Position = drawPosition;
                        _vertexBuffer[0].Position.X -= drawX;
                        _vertexBuffer[0].Position.Y -= drawY;
                        _vertexBuffer[0].Position.Z = drawZ;

                        _vertexBuffer[1].Position = drawPosition;
                        _vertexBuffer[1].Position.X += width - drawX;
                        _vertexBuffer[1].Position.Y -= drawY;
                        _vertexBuffer[1].Position.Z = drawZ;

                        _vertexBuffer[2].Position = drawPosition;
                        _vertexBuffer[2].Position.X -= drawX;
                        _vertexBuffer[2].Position.Y += height - drawY;
                        _vertexBuffer[2].Position.Z = drawZ;

                        _vertexBuffer[3].Position = drawPosition;
                        _vertexBuffer[3].Position.X += width - drawX;
                        _vertexBuffer[3].Position.Y += height - drawY;
                        _vertexBuffer[3].Position.Z = drawZ;

                        // hueVector: x is the hue, y sets whether or not to use it.
                        // y = 1, total hue.
                        // y = 2, partial hue.
                        _vertexBuffer[0].Hue =
                            _vertexBuffer[1].Hue =
                            _vertexBuffer[2].Hue =
                            _vertexBuffer[3].Hue = getHueVector(iObject.Hue);

                        if (!_spriteBatch.Draw(texture, _vertexBuffer))
                            continue;

                        if ((PickType & PickTypes.PickStatics) == PickTypes.PickStatics)
                            if (isMouseOverObject(_vertexBuffer[0].Position, _vertexBuffer[3].Position))
                                iMouseOverList.AddItem(new MouseOverItem(texture, _vertexBuffer[0].Position, mapObject));

                    }
                    else if (mapObject is MapObjectText)
                    {
                        MapObjectText textObject = (MapObjectText)mapObject;
                        texture = textObject.Texture;
                        width = texture.Width;
                        height = texture.Height;

                        drawX = (width >> 1) - 22 - (int)((textObject.Offset.X - textObject.Offset.Y) * 22);
                        drawY = (textObject.Z << 2) + height - 44 - (int)((textObject.Offset.X + textObject.Offset.Y) * 22);

                        _vertexBuffer[0].Position = drawPosition;
                        _vertexBuffer[0].Position.X -= drawX;
                        _vertexBuffer[0].Position.Y -= drawY;
                        _vertexBuffer[0].Position.Z = drawZ;

                        _vertexBuffer[1].Position = drawPosition;
                        _vertexBuffer[1].Position.X += width - drawX;
                        _vertexBuffer[1].Position.Y -= drawY;
                        _vertexBuffer[1].Position.Z = drawZ;

                        _vertexBuffer[2].Position = drawPosition;
                        _vertexBuffer[2].Position.X -= drawX;
                        _vertexBuffer[2].Position.Y += height - drawY;
                        _vertexBuffer[2].Position.Z = drawZ;

                        _vertexBuffer[3].Position = drawPosition;
                        _vertexBuffer[3].Position.X += width - drawX;
                        _vertexBuffer[3].Position.Y += height - drawY;
                        _vertexBuffer[3].Position.Z = drawZ;

                        _vertexBuffer[0].Hue =
                            _vertexBuffer[1].Hue =
                            _vertexBuffer[2].Hue =
                            _vertexBuffer[3].Hue = getHueVector(textObject.Hue);

                        if (!_spriteBatch.Draw(texture, _vertexBuffer))
                            continue;
                    }

                    drawZ += 1000;
                    ObjectsRendered++;
                }
            }
            MouseOverObject = iMouseOverList.GetForemostMouseOverItem(_input.CurrentMousePosition);

            if ((PickType & PickTypes.PickGroundTiles) == PickTypes.PickGroundTiles)
                if (_rayPicker.PickTest(_input.CurrentMousePosition, Matrix.Identity, _spriteBatch.WorldMatrix))
                    MouseOverGroundTile = _rayPicker.pickedObject;
        }

        private bool onScreen(ref float left, ref float top, ref float right, ref float bottom)
        {
            if (bottom < 0)
                return false;
            if (right < 0)
                return false;
            if (top > GameState.BackBufferHeight)
                return false;
            if (left > GameState.BackBufferWidth)
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

        private bool isMouseOverObject(Vector3 iMin, Vector3 iMax)
        {
            // Added cursor picking -Poplicola 5/19/2009
            BoundingBox iBoundingBox = new BoundingBox(iMin, iMax);
            // Must correct for bounding box being one pixel larger than actual texture.
            iBoundingBox.Max.X--; iBoundingBox.Max.Y--;

            Vector3 iMousePosition = new Vector3(_input.CurrentMousePosition.X, _input.CurrentMousePosition.Y, iMin.Z);
            if (iBoundingBox.Contains(iMousePosition) == ContainmentType.Contains)
                return true;
            return false;
        }
    }
}