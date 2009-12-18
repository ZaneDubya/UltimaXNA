using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.Graphics
{
    /// <summary>
    /// Simple extension of the SpriteBatch class to allow use of custom shaders.
    /// </summary>
    public class ExtendedSpriteBatch
    {
        private bool _beginCalled;
        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spriteBatch;
        private Effect _effect;

        private SpriteBlendMode _currentBlendMode = SpriteBlendMode.AlphaBlend;
        private SpriteSortMode _currentSortMode = SpriteSortMode.Immediate;
        private SaveStateMode _currentStateMode = SaveStateMode.None;
        private Matrix _currentTransformMatrix = Matrix.Identity;

        /// <summary>
        /// The effect to use with the SpriteBatch
        /// </summary>
        public Effect Effect
        {
            get { return _effect; }
            set
            {
                if (_effect != value)
                {
                    End();

                    _effect = value;
                }
            }
        }

        /// <summary>
        /// Gets a boolean value indicating wether Begin was called
        /// </summary>
        public bool BeginCalled
        {
            get { return _beginCalled; }
        }

        /// <summary>
        /// Gets the GraphicsDevice used by the ExtendedSpriteBatch
        /// </summary>
        public GraphicsDevice GraphicsDevice
        {
            get { return _graphicsDevice; }
        }

        /// <summary>
        /// Creates an instance of ExtendedSpriteBatch
        /// </summary>
        /// <param name="graphicsDevice"></param>
        public ExtendedSpriteBatch(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = new SpriteBatch(graphicsDevice);
        }

        /// <summary>
        /// Calls Begin with the SpriteBlendState, SpriteSortMode, SaveStateMode and Transformation Matrix used from the last Begin call.
        /// </summary>
        public void BeginLast()
        {
            Begin(_currentBlendMode, _currentSortMode, _currentStateMode, _currentTransformMatrix);
        }

        /// <summary>
        /// Prepares the graphics device for drawing sprites with specified blending,
        /// sorting, and render state options, and a global transform matrix. Reference
        /// page contains links to related code samples.
        /// </summary>
        public void Begin()
        {
            Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None, Matrix.Identity);
        }

        /// <summary>
        /// Prepares the graphics device for drawing sprites with specified blending,
        /// sorting, and render state options, and a global transform matrix. Reference
        /// page contains links to related code samples.
        /// </summary>
        /// <param name="blendMode">Blending options to use when rendering.</param>
        public void Begin(SpriteBlendMode blendMode)
        {
            Begin(blendMode, SpriteSortMode.Immediate, SaveStateMode.None, Matrix.Identity);
        }

        /// <summary>
        /// Prepares the graphics device for drawing sprites with specified blending,
        /// sorting, and render state options, and a global transform matrix. Reference
        /// page contains links to related code samples.
        /// </summary>
        /// <param name="blendMode">Blending options to use when rendering.</param>
        /// <param name="sortMode">Sorting options to use when rendering.</param>
        /// <param name="stateMode">Rendering state options.</param>
        public void Begin(SpriteBlendMode blendMode, SpriteSortMode sortMode, SaveStateMode stateMode)
        {
            Begin(blendMode, sortMode, stateMode, Matrix.Identity);
        }

        /// <summary>
        /// Prepares the graphics device for drawing sprites with specified blending,
        /// sorting, and render state options, and a global transform matrix. Reference
        /// page contains links to related code samples.
        /// </summary>
        /// <param name="blendMode">Blending options to use when rendering.</param>
        /// <param name="sortMode">Sorting options to use when rendering.</param>
        /// <param name="stateMode">Rendering state options.</param>
        /// <param name="transformMatrix"> A matrix to apply to position, rotation, scale, and depth data passed to Draw.</param>
        public void Begin(SpriteBlendMode blendMode, SpriteSortMode sortMode, SaveStateMode stateMode, Matrix transformMatrix)
        {
            if (_beginCalled)
            {
                End();
            }

            _beginCalled = true;

            _currentBlendMode = blendMode;
            _currentSortMode = sortMode;
            _currentStateMode = stateMode;
            _currentTransformMatrix = transformMatrix;

            _spriteBatch.Begin(blendMode, sortMode, stateMode, transformMatrix);

            if (_effect != null)
            {
                _effect.Begin();
                _effect.CurrentTechnique.Passes[0].Begin();
            }
        }

        /// <summary>
        /// Adds a sprite to the batch of sprites to be rendered, specifying the texture,
        /// screen position, source rectangle, color tint, rotation, origin, scale, effects,
        /// and sort depth. Reference page contains links to related code samples.
        /// </summary>
        /// <param name="texture">The sprite texture.</param>
        /// <param name="destinationRectangle"> A rectangle specifying, in screen coordinates, where the sprite will be drawn.
        /// If this rectangle is not the same size as sourceRectangle, the sprite is scaled to fit.</param>
        /// <param name="color">The color channel modulation to use. Use Color.White for full color with no tinting.</param>
        public void Draw(Texture2D texture, Rectangle destinationRectangle, Color color)
        {
            _spriteBatch.Draw(texture, destinationRectangle, color);
        }

        /// <summary>
        /// Adds a sprite to the batch of sprites to be rendered, specifying the texture,
        /// screen position, source rectangle, color tint, rotation, origin, scale, effects,
        /// and sort depth. Reference page contains links to related code samples.
        /// </summary>
        /// <param name="texture">The sprite texture.</param>
        /// <param name="position">The location, in screen coordinates, where the sprite will be drawn.</param>
        /// <param name="color">The color channel modulation to use. Use Color.White for full color with no tinting.</param>
        public void Draw(Texture2D texture, Vector2 position, Color color)
        {
            _spriteBatch.Draw(texture, position, color);
        }

        /// <summary>
        /// Adds a sprite to the batch of sprites to be rendered, specifying the texture,
        /// screen position, source rectangle, color tint, rotation, origin, scale, effects,
        /// and sort depth. Reference page contains links to related code samples.
        /// </summary>
        /// <param name="texture">The sprite texture.</param>
        /// <param name="destinationRectangle"> A rectangle specifying, in screen coordinates, where the sprite will be drawn.
        /// If this rectangle is not the same size as sourceRectangle, the sprite is scaled to fit.</param>
        /// <param name="sourceRectangle">A rectangle specifying, in texels, which section of the rectangle to draw. Use null to draw the entire texture.</param>
        /// <param name="color">The color channel modulation to use. Use Color.White for full color with no tinting.</param>
        public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
        {
            _spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color);
        }

        /// <summary>
        /// Adds a sprite to the batch of sprites to be rendered, specifying the texture,
        /// screen position, source rectangle, color tint, rotation, origin, scale, effects,
        /// and sort depth. Reference page contains links to related code samples.
        /// </summary>
        /// <param name="texture">The sprite texture.</param>
        /// <param name="position">The location, in screen coordinates, where the sprite will be drawn.</param>
        /// <param name="sourceRectangle">A rectangle specifying, in texels, which section of the rectangle to draw. Use null to draw the entire texture.</param>
        /// <param name="color">The color channel modulation to use. Use Color.White for full color with no tinting.</param>
        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
        {
            _spriteBatch.Draw(texture, position, sourceRectangle, color);
        }

        /// <summary>
        /// Adds a sprite to the batch of sprites to be rendered, specifying the texture,
        /// screen position, source rectangle, color tint, rotation, origin, scale, effects,
        /// and sort depth. Reference page contains links to related code samples.
        /// </summary>
        /// <param name="texture">The sprite texture.</param>
        /// <param name="destinationRectangle"> A rectangle specifying, in screen coordinates, where the sprite will be drawn.
        /// If this rectangle is not the same size as sourceRectangle, the sprite is scaled to fit.</param>
        /// <param name="sourceRectangle">A rectangle specifying, in texels, which section of the rectangle to draw. Use null to draw the entire texture.</param>
        /// <param name="color">The color channel modulation to use. Use Color.White for full color with no tinting.</param>
        /// <param name="rotation">The angle, in radians, to rotate the sprite around the origin.</param>
        /// <param name="origin">The origin of the sprite. Specify (0,0) for the upper-left corner.</param>
        /// <param name="scale">Uniform multiple by which to scale the sprite width and height.</param>
        /// <param name="effects">Rotations to apply before rendering.</param>
        /// <param name="layerDepth">The sorting depth of the sprite, between 0 (front) and 1 (back). You must
        /// specify either SpriteSortMode.FrontToBack or SpriteSortMode.BackToFront forthis parameter to affect sprite drawing.</param>
        public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
        {
            _spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth);
        }

        /// <summary>
        /// Adds a sprite to the batch of sprites to be rendered, specifying the texture,
        /// screen position, source rectangle, color tint, rotation, origin, scale, effects,
        /// and sort depth. Reference page contains links to related code samples.
        /// </summary>
        /// <param name="texture">The sprite texture.</param>
        /// <param name="position">The location, in screen coordinates, where the sprite will be drawn.</param>
        /// <param name="sourceRectangle">A rectangle specifying, in texels, which section of the rectangle to draw. Use null to draw the entire texture.</param>
        /// <param name="color">The color channel modulation to use. Use Color.White for full color with no tinting.</param>
        /// <param name="rotation">The angle, in radians, to rotate the sprite around the origin.</param>
        /// <param name="origin">The origin of the sprite. Specify (0,0) for the upper-left corner.</param>
        /// <param name="scale">Vector containing separate scalar multiples for the x- and y-axes of the sprite.</param>
        /// <param name="effects">Rotations to apply before rendering.</param>
        /// <param name="layerDepth">The sorting depth of the sprite, between 0 (front) and 1 (back). You must
        /// specify either SpriteSortMode.FrontToBack or SpriteSortMode.BackToFront forthis parameter to affect sprite drawing.</param>
        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            _spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        }

        /// <summary>
        /// Adds a sprite to the batch of sprites to be rendered, specifying the texture,
        /// screen position, source rectangle, color tint, rotation, origin, scale, effects,
        /// and sort depth. Reference page contains links to related code samples.
        /// </summary>
        /// <param name="texture">The sprite texture.</param>
        /// <param name="position">The location, in screen coordinates, where the sprite will be drawn.</param>
        /// <param name="sourceRectangle">A rectangle specifying, in texels, which section of the rectangle to draw. Use null to draw the entire texture.</param>
        /// <param name="color">The color channel modulation to use. Use Color.White for full color with no tinting.</param>
        /// <param name="rotation">The angle, in radians, to rotate the sprite around the origin.</param>
        /// <param name="origin">The origin of the sprite. Specify (0,0) for the upper-left corner.</param>
        /// <param name="scale">Uniform multiple by which to scale the sprite width and height.</param>
        /// <param name="effects"> Rotations to apply before rendering.</param>
        /// <param name="layerDepth">The sorting depth of the sprite, between 0 (front) and 1 (back). You must
        /// specify either SpriteSortMode.FrontToBack or SpriteSortMode.BackToFront forthis parameter to affect sprite drawing.</param>
        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            _spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
        }

        /// <summary>
        /// Adds a mutable sprite string to the batch of sprites to be rendered, specifying 
        /// the font, output text, screen position, color tint, rotation, origin, scale, effects, 
        /// and depth. Reference page contains links to related code samples.
        /// </summary>
        /// <param name="spriteFont">The sprite font.</param>
        /// <param name="text">The string to draw.</param>
        /// <param name="position">The location, in screen coordinates, where the text will be drawn.</param>
        /// <param name="color">The desired color of the text.</param>
        public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color)
        {
            _spriteBatch.DrawString(spriteFont, text, position, color);
        }

        /// <summary>
        /// Adds a mutable sprite string to the batch of sprites to be rendered, specifying 
        /// the font, output text, screen position, color tint, rotation, origin, scale, effects, 
        /// and depth. Reference page contains links to related code samples.
        /// </summary>
        /// <param name="spriteFont">The sprite font.</param>
        /// <param name="text">The mutable (read/write) string to draw.</param>
        /// <param name="position">The location, in screen coordinates, where the text will be drawn.</param>
        /// <param name="color">The desired color of the text.</param>
        public void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color)
        {
            _spriteBatch.DrawString(spriteFont, text, position, color);
        }

        /// <summary>
        /// Adds a mutable sprite string to the batch of sprites to be rendered, specifying 
        /// the font, output text, screen position, color tint, rotation, origin, scale, effects, 
        /// and depth. Reference page contains links to related code samples.
        /// </summary>
        /// <param name="spriteFont">The sprite font.</param>
        /// <param name="text">The string to draw.</param>
        /// <param name="position">The location, in screen coordinates, where the text will be drawn.</param>
        /// <param name="color">The desired color of the text.</param>
        /// <param name="rotation">The angle, in radians, to rotate the text around the origin.</param>
        /// <param name="origin">The origin of the string. Specify (0,0) for the upper-left corner.</param>
        /// <param name="scale"> Vector containing separate scalar multiples for the x- and y-axes of the sprite.</param>
        /// <param name="effects">Rotations to apply prior to rendering.</param>
        /// <param name="layerDepth">The sorting depth of the sprite, between 0 (front) and 1 (back).</param>
        public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            _spriteBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
        }

        /// <summary>
        /// Adds a mutable sprite string to the batch of sprites to be rendered, specifying 
        /// the font, output text, screen position, color tint, rotation, origin, scale, effects, 
        /// and depth. Reference page contains links to related code samples.
        /// </summary>
        /// <param name="spriteFont">The sprite font.</param>
        /// <param name="text">The string to draw.</param>
        /// <param name="position">The location, in screen coordinates, where the text will be drawn.</param>
        /// <param name="color">The desired color of the text.</param>
        /// <param name="rotation">The angle, in radians, to rotate the text around the origin.</param>
        /// <param name="origin">The origin of the string. Specify (0,0) for the upper-left corner.</param>
        /// <param name="scale">Uniform multiple by which to scale the sprite width and height.</param>
        /// <param name="effects">Rotations to apply prior to rendering.</param>
        /// <param name="layerDepth">The sorting depth of the sprite, between 0 (front) and 1 (back).</param>
        public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            _spriteBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
        }

        /// <summary>
        /// Adds a mutable sprite string to the batch of sprites to be rendered, specifying 
        /// the font, output text, screen position, color tint, rotation, origin, scale, effects, 
        /// and depth. Reference page contains links to related code samples.
        /// </summary>
        /// <param name="spriteFont">The sprite font.</param>
        /// <param name="text">The mutable (read/write) string to draw.</param>
        /// <param name="position">The location, in screen coordinates, where the text will be drawn.</param>
        /// <param name="color">The desired color of the text.</param>
        /// <param name="rotation">The angle, in radians, to rotate the text around the origin.</param>
        /// <param name="origin">The origin of the string. Specify (0,0) for the upper-left corner.</param>
        /// <param name="scale">Vector containing separate scalar multiples for the x- and y-axes of the sprite.</param>
        /// <param name="effects">Rotations to apply prior to rendering.</param>
        /// <param name="layerDepth">The sorting depth of the sprite, between 0 (front) and 1 (back).</param>
        public void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            _spriteBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
        }

        /// <summary>
        /// Adds a mutable sprite string to the batch of sprites to be rendered, specifying 
        /// the font, output text, screen position, color tint, rotation, origin, scale, effects, 
        /// and depth. Reference page contains links to related code samples.
        /// </summary>
        /// <param name="spriteFont">The sprite font.</param>
        /// <param name="text">The mutable (read/write) string to draw.</param>
        /// <param name="position">The location, in screen coordinates, where the text will be drawn.</param>
        /// <param name="color">The desired color of the text.</param>
        /// <param name="rotation">The angle, in radians, to rotate the text around the origin.</param>
        /// <param name="origin">The origin of the string. Specify (0,0) for the upper-left corner.</param>
        /// <param name="scale"> Vector containing separate scalar multiples for the x- and y-axes of the sprite.</param>
        /// <param name="effects">Rotations to apply prior to rendering.</param>
        /// <param name="layerDepth">The sorting depth of the sprite, between 0 (front) and 1 (back).</param>
        public void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            _spriteBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, effects, layerDepth);
        }

        /// <summary>
        /// Flushes the sprite batch and restores the device state to how it was before
        /// Begin was called. 
        /// </summary>
        public void End()
        {
            End(false);
        }

        /// <summary>
        /// Flushes the sprite batch and restores the device state to how it was before
        /// Begin was called. 
        /// </summary>
        /// <param name="dropShader">If true, this will set Effect to null</param>
        public void End(bool dropShader)
        {
            if (_beginCalled)
            {
                _beginCalled = false;
                _spriteBatch.End();

                if (_effect != null)
                {
                    _effect.CurrentTechnique.Passes[0].End();
                    _effect.End();
                }
            }

            if (dropShader)
            {
                _effect = null;
            }
        }
    }
}
