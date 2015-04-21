using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.UI.Controls;
using System.IO;

namespace UltimaXNA.Ultima.Login.States
{
    class HueTestState : AState
    {
        Gump m_Gump;
        TextLabel m_Label;
        HuedControl m_HueDisplay;

        UserInterfaceService m_UserInterface;

        public HueTestState()
            : base()
        {
            OverHue = -1;

            m_UserInterface = UltimaServices.GetService<UserInterfaceService>();
        }

        public static string Caption
        {
            get
            {
                if (OverHue <= -1)
                {
                    return "Over: None";
                }
                else
                {
                    return string.Format("Over: {0} [Hue index {1} 0x{1:x}]", OverHue, OverHue - 2);
                }
            }
        }

        public static int OverHue
        {
            get;
            set;
        }

        public override void Intitialize()
        {
            base.Intitialize();

            m_Gump = (Gump)m_UserInterface.AddControl(new Gump(Serial.Null, Serial.Null), 0, 0);
            m_Gump.Size = new Point(800, 600);
            m_Gump.AddControl(new ResizePic(m_Gump, 0, 5, 5, 3000, 790, 590));

            int rowwidth = 60;

            // caption string
            m_Label = (TextLabel)m_Gump.AddControl(new TextLabel(m_Gump, 0, 50, 8, 0, null));

            // object that is hued based on the current overhue.
            m_HueDisplay = (HuedControl)m_Gump.AddControl(new HuedControl(m_Gump, 8305));
            m_Gump.LastControl.Position = new Point(745, 15);
            ((HuedControl)m_Gump.LastControl).Hue = 0;

            // unhued object
            m_Gump.AddControl(new HuedControl(m_Gump));
            m_Gump.LastControl.Position = new Point(-5, 10);
            ((HuedControl)m_Gump.LastControl).Hue = 0;

            // hue index 1 (uo hue -1), aka one of the "True Black" hues
            m_Gump.AddControl(new HuedControl(m_Gump));
            m_Gump.LastControl.Position = new Point(3, 10);
            ((HuedControl)m_Gump.LastControl).Hue = 1;

            for (int i = 0; i < 3000; i++)
            {
                m_Gump.AddControl(new HuedControl(m_Gump));
                m_Gump.LastControl.Position = new Point((i % rowwidth) * 11 - 5, (i / rowwidth) * 10 + 28);
                ((HuedControl)m_Gump.LastControl).Hue = i + 2;
            }

            using (System.IO.FileStream file = new System.IO.FileStream("hues0.png", System.IO.FileMode.Create))
            {
                IO.HuesXNA.HueTexture0.SaveAsPng(file, IO.HuesXNA.HueTexture0.Width, IO.HuesXNA.HueTexture0.Height);
            }

            using (System.IO.FileStream file = new System.IO.FileStream("hues1.png", System.IO.FileMode.Create))
            {
                IO.HuesXNA.HueTexture1.SaveAsPng(file, IO.HuesXNA.HueTexture1.Width, IO.HuesXNA.HueTexture1.Height);
            }
        }

        public override void Update(double totalTime, double frameTime)
        {
            m_Label.Text = Caption;
            m_HueDisplay.Hue = OverHue <= -1 ? 0 : OverHue;
            base.Update(totalTime, frameTime);
        }

        class HuedControl : AControl
        {
            private int m_StaticTextureID = 0;

            public HuedControl(AControl owner, int staticID = 0x1bf5)
                : base(owner, 0)
            {
                HandlesMouseInput = true;
                m_StaticTextureID = staticID;
            }

            public int Hue = 0;

            private Texture2D m_texture;

            public override void Draw(Core.Graphics.SpriteBatchUI spriteBatch)
            {
                if (m_texture == null)
                {
                    m_texture = IO.ArtData.GetStaticTexture(m_StaticTextureID);
                    Size = new Point(m_texture.Width, m_texture.Height);
                }
                spriteBatch.Draw2D(m_texture, new Vector3(X, Y, 0), Utility.GetHueVector(Hue));
                base.Draw(spriteBatch);
            }

            protected override void OnMouseOver(int x, int y)
            {
                OverHue = Hue;
            }

            protected override void OnMouseOut(int x, int y)
            {
                OverHue = -1;
            }

            protected override bool InternalHitTest(int x, int y)
            {
                if (m_texture != null)
                {
                    uint[] pixel = new uint[m_texture.Width * m_texture.Height];
                    m_texture.GetData<uint>(pixel);
                    if (pixel[y * m_texture.Width + x] != 0x00000000)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
