using InterXLib.Display;
using InterXLib.XGUI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InterXLib.XGUI.Elements.Views
{
    public class ImageBoxView : AElementView
    {
        public ImageBoxView(ImageBox image_box, GUIManager manager)
            : base(image_box, manager)
        {

        }

        new ImageBox Model
        {
            get
            {
                return (ImageBox)base.Model;
            }
        }

        protected override void InternalDraw(YSpriteBatch spritebatch, double frameTime)
        {
            Color background = new Color(32, 32, 32, 255);
            Color border = Color.LightGray;

            DrawCommon_FillBackground(spritebatch, background);

            if (Model.Texture != null)
            {
                Rectangle area = Model.ScreenArea;
                area.X += 1;
                area.Y += 1;
                area.Width -= 2;
                area.Height -= 2;
                Texture2D texture = Model.Texture;

                if (Model.StretchImage)
                {
                    spritebatch.GUIDrawSprite(Model.Texture, Model.ScreenArea);
                }
                else if (texture.Width <= area.Width && texture.Height <= area.Height)
                {
                    Point xy = Model.Centered ? new Point(
                        area.X + (area.Width - texture.Width) / 2, 
                        area.Y + (area.Height - texture.Height) / 2) : 
                        new Point(area.X, area.Y);
                    Rectangle dest = new Rectangle(
                        xy.X, xy.Y,
                        texture.Width,
                        texture.Height);
                    spritebatch.GUIDrawSprite(Model.Texture, dest);
                }
                else
                {
                    float reduce_value = 1f;
                    if (texture.Width >= texture.Height)
                        reduce_value = ((float)area.Width / texture.Width);
                    else
                        reduce_value = ((float)area.Height / texture.Height);
                    Point xy = Model.Centered ? new Point(
                        area.X + area.Width - (int)(texture.Width * reduce_value), 
                        area.Y + area.Height - (int)(texture.Height * reduce_value)) :
                        new Point(area.X, area.Y);
                    Rectangle dest = new Rectangle(
                        xy.X, xy.Y,
                        (int)(texture.Width * reduce_value),
                        (int)(texture.Height * reduce_value));
                    spritebatch.GUIDrawSprite(Model.Texture, dest);
                }
            }

            DrawCommon_Border(spritebatch, border);
        }

        protected override void LoadRenderers()
        {

        }
    }
}
