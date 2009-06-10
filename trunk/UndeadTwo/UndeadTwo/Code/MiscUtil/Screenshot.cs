#region File Description & Usings
//-----------------------------------------------------------------------------
// Screenshot.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace MiscUtil
{
    class Screenshot : DrawableGameComponent
    {
        // Screenshot Declarations
        Texture2D m_Screenshot;
        RenderTarget2D m_ScreenshotRenderTarget;

        public Screenshot(Game game)
            : base(game)
        {
            
        }

        public override void Initialize()
        {
            base.Initialize();
            m_ScreenshotRenderTarget = new RenderTarget2D(
                GraphicsDevice,
                Game.Window.ClientBounds.Width,
                Game.Window.ClientBounds.Height,
                0,
                SurfaceFormat.Rgba64);
        }

        public void PrepareForCapture()
        {
            // Set our RenderTarget
            GraphicsDevice.SetRenderTarget(0, m_ScreenshotRenderTarget);
        }

        public void CaptureScreenshot()
        {
            // Reset our rendertarget
            GraphicsDevice.SetRenderTarget(0, null);

            // Get the picture/texture from our RenderTarget
            m_Screenshot = m_ScreenshotRenderTarget.GetTexture();

            // Now lets just save our screenshot!
            m_Screenshot.Save("Screenshot.png", ImageFileFormat.Png);
        }
    }
}
