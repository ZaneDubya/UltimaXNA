using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Ultima.Player;

namespace UltimaXNA.Ultima.UI.Controls
{
    /// <summary>
    /// we need convert this class for party system
    /// </summary>
    public class GHealthBar: IMobileStatus/* ,GDragable */
    {
        //private GImageClip m_HP;
        //private GImageClip m_Mana;
        //private Mobile m_Mobile;
        //private GLabel m_Name;
        //private bool m_Player;
        //private GImageClip m_Stam;
        //private MobileFlags m_xFlags;
        //private int m_xHPCur;
        //private int m_xHPMax;
        //private int m_xManaCur;
        //private int m_xManaMax;
        //private string m_xName;
        //private int m_xStamCur;
        //private int m_xStamMax;

        //public GHealthBar(Mobile m, int X, int Y) : base(m.Player ? (m.Flags[MobileFlag.Warmode] ? 0x807 : 0x803) : 0x804, X, Y)
        //{
        //    this.m_xName = "";
        //    this.m_Mobile = m;
        //    base.m_CanDrop = true;
        //    base.m_QuickDrag = false;
        //    this.m_Player = m.Player;
        //    if (!this.m_Player)
        //    {
        //        base.Hue = Hues.GetNotoriety(m.Notoriety);
        //    }
        //    if (Engine.Features.AOS)
        //    {
        //        base.Tooltip = new MobileTooltip(m);
        //    }
        //    if (this.m_Player)
        //    {
        //        this.m_HP = new GImageClip(this.GetHealthGumpID(m.Flags), 0x22, 12, m.HPCur, m.HPMax);
        //        this.m_Mana = new GImageClip(0x806, 0x22, 0x19, m.ManaCur, m.ManaMax);
        //        this.m_Stam = new GImageClip(0x806, 0x22, 0x26, m.StamCur, m.StamMax);
        //        base.m_Children.Add(new GImage(0x805, 0x22, 12));
        //        base.m_Children.Add(new GImage(0x805, 0x22, 0x19));
        //        base.m_Children.Add(new GImage(0x805, 0x22, 0x26));
        //        base.m_Children.Add(this.m_HP);
        //        base.m_Children.Add(this.m_Mana);
        //        base.m_Children.Add(this.m_Stam);
        //        this.m_xHPCur = m.HPCur;
        //        this.m_xHPMax = m.HPMax;
        //        this.m_xStamCur = m.StamCur;
        //        this.m_xStamMax = m.StamMax;
        //        this.m_xManaCur = m.ManaCur;
        //        this.m_xManaMax = m.ManaMax;
        //        this.m_xFlags = m.Flags.Clone();
        //    }
        //    else
        //    {
        //        this.m_Name = new GLabel(m.Name, Engine.GetFont(1), Hues.Load(0x455), 0x10, 0);
        //        this.m_Name.Y = 11 + ((0x18 - this.m_Name.Height) / 2);
        //        this.m_Name.Scissor(0, 0, 0x7a, this.m_Name.Height);
        //        this.m_HP = new GImageClip(this.GetHealthGumpID(m.Flags), 0x22, 0x26, m.HPCur, m.HPMax);
        //        base.m_Children.Add(new GImage(0x805, 0x22, 0x26));
        //        base.m_Children.Add(this.m_Name);
        //        base.m_Children.Add(this.m_HP);
        //        this.m_xName = m.Name;
        //        this.m_xHPCur = m.HPCur;
        //        this.m_xHPMax = m.HPMax;
        //        this.m_xFlags = m.Flags.Clone();
        //    }
        //}

        //public void Close()
        //{
        //    if ((Engine.m_Highlight == this.m_Mobile) && !this.m_Mobile.Player)
        //    {
        //        Engine.m_Highlight = null;
        //    }
        //    Gumps.Destroy(this);
        //    if (this.m_Mobile != null)
        //    {
        //        this.m_Mobile.StatusBar = null;
        //    }
        //}

        //private int GetHealthGumpID(MobileFlags flags)
        //{
        //    if (flags[MobileFlag.Poisoned])
        //    {
        //        return 0x808;
        //    }
        //    if (flags[MobileFlag.YellowHits])
        //    {
        //        return 0x809;
        //    }
        //    return 0x806;
        //}

