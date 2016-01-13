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
    public class GStatusBar:IMobileStatus/* , GFader */
    {
        //private GLabel m_Armor;
        //private GLabel m_Cold;
        //private GLabel m_Damages;
        //private GLabel m_Dex;
        //private GLabel m_Energy;
        //private GLabel m_Fire;
        //private GLabel m_Followers;
        //private GLabel m_Gold;
        //private GAttributeCurMax m_Hits;
        //private GLabel m_Int;
        //private GLabel m_Luck;
        //private GAttributeCurMax m_Mana;
        //private Mobile m_Mobile;
        //private GLabel m_Name;
        //private GLabel m_Poison;
        //private GAttributeCurMax m_Stam;
        //private GLabel m_StatCap;
        //private GLabel m_Str;
        //private GAttributeCurMax m_Weight;
        //private int m_xArmor;
        //private int m_xCold;
        //private int m_xDamageMax;
        //private int m_xDamageMin;
        //private int m_xDex;
        //private int m_xEnergy;
        //private int m_xFire;
        //private int m_xFollCur;
        //private int m_xFollMax;
        //private int m_xGold;
        //private int m_xInt;
        //private int m_xLuck;
        //private string m_xName;
        //private int m_xPoison;
        //private int m_xStatCap;
        //private int m_xStr;

        //public GStatusBar(Mobile m, int X, int Y) : base(0.25f, 0.25f, 0.6f, 0x2a6c, X, Y)
        //{
        //    this.m_xName = "";
        //    this.m_Mobile = m;
        //    base.m_CanDrop = true;
        //    base.m_QuickDrag = true;
        //    Client.IFont font = Engine.GetFont(9);
        //    IHue hue = Hues.Load(0x455);
        //    this.m_Name = new GLabel("", font, hue, 0x26, 50);
        //    this.m_Str = new GLabel("0", font, hue, 0x58, 0x4d);
        //    this.m_Hits = new GAttributeCurMax(0x92, 0x48, 0x22, 0x15, this.m_Mobile.HPCur, this.m_Mobile.HPMax, font, hue);
        //    this.m_Dex = new GLabel("0", font, hue, 0x58, 0x69);
        //    this.m_Stam = new GAttributeCurMax(0x92, 100, 0x22, 0x15, this.m_Mobile.StamCur, this.m_Mobile.StamMax, font, hue);
        //    this.m_Int = new GLabel("0", font, hue, 0x58, 0x85);
        //    this.m_Mana = new GAttributeCurMax(0x92, 0x80, 0x22, 0x15, this.m_Mobile.ManaCur, this.m_Mobile.ManaMax, font, hue);
        //    this.m_Armor = new GLabel("0", font, hue, 0x163, 0x4a);
        //    this.m_Fire = new GLabel("0", font, hue, 0x163, 0x5b);
        //    this.m_Cold = new GLabel("0", font, hue, 0x163, 0x6a);
        //    this.m_Poison = new GLabel("0", font, hue, 0x163, 0x77);
        //    this.m_Energy = new GLabel("0", font, hue, 0x163, 0x85);
        //    this.m_Luck = new GLabel("0", font, hue, 220, 0x69);
        //    this.m_Damages = new GLabel("0-0", font, hue, 280, 0x4d);
        //    this.m_Gold = new GLabel("0", font, hue, 280, 0x69);
        //    this.m_Weight = new GAttributeCurMax(0xd8, 0x80, 0x22, 0x15, this.m_Mobile.Weight, this.GetMaxWeight(this.m_Mobile.Str), font, hue);
        //    this.m_StatCap = new GLabel("0", font, hue, 220, 0x4d);
        //    this.m_Followers = new GLabel("0/0", font, hue, 0x11d, 0x85);
        //    this.m_Name.X = 0x27 + ((0x160 - ((this.m_Name.Image.xMax - this.m_Name.Image.xMin) + 1)) / 2);
        //    this.m_Name.X -= this.m_Name.Image.xMin;
        //    base.m_Children.Add(this.m_Name);
        //    base.m_Children.Add(this.m_Str);
        //    base.m_Children.Add(this.m_Hits);
        //    base.m_Children.Add(this.m_Dex);
        //    base.m_Children.Add(this.m_Stam);
        //    base.m_Children.Add(this.m_Int);
        //    base.m_Children.Add(this.m_Mana);
        //    base.m_Children.Add(this.m_Armor);
        //    base.m_Children.Add(this.m_Fire);
        //    base.m_Children.Add(this.m_Cold);
        //    base.m_Children.Add(this.m_Poison);
        //    base.m_Children.Add(this.m_Energy);
        //    base.m_Children.Add(this.m_Luck);
        //    base.m_Children.Add(this.m_Damages);
        //    base.m_Children.Add(this.m_Gold);
        //    base.m_Children.Add(this.m_Weight);
        //    base.m_Children.Add(this.m_StatCap);
        //    base.m_Children.Add(this.m_Followers);
        //    base.m_Children.Add(new GMinimizer(this));
        //    this.AddTooltip(0x37, 70, 0x40, 0x1a, 0x10311a);
        //    this.AddTooltip(0x37, 0x62, 0x40, 0x1a, 0x10311b);
        //    this.AddTooltip(0x37, 0x7e, 0x40, 0x1a, 0x10311c);
        //    this.AddTooltip(0x79, 70, 0x3f, 0x1a, 0x10311d);
        //    this.AddTooltip(0x79, 0x62, 0x3f, 0x1a, 0x10311e);
        //    this.AddTooltip(0x79, 0x7e, 0x3f, 0x1a, 0x10311f);
        //    this.AddTooltip(0xba, 70, 0x45, 0x1a, 0x103120);
        //    this.AddTooltip(0xba, 0x62, 0x45, 0x1a, 0x103121);
        //    this.AddTooltip(0xba, 0x7e, 0x45, 0x1a, 0x103122);
        //    this.AddTooltip(0x101, 70, 0x48, 0x1a, 0x103123);
        //    this.AddTooltip(0x101, 0x62, 0x48, 0x1a, 0x103124);
        //    this.AddTooltip(0x101, 0x7e, 0x48, 0x1a, 0x103125);
        //    this.AddTooltip(0x14d, 0x4a, 0x2e, 14, 0x103126);
        //    this.AddTooltip(0x14d, 0x5b, 0x2e, 14, 0x103127);
        //    this.AddTooltip(0x14d, 0x6a, 0x2e, 13, 0x103128);
        //    this.AddTooltip(0x14d, 120, 0x2e, 11, 0x103129);
        //    this.AddTooltip(0x14d, 0x85, 0x2e, 0x10, 0x10312a);
        //    this.OnRefresh();
        //    if (Engine.Features.AOS)
        //    {
        //        base.Tooltip = new MobileTooltip(m);
        //    }
        //}

        //private void AddTooltip(int x, int y, int w, int h, int num)
        //{
        //    GHotspot toAdd = new GHotspot(x, y, w, h, this);
        //    toAdd.m_CanDrag = true;
        //    toAdd.m_QuickDrag = false;
        //    toAdd.Tooltip = new Tooltip(Localization.GetString(num));
        //    base.m_Children.Add(toAdd);
        //}

        //public void Close()
        //{
        //    if ((Engine.m_Highlight == this.m_Mobile) && !this.m_Mobile.Player)
        //    {
        //        Engine.m_Highlight = null;
        //    }
        //    Gumps.Destroy(this);
        //}

        //private string FormatMinMax(int min, int max)
        //{
        //    return (min.ToString() + "/" + max.ToString());
        //}

        //private int GetMaxWeight(int str)
        //{
        //    return (40 + ((int)(3.5 * str)));
        //}

        //protected internal override bool HitTest(int X, int Y)
        //{
        //    base.m_QuickDrag = Engine.TargetHandler == null;
        //    return base.HitTest(X, Y);
        //}

        //public void OnArmorChange(int Armor)
        //{
        //    if (this.m_xArmor != Armor)
        //    {
        //        this.m_Armor.Text = Armor.ToString();
        //        this.m_xArmor = Armor;
        //    }
        //}

        //public void OnColdChange()
        //{
        //    if (this.m_xCold != this.m_Mobile.ColdResist)
        //    {
        //        this.m_xCold = this.m_Mobile.ColdResist;
        //        this.m_Cold.Text = this.m_xCold.ToString();
        //    }
        //}

        //public void OnDamageChange()
        //{
        //    if ((this.m_xDamageMin != this.m_Mobile.DamageMin) || (this.m_xDamageMax != this.m_Mobile.DamageMax))
        //    {
        //        this.m_xDamageMin = this.m_Mobile.DamageMin;
        //        this.m_xDamageMax = this.m_Mobile.DamageMax;
        //        this.m_Damages.Text = string.Format("{0}-{1}", this.m_xDamageMin, this.m_xDamageMax);
        //    }
        //}

        //public void OnDexChange(int Dex)
        //{
        //    if (this.m_xDex != Dex)
        //    {
        //        this.m_Dex.Text = Dex.ToString();
        //        this.m_xDex = Dex;
        //    }
        //}

        //protected internal override void OnDispose()
        //{
        //    if (this.m_Mobile != null)
        //    {
        //        this.m_Mobile.StatusBar = null;
        //        this.m_Mobile = null;
        //    }
        //}

        //protected internal override void OnDoubleClick(int X, int Y)
        //{
        //    if (Engine.TargetHandler == null)
        //    {
        //        this.m_Mobile.OnDoubleClick();
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
        //    if (this.m_xEnergy != this.m_Mobile.EnergyResist)
        //    {
        //        this.m_xEnergy = this.m_Mobile.EnergyResist;
        //        this.m_Energy.Text = this.m_xEnergy.ToString();
        //    }
        //}

        //public void OnFireChange()
        //{
        //    if (this.m_xFire != this.m_Mobile.FireResist)
        //    {
        //        this.m_xFire = this.m_Mobile.FireResist;
        //        this.m_Fire.Text = this.m_xFire.ToString();
        //    }
        //}

        //public void OnFlagsChange(MobileFlags flags)
        //{
        //}

        //public void OnFollCurChange(int current)
        //{
        //    if ((this.m_Followers != null) && ((this.m_xFollCur != current) || (this.m_xFollMax != this.m_Mobile.FollowersMax)))
        //    {
        //        this.m_Followers.Text = this.FormatMinMax(current, this.m_Mobile.FollowersMax);
        //        this.m_xFollCur = current;
        //        this.m_xFollMax = this.m_Mobile.FollowersMax;
        //    }
        //}

        //public void OnFollMaxChange(int maximum)
        //{
        //    if ((this.m_Followers != null) && ((this.m_xFollMax != maximum) || (this.m_xFollCur != this.m_Mobile.FollowersCur)))
        //    {
        //        this.m_Followers.Text = this.FormatMinMax(this.m_Mobile.FollowersCur, maximum);
        //        this.m_xFollCur = this.m_Mobile.FollowersCur;
        //        this.m_xFollMax = this.m_Mobile.FollowersMax;
        //    }
        //}

        //public void OnGenderChange(int Gender)
        //{
        //}

        //public void OnGoldChange(int Gold)
        //{
        //    if (this.m_xGold != Gold)
        //    {
        //        this.m_Gold.Text = Gold.ToString();
        //        this.m_xGold = Gold;
        //    }
        //}

        //public void OnHPCurChange(int HPCur)
        //{
        //    this.m_Hits.SetValues(HPCur, this.m_Mobile.HPMax);
        //}

        //public void OnHPMaxChange(int HPMax)
        //{
        //    this.m_Hits.SetValues(this.m_Mobile.HPCur, HPMax);
        //}

        //public void OnIntChange(int Int)
        //{
        //    if (this.m_xInt != Int)
        //    {
        //        this.m_Int.Text = Int.ToString();
        //        this.m_xInt = Int;
        //    }
        //}

        //public void OnLuckChange()
        //{
        //    if (this.m_xLuck != this.m_Mobile.Luck)
        //    {
        //        this.m_xLuck = this.m_Mobile.Luck;
        //        this.m_Luck.Text = this.m_xLuck.ToString();
        //    }
        //}

        //public void OnManaCurChange(int ManaCur)
        //{
        //    this.m_Mana.SetValues(ManaCur, this.m_Mobile.ManaMax);
        //}

        //public void OnManaMaxChange(int ManaMax)
        //{
        //    this.m_Mana.SetValues(this.m_Mobile.ManaCur, ManaMax);
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
        //        Mobile mobile = this.m_Mobile;
        //        this.Close();
        //        mobile.BigStatus = false;
        //        Engine.CancelClick();
        //    }
        //    if ((mb & MouseButtons.Left) != MouseButtons.None)
        //    {
        //        if (Engine.TargetHandler != null)
        //        {
        //            this.m_Mobile.OnTarget();
        //            Engine.CancelClick();
        //        }
        //        else
        //        {
        //            this.m_Mobile.Look();
        //        }
        //    }
        //}

        //public void OnNameChange(string Name)
        //{
        //    if (this.m_xName != Name)
        //    {
        //        this.m_Name.Text = Name;
        //        this.m_xName = Name;
        //        this.m_Name.X = 0x27 + ((0x160 - ((this.m_Name.Image.xMax - this.m_Name.Image.xMin) + 1)) / 2);
        //        this.m_Name.X -= this.m_Name.Image.xMin;
        //    }
        //}

        //public void OnNotorietyChange(Notoriety n)
        //{
        //}

        //public void OnPoisonChange()
        //{
        //    if (this.m_xPoison != this.m_Mobile.PoisonResist)
        //    {
        //        this.m_xPoison = this.m_Mobile.PoisonResist;
        //        this.m_Poison.Text = this.m_xPoison.ToString();
        //    }
        //}

        //public void OnRefresh()
        //{
        //    Mobile mobile = this.m_Mobile;
        //    if (mobile == null)
        //    {
        //        this.Close();
        //    }
        //    this.OnNameChange(mobile.Name);
        //    this.OnStrChange(mobile.Str);
        //    this.OnHPCurChange(mobile.HPCur);
        //    this.OnDexChange(mobile.Dex);
        //    this.OnStamCurChange(mobile.StamCur);
        //    this.OnIntChange(mobile.Int);
        //    this.OnManaCurChange(mobile.ManaCur);
        //    this.OnArmorChange(mobile.Armor);
        //    this.OnFireChange();
        //    this.OnColdChange();
        //    this.OnPoisonChange();
        //    this.OnEnergyChange();
        //    this.OnLuckChange();
        //    this.OnDamageChange();
        //    this.OnGoldChange(mobile.Gold);
        //    this.OnWeightChange(mobile.Weight);
        //    this.OnNotorietyChange(mobile.Notoriety);
        //    this.OnStatCapChange(mobile.StatCap);
        //    this.OnFollCurChange(mobile.FollowersCur);
        //}

        //public void OnStamCurChange(int StamCur)
        //{
        //    this.m_Stam.SetValues(StamCur, this.m_Mobile.StamMax);
        //}

        //public void OnStamMaxChange(int StamMax)
        //{
        //    this.m_Stam.SetValues(this.m_Mobile.StamCur, StamMax);
        //}

        //public void OnStatCapChange(int statCap)
        //{
        //    if ((this.m_StatCap != null) && (this.m_xStatCap != statCap))
        //    {
        //        this.m_StatCap.Text = statCap.ToString();
        //        this.m_xStatCap = statCap;
        //    }
        //}

        //public void OnStrChange(int Str)
        //{
        //    if (this.m_xStr != Str)
        //    {
        //        this.m_Str.Text = Str.ToString();
        //        this.m_xStr = Str;
        //        this.m_Weight.SetValues(this.m_Mobile.Weight, this.GetMaxWeight(Str));
        //    }
        //}

        //public void OnWeightChange(int Weight)
        //{
        //    this.m_Weight.SetValues(Weight, this.GetMaxWeight(this.m_Mobile.Str));
        //}

        //public Client.Gump Gump
        //{
        //    get
        //    {
        //        return this;
        //    }
        //}

        //private class GMinimizer : GRegion
        //{
        //    private GStatusBar m_Owner;

        //    public GMinimizer(GStatusBar owner) : base(0x180, 0x92, 0x18, 0x19)
        //    {
        //        this.m_Owner = owner;
        //        base.m_Tooltip = new Tooltip("Minimize");
        //    }

        //    protected internal override void OnMouseUp(int x, int y, MouseButtons mb)
        //    {
        //        if ((mb & MouseButtons.Left) != MouseButtons.None)
        //        {
        //            Mobile mobile = this.m_Owner.m_Mobile;
        //            this.m_Owner.Close();
        //            mobile.BigStatus = false;
        //            mobile.OpenStatus(false);
        //        }
        //    }
        //}
    }
}
