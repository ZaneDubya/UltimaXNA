#region File Description & Usings
//-----------------------------------------------------------------------------
// Unit.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
#endregion

namespace UltimaXNA.GameObjects
{
    public class Mobile : Entity
    {
        public string Name = string.Empty;
        public CurrentMaxValue Health, Stamina, Mana;
        internal WornEquipment equipment;
        internal MobileAnimation animation;

        public bool IsMounted { get { return equipment[(int)EquipLayer.Mount] != null; } }
        public bool IsWarMode { get { return animation.WarMode; } set { animation.WarMode = value; } }
        
		public int DisplayBodyID
		{
			get { return animation.BodyID; }
            set { animation.BodyID = value; }
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
            base.Update(gameTime);
            animation.Update(gameTime);
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

			// Issue 6 - Missing mounted animations - http://code.google.com/p/ultimaxna/issues/detail?id=6 - Smjert
            Item mount = equipment[(int)EquipLayer.Mount];
			TileEngine.MobileTile mobtile = null;
            if (mount != null && mount.ObjectTypeID != 0)
            {
                Movement.Mounted = animation.IsMounted = true;
                mobtile = new TileEngine.MobileTile(
                                mount.ObjectTypeID, nLocation, nOffset,
                                iDirection, (int)animation.Action, animation.AnimationFrame,
                                Serial, 0x1A, mount.Hue, false);

                mobtile.SubType = TileEngine.MobileTileTypes.Mount;
                nCell.AddMobileTile(mobtile);
            }
            else
            {
                Movement.Mounted = animation.IsMounted = false;
            }

			int action = animation.GetAction();
            
			mobtile = new TileEngine.MobileTile(DisplayBodyID, nLocation, nOffset, iDirection, action, animation.AnimationFrame, Serial, 1, Hue, animation.IsMounted);
			mobtile.SubType = TileEngine.MobileTileTypes.Body;
			nCell.AddMobileTile(mobtile);
			// Issue 6 - End

            
            for (int i = 0; i < m_DrawLayers.Length; i++)
            {
                // Issue 6 - Missing mounted animations - http://code.google.com/p/ultimaxna/issues/detail?id=6 - Smjert
                if ( equipment[m_DrawLayers[i]] != null && equipment[m_DrawLayers[i]].AnimationDisplayID != 0 )
                {
                    mobtile = new TileEngine.MobileTile(
                            equipment[m_DrawLayers[i]].AnimationDisplayID, nLocation, nOffset,
                            iDirection, action, animation.AnimationFrame,
                            Serial, i + 1, equipment[m_DrawLayers[i]].Hue, animation.IsMounted);
					
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
