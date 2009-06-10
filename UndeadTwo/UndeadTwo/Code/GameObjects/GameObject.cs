#region File Description & Usings
//-----------------------------------------------------------------------------
// GameObject.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using Microsoft.Xna.Framework;
#endregion

namespace UndeadClient.GameObjects
{
    class GameObject : UndeadClient.GameObjects.BaseObject
    {
        public int DisplayID = 0;
        public int Flags = 0;
        public int DynamicFlags = 0;
        public int Faction = 0;
        public int State = 0;

        public GameObject(int nGUID)
            : base(nGUID)
        {
            ObjectType = ObjectType.GameObject;
        }

        protected override void Draw(UndeadClient.TileEngine.MapCell nCell, Vector3 nLocation, Vector3 nOffset)
        {
            nCell.AddGameObjectTile(
                new TileEngine.GameObjectTile(DisplayID, nLocation, Movement.DrawFacing, this.GUID, 0));
        }
    }
}
