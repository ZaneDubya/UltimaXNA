/***************************************************************************
 *   MapObjectMobile.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   Based on code from ClintXNA's renderer: http://www.runuo.com/forums/xna/92023-hi.html
 *   
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Entities;
#endregion

namespace UltimaXNA.TileEngine
{
    public class MapObjectMobile : MapObject
    {
        public int BodyID { get; set; }
        public int Action { get; set; }
        public int Facing { get; set; }
        public int Hue { get; set; }

        private float _frame;
        public int Frame(int nMaxFrames)
        {
            return (int)(_frame * (float)nMaxFrames);
        }

        private bool _noDraw = false;

        public MapObjectMobile(int bodyID, Position3D position, int facing, int action, float frame, Entities.Entity ownerEntity, int layer, int hue)
            : base(position)
        {
            BodyID = bodyID;
            Facing = facing;
            Action = action;
            if (_frame >= 1.0f)
                return;
            _frame = frame;
            Hue = hue;
            SortTiebreaker = layer;
            OwnerEntity = ownerEntity;

            // set pick type
            _pickType = PickTypes.PickObjects;

            // set up draw data
            Data.FrameXNA frameXNA = getFrame();
            if (frameXNA == null)
            {
                _noDraw = true;
                return;
            }
            _draw_texture = frameXNA.Texture;
            _draw_width = _draw_texture.Width;
            _draw_height = _draw_texture.Height;
            _draw_flip = (Facing > 4) ? true : false;
            _draw_IsometricOverlap = true;
            if (_draw_flip)
            {
                _draw_X = frameXNA.Center.X - 22 + (int)((Position.X_offset - Position.Y_offset) * 22);
                _draw_Y = frameXNA.Center.Y + (Z << 2) + _draw_height - 22 - (int)((Position.X_offset + Position.Y_offset) * 22);
            }
            else
            {
                _draw_X = frameXNA.Center.X - 22 - (int)((Position.X_offset - Position.Y_offset) * 22);
                _draw_Y = frameXNA.Center.Y + (Z << 2) + _draw_height - 22 - (int)((Position.X_offset + Position.Y_offset) * 22);
            }

            _draw_hue = Utility.GetHueVector(Hue);
            if (ClientVars.LastTarget != null && ClientVars.LastTarget == OwnerSerial)
                _draw_hue = new Vector2(((Entities.Mobile)OwnerEntity).NotorietyHue - 1, 1);
        }

        internal override bool Draw(SpriteBatch3D sb, Vector3 drawPosition, MouseOverList molist, PickTypes pickType, int maxAlt)
        {
            if (_noDraw)
                return false;
            return base.Draw(sb, drawPosition, molist, pickType, maxAlt);
        }

        private Data.FrameXNA getFrame()
        {
            Data.FrameXNA[] iFrames = Data.AnimationsXNA.GetAnimation(BodyID, Action, Facing, Hue);
            if (iFrames == null)
                return null;
            int iFrame = Frame(iFrames.Length);
            if (iFrames[iFrame].Texture == null)
                return null;
            return iFrames[iFrame];
        }

        public override string ToString()
        {
            return string.Format("moMobile of {0}", Data.TileData.ItemData_ByAnimID(BodyID).Name);
        }
    }
}