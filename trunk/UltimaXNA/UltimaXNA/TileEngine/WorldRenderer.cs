/***************************************************************************
 *   TileEngine.cs
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
using UltimaXNA.Input;
#endregion

namespace UltimaXNA.TileEngine
{
    static class WorldRenderer
    {
        public static MiniMap MiniMap { get; internal set; }
        public static int ObjectsRendered { get; internal set; }
        private static SpriteBatch3D _spriteBatch;
        private static VertexPositionNormalTextureHue[] _vertexBuffer;
        private static VertexPositionNormalTextureHue[] _vertexBufferForStretchedTile;
        private static bool _isFirstUpdate = true; // This variable is used to skip the first 'update' cycle.

        // Used for mousepicking.
        public static MapObject MouseOverObject { get; internal set; }
        public static MapObject MouseOverGroundTile { get; internal set; }
        public static PickTypes PickType { get; set; }
        private static RayPicker _rayPicker;

	    // lightning variables
        private static int _personalLightning;
        private static int _overallLightning;

        public static int PersonalLightning
	    {
		    get { return _personalLightning; }
		    set
		    {
			    _personalLightning = value;
			    recalculateLightning();
		    }
	    }

        public static int OverallLightning
	    {
		    get { return _overallLightning; }
		    set
		    {
			    _overallLightning = value;
			    recalculateLightning();
		    }
	    }

        static WorldRenderer()
        {
            const float iUVMin = .01f;
            const float iUVMax = .99f;
            _vertexBuffer = new VertexPositionNormalTextureHue[] {
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector2(0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector2(1, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector2(0, 1)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector2(1, 1))
            };
            _vertexBufferForStretchedTile = new VertexPositionNormalTextureHue[] {
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(), new Vector2(iUVMin, iUVMin)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(), new Vector2(iUVMax, iUVMin)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(), new Vector2(iUVMin, iUVMax)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(), new Vector2(iUVMax, iUVMax))
            };

            
        }

        public static void Initialize(Game game)
        {
            _spriteBatch = new SpriteBatch3D(game);
            _rayPicker = new RayPicker(game);
            if (MiniMap == null)
                MiniMap = new MiniMap(game.GraphicsDevice, World.Map);
        }

        public static void Draw(GameTime gameTime)
        {
            if (!GameState.InWorld)
                return;

            _spriteBatch.Flush();
            if (MouseOverGroundTile != null)
                _rayPicker.DrawPickedTriangle(_spriteBatch.WorldMatrix);
        }

        public static void SetLightDirection(Vector3 nDirection)
        {
            _spriteBatch.SetLightDirection(nDirection);
        }

        private static void recalculateLightning()
		{
			float light = Math.Min(30 - OverallLightning + PersonalLightning, 30f);
			light = Math.Max ( light, 0 );
			light /= 30; // bring it between 0-1

			// -0.3 corresponds pretty well to the darkest possible light in the original client
			// 0.5 is quite okay for the brightest light.
			// so we'll just interpolate between those two values

			light *= 0.8f;
			light -= 0.3f;

			// i'd use a fixed lightning direction for now - maybe enable this effect with a custom packet?
			Vector3 lightDirection = new Vector3 ( 0f, (float)-Math.Cos ( 0.7 ), (float)Math.Sin ( 0.7 ) );

			_spriteBatch.SetLightDirection ( lightDirection );

			// again some guesstimated values, but to me it looks okay this way :) 
			_spriteBatch.SetAmbientLightIntensity ( light * 0.8f );
			_spriteBatch.SetDirectionalLightIntensity ( light );
		}


        public static void Update(GameTime gameTime)
        {
            if (_isFirstUpdate)
            {
                _isFirstUpdate = false;
                return;
            }

            if (!GameState.InWorld)
                return;

            Vector3 drawPosition = new Vector3();
            float drawX, drawY, drawZ = 0;
            int width, height;
            Map map = World.Map;
            MapObject mapObject;
            MapObject[] mapObjects;
            Texture2D texture;

            ObjectsRendered = 0; // Count of objects rendered for statistics and debug
            // List of items for mouse over
            MouseOverList iMouseOverList = new MouseOverList();
            int MaxRoofAltitude = World.MaxRoofAltitude;

            // Now determine where to draw. First retrieve the position of the center object.
            Entities.DrawPosition iDrawPosition = EntitiesCollection.GetPlayerObject().Movement.DrawPosition;
            
            float xOffset = (GameState.BackBufferWidth / 2) - 22;
            float yOffset = (GameState.BackBufferHeight / 2) - ((map.GameSize * 44) / 2);
            yOffset += ((float)iDrawPosition.TileZ + iDrawPosition.OffsetZ) * 4;
            xOffset -= (int)((iDrawPosition.OffsetX - iDrawPosition.OffsetY) * 22);
            yOffset -= (int)((iDrawPosition.OffsetX + iDrawPosition.OffsetY) * 22);

            // If we are going to be checking for GroundTiles, Clear the RayPicker.
            // If we're not, then make sure we clear the last picked ground tile.
            if ((PickType & PickTypes.PickGroundTiles) == PickTypes.PickGroundTiles)
                _rayPicker.FlushObjects();
            else
                MouseOverGroundTile = null;

            foreach(MapCell mapCell in map.m_MapCells.Values)
            {
                int x = mapCell.X - World.RenderBeginX;
                int y = mapCell.Y - World.RenderBeginY;
                drawPosition.X = (x - y) * 22 + xOffset;
                drawPosition.Y = (x + y) * 22 + yOffset;
                mapObjects = mapCell.GetSortedObjects();

                for (int i = 0; i < mapObjects.Length; i++)
                {
                    mapObject = mapObjects[i];

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

                            _spriteBatch.Draw(texture, _vertexBuffer);
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

                            _vertexBufferForStretchedTile[0].Hue = Vector2.Zero;
                            _vertexBufferForStretchedTile[1].Hue = Vector2.Zero;
                            _vertexBufferForStretchedTile[2].Hue = Vector2.Zero;
                            _vertexBufferForStretchedTile[3].Hue = Vector2.Zero;

                            _spriteBatch.Draw(texture, _vertexBufferForStretchedTile);

                            if ((PickType & PickTypes.PickGroundTiles) == PickTypes.PickGroundTiles)
                                _rayPicker.AddObject(mapObject, _vertexBufferForStretchedTile);
                        }
                    }
                    else if (mapObject is MapObjectStatic) // StaticItem
                    {
                        MapObjectStatic staticItem = (MapObjectStatic)mapObject;

                        if (staticItem.Ignored)
                            continue;
                        if (staticItem.Z >= MaxRoofAltitude)
                            continue;

                        Data.Art.GetStaticDimensions(staticItem.ItemID, out width, out height);

                        texture = Data.Art.GetStaticTexture(staticItem.ItemID);

                        drawX = (width >> 1) - 22;
                        drawY = (staticItem.Z << 2) + height - 44;

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

                        _vertexBuffer[0].Hue = Vector2.Zero;
                        _vertexBuffer[1].Hue = Vector2.Zero;
                        _vertexBuffer[2].Hue = Vector2.Zero;
                        _vertexBuffer[3].Hue = Vector2.Zero;

                        _spriteBatch.Draw(texture, _vertexBuffer);

                        if ((PickType & PickTypes.PickStatics) == PickTypes.PickStatics)
                            if (isMouseOverObject(_vertexBuffer[0].Position, _vertexBuffer[3].Position))
                                iMouseOverList.AddItem(new MouseOverItem(texture, _vertexBuffer[0].Position, mapObject));
                    }
                    else if (mapObject is MapObjectMobile) // Mobile
                    {
                        MapObjectMobile iMobile = (MapObjectMobile)mapObject;

                        if (iMobile.Z >= MaxRoofAltitude)
                            continue;

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
                        if (GameState.LastTarget != null)
                            if (GameState.LastTarget == iMobile.OwnerSerial)
                                hueVector = new Vector2(((Entities.Mobile)iMobile.OwnerEntity).NotorietyHue - 1, 1);

                        _vertexBuffer[0].Hue = hueVector;
                        _vertexBuffer[1].Hue = hueVector;
                        _vertexBuffer[2].Hue = hueVector;
                        _vertexBuffer[3].Hue = hueVector;

                        _spriteBatch.Draw(iFrames[iFrame].Texture, _vertexBuffer);

                        if ((PickType & PickTypes.PickObjects) == PickTypes.PickObjects)
                            if (isMouseOverObject(_vertexBuffer[0].Position, _vertexBuffer[3].Position))
                                iMouseOverList.AddItem(new MouseOverItem(iFrames[iFrame].Texture, _vertexBuffer[0].Position, mapObject));
                    }
                    else if (mapObject is MapObjectCorpse)
                    {
                        MapObjectCorpse iObject = (MapObjectCorpse)mapObject;
                        if (iObject.Z >= MaxRoofAltitude)
                            continue;

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
                        Vector2 hueVector = getHueVector(iObject.Hue);
                        _vertexBuffer[0].Hue = hueVector;
                        _vertexBuffer[1].Hue = hueVector;
                        _vertexBuffer[2].Hue = hueVector;
                        _vertexBuffer[3].Hue = hueVector;

                        _spriteBatch.Draw(texture, _vertexBuffer);

                        if ((PickType & PickTypes.PickStatics) == PickTypes.PickStatics)
                            if (isMouseOverObject(_vertexBuffer[0].Position, _vertexBuffer[3].Position))
                                iMouseOverList.AddItem(new MouseOverItem(texture, _vertexBuffer[0].Position, mapObject));
                    }
                    else if (mapObject is MapObjectItem)
                    {
                        MapObjectItem iObject = (MapObjectItem)mapObject;
                        if (iObject.Z >= MaxRoofAltitude)
                            continue;

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
                        Vector2 hueVector = getHueVector(iObject.Hue);
                        _vertexBuffer[0].Hue = hueVector;
                        _vertexBuffer[1].Hue = hueVector;
                        _vertexBuffer[2].Hue = hueVector;
                        _vertexBuffer[3].Hue = hueVector;

                        _spriteBatch.Draw(texture, _vertexBuffer);

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

                        Vector2 hueVector = getHueVector(textObject.Hue);
                        _vertexBuffer[0].Hue = hueVector;
                        _vertexBuffer[1].Hue = hueVector;
                        _vertexBuffer[2].Hue = hueVector;
                        _vertexBuffer[3].Hue = hueVector;

                        _spriteBatch.Draw(texture, _vertexBuffer);
                    }

                    drawZ += 1000;
                    ObjectsRendered++;
                }
            }
            MouseOverObject = iMouseOverList.GetForemostMouseOverItem(InputHandler.Mouse.Position);

            if ((PickType & PickTypes.PickGroundTiles) == PickTypes.PickGroundTiles)
                if (_rayPicker.PickTest(InputHandler.Mouse.Position, Matrix.Identity, _spriteBatch.WorldMatrix))
                    MouseOverGroundTile = _rayPicker.pickedObject;
        }

        private static Vector2 getHueVector(int hue)
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

        private static bool isMouseOverObject(Vector3 iMin, Vector3 iMax)
        {
            // Added cursor picking -Poplicola 5/19/2009
            BoundingBox iBoundingBox = new BoundingBox(iMin, iMax);
            // Must correct for bounding box being one pixel larger than actual texture.
            iBoundingBox.Max.X--; iBoundingBox.Max.Y--;

            Vector3 iMousePosition = new Vector3(InputHandler.Mouse.Position.X, InputHandler.Mouse.Position.Y, iMin.Z);
            if (iBoundingBox.Contains(iMousePosition) == ContainmentType.Contains)
                return true;
            return false;
        }
    }
}
