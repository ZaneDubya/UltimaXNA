#region File Description & Usings
//-----------------------------------------------------------------------------
// SpriteSheet.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace UltimaXNA.GUI
{
    class SpriteSheet
    {
        Texture2D textureImage;
        public Vector2 position;
        protected Point frameSize;
        public Point currentFrame;
        protected Point sheetSize;

        public SpriteSheet(Texture2D textureImage, Vector2 position, Point frameSize,
            Point currentFrame, Point sheetSize)
        {
            this.textureImage = textureImage;
            this.position = position;
            this.frameSize = frameSize;
            this.currentFrame = currentFrame;
            this.sheetSize = sheetSize;
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Draw the sprite
            spriteBatch.Draw(textureImage,
                position,
                new Rectangle(currentFrame.X * frameSize.X,
                    currentFrame.Y * frameSize.Y,
                    frameSize.X, frameSize.Y),
                Color.White, 0, Vector2.Zero,
                1f, SpriteEffects.None, 0);
        }
    }
}