        //public void OnArmorChange(int Armor)
        //{
        //}

        //public void OnColdChange()
        //{
        //}

        //public void OnDamageChange()
        //{
        //}

        //public void OnDexChange(int Dex)
        //{
        //}

        //protected internal override void OnDoubleClick(int X, int Y)
        //{
        //    if (Engine.TargetHandler == null)
        //    {
        //        if (this.m_Mobile.Player)
        //        {
        //            this.Close();
        //            this.m_Mobile.BigStatus = true;
        //            this.m_Mobile.OpenStatus(false);
        //        }
        //        else
        //        {
        //            this.m_Mobile.OnDoubleClick();
        //        }
        //    }
        //}

        //protected internal override void OnDragDrop(Client.Gump g)
        //{
        //    if (g is GDraggedItem)
        //    {
        //        Item item = ((GDraggedItem)g).Item;
        //        if ((item != null) && (this.m_Mobile != null))
        //        {
        //            Network.Send(new PDropItem(item.Serial, -1, -1, 0, this.m_Mobile.Serial));
        //            Gumps.Destroy(g);
        //        }
        //    }
        //}

        //public void OnEnergyChange()
        //{
        //}

        //public void OnFireChange()
        //{
        //}

        //public void OnFlagsChange(MobileFlags Flags)
        //{
        //    if ((this.m_xFlags == null) || (this.m_xFlags.Value != Flags.Value))
        //    {
        //        if (this.m_Player && ((this.m_xFlags == null) || (this.m_xFlags[MobileFlag.Warmode] != Flags[MobileFlag.Warmode])))
        //        {
        //            base.GumpID = Flags[MobileFlag.Warmode] ? 0x807 : 0x803;
        //        }
        //        if (((this.m_xFlags == null) || (this.m_xFlags[MobileFlag.Poisoned] != Flags[MobileFlag.Poisoned])) || (this.m_xFlags[MobileFlag.YellowHits] != Flags[MobileFlag.YellowHits]))
        //        {
        //            this.m_HP.GumpID = this.GetHealthGumpID(Flags);
        //        }
        //        this.m_xFlags = Flags.Clone();
        //    }
        //}

        //public void OnFollCurChange(int current)
        //{
        //}

        //public void OnFollMaxChange(int maximum)
        //{
        //}

        //public void OnGenderChange(int Gender)
        //{
        //}

        //public void OnGoldChange(int Gold)
        //{
        //}

        //public void OnHPCurChange(int HPCur)
        //{
        //    if ((this.m_xHPCur != HPCur) || (this.m_xHPMax != this.m_Mobile.HPMax))
        //    {
        //        this.m_HP.Resize(HPCur, this.m_Mobile.HPMax);
        //        this.m_xHPCur = HPCur;
        //        this.m_xHPMax = this.m_Mobile.HPMax;
        //    }
        //}

        //public void OnHPMaxChange(int HPMax)
        //{
        //    if ((this.m_xHPMax != HPMax) || (this.m_xHPCur != this.m_Mobile.HPCur))
        //    {
        //        this.m_HP.Resize(this.m_Mobile.HPCur, HPMax);
        //        this.m_xHPCur = this.m_Mobile.HPCur;
        //        this.m_xHPMax = HPMax;
        //    }
        //}

        //public void OnIntChange(int Int)
        //{
        //}

        //public void OnLuckChange()
        //{
        //}

        //public void OnManaCurChange(int ManaCur)
        //{
        //    if ((this.m_Player && (this.m_xManaCur != ManaCur)) || (this.m_xManaMax != this.m_Mobile.ManaMax))
        //    {
        //        this.m_Mana.Resize(ManaCur, this.m_Mobile.ManaMax);
        //        this.m_xManaCur = ManaCur;
        //        this.m_xManaMax = this.m_Mobile.ManaMax;
        //    }
        //}

