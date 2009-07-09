#region File Description & Usings
//-----------------------------------------------------------------------------
// Renderer.cs
//
// Created by ClintXNA, modifications by Poplicola
//-----------------------------------------------------------------------------
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

    public interface ITileEngine
    {
        void SetLightDirection(Vector3 nDirection);
        IMapObject MouseOverObject { get; }
        IMapObject MouseOverGroundTile { get; }
        PickTypes PickType { set; }
        int ObjectsRendered { get; }
    }

    class TileEngine : DrawableGameComponent, ITileEngine
    {
        public int ObjectsRendered { get; protected set; }
        private SpriteBatch3D m_SpriteBatch;
        private VertexPositionNormalTextureHue[] m_VertexBuffer;
        private VertexPositionNormalTextureHue[] m_VertexBufferForStretchedTile;
        private bool m_IsFirstUpdate = true; // This variable is used to skip the first 'update' cycle.

        // Used for mousepicking.
        private IMapObject m_MouseOverObject;
        private IMapObject m_MouseOverGroundTile;
        private PickTypes m_PickType = PickTypes.PickNothing;
        private RayPicker m_RayPicker;

        // Reference for services
        private Input.IInputHandler m_InputService;
        private GameObjects.IGameObjects m_ObjectsService;
        private IWorld m_WorldService;
        private IGameState m_GameStateService;

        public TileEngine(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(ITileEngine), this);
        }

        public IMapObject MouseOverObject
        { get { return m_MouseOverObject; } }
        public IMapObject MouseOverGroundTile
        { get { return m_MouseOverGroundTile; } }
        public PickTypes PickType
        { set { m_PickType = value; } }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (!m_GameStateService.InWorld)
                return;

            m_SpriteBatch.Flush();
            if (m_MouseOverGroundTile != null)
                m_RayPicker.DrawPickedTriangle(m_SpriteBatch.WorldMatrix);
        }

        public void SetLightDirection(Vector3 nDirection)
        {
            m_SpriteBatch.SetLightDirection(nDirection);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (m_IsFirstUpdate)
            {
                m_IsFirstUpdate = false;
                return;
            }

            if (!m_GameStateService.InWorld)
                return;

            Vector3 drawPosition = new Vector3();
            float drawX;
            float drawY;
            float drawZ = 0;
            GroundTile groundTile;
            int height;
            Map map = m_WorldService.Map;
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
            int MaxRoofAltitude = m_WorldService.MaxRoofAltitude;

            // Now determine where to draw. First retrieve the position of the center object.
            GameObjects.DrawPosition iDrawPosition = m_ObjectsService.GetPlayerObject().Movement.DrawPosition;
            
            float xOffset = (this.Game.GraphicsDevice.PresentationParameters.BackBufferWidth / 2) - 22;
            float yOffset = (this.Game.GraphicsDevice.PresentationParameters.BackBufferHeight / 2) -
                    ((map.GameSize * 44) / 2);
            yOffset += ((float)iDrawPosition.TileZ + iDrawPosition.OffsetZ) * 4;
            xOffset -= (int)((iDrawPosition.OffsetX - iDrawPosition.OffsetY) * 22);
            yOffset -= (int)((iDrawPosition.OffsetX + iDrawPosition.OffsetY) * 22);

            int startX = iDrawPosition.TileX - map.GameSize / 2;
            int startY = iDrawPosition.TileY - map.GameSize / 2;

            Random iRand = new Random();

            // If we are going to be checking for GroundTiles, Clear the RayPicker.
            // If we're not, then make sure we clear the last picked ground tile.
            if ((m_PickType & PickTypes.PickGroundTiles) == PickTypes.PickGroundTiles)
                m_RayPicker.FlushObjects();
            else
                m_MouseOverGroundTile = null;

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

                                m_VertexBuffer[0].Position = drawPosition;
                                m_VertexBuffer[0].Position.Y -= drawY;
                                m_VertexBuffer[0].Position.Z = drawZ;

                                m_VertexBuffer[1].Position = drawPosition;
                                m_VertexBuffer[1].Position.X += 44;
                                m_VertexBuffer[1].Position.Y -= drawY;
                                m_VertexBuffer[1].Position.Z = drawZ;

                                m_VertexBuffer[2].Position = drawPosition;
                                m_VertexBuffer[2].Position.Y += 44 - drawY;
                                m_VertexBuffer[2].Position.Z = drawZ;

                                m_VertexBuffer[3].Position = drawPosition;
                                m_VertexBuffer[3].Position.X += 44;
                                m_VertexBuffer[3].Position.Y += 44 - drawY;
                                m_VertexBuffer[3].Position.Z = drawZ;

                                m_VertexBuffer[0].Hue = Vector2.Zero;
                                m_VertexBuffer[1].Hue = Vector2.Zero;
                                m_VertexBuffer[2].Hue = Vector2.Zero;
                                m_VertexBuffer[3].Hue = Vector2.Zero;

                                m_SpriteBatch.Draw(texture, m_VertexBuffer);
                            }
                            else // Stretched
                            {
                                texture = Data.Texmaps.GetTexmapTexture(landData.TextureID, this.GraphicsDevice);

                                m_VertexBufferForStretchedTile[0].Position = drawPosition;
                                m_VertexBufferForStretchedTile[0].Position.X += 22;
                                
                                m_VertexBufferForStretchedTile[1].Position = drawPosition;
                                m_VertexBufferForStretchedTile[1].Position.X += 44;
                                m_VertexBufferForStretchedTile[1].Position.Y += 22;

                                m_VertexBufferForStretchedTile[2].Position = drawPosition;
                                m_VertexBufferForStretchedTile[2].Position.Y += 22;

                                m_VertexBufferForStretchedTile[3].Position = drawPosition;
                                m_VertexBufferForStretchedTile[3].Position.X += 22;
                                m_VertexBufferForStretchedTile[3].Position.Y += 44;

                                m_VertexBufferForStretchedTile[0].Normal = groundTile.Normals[0];
                                m_VertexBufferForStretchedTile[1].Normal = groundTile.Normals[1];
                                m_VertexBufferForStretchedTile[2].Normal = groundTile.Normals[2];
                                m_VertexBufferForStretchedTile[3].Normal = groundTile.Normals[3];

                                m_VertexBufferForStretchedTile[0].Position.Y -= drawY;
                                m_VertexBufferForStretchedTile[1].Position.Y -= (groundTile.Surroundings.East << 2);
                                m_VertexBufferForStretchedTile[2].Position.Y -= (groundTile.Surroundings.South << 2);
                                m_VertexBufferForStretchedTile[3].Position.Y -= (groundTile.Surroundings.Down << 2);

                                m_VertexBufferForStretchedTile[0].Position.Z = drawZ;
                                m_VertexBufferForStretchedTile[1].Position.Z = drawZ;
                                m_VertexBufferForStretchedTile[2].Position.Z = drawZ;
                                m_VertexBufferForStretchedTile[3].Position.Z = drawZ;

                                m_VertexBufferForStretchedTile[0].Hue = Vector2.Zero;
                                m_VertexBufferForStretchedTile[1].Hue = Vector2.Zero;
                                m_VertexBufferForStretchedTile[2].Hue = Vector2.Zero;
                                m_VertexBufferForStretchedTile[3].Hue = Vector2.Zero;

                                m_SpriteBatch.Draw(texture, m_VertexBufferForStretchedTile);

                                if ((m_PickType & PickTypes.PickGroundTiles) == PickTypes.PickGroundTiles)
                                    m_RayPicker.AddObject(mapObject, m_VertexBufferForStretchedTile);
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

                            m_VertexBuffer[0].Position = drawPosition;
                            m_VertexBuffer[0].Position.X -= drawX;
                            m_VertexBuffer[0].Position.Y -= drawY;
                            m_VertexBuffer[0].Position.Z = drawZ;

                            m_VertexBuffer[1].Position = drawPosition;
                            m_VertexBuffer[1].Position.X += width - drawX;
                            m_VertexBuffer[1].Position.Y -= drawY;
                            m_VertexBuffer[1].Position.Z = drawZ;

                            m_VertexBuffer[2].Position = drawPosition;
                            m_VertexBuffer[2].Position.X -= drawX;
                            m_VertexBuffer[2].Position.Y += height - drawY;
                            m_VertexBuffer[2].Position.Z = drawZ;

                            m_VertexBuffer[3].Position = drawPosition;
                            m_VertexBuffer[3].Position.X += width - drawX;
                            m_VertexBuffer[3].Position.Y += height - drawY;
                            m_VertexBuffer[3].Position.Z = drawZ;

                            m_VertexBuffer[0].Hue = Vector2.Zero;
                            m_VertexBuffer[1].Hue = Vector2.Zero;
                            m_VertexBuffer[2].Hue = Vector2.Zero;
                            m_VertexBuffer[3].Hue = Vector2.Zero;

                            m_SpriteBatch.Draw(texture, m_VertexBuffer);

                            if ((m_PickType & PickTypes.PickStatics) == PickTypes.PickStatics)
                                if (mIsMouseOverObject(m_VertexBuffer[0].Position, m_VertexBuffer[3].Position))
                                    iMouseOverList.AddItem(new MouseOverItem(texture, m_VertexBuffer[0].Position, mapObject));
                        }
                        else if (mapObject.Type == MapObjectTypes.MobileTile) // Mobile
                        {
                            iMobile = (MobileTile)mapObject;

                            if (iMobile.Z >= MaxRoofAltitude)
                                continue;

                            Data.FrameXNA[] iFrames = Data.AnimationsXNA.GetAnimation(this.Game.GraphicsDevice,
                                iMobile.ID, iMobile.Action, iMobile.Direction, iMobile.Hue, false);
                            // GetAnimation fails so it returns null, temporary fix - Smjert
                            if (iFrames == null)
                                continue;
                            int iFrame = iMobile.Frame(iFrames.Length);
                            // If the frame data is corrupt, then the texture will not load. Fix for broken cleaver data, maybe others. --Poplicola 6/15/2009
                            if (iFrames[iFrame].Texture == null)
                                continue;
                            width = iFrames[iFrame].Texture.Width;
                            height = iFrames[iFrame].Texture.Height;
                            drawX = iFrames[iFrame].Center.X - 22 - (int)((iMobile.Offset.X - iMobile.Offset.Y) * 22);
                            drawY = iFrames[iFrame].Center.Y + (iMobile.Z << 2) + height - 22 - (int)((iMobile.Offset.X + iMobile.Offset.Y) * 22);

                            m_VertexBuffer[0].Position = drawPosition;
                            m_VertexBuffer[0].Position.X -= drawX;
                            m_VertexBuffer[0].Position.Y -= drawY;
                            m_VertexBuffer[0].Position.Z = drawZ;

                            m_VertexBuffer[1].Position = drawPosition;
                            m_VertexBuffer[1].Position.X += width - drawX;
                            m_VertexBuffer[1].Position.Y -= drawY;
                            m_VertexBuffer[1].Position.Z = drawZ;

                            m_VertexBuffer[2].Position = drawPosition;
                            m_VertexBuffer[2].Position.X -= drawX;
                            m_VertexBuffer[2].Position.Y += height - drawY;
                            m_VertexBuffer[2].Position.Z = drawZ;

                            m_VertexBuffer[3].Position = drawPosition;
                            m_VertexBuffer[3].Position.X += width - drawX;
                            m_VertexBuffer[3].Position.Y += height - drawY;
                            m_VertexBuffer[3].Position.Z = drawZ;

                            //TODO: Check to see if its a partial hue, if true set Y = 1, if false then set y = 0
                            // We subtract 1 since hue = 0 is valid, and the client uses hue = 0 to indicate no hue.
                            Vector2 hueVector = new Vector2(iMobile.Hue - 1, 0);

                            m_VertexBuffer[0].Hue = hueVector;
                            m_VertexBuffer[1].Hue = hueVector;
                            m_VertexBuffer[2].Hue = hueVector;
                            m_VertexBuffer[3].Hue = hueVector;

                            m_SpriteBatch.Draw(iFrames[iFrame].Texture, m_VertexBuffer);

                            if ((m_PickType & PickTypes.PickObjects) == PickTypes.PickObjects)
                                if (mIsMouseOverObject(m_VertexBuffer[0].Position, m_VertexBuffer[3].Position))
                                    iMouseOverList.AddItem(new MouseOverItem(iFrames[iFrame].Texture, m_VertexBuffer[0].Position, mapObject));
                        }
                        else if (mapObject.Type == MapObjectTypes.GameObjectTile)
                        {
                            iObject = (GameObjectTile)mapObject;
                            if (iObject.Z >= MaxRoofAltitude)
                                continue;

                            Data.Art.GetStaticDimensions(iObject.ID, out width, out height);

                            texture = Data.Art.GetStaticTexture(iObject.ID, this.GraphicsDevice);

                            drawX = (width >> 1) - 22;
                            drawY = (iObject.Z << 2) + height - 44;

                            m_VertexBuffer[0].Position = drawPosition;
                            m_VertexBuffer[0].Position.X -= drawX;
                            m_VertexBuffer[0].Position.Y -= drawY;
                            m_VertexBuffer[0].Position.Z = drawZ;

                            m_VertexBuffer[1].Position = drawPosition;
                            m_VertexBuffer[1].Position.X += width - drawX;
                            m_VertexBuffer[1].Position.Y -= drawY;
                            m_VertexBuffer[1].Position.Z = drawZ;

                            m_VertexBuffer[2].Position = drawPosition;
                            m_VertexBuffer[2].Position.X -= drawX;
                            m_VertexBuffer[2].Position.Y += height - drawY;
                            m_VertexBuffer[2].Position.Z = drawZ;

                            m_VertexBuffer[3].Position = drawPosition;
                            m_VertexBuffer[3].Position.X += width - drawX;
                            m_VertexBuffer[3].Position.Y += height - drawY;
                            m_VertexBuffer[3].Position.Z = drawZ;

                            //TODO: Check to see if its a partial hue, if true set Y = 1, if false then set y = 0
                            Vector2 hueVector = new Vector2(iObject.Hue, 0);

                            m_VertexBuffer[0].Hue = hueVector;
                            m_VertexBuffer[1].Hue = hueVector;
                            m_VertexBuffer[2].Hue = hueVector;
                            m_VertexBuffer[3].Hue = hueVector;

                            m_SpriteBatch.Draw(texture, m_VertexBuffer);

                            if ((m_PickType & PickTypes.PickStatics) == PickTypes.PickStatics)
                                if (mIsMouseOverObject(m_VertexBuffer[0].Position, m_VertexBuffer[3].Position))
                                    iMouseOverList.AddItem(new MouseOverItem(texture, m_VertexBuffer[0].Position, mapObject));
                        
                        }

                        drawZ += 1000;
                        ObjectsRendered++;
                    }
                }
            }
            m_MouseOverObject = iMouseOverList.GetForemostMouseOverItem(m_InputService.Mouse.Position);

            if ((m_PickType & PickTypes.PickGroundTiles) == PickTypes.PickGroundTiles)
                if (m_RayPicker.PickTest(m_InputService.Mouse.Position, Matrix.Identity, m_SpriteBatch.WorldMatrix))
                    m_MouseOverGroundTile = m_RayPicker.pickedObject;
        }

        private bool mIsMouseOverObject(Vector3 iMin, Vector3 iMax)
        {
            // Added cursor picking -Poplicola 5/19/2009
            BoundingBox iBoundingBox = new BoundingBox(iMin, iMax);
            // Must correct for bounding box being one pixel larger than actual texture.
            iBoundingBox.Max.X--; iBoundingBox.Max.Y--;

            Vector3 iMousePosition = new Vector3(m_InputService.Mouse.Position.X, m_InputService.Mouse.Position.Y, iMin.Z);
            if (iBoundingBox.Contains(iMousePosition) == ContainmentType.Contains)
                return true;
            return false;
        }

        public override void Initialize()
        {
            base.Initialize();

            m_InputService = (Input.IInputHandler)Game.Services.GetService(typeof(Input.IInputHandler));
            m_ObjectsService = (GameObjects.IGameObjects)Game.Services.GetService(typeof(GameObjects.IGameObjects));
            m_WorldService = (IWorld)Game.Services.GetService(typeof(IWorld));
            m_GameStateService = (IGameState)Game.Services.GetService(typeof(IGameState));

            m_VertexBuffer = new VertexPositionNormalTextureHue[] {
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector2(0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector2(1, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector2(0, 1)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector2(1, 1))
            };
            const float iUVMin = .01f;
            const float iUVMax = .99f;
            m_VertexBufferForStretchedTile = new VertexPositionNormalTextureHue[] {
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(), new Vector2(iUVMin, iUVMin)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(), new Vector2(iUVMax, iUVMin)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(), new Vector2(iUVMin, iUVMax)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(), new Vector2(iUVMax, iUVMax))
            };
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            m_SpriteBatch = new SpriteBatch3D(this.Game);
            m_RayPicker = new RayPicker(this.Game);
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