using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.UltimaGUI;
using UltimaXNA.UltimaGUI.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UltimaLogin.Scenes
{
    class HueTestScene : AScene
    {
        Gump m_Gump;

        public HueTestScene()
        {
            
        }

        public override void Intitialize(UltimaClient client)
        {
            base.Intitialize(client);

            m_Gump = (Gump)UltimaEngine.UserInterface.AddControl(new Gump(Serial.Null, Serial.Null), 0, 0);
            m_Gump.Size = new Point(800, 600);


            {
                m_Gump.AddControl(new HuedControl(m_Gump));
                ((HuedControl)m_Gump.LastControl).Hue = i + 2;
            }
        }

        class HuedControl : Control
        {
            public HuedControl(Control owner)
                : base(owner, 0)
            {

            }

            public int Hue = 0;

            private Texture2D m_texture;

            public override void Draw(Core.Rendering.SpriteBatchUI spriteBatch)
            {
                if (m_texture == null)
                {
                    Size = new Point(m_texture.Width, m_texture.Height);
                }
                spriteBatch.Draw2D(m_texture, Position, Hue, false, false);
                base.Draw(spriteBatch);
            }
        }
    }
}
