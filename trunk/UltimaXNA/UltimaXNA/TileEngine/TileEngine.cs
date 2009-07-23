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
#endregion

namespace UltimaXNA.TileEngine
{
    [Flags]
    public enum PickTypes : int
    {
        PickNothing = 0,
        PickObjects = 1,
        PickStatics = 2,
        PickGroundTiles = 4
    }

    

    class TileEngine : DrawableGameComponent, ITileEngine
    {
        public static int startX, startY;
        public MiniMap MiniMap { get; protected set; }
        public int ObjectsRendered { get; protected set; }
        private SpriteBatch3D _spriteBatch;
        private VertexPositionNormalTextureHue[] _vertexBuffer;
        private VertexPositionNormalTextureHue[] _vertexBufferForStretchedTile;
        private bool _isFirstUpdate = true; // This variable is used to skip the first 'update' cycle.

        // Used for mousepicking.
        private IMapObject _mouseOverObject;
        private IMapObject _mouseOverGroundTile;
        private PickTypes _pickType = PickTypes.PickNothing;
        private RayPicker _rayPicker;

        // Reference for services
        private Input.IInputService _inputService;
        private Entities.IEntitiesService _objectsService;
        private IWorld _worldService;
        private IGameState _gameStateService;

        public IMapObject MouseOverObject
        { get { return _mouseOverObject; } }
        public IMapObject MouseOverGroundTile
        { get { return _mouseOverGroundTile; } }
        public PickTypes PickType
        { set { _pickType = value; } }

        public TileEngine(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(ITileEngine), this);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (!_gameStateService.InWorld)
                return;

            _spriteBatch.Flush();
            if (_mouseOverGroundTile != null)
                _rayPicker.DrawPickedTriangle(_spriteBatch.WorldMatrix);
        }

