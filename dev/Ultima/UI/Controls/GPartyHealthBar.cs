using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Ultima.Player;
using UltimaXNA.Ultima.World.Entities.Mobiles;

namespace UltimaXNA.Ultima.UI.Controls
{
    /// <summary>
    /// we need convert this class for party system
    /// </summary>
    public class GPartyHealthBar : IMobileStatus/*,Gump */
    {
        //private int m_Height;
        //private Mobile m_Mobile;
        //private GLabel m_Name;
        //private int m_Width;
        //private string m_xName;

        //public GPartyHealthBar(Mobile m, int x, int y) : base(x, y)
        //{
        //    base.m_CanDrag = true;
        //    base.m_QuickDrag = true;
        //    base.m_CanDrop = true;
        //    this.m_Mobile = m;
        //    this.m_Name = new GLabel("", Engine.GetUniFont(0), m.Visible ? Hues.GetNotoriety(m.Notoriety) : Hues.Grayscale, 4, 4);
        //    this.SetName(m.Name);
        //    base.m_Children.Add(this.m_Name);
        //}

        //public void Close()
        //{
        //    if ((Engine.m_Highlight == this.m_Mobile) && !this.m_Mobile.Player)
        //    {
        //        Engine.m_Highlight = null;
        //    }
        //    Gumps.Destroy(this);
        //    this.m_Mobile.StatusBar = null;
        //}

        //protected internal override void Draw(int x, int y)
        //{
        //    Renderer.SetTexture(null);
        //    Renderer.AlphaTestEnable = false;
        //    Renderer.SetAlphaEnable(true);
        //    Renderer.SetAlpha(0.4f);
        //    Renderer.SolidRect(0, x + 1, y + 1, this.m_Width - 2, this.m_Height - 2);
        //    Renderer.SetAlphaEnable(false);
        //    Renderer.TransparentRect(0, x, y, this.m_Width, this.m_Height);
        //    y += this.m_Height;
        //    y -= 0x13;
        //    Renderer.SolidRect(0, x, y, this.m_Width, 0x13);
        //    x++;
        //    y++;
        //    int width = this.m_Width - 2;
        //    if (this.m_Mobile.Ghost)
        //    {
        //        Renderer.GradientRect(0xc0c0c0, 0x606060, x, y, width, 5);
        //        y += 6;
        //        Renderer.GradientRect(0xc0c0c0, 0x606060, x, y, width, 5);
        //        y += 6;
        //        Renderer.GradientRect(0xc0c0c0, 0x606060, x, y, width, 5);
        //    }
        //    else
        //    {
        //        int num2;
        //        int num3;
        //        MobileFlags flags = this.m_Mobile.Flags;
        //        if (flags[MobileFlag.Poisoned])
        //        {
        //            num2 = 0xff00;
        //            num3 = 0x8000;
        //        }
        //        else if (flags[MobileFlag.YellowHits])
        //        {
        //            num2 = 0xffc000;
        //            num3 = 0x806000;
        //        }
        //        else
        //        {
        //            num2 = 0x20c0ff;
        //            num3 = 0x106080;
        //        }
        //        int num4 = (this.m_Mobile.HPCur * width) / Math.Max(1, this.m_Mobile.HPMax);
        //        if (num4 > width)
        //        {
        //            num4 = width;
        //        }
        //        else if (num4 < 0)
        //        {
        //            num4 = 0;
        //        }
        //        Renderer.GradientRect(num2, num3, x, y, num4, 5);
        //        Renderer.GradientRect(0xff0000, 0x800000, x + num4, y, width - num4, 5);
        //        y += 6;
        //        num4 = (this.m_Mobile.ManaCur * width) / Math.Max(1, this.m_Mobile.ManaMax);
        //        if (num4 > width)
        //        {
        //            num4 = width;
        //        }
        //        else if (num4 < 0)
        //        {
        //            num4 = 0;
        //        }
        //        Renderer.GradientRect(0x20c0ff, 0x106080, x, y, num4, 5);
        //        Renderer.GradientRect(0xff0000, 0x800000, x + num4, y, width - num4, 5);
        //        y += 6;
        //        num4 = (this.m_Mobile.StamCur * width) / Math.Max(1, this.m_Mobile.StamMax);
        //        if (num4 > width)
        //        {
        //            num4 = width;
        //        }
        //        else if (num4 < 0)
        //        {
        //            num4 = 0;
        //        }
        //        Renderer.GradientRect(0x20c0ff, 0x106080, x, y, num4, 5);
        //        Renderer.GradientRect(0xff0000, 0x800000, x + num4, y, width - num4, 5);
        //    }
        //    Renderer.AlphaTestEnable = true;
        //}

        //protected internal override bool HitTest(int x, int y)
        //{
        //    return true;
        //}

        //public void OnArmorChange(int armor)
        //{
        //}

        //public void OnColdChange()
        //{
        //}

        //public void OnDamageChange()
        //{
        //}

        //public void OnDexChange(int dex)
        //{
        //}

        //protected internal override void OnDoubleClick(int x, int y)
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
        //        if (item != null)
        //        {
        //            Network.Send(new PDropItem(item.Serial, -1, -1, 0, this.m_Mobile.Serial));
        //            Gumps.Destroy(g);
        //        }
        //    }
        //}

        //protected internal override void OnDragStart()
        //{
        //    if ((Control.ModifierKeys & Keys.Shift) == Keys.None)
        //    {
        //        base.m_IsDragging = false;
        //        Gumps.Drag = null;
        //    }
        //}

        //public void OnEnergyChange()
        //{
        //}

        //public void OnFireChange()
        //{
        //}

