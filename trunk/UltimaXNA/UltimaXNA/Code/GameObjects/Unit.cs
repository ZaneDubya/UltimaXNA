#region File Description & Usings
//-----------------------------------------------------------------------------
// Unit.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using Microsoft.Xna.Framework;
#endregion

namespace UltimaXNA.GameObjects
{
    enum UnitActions : int 
    {
        stopmovement = 0x7fffffff,
        walk = 0x00,
        walkfaster = 0x01,
        run = 0x02,
        runfaster = 0x03,
        nothing = 0x04,
        shiftshoulders = 0x05,
        handsonhips = 0x06,
        attackstanceshort = 0x07,
        attackstancelonger = 0x08,
        swingattackwithknofe = 0x09,
        stabunderhanded = 0x0a,
        swingattackoverhandwithsword = 0x0b,
        swingattackwithswordoverandside = 0x0c,
        swingattackwithswordside = 0x0d,
        stabwithpointofsword = 0x0e,
        readystance = 0x0f,
        magicbutterchurn = 0x10,
        handsoverheadbalerina = 0x11,
        bowshot = 0x12,
        crossbow = 0x13,
        gethit = 0x14,
        falldownanddiebackwards = 0x15,
        falldownanddieforwards = 0x16,
        ridehorselong = 0x17,
        ridehorsemedium = 0x18,
        ridehorseshort = 0x19,
        swingswordfromhorse = 0x1a,
        normalbowshotonhorse = 0x1b,
        crossbowshot = 0x1c,
        block2onhorsewithshield = 0x1d,
        blockongroundwithshield = 0x1e,
        swinginterrupt = 0x1f,
        bowdeep = 0x20,
        salute = 0x21,
        scratchhead = 0x22,
        onefootforwardfor2secs = 0x23,
        same = 0x24
    }
    public enum EquipLayer : int
    {
        /// <summary>
        /// Invalid layer.
        /// </summary>
        Invalid = 0x00,
        /// <summary>
        /// First valid layer. Equivalent to <c>Layer.OneHanded</c>.
        /// </summary>
        FirstValid = 0x01,
        /// <summary>
        /// One handed weapon.
        /// </summary>
        OneHanded = 0x01,
        /// <summary>
        /// Two handed weapon or shield.
        /// </summary>
        TwoHanded = 0x02,
        /// <summary>
        /// Shoes.
        /// </summary>
        Shoes = 0x03,
        /// <summary>
        /// Pants.
        /// </summary>
        Pants = 0x04,
        /// <summary>
        /// Shirts.
        /// </summary>
        Shirt = 0x05,
        /// <summary>
        /// Helmets, hats, and masks.
        /// </summary>
        Helm = 0x06,
        /// <summary>
        /// Gloves.
        /// </summary>
        Gloves = 0x07,
        /// <summary>
        /// Rings.
        /// </summary>
        Ring = 0x08,
        /// <summary>
        /// Talismans.
        /// </summary>
        Talisman = 0x09,
        /// <summary>
        /// Gorgets and necklaces.
        /// </summary>
        Neck = 0x0A,
        /// <summary>
        /// Hair.
        /// </summary>
        Hair = 0x0B,
        /// <summary>
        /// Half aprons.
        /// </summary>
        Waist = 0x0C,
        /// <summary>
        /// Torso, inner layer.
        /// </summary>
        InnerTorso = 0x0D,
        /// <summary>
        /// Bracelets.
        /// </summary>
        Bracelet = 0x0E,
        /// <summary>
        /// Unused.
        /// </summary>
        Unused_xF = 0x0F,
        /// <summary>
        /// Beards and mustaches.
        /// </summary>
        FacialHair = 0x10,
        /// <summary>
        /// Torso, outer layer.
        /// </summary>
        MiddleTorso = 0x11,
        /// <summary>
        /// Earings.
        /// </summary>
        Earrings = 0x12,
        /// <summary>
        /// Arms and sleeves.
        /// </summary>
        Arms = 0x13,
        /// <summary>
        /// Cloaks.
        /// </summary>
        Cloak = 0x14,
        /// <summary>
        /// Backpacks.
        /// </summary>
        Backpack = 0x15,
        /// <summary>
        /// Torso, outer layer.
        /// </summary>
        OuterTorso = 0x16,
        /// <summary>
        /// Leggings, outer layer.
        /// </summary>
        OuterLegs = 0x17,
        /// <summary>
        /// Leggings, inner layer.
        /// </summary>
        InnerLegs = 0x18,
        /// <summary>
        /// Last valid non-internal layer. Equivalent to <c>Layer.InnerLegs</c>.
        /// </summary>
        LastUserValid = 0x18,
        /// <summary>
        /// Mount item layer.
        /// </summary>
        Mount = 0x19,
        /// <summary>
        /// Vendor 'buy pack' layer.
        /// </summary>
        ShopBuy = 0x1A,
        /// <summary>
        /// Vendor 'resale pack' layer.
        /// </summary>
        ShopResale = 0x1B,
        /// <summary>
        /// Vendor 'sell pack' layer.
        /// </summary>
        ShopSell = 0x1C,
        /// <summary>
        /// Bank box layer.
        /// </summary>
        Bank = 0x1D,
        /// <summary>
        /// Last valid layer. Equivalent to <c>Layer.Bank</c>.
        /// </summary>
        LastValid = 0x1D
    }

