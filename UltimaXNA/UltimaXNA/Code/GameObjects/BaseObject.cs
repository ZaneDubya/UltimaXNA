#region File Description & Usings
//-----------------------------------------------------------------------------
// BaseObject.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
#endregion

namespace UltimaXNA.GameObjects
{
    public class BaseObject
    {
        public Movement Movement;
        public ObjectType ObjectType;
        public int GUID;
        internal bool m_HasBeenDrawn;
        internal bool m_Dispose = false; // set this to true to have the object deleted.
        public bool IsDisposed { get { return m_Dispose; } }

        public TileEngine.IWorld World
        {
            set
            {
                this.Movement.World = value;
            }
        }

        public BaseObject(int nGUID)
        {

            ObjectType = ObjectType.Object;
            GUID = nGUID;
            Movement = new Movement(GUID);
            m_HasBeenDrawn = false;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (Movement.RequiresUpdate)
            {
                Movement.Update(gameTime);

                TileEngine.MapCell iThisMapCell = Movement.World.Map.GetMapCell(Movement.DrawPosition.TileX, Movement.DrawPosition.TileY);
                if (iThisMapCell != null)
                    this.Draw(iThisMapCell, Movement.DrawPosition.PositionV3, Movement.DrawPosition.OffsetV3);
            }
            else
            {
                if (Movement.DrawPosition != null)
                {
                    TileEngine.MapCell iThisMapCell = Movement.World.Map.GetMapCell(Movement.DrawPosition.TileX, Movement.DrawPosition.TileY);
                    if (iThisMapCell == null)
                    {
                        m_HasBeenDrawn = false;
                    }
                    else
                    {
                        if (m_HasBeenDrawn == false)
                        {
                            this.Draw(iThisMapCell, Movement.DrawPosition.PositionV3, Movement.DrawPosition.OffsetV3);
                            m_HasBeenDrawn = true;
                        }
                    }
                }
            }
        }

        protected virtual void Draw(TileEngine.MapCell nCell, Vector3 nLocation, Vector3 nOffset)
        {
            // do nothing. Base Objects do not draw.
        }

        public virtual void Dispose()
        {
            m_Dispose = true;
            Movement.ClearImmediate();
        }

        public override string ToString()
        {
            return ObjectType.ToString() + " | " + GUID.ToString();
        }
    }
}
