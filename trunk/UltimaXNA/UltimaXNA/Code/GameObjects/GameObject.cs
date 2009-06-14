#region File Description & Usings
//-----------------------------------------------------------------------------
// GameObject.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using Microsoft.Xna.Framework;
#endregion

namespace UltimaXNA.GameObjects
{
    class GameObject : UltimaXNA.GameObjects.BaseObject
    {
        public int Flags = 0;
        public int DynamicFlags = 0;
        public int Faction = 0;
        public int State = 0;

        // GameObjects can potentially have inventory (chests, for example).
        // The GUID for the container for this inventory is the same as the
        // GameObject's GUID.
        private Container m_ContainerObject = null;
        public Container ContainerObject
        {
            get
            {
                if (m_ContainerObject == null)
                    m_ContainerObject = new Container(this.GUID);
                return m_ContainerObject;
            }
        }

        private int m_DisplayID;
        public int DisplayID
        {
            get { return m_DisplayID; }
            set
            {
                this.m_HasBeenDrawn = false;
                m_DisplayID = value;
            }
        }

        public GameObject(int nGUID)
            : base(nGUID)
        {
            ObjectType = ObjectType.GameObject;
        }

        protected override void Draw(UltimaXNA.TileEngine.MapCell nCell, Vector3 nLocation, Vector3 nOffset)
        {
            nCell.AddGameObjectTile(
                new TileEngine.GameObjectTile(DisplayID, nLocation, Movement.DrawFacing, this.GUID, 0));
        }
    }
}