        //public void OnManaMaxChange(int ManaMax)
        //{
        //    if ((this.m_Player && (this.m_xManaMax != ManaMax)) || (this.m_xManaCur != this.m_Mobile.ManaCur))
        //    {
        //        this.m_Mana.Resize(this.m_Mobile.ManaCur, ManaMax);
        //        this.m_xManaCur = this.m_Mobile.ManaCur;
        //        this.m_xManaMax = ManaMax;
        //    }
        //}

        //protected internal override void OnMouseDown(int x, int y, MouseButtons mb)
        //{
        //    base.BringToTop();
        //}

        //protected internal override void OnMouseEnter(int x, int y, MouseButtons mb)
        //{
        //    if (!this.m_Mobile.Player)
        //    {
        //        Engine.m_Highlight = this.m_Mobile;
        //    }
        //}

        //protected internal override void OnMouseLeave()
        //{
        //    if ((Engine.m_Highlight == this.m_Mobile) && !this.m_Mobile.Player)
        //    {
        //        Engine.m_Highlight = null;
        //    }
        //}

        //protected internal override void OnMouseUp(int X, int Y, MouseButtons mb)
        //{
        //    if ((mb & MouseButtons.Right) != MouseButtons.None)
        //    {
        //        this.Close();
        //        Engine.CancelClick();
        //    }
        //    else if ((Engine.TargetHandler != null) && ((mb & MouseButtons.Left) != MouseButtons.None))
        //    {
        //        this.m_Mobile.OnTarget();
        //        Engine.CancelClick();
        //    }
        //}

        //public void OnNameChange(string Name)
        //{
        //    if (!this.m_Player && (this.m_xName != Name))
        //    {
        //        this.m_Name.Text = Name;
        //        this.m_Name.Y = 11 + ((0x18 - this.m_Name.Height) / 2);
        //        this.m_Name.Scissor(0, 0, 0x7a, this.m_Name.Height);
        //        this.m_xName = Name;
        //    }
        //}

        //public void OnNotorietyChange(Notoriety n)
        //{
        //    if (!this.m_Player)
        //    {
        //        base.Hue = Hues.GetNotoriety(n);
        //    }
        //}

        //public void OnPoisonChange()
        //{
        //}

        //public void OnRefresh()
        //{
        //    Mobile mobile = this.m_Mobile;
        //    if (mobile == null)
        //    {
        //        this.Close();
        //    }
        //    this.OnHPCurChange(mobile.HPCur);
        //    this.OnFlagsChange(mobile.Flags);
        //    if (this.m_Player)
        //    {
        //        this.OnStamCurChange(mobile.StamCur);
        //        this.OnManaCurChange(mobile.ManaCur);
        //    }
        //    else
        //    {
        //        this.OnNotorietyChange(mobile.Notoriety);
        //        this.OnNameChange(mobile.Name);
        //    }
        //}

        //protected internal override void OnSingleClick(int x, int y)
        //{
        //    if (Engine.TargetHandler == null)
        //    {
        //        this.m_Mobile.OnSingleClick();
        //    }
        //}

        //public void OnStamCurChange(int StamCur)
        //{
        //    if ((this.m_Player && (this.m_xStamCur != StamCur)) || (this.m_xStamMax != this.m_Mobile.StamMax))
        //    {
        //        this.m_Stam.Resize(StamCur, this.m_Mobile.StamMax);
        //        this.m_xStamCur = StamCur;
        //        this.m_xStamMax = this.m_Mobile.StamMax;
        //    }
        //}

        //public void OnStamMaxChange(int StamMax)
        //{
        //    if ((this.m_Player && (this.m_xStamMax != StamMax)) || (this.m_xStamCur != this.m_Mobile.StamCur))
        //    {
        //        this.m_Stam.Resize(this.m_Mobile.StamCur, StamMax);
        //        this.m_xStamCur = this.m_Mobile.StamCur;
        //        this.m_xStamMax = StamMax;
        //    }
        //}

        //public void OnStatCapChange(int statCap)
        //{
        //}

        //public void OnStrChange(int Str)
        //{
        //}

        //public void OnWeightChange(int Weight)
        //{
        //}

        //public Client.Gump Gump
        //{
        //    get
        //    {
        //        return this;
        //    }
        //}
    }
}
