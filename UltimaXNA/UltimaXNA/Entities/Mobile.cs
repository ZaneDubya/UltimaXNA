/***************************************************************************
 *   Mobile.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
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
using UltimaXNA.TileEngine;
#endregion

namespace UltimaXNA.Entities
{
    public class Mobile : Entity
    {
        public string Name = string.Empty;
        public CurrentMaxValue Health, Stamina, Mana;
        internal WornEquipment equipment;
        internal MobileAnimation animation;

        public bool IsMounted { get { return equipment[(int)EquipLayer.Mount] != null; } }
        public bool IsWarMode { get { return animation.WarMode; } set { animation.WarMode = value; } }

        public int BodyID
        {
            get { return animation.BodyID; }
            set { animation.BodyID = value; }
        }

        public bool Alive
        {
            get { return Health.Current != 0; }
        }

        private int _hue;
        public int Hue // Fix for large hue values per issue12 (http://code.google.com/p/ultimaxna/issues/detail?id=12) --ZDW 6/15/2009
        {
            get { return _hue; }
            set
            {
                if (value > 2998)
                    _hue = (int)(value / 32);
                else
                    _hue = value;
            }
        }

        // Issue 14 - Wrong layer draw order - http://code.google.com/p/ultimaxna/issues/detail?id=14 - Smjert
        private int[] m_DrawLayers = new int[20]
		{
			(int)EquipLayer.Mount,
			(int)EquipLayer.OneHanded,
            (int)EquipLayer.Shoes,
            (int)EquipLayer.Pants,
            (int)EquipLayer.Shirt,
            (int)EquipLayer.Gloves,
            (int)EquipLayer.Neck,
			(int)EquipLayer.Hair,
            (int)EquipLayer.Waist,
            (int)EquipLayer.InnerTorso,
            (int)EquipLayer.FacialHair,
            (int)EquipLayer.MiddleTorso,
            (int)EquipLayer.OuterLegs,
            (int)EquipLayer.OuterLegs,
            (int)EquipLayer.InnerLegs,
            (int)EquipLayer.OuterTorso,
            (int)EquipLayer.Arms,
            (int)EquipLayer.Cloak,
			(int)EquipLayer.Helm,
            (int)EquipLayer.TwoHanded,
		};

        public Mobile(Serial serial)
            : base(serial)
        {
            animation = new MobileAnimation();
            equipment = new WornEquipment(this);
            Movement.RequiresUpdate = true;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (equipment[(int)EquipLayer.Mount] != null &&
                equipment[(int)EquipLayer.Mount].ItemID != 0)
            {
                Movement.IsMounted = animation.IsMounted = true;
            }
            else
            {
                Movement.IsMounted = animation.IsMounted = false;
            }
            animation.Update(gameTime);
            base.Update(gameTime);
        }

        internal override void Draw(UltimaXNA.TileEngine.MapCell nCell, Vector3 nLocation, Vector3 nOffset)
        {
            if (Movement.IsMoving)
            {
                MobileAction iAnimationAction = (Movement.IsRunning) ? MobileAction.Run : MobileAction.Walk;
                animation.SetAnimation(iAnimationAction, 10, 0, false, false, 0);
            }
            else
            {
                if (animation.IsMovementAction(animation.Action))
                    animation.HaltAnimation = true;
            }

            int iDirection = Movement.DrawFacing;

            int action = animation.GetAction();
            MobileTile mobtile = null;
            mobtile = new MobileTile(BodyID, nLocation, nOffset, iDirection, action, animation.AnimationFrame, this, 1, Hue, animation.IsMounted);
            mobtile.SubType = MobileTileTypes.Body;
            nCell.AddMobileTile(mobtile);
            // Issue 6 - End


            for (int i = 0; i < m_DrawLayers.Length; i++)
            {
                // Issue 6 - Missing mounted animations - http://code.google.com/p/ultimaxna/issues/detail?id=6 - Smjert
                if (equipment[m_DrawLayers[i]] != null && equipment[m_DrawLayers[i]].AnimationDisplayID != 0)
                {
                    mobtile = new TileEngine.MobileTile(
                            equipment[m_DrawLayers[i]].AnimationDisplayID, nLocation, nOffset,
                            iDirection, action, animation.AnimationFrame,
                            this, i + 1, equipment[m_DrawLayers[i]].Hue, animation.IsMounted);

                    mobtile.SubType = TileEngine.MobileTileTypes.Equipment;
                    nCell.AddMobileTile(mobtile);
                }
                // Issue 6 - End
            }
        }

        public override void Dispose()
        {
            equipment.ClearEquipment();
            base.Dispose();
        }

        public void UnWearItem(Serial serial)
        {
            equipment.RemoveBySerial(serial);
        }

        public void Animation(int action, int frameCount, int repeatCount, bool reverse, bool repeat, int delay)
        {
            animation.SetAnimation((MobileAction)action, frameCount, repeatCount, reverse, repeat, delay);
        }

        public void Move(int nX, int nY, int nZ, int nFacing)
        {
            if (nFacing != -1)
                Movement.Facing = (Direction)nFacing;
            Movement.SetGoalTile((float)nX, (float)nY, (float)nZ);
        }
    }
}