        public void SetLightDirection(Vector3 nDirection)
        {
            _spriteBatch.SetLightDirection(nDirection);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (_isFirstUpdate)
            {
                _isFirstUpdate = false;
                return;
            }

            if (!_gameStateService.InWorld)
                return;

            Vector3 drawPosition = new Vector3();
            float drawX;
            float drawY;
            float drawZ = 0;
            GroundTile groundTile;
            int height;
            Map map = _worldService.Map;
            IMapObject mapObject;
            IMapObject[] mapObjects;
            StaticItem staticItem;
            MobileTile iMobile;
            GameObjectTile iObject;
            Texture2D texture;
            int width;
            // Count of objects rendered for statistics and debug - Poplicola
            ObjectsRendered = 0;
            // List of items for mouse over
            MouseOverList iMouseOverList = new MouseOverList();
            int MaxRoofAltitude = _worldService.MaxRoofAltitude;

            // Now determine where to draw. First retrieve the position of the center object.
            Entities.DrawPosition iDrawPosition = _objectsService.GetPlayerObject().Movement.DrawPosition;
            
            float xOffset = (this.Game.GraphicsDevice.PresentationParameters.BackBufferWidth / 2) - 22;
            float yOffset = (this.Game.GraphicsDevice.PresentationParameters.BackBufferHeight / 2) -
                    ((map.GameSize * 44) / 2);
            yOffset += ((float)iDrawPosition.TileZ + iDrawPosition.OffsetZ) * 4;
            xOffset -= (int)((iDrawPosition.OffsetX - iDrawPosition.OffsetY) * 22);
            yOffset -= (int)((iDrawPosition.OffsetX + iDrawPosition.OffsetY) * 22);

            startX = iDrawPosition.TileX - map.GameSize / 2;
            startY = iDrawPosition.TileY - map.GameSize / 2;

            Random iRand = new Random();

            // If we are going to be checking for GroundTiles, Clear the RayPicker.
            // If we're not, then make sure we clear the last picked ground tile.
            if ((_pickType & PickTypes.PickGroundTiles) == PickTypes.PickGroundTiles)
                _rayPicker.FlushObjects();
            else
                _mouseOverGroundTile = null;

            // MapCell mapCell;
            /*for (int x = 0; x < map.GameSize; x++)
            {
                for (int y = 0; y < map.GameSize; y++)
                {
                    currentX = startX + x;
                    currentY = startY + y;

                    drawPosition.X = (x - y) * 22 + xOffset;
                    drawPosition.Y = (x + y) * 22 + yOffset;

                    mapCell = map.GetMapCell(currentX, currentY);
            */
            {
                foreach(MapCell mapCell in map.m_MapCells.Values)
                {
                    int x = mapCell.X - startX;
                    int y = mapCell.Y - startY;
                    drawPosition.X = (x - y) * 22 + xOffset;
                    drawPosition.Y = (x + y) * 22 + yOffset;
                    mapObjects = mapCell.GetSortedObjects();

                    for (int i = 0; i < mapObjects.Length; i++)
                    {
                        mapObject = mapObjects[i];

                        if (mapObject.Type == MapObjectTypes.GroundTile) // GroundTile
                        {
                            groundTile = (GroundTile)mapObject;

                            if (groundTile.Ignored)
                            {
                                continue;
                            }

                            drawY = groundTile.Z << 2;

                            Data.LandData landData = Data.TileData.LandData[groundTile.ID & 0x3FFF];

                            if (landData.TextureID <= 0 || landData.Wet) // Not Stretched
                            {
                                texture = Data.Art.GetLandTexture(groundTile.ID, this.GraphicsDevice);

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
                                texture = Data.Texmaps.GetTexmapTexture(landData.TextureID, this.GraphicsDevice);

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

                                if ((_pickType & PickTypes.PickGroundTiles) == PickTypes.PickGroundTiles)
                                    _rayPicker.AddObject(mapObject, _vertexBufferForStretchedTile);
                            }
                        }
                        else if (mapObject.Type == MapObjectTypes.StaticTile) // StaticItem
                        {
                            staticItem = (StaticItem)mapObject;

                            if (staticItem.Ignored)
                                continue;
                            if (staticItem.Z >= MaxRoofAltitude)
                                continue;

                            Data.Art.GetStaticDimensions(staticItem.ID, out width, out height);

                            texture = Data.Art.GetStaticTexture(staticItem.ID, this.GraphicsDevice);

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

                            if ((_pickType & PickTypes.PickStatics) == PickTypes.PickStatics)
                                if (isMouseOverObject(_vertexBuffer[0].Position, _vertexBuffer[3].Position))
                                    iMouseOverList.AddItem(new MouseOverItem(texture, _vertexBuffer[0].Position, mapObject));
                        }
                        else if (mapObject.Type == MapObjectTypes.MobileTile) // Mobile
                        {
                            iMobile = (MobileTile)mapObject;

                            if (iMobile.Z >= MaxRoofAltitude)
                                continue;

                            Data.FrameXNA[] iFrames = Data.AnimationsXNA.GetAnimation(this.Game.GraphicsDevice,
                                iMobile.ID, iMobile.Action, iMobile.Direction, iMobile.Hue, false);
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
                            if (_gameStateService.LastTarget != null)
                                if (_gameStateService.LastTarget == iMobile.OwnerSerial)
                                    hueVector = new Vector2(((Entities.Mobile)iMobile.OwnerEntity).NotorietyHue - 1, 1);

                            _vertexBuffer[0].Hue = hueVector;
                            _vertexBuffer[1].Hue = hueVector;
                            _vertexBuffer[2].Hue = hueVector;
                            _vertexBuffer[3].Hue = hueVector;

                            _spriteBatch.Draw(iFrames[iFrame].Texture, _vertexBuffer);

                            if ((_pickType & PickTypes.PickObjects) == PickTypes.PickObjects)
                                if (isMouseOverObject(_vertexBuffer[0].Position, _vertexBuffer[3].Position))
                                    iMouseOverList.AddItem(new MouseOverItem(iFrames[iFrame].Texture, _vertexBuffer[0].Position, mapObject));
                        }
                        else if (mapObject.Type == MapObjectTypes.GameObjectTile)
                        {
                            iObject = (GameObjectTile)mapObject;
                            if (iObject.Z >= MaxRoofAltitude)
                                continue;

                            if (iObject.IsCorpse)
                            {
                                Data.FrameXNA[] iFrames = Data.AnimationsXNA.GetAnimation(this.Game.GraphicsDevice,
                                    iObject.CorpseBody, Data.BodyConverter.DeathAnimationIndex(iObject.CorpseBody), iObject.Direction, iObject.Hue, false);
                                // GetAnimation fails so it returns null, temporary fix - Smjert
                                if (iFrames == null)
                                    continue;
                                int iFrame = iObject.CorpseFrame;
                                // If the frame data is corrupt, then the texture will not load. Fix for broken cleaver data, maybe others. --Poplicola 6/15/2009
                                if (iFrames[iFrame].Texture == null)
                                    continue;
                                width = iFrames[iFrame].Texture.Width;
                                height = iFrames[iFrame].Texture.Height;
                                drawX = iFrames[iFrame].Center.X - 22;
                                drawY = iFrames[iFrame].Center.Y + (iObject.Z << 2) + height - 22;
                                texture = iFrames[iFrame].Texture;
                            }
                            else
                            {
                                Data.Art.GetStaticDimensions(iObject.ID, out width, out height);

                                texture = Data.Art.GetStaticTexture(iObject.ID, this.GraphicsDevice);

                                drawX = (width >> 1) - 22;
                                drawY = (iObject.Z << 2) + height - 44;
                            }
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

                            if ((_pickType & PickTypes.PickStatics) == PickTypes.PickStatics)
                                if (isMouseOverObject(_vertexBuffer[0].Position, _vertexBuffer[3].Position))
                                    iMouseOverList.AddItem(new MouseOverItem(texture, _vertexBuffer[0].Position, mapObject));
                        
                        }

                        drawZ += 1000;
                        ObjectsRendered++;
                    }
                }
            }
            _mouseOverObject = iMouseOverList.GetForemostMouseOverItem(_inputService.Mouse.Position);

            if ((_pickType & PickTypes.PickGroundTiles) == PickTypes.PickGroundTiles)
                if (_rayPicker.PickTest(_inputService.Mouse.Position, Matrix.Identity, _spriteBatch.WorldMatrix))
                    _mouseOverGroundTile = _rayPicker.pickedObject;
        }