    class Unit : UltimaXNA.GameObjects.BaseObject
    {
        public int DisplayBodyID = 0, MountDisplayID = 0;
        public Item[] Equipment = new Item[(int)EquipLayer.LastValid + 1];

        private int m_Hue;
        public int Hue // Fix for large hue values per issue12 (http://code.google.com/p/ultimaxna/issues/detail?id=12) --ZDW 6/15/2009
        {
            get { return m_Hue; }
            set
            {
                if (value > 2998)
                    m_Hue = (int)(value / 32);
                else
                    m_Hue = value;
            }
        }


        // These will be added later ...
        // public int CharmingGUID = 0;
        // public int SummoningGUID = 0;
        // public int CharmedByGUID = 0;
        // public int SummonedByGUID = 0;
        // public int CreatedByGUID = 0;
        // public int CritterGUID = 0;
        // public int PetGUID = 0;
        // public int TargetGUID = 0;
        // public int ChannelObjectGUID = 0;

        // public int Bytes0 = 0, Bytes1 = 0, Bytes2 = 0;
        // public CurrentMaxValue Health, Power1;
        // public int Level = 0, PetLevel = 0;
        // public int FactionTemplate = 0;

        // public int Flags1 = 0, Flags2 = 0, FlagsDynamic = 0, FlagsNPC = 0;
        // public int BaseAttackTime = 0, RangedAttackTime = 0;
        
        // public int[] VirtualDisplayID = new int[3];

        // public int MinDamage = 0, MaxDamage = 0;
        // public int MinDamageOffhand = 0, MaxDamageOffhand = 0;
        // public int MinDamageRanged = 0, MaxDamageRanged = 0;
        // public CurrentMaxValue AttackPower;

        // public BaseModValue Stat0, Stat1, Stat2, Stat3, Stat4;
        // public BaseModValue[] Resistances = new BaseModValue[8];

        private int[] m_DrawLayers = new int[20]
            {(int)EquipLayer.OneHanded,
            (int)EquipLayer.TwoHanded,
            (int)EquipLayer.Shoes,
            (int)EquipLayer.Pants,
            (int)EquipLayer.Shirt,
            (int)EquipLayer.Helm,
            (int)EquipLayer.Gloves,
            (int)EquipLayer.Neck,
            (int)EquipLayer.Hair,
            (int)EquipLayer.Waist,
            (int)EquipLayer.InnerTorso,
            (int)EquipLayer.FacialHair,
            (int)EquipLayer.MiddleTorso,
            (int)EquipLayer.OuterTorso,
            (int)EquipLayer.Arms,
            (int)EquipLayer.Cloak,
            (int)EquipLayer.OuterLegs,
            (int)EquipLayer.OuterLegs,
            (int)EquipLayer.InnerLegs,
            (int)EquipLayer.Mount};

        public UnitAnimation m_Animation;

