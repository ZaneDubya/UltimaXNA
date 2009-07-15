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
    public class Entity
    {
        public Serial Serial;
        public Movement Movement;
        public PropertyList PropertyList = new PropertyList();

        internal bool _hasBeenDrawn = false; // if this is false this object will redraw itself in the tileengine.

        private bool _visible = true;
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; if (_visible) { _hasBeenDrawn = false; } }
        }
        
        internal bool _Disposed = false; // set this to true to have the object deleted.
        public bool IsDisposed { get { return _Disposed; } }
        
        public TileEngine.IWorld World { set { Movement.World = value; } }

        public Entity(Serial serial)
        {
            Serial = serial;
            Movement = new Movement(Serial);
            _hasBeenDrawn = false;
        }

        public virtual void Update(GameTime gameTime)
        {
            if ((Movement.RequiresUpdate || _hasBeenDrawn == false) && _visible && Movement.DrawPosition != null)
            {
                Movement.Update(gameTime);

                TileEngine.MapCell iThisMapCell = Movement.World.Map.GetMapCell(Movement.DrawPosition.TileX, Movement.DrawPosition.TileY);
                if (iThisMapCell != null)
                {
                    this.Draw(iThisMapCell, Movement.DrawPosition.PositionV3, Movement.DrawPosition.OffsetV3);
                    _hasBeenDrawn = true;
                }
                else
                {
                    _hasBeenDrawn = false;
                }
            }
        }

        internal virtual void Draw(TileEngine.MapCell nCell, Vector3 nLocation, Vector3 nOffset)
        {
            // do nothing. Base Objects do not draw.
            // inheriting classes can override this though.
        }

        public virtual void Dispose()
        {
            _Disposed = true;
            Movement.ClearImmediate();
        }

        public override string ToString()
        {
            return Serial.ToString();
        }
    }
}