        private Vector2 getHueVector(int hue)
        {
            if (hue == 0)
                return new Vector2(0);

            int hueType = 1;
            if ((hue & 0x8000) != 0) // partial hue
                hueType = 2;

            return new Vector2(hue & 0x7FFF - 1, hueType);
        }

        private bool isMouseOverObject(Vector3 iMin, Vector3 iMax)
        {
            // Added cursor picking -Poplicola 5/19/2009
            BoundingBox iBoundingBox = new BoundingBox(iMin, iMax);
            // Must correct for bounding box being one pixel larger than actual texture.
            iBoundingBox.Max.X--; iBoundingBox.Max.Y--;

            Vector3 iMousePosition = new Vector3(_inputService.Mouse.Position.X, _inputService.Mouse.Position.Y, iMin.Z);
            if (iBoundingBox.Contains(iMousePosition) == ContainmentType.Contains)
                return true;
            return false;
        }

        public override void Initialize()
        {
            base.Initialize();

            _inputService = (Input.IInputService)Game.Services.GetService(typeof(Input.IInputService));
            _objectsService = (Entities.IEntitiesService)Game.Services.GetService(typeof(Entities.IEntitiesService));
            _worldService = (IWorld)Game.Services.GetService(typeof(IWorld));
            _gameStateService = (IGameState)Game.Services.GetService(typeof(IGameState));

            _vertexBuffer = new VertexPositionNormalTextureHue[] {
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector2(0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector2(1, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector2(0, 1)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector2(1, 1))
            };
            const float iUVMin = .01f;
            const float iUVMax = .99f;
            _vertexBufferForStretchedTile = new VertexPositionNormalTextureHue[] {
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(), new Vector2(iUVMin, iUVMin)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(), new Vector2(iUVMax, iUVMin)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(), new Vector2(iUVMin, iUVMax)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(), new Vector2(iUVMax, iUVMax))
            };

            if (MiniMap == null)
                MiniMap = new MiniMap(Game.GraphicsDevice, _worldService.Map);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            _spriteBatch = new SpriteBatch3D(this.Game);
            _rayPicker = new RayPicker(this.Game);
        }
    }

    class MouseOverList
    {
        List<MouseOverItem> m_List;

        public MouseOverList()
        {
            m_List = new List<MouseOverItem>();
        }

        public IMapObject GetForemostMouseOverItem(Vector2 nMousePosition)
        {
            // Parse list backwards to find topmost mouse over object.
            foreach (MouseOverItem iItem in CreateReverseIterator(m_List))
            {
                UInt16[] iPixel = new UInt16[1];
                iItem.Texture.GetData<UInt16>(0,
                    new Rectangle((int)nMousePosition.X - (int)iItem.Position.X, (int)nMousePosition.Y - (int)iItem.Position.Y, 1, 1), 
                    iPixel, 0, 1);
                if (iPixel[0] != 0)
                    return iItem.Object;
            }
            return null;
        }

        static IEnumerable<MouseOverItem> CreateReverseIterator<MouseOverItem>(IList<MouseOverItem> list)
        {
            int count = list.Count;
            for (int i = count - 1; i >= 0; --i)
            {
                yield return list[i];
            }
        }

        public void AddItem(MouseOverItem nItem)
        {
            m_List.Add(nItem);
        }
    }

    class MouseOverItem
    {
        public Texture2D Texture;
        public Vector3 Position;
        public IMapObject Object;

        public MouseOverItem(Texture2D nTexture, Vector3 nPosition, IMapObject nObject)
        {
            Texture = nTexture;
            Position = nPosition;
            Object = nObject;
        }
    }
}