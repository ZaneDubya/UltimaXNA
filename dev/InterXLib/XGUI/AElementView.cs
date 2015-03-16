using InterXLib.Display;
using InterXLib.XGUI.Rendering;
using Microsoft.Xna.Framework;

namespace InterXLib.XGUI
{
    public abstract class AElementView : Patterns.MVC.AView
    {
        new AElement Model
        {
            get
            {
                return (AElement)base.Model;
            }
        }

        protected YSpriteFont Font
        {
            get
            {
                return Manager.GetFont(Model.FontName);
            }
        }

        protected GUIManager Manager;

        public virtual Rectangle BuildChildArea(Point area)
        {
            return new Rectangle(0, 0, area.X, area.Y);
        }

        public AElementView(AElement model, GUIManager manager)
            : base(model)
        {
            Manager = manager;
        }

        public override void Draw(YSpriteBatch spritebatch, double frameTime)
        {
            
            if (!Model.IsActive)
                return;
            if (!m_RenderersLoaded)
                LoadRenderers();

            InternalBeforeDraw();

            InternalDraw(spritebatch, frameTime);

            if (Model.Children != null)
            {
                foreach (AElement c in Model.Children)
                {
                    if (c.IsActive)
                        c.GetView().Draw(spritebatch, frameTime);
                }
            }

            InternalAfterDraw();
        }

        protected void DrawCommon_FillBackground(YSpriteBatch spritebatch, Color color)
        {
            spritebatch.DrawRectangleFilled(
                new Vector3(Model.ScreenArea.X, Model.ScreenArea.Y, 0),
                new Vector2(Model.ScreenArea.Width, Model.ScreenArea.Height), color);
        }

        protected void DrawCommon_Border(YSpriteBatch spritebatch, Color color)
        {
            spritebatch.DrawRectangle(
                new Vector3(Model.ScreenArea.X, Model.ScreenArea.Y, 0),
                new Vector2(Model.ScreenArea.Width, Model.ScreenArea.Height), color);
        }

        protected T LoadRenderer<T>(string name_skin, string name_renderer) where T : ARenderer
        {
            return (T)Manager.Skins[name_skin].Renderers[name_renderer];
        }

        protected abstract void InternalDraw(YSpriteBatch spritebatch, double frameTime);
        protected virtual void InternalBeforeDraw() { }
        protected virtual void InternalAfterDraw() { }

        private bool m_RenderersLoaded = false;
        protected abstract void LoadRenderers();
    }
}
