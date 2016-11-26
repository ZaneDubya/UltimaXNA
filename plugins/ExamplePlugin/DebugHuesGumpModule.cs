using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Patterns;
using UltimaXNA.Core.Resources;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Ultima.UI.LoginGumps;

namespace UltimaXNA.Ultima.Login {
    class DebugHuesGumpModule : IModule {
        public string Name => "DebugHueTest";

        public void Load() {
            LoginGump.AddButton("Debug:Hues", OnClickDebugGump);
        }

        public void Unload() {
            LoginGump.RemoveButton(OnClickDebugGump);
        }

        void OnClickDebugGump() {
            Services.Get<UserInterfaceService>().AddControl(new DebugHuesGump(), 0, 0);
        }
    }

    class DebugHuesGump : Gump {
        TextLabel m_Label;
        HuedControl m_HueDisplay;

        public DebugHuesGump()
            : base(0, 0) {
            OverHue = -1;
        }

        public static string Caption {
            get {
                if (OverHue <= -1) {
                    return "Over: None";
                }
                return string.Format("Over: {0} [Hue index {1} 0x{1:x}]", OverHue, OverHue - 2);
            }
        }

        public static int OverHue {
            get;
            set;
        }

        protected override void OnInitialize() {
            Position = new Point(0, 0);
            Size = new Point(800, 600);
            AddControl(new ResizePic(this, 0, 0, 3000, 800, 600));
            int rowwidth = 60;
            // caption string
            m_Label = (TextLabel)AddControl(new TextLabel(this, 50, 8, 0, null));
            // object that is hued based on the current overhue.
            m_HueDisplay = (HuedControl)AddControl(new HuedControl(this, 8305));
            LastControl.Position = new Point(745, 15);
            ((HuedControl)LastControl).Hue = 0;
            // unhued object
            AddControl(new HuedControl(this));
            LastControl.Position = new Point(-5, 10);
            ((HuedControl)LastControl).Hue = 0;
            // hue index 1 (uo hue -1), aka one of the "True Black" hues
            AddControl(new HuedControl(this));
            LastControl.Position = new Point(3, 10);
            ((HuedControl)LastControl).Hue = 1;
            for (int i = 0; i < 3000; i++) {
                AddControl(new HuedControl(this));
                LastControl.Position = new Point((i % rowwidth) * 11 - 5, (i / rowwidth) * 10 + 28);
                ((HuedControl)LastControl).Hue = i + 2;
            }
            using (FileStream file = new FileStream("hues0.png", FileMode.Create)) {
                HueData.HueTexture0.SaveAsPng(file, HueData.HueTexture0.Width, HueData.HueTexture0.Height);
            }
            using (FileStream file = new FileStream("hues1.png", FileMode.Create)) {
                HueData.HueTexture1.SaveAsPng(file, HueData.HueTexture1.Width, HueData.HueTexture1.Height);
            }
        }

        public override void Update(double totalMS, double frameMS) {
            m_Label.Text = Caption;
            m_HueDisplay.Hue = OverHue <= -1 ? 0 : OverHue;
            base.Update(totalMS, frameMS);
        }

        class HuedControl : AControl {
            public int Hue;
            Texture2D m_Texture;
            int m_StaticTextureID;

            public HuedControl(AControl parent, int staticID = 0x1bf5)
                : base(parent) {
                HandlesMouseInput = true;
                m_StaticTextureID = staticID;
            }

            public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS) {
                if (m_Texture == null) {
                    IResourceProvider provider = Services.Get<IResourceProvider>();
                    m_Texture = provider.GetItemTexture(m_StaticTextureID);
                    Size = new Point(m_Texture.Width, m_Texture.Height);
                }
                spriteBatch.Draw2D(m_Texture, new Vector3(position.X, position.Y, 0), Utility.GetHueVector(Hue));
                base.Draw(spriteBatch, position, frameMS);
            }

            protected override void OnMouseOver(int x, int y) {
                OverHue = Hue;
            }

            protected override void OnMouseOut(int x, int y) {
                OverHue = -1;
            }

            protected override bool IsPointWithinControl(int x, int y) {
                IResourceProvider provider = Services.Get<IResourceProvider>();
                return provider.IsPointInUITexture(m_StaticTextureID, x, y);
            }
        }
    }
}
