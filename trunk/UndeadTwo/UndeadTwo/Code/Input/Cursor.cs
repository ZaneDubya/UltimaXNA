#region File Description
//-----------------------------------------------------------------------------
// Cursor.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace UndeadClient.Input
{
    /// <summary>
    /// Cursor is a DrawableGameComponent that draws a cursor on the screen. It works
    /// differently on Xbox and Windows. On windows, this will be a cursor that is
    /// controlled using both the mouse and the gamepad. On Xbox, the cursor will be
    /// controlled using only the gamepad.
    /// </summary>
    public class Cursors : DrawableGameComponent
    {
        #region Constants
        // this constant controls how fast the gamepad moves the cursor. this constant
        // is in pixels per second.
        const float CursorSpeed = 250.0f;
        #endregion

        #region Fields and properties
        // Position is the cursor position, and is in screen space. 
        private Vector2 position;
        public Vector2 Position
        {
            get { return position; }
        }

        #endregion

        #region Creation and initialization

        // this constructor doesn't really do much of anything, just calls the base 
        // constructor, and saves the contentmanager so it can be used in
        // LoadContent.
        public Cursors(Game game, ContentManager content)
            : base(game)
        {
        }
        // on Xbox360, initialize is overriden so that we can center the cursor once we
        // know how big the viewport will be.
        #if XBOX360
                public override void Initialize()
                {            
                    base.Initialize();

                    Viewport vp = GraphicsDevice.Viewport;

                    position.X = vp.X + (vp.Width / 2);
                    position.Y = vp.Y + (vp.Height / 2);
                }
        #endif
        #endregion

        #region Update

        // Update gets the current gamepad state and mouse state and uses that data to
        // calculate where the cursor's position is on the screen. On xbox, the position
        // is clamped to the viewport so that the cursor can't go off the screen. On
        // windows, doing something like that would be rude :)
        public override void Update(GameTime gameTime)
        {
            if (gameTime == null)
                throw new ArgumentNullException("gameTime");

            GamePadState currentState = GamePad.GetState(PlayerIndex.One);

            // we'll create a vector2, called delta, which will store how much the
            // cursor position should change.
            Vector2 delta = currentState.ThumbSticks.Left;

            // down on the thumbstick is -1. however, in screen coordinates, values
            // increase as they go down the screen. so, we have to flip the sign of the
            // y component of delta.
            delta.Y *= -1;

            // check the dpad: if any of its buttons are pressed, that will change delta
            // as well.
            if (currentState.DPad.Up == ButtonState.Pressed)
            {
                delta.Y = -1;
            }
            if (currentState.DPad.Down == ButtonState.Pressed)
            {
                delta.Y = 1;
            }
            if (currentState.DPad.Left == ButtonState.Pressed)
            {
                delta.X = -1;
            }
            if (currentState.DPad.Right == ButtonState.Pressed)
            {
                delta.X = 1;
            }

            // normalize delta so that we know the cursor can't move faster than
            // CursorSpeed.
            if (delta != Vector2.Zero)
            {
                delta.Normalize();
            }

#if XBOX360
            // modify position using delta, the CursorSpeed constant defined above, and
            // the elapsed game time.
            position += delta * CursorSpeed *
                (float)gameTime.ElapsedGameTime.TotalSeconds;

            // clamp the cursor position to the viewport, so that it can't move off the
            // screen.
            Viewport vp = GraphicsDevice.Viewport;
            position.X = MathHelper.Clamp(position.X, vp.X, vp.X + vp.Width);
            position.Y = MathHelper.Clamp(position.Y, vp.Y, vp.Y + vp.Height);
#else
            MouseState mouseState = Mouse.GetState();
            position.X = mouseState.X;
            position.Y = mouseState.Y;

            if (Game.IsActive)
            {
                // modify position using delta, the CursorSpeed constant defined above,
                // and the elapsed game time, only if the cursor is on the screen
                Viewport vp = GraphicsDevice.Viewport;
                if ((vp.X <= position.X) && (position.X <= (vp.X + vp.Width)) &&
                    (vp.Y <= position.Y) && (position.Y <= (vp.Y + vp.Height)))
                {
                    position += delta * CursorSpeed *
                        (float)gameTime.ElapsedGameTime.TotalSeconds;
                    position.X = MathHelper.Clamp(position.X, vp.X, vp.X + vp.Width);
                    position.Y = MathHelper.Clamp(position.Y, vp.Y, vp.Y + vp.Height);
                }
                else if (delta.LengthSquared() > 0f)
                {
                    position.X = vp.X + vp.Width / 2;
                    position.Y = vp.Y + vp.Height / 2;
                }

                // set the new mouse position using the combination of mouse and gamepad
                // data.
                Mouse.SetPosition((int)position.X, (int)position.Y);
            }
#endif
            base.Update(gameTime);
        }

        #endregion

        // CalculateCursorRay Calculates a world space ray starting at the camera's
        // "eye" and pointing in the direction of the cursor. Viewport.Unproject is used
        // to accomplish this. see the accompanying documentation for more explanation
        // of the math behind this function.
        public Ray CalculateCursorRay(Matrix projectionMatrix, Matrix viewMatrix)
        {
            // create 2 positions in screenspace using the cursor position. 0 is as
            // close as possible to the camera, 1 is as far away as possible.
            Vector3 nearSource = new Vector3(Position, 0f);
            Vector3 farSource = new Vector3(Position, 1f);

            // use Viewport.Unproject to tell what those two screen space positions
            // would be in world space. we'll need the projection matrix and view
            // matrix, which we have saved as member variables. We also need a world
            // matrix, which can just be identity.
            Vector3 nearPoint = GraphicsDevice.Viewport.Unproject(nearSource,
                projectionMatrix, viewMatrix, Matrix.Identity);

            Vector3 farPoint = GraphicsDevice.Viewport.Unproject(farSource,
                projectionMatrix, viewMatrix, Matrix.Identity);

            // find the direction vector that goes from the nearPoint to the farPoint
            // and normalize it....
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            // and then create a new ray using nearPoint as the source.
            return new Ray(nearPoint, direction);
        }
    }
}