        //public void OnFlagsChange(MobileFlags flags)
        //{
        //    if (this.m_Mobile.Visible)
        //    {
        //        this.m_Name.Hue = Hues.GetNotoriety(this.m_Mobile.Notoriety);
        //    }
        //    else
        //    {
        //        this.m_Name.Hue = Hues.Grayscale;
        //    }
        //}

        //public void OnFollCurChange(int current)
        //{
        //}

        //public void OnFollMaxChange(int maximum)
        //{
        //}

        //public void OnGenderChange(int gender)
        //{
        //}

        //public void OnGoldChange(int gold)
        //{
        //}

        //public void OnHPCurChange(int cur)
        //{
        //}

        //public void OnHPMaxChange(int max)
        //{
        //}

        //public void OnIntChange(int intel)
        //{
        //}

        //public void OnLuckChange()
        //{
        //}

        //public void OnManaCurChange(int cur)
        //{
        //}

        //public void OnManaMaxChange(int max)
        //{
        //}

        //protected internal override void OnMouseDown(int x, int y, MouseButtons mb)
        //{
        //    if (((mb & MouseButtons.Right) != MouseButtons.None) && ((Control.ModifierKeys & Keys.Shift) == Keys.None))
        //    {
        //        Point point = base.PointToScreen(new Point(x, y));
        //        int distance = 0;
        //        Engine.movingDir = Engine.GetDirection(point.X, point.Y, ref distance);
        //        Engine.amMoving = true;
        //    }
        //    else
        //    {
        //        base.BringToTop();
        //    }
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

        //protected internal override void OnMouseMove(int X, int Y, MouseButtons mb)
        //{
        //    if (((mb & MouseButtons.Right) != MouseButtons.None) && Engine.amMoving)
        //    {
        //        Point point = base.PointToScreen(new Point(X, Y));
        //        int distance = 0;
        //        Engine.movingDir = Engine.GetDirection(point.X, point.Y, ref distance);
        //    }
        //}

        //protected internal override void OnMouseUp(int X, int Y, MouseButtons mb)
        //{
        //    if ((mb & MouseButtons.Right) != MouseButtons.None)
        //    {
        //        if ((Control.ModifierKeys & Keys.Shift) != Keys.None)
        //        {
        //            this.Close();
        //            Engine.CancelClick();
        //        }
        //        else
        //        {
        //            Engine.amMoving = false;
        //        }
        //    }
        //    else if (((mb & MouseButtons.Left) != MouseButtons.None) && ((Control.ModifierKeys & Keys.Control) != Keys.None))
        //    {
        //        GRadar.m_FocusMob = this.m_Mobile;
        //    }
        //    else if ((Engine.TargetHandler != null) && ((mb & MouseButtons.Left) != MouseButtons.None))
        //    {
        //        this.m_Mobile.OnTarget();
        //        Engine.CancelClick();
        //    }
        //}

        //public void OnNameChange(string name)
        //{
        //    if (this.m_xName != name)
        //    {
        //        this.SetName(name);
        //    }
        //}

        //public void OnNotorietyChange(Notoriety n)
        //{
        //    if (this.m_Mobile.Visible)
        //    {
        //        this.m_Name.Hue = Hues.GetNotoriety(n);
        //    }
        //    else
        //    {
        //        this.m_Name.Hue = Hues.Grayscale;
        //    }
        //}

        //public void OnPoisonChange()
        //{
        //}

        //public void OnRefresh()
        //{
        //    this.OnNameChange(this.m_Mobile.Name);
        //    this.OnNotorietyChange(this.m_Mobile.Notoriety);
        //}

        //protected internal override void OnSingleClick(int x, int y)
        //{
        //    if (Engine.TargetHandler == null)
        //    {
        //        this.m_Mobile.OnSingleClick();
        //    }
        //}

        //public void OnStamCurChange(int cur)
        //{
        //}

        //public void OnStamMaxChange(int max)
        //{
        //}

        //public void OnStatCapChange(int statCap)
        //{
        //}

        //public void OnStrChange(int str)
        //{
        //}

        //public void OnWeightChange(int weight)
        //{
        //}

        //protected internal override void Render(int x, int y)
        //{
        //    base.Render(x, y);
        //}

        //private void SetName(string name)
        //{
        //    this.m_xName = name;
        //    if (this.m_Name.Font.GetStringWidth(name) > 70)
        //    {
        //        while ((name.Length > 0) && (this.m_Name.Font.GetStringWidth(name + "...") > 70))
        //        {
        //            name = name.Substring(0, name.Length - 1);
        //        }
        //        name = name + "...";
        //    }
        //    this.m_Name.Text = name;
        //    int num = ((this.m_Name.Image.xMax - this.m_Name.Image.xMin) + 1) + 6;
        //    if (num < 80)
        //    {
        //        num = 80;
        //    }
        //    this.m_Name.Y = 3 - this.m_Name.Image.yMin;
        //    this.m_Name.X = ((num - ((this.m_Name.Image.xMax - this.m_Name.Image.xMin) + 1)) / 2) - this.m_Name.Image.xMin;
        //    this.m_Width = num;
        //    this.m_Height = 0x26;
        //    base.m_DragClipX = this.m_Width - 1;
        //    base.m_DragClipY = this.m_Height - 1;
        //}

        //public Client.Gump Gump
        //{
        //    get
        //    {
        //        return this;
        //    }
        //}

        //public override int Height
        //{
        //    get
        //    {
        //        return this.m_Height;
        //    }
        //}

        //public override int Width
        //{
        //    get
        //    {
        //        return this.m_Width;
        //    }
        //}
    
    }
}
