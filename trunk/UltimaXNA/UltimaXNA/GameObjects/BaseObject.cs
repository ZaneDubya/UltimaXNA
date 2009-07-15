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
        private bool _visible = true;
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (value)
                {
                    _HasBeenDrawn = false;
                }
                _visible = value;
            }
        }
        public Movement Movement;
        public ObjectType ObjectType;
        public Serial Serial;
        internal bool _HasBeenDrawn = false;
        internal bool _Disposed = false; // set this to true to have the object deleted.
        public bool IsDisposed { get { return _Disposed; } }
        public PropertyList PropertyList = new PropertyList();

        public TileEngine.IWorld World
        {
            set
            {
                this.Movement.World = value;
            }
        }

        public BaseObject(Serial serial)
        {
            Serial = serial;
            ObjectType = ObjectType.Object;
            Movement = new Movement(Serial);
            _HasBeenDrawn = false;
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
                        _HasBeenDrawn = false;
                    }
                    else
                    {
                        if (_HasBeenDrawn == false)
                        {
                            this.Draw(iThisMapCell, Movement.DrawPosition.PositionV3, Movement.DrawPosition.OffsetV3);
                            _HasBeenDrawn = true;
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
            _Disposed = true;
            Movement.ClearImmediate();
        }

        public override string ToString()
        {
            return ObjectType.ToString() + " | " + Serial.ToString();
        }
    }
}