        public Unit(int nGUID)
            : base(nGUID)
        {
            ObjectType = ObjectType.Unit;

            // Health = new CurrentMaxValue();
            // Power1 = new CurrentMaxValue();
            // Stat0 = new BaseModValue();
            // Stat1 = new BaseModValue();
            // Stat2 = new BaseModValue();
            // Stat3 = new BaseModValue();
            // Stat4 = new BaseModValue();
            // AttackPower = new CurrentMaxValue();

            Movement.RequiresUpdate = true;
            this.m_Animation = new UnitAnimation();
        }

        protected override void Draw(UltimaXNA.TileEngine.MapCell nCell, Vector3 nLocation, Vector3 nOffset)
        {
            if (Movement.IsMoving)
            {
                bool nRunning = false;
                UnitActions iAnimationAction = (nRunning == true) ? UnitActions.run : UnitActions.walk;
                m_Animation.SetAnimation(iAnimationAction, 10, 0, false, false, 1);
            }
            else
            {
                m_Animation.SetAnimation(UnitActions.stopmovement);
            }
            m_Animation.Update();

            int iDirection = Movement.DrawFacing;
            int iAction = m_Animation.GetAction_People();

            nCell.AddMobileTile(
                new TileEngine.MobileTile(DisplayBodyID, nLocation, nOffset, iDirection, iAction, m_Animation.AnimationFrame, this.GUID, 1, this.Hue));
            for (int i = 0; i < m_DrawLayers.Length; i++)
            {
                if ((this.Equipment[m_DrawLayers[i]] != null) && (this.Equipment[m_DrawLayers[i]].DisplayID != 0))
                {
                    nCell.AddMobileTile(
                        new TileEngine.MobileTile(
                            this.Equipment[m_DrawLayers[i]].DisplayID, nLocation, nOffset,
                            iDirection, iAction, m_Animation.AnimationFrame,
                            this.GUID, i + 1, this.Equipment[m_DrawLayers[i]].Hue));
                }
            }
        }

        public void Animation(int action, int frameCount, int repeatCount, bool reverse, bool repeat, int delay)
        {
            m_Animation.SetAnimation((UnitActions)action, frameCount, repeatCount, reverse, repeat, delay);
        }

        public void SetFacing(int nDirection)
        {
            // This should be called exclusively from the server...
            Movement.Facing = (Direction)(nDirection & 0x0F);
        }

        public void Move(int nX, int nY, int nZ)
        {
            Movement.SetGoalTile((float)nX, (float)nY, (float)nZ);
        }
    }

    class UnitAnimation
    {
        UnitActions Action;
        public float AnimationFrame = 0f;
        private float m_AnimationStep = 0f;

        public UnitAnimation()
        {
            m_AnimationStep = 0f;
            Action = UnitActions.nothing;
        }

        public void SetAnimation(UnitActions nAction)
        {
            if (nAction == UnitActions.stopmovement)
            {
                if ((this.Action == UnitActions.walk) ||
                    (this.Action == UnitActions.walkfaster) ||
                    (this.Action == UnitActions.run) ||
                    (this.Action == UnitActions.runfaster))
                {
                    this.SetAnimation(UnitActions.nothing);
                }
                return;
            }

            if (this.Action != nAction)
            {
                this.Action = (UnitActions)nAction;
                AnimationFrame = 0f;
                m_AnimationStep = 0f;
            }
        }

        public void SetAnimation(UnitActions action, int frameCount, int repeatCount, bool reverse, bool repeat, int delay)
        {
            if (this.Action != action)
            {
                this.Action = (UnitActions)action;
                AnimationFrame = 0f;
                m_AnimationStep = 1f / (float)((frameCount * (delay + 1)) * 5);
            }
        }

        public void Update()
        {
            AnimationFrame = AnimationFrame + m_AnimationStep;
            if (AnimationFrame >= 1f)
            {
                AnimationFrame = AnimationFrame - 1f;
            }
        }

        public int GetAction_People()
        {
            switch (Action)
            {
                case UnitActions.walk :
                    return 0;
                case UnitActions.nothing :
                    return 4;
                case UnitActions.shiftshoulders :
                    return 5;
                case UnitActions.handsonhips :
                    return 6;
                default :
                    return 4;
            }
        }
    }
}
