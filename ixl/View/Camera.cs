//-----------------------------------------------------------------------------
// Copyright (c) 2008-2011 dhpoware. All Rights Reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------

using System;
using Microsoft.Xna.Framework;

namespace InterXLib.View
{
    /// <summary>
    /// A quaternion based third person camera class. This camera model
    /// incorporates a spring system to smooth out camera movement. It is
    /// enabled by default but can be disabled.
    /// 
    /// <para>Call LookAt(eye, target, up) to establish the camera's initial
    /// settings. This defines the camera's initial position in relation to the
    /// target that the camera will be looking. When the target moves call the
    /// camera's LookAt(target) method to set the target's new world position.
    /// When the target rotates call the camera's Rotate() method.</para>
    /// 
    /// <para>The camera's Update() method must be called once per frame. The
    /// Update() method performs any pending camera rotations and recalculates
    /// the camera's view matrix.</para>
    /// </summary>
    public class ThirdPersonCamera
    {
        private const float DEFAULT_SPRING_CONSTANT = 16.0f;
        private const float DEFAULT_DAMPING_CONSTANT = 8.0f;
        private const float DEFAULT_FOVX = 80.0f;
        private const float DEFAULT_ZFAR = 1000.0f;
        private const float DEFAULT_ZNEAR = 1.0f;

        private float m_PitchLimiter = 0.0f;

        private bool enableSpringSystem;
        private float springConstant;
        private float dampingConstant;
        private float offsetDistance;
        private float headingDegrees;
        private float pitchDegrees;
        private float fovx;
        private float znear;
        private float zfar;
        private Vector3 eye;
        private Vector3 target;
        private Vector3 targetYAxis;
        private Vector3 xAxis;
        private Vector3 yAxis;
        private Vector3 zAxis;
        private Vector3 viewDir;
        private Vector3 velocity;
        private Matrix viewMatrix;
        private Matrix projMatrix;
        private Quaternion orientation;

        #region Public Methods

        /// <summary>
        /// Constructs a new ThirdPersonCamera object. The camera's spring
        /// system is enabled by default.
        /// </summary>
        public ThirdPersonCamera()
        {
            enableSpringSystem = true;
            springConstant = DEFAULT_SPRING_CONSTANT;
            dampingConstant = DEFAULT_DAMPING_CONSTANT;

            offsetDistance = 0.0f;
            headingDegrees = 0.0f;
            pitchDegrees = 0.0f;

            fovx = DEFAULT_FOVX;
            znear = DEFAULT_ZNEAR;
            zfar = DEFAULT_ZFAR;

            eye = Vector3.Zero;
            target = Vector3.Zero;
            targetYAxis = Vector3.Up;

            xAxis = Vector3.UnitX;
            yAxis = Vector3.UnitY;
            zAxis = Vector3.UnitZ;
            viewDir = Vector3.Forward;

            viewMatrix = Matrix.Identity;
            projMatrix = Matrix.Identity;
            orientation = Quaternion.Identity;
        }

        /// <summary>
        /// Builds a look at style viewing matrix using the camera's current
        /// world position, and its current local y axis.
        /// </summary>
        /// <param name="target">The target position to look at.</param>
        public void LookAt(Vector3 target)
        {
            this.target = target;
        }

        /// <summary>
        /// Builds a look at style viewing matrix.
        /// </summary>
        /// <param name="eye">The camera position.</param>
        /// <param name="target">The target position to look at.</param>
        /// <param name="up">The up direction.</param>
        public void LookAt(Vector3 eye, Vector3 target, Vector3 up)
        {
            this.eye = eye;
            this.target = target;

            zAxis = eye - target;
            zAxis.Normalize();

            Vector3.Negate(ref zAxis, out viewDir);

            Vector3.Cross(ref up, ref zAxis, out xAxis);
            xAxis.Normalize();

            Vector3.Cross(ref zAxis, ref xAxis, out yAxis);
            yAxis.Normalize();
            xAxis.Normalize();

            viewMatrix.M11 = xAxis.X;
            viewMatrix.M21 = xAxis.Y;
            viewMatrix.M31 = xAxis.Z;
            Vector3.Dot(ref xAxis, ref eye, out viewMatrix.M41);
            viewMatrix.M41 = -viewMatrix.M41;

            viewMatrix.M12 = yAxis.X;
            viewMatrix.M22 = yAxis.Y;
            viewMatrix.M32 = yAxis.Z;
            Vector3.Dot(ref yAxis, ref eye, out viewMatrix.M42);
            viewMatrix.M42 = -viewMatrix.M42;

            viewMatrix.M13 = zAxis.X;
            viewMatrix.M23 = zAxis.Y;
            viewMatrix.M33 = zAxis.Z;
            Vector3.Dot(ref zAxis, ref eye, out viewMatrix.M43);
            viewMatrix.M43 = -viewMatrix.M43;

            viewMatrix.M14 = 0.0f;
            viewMatrix.M24 = 0.0f;
            viewMatrix.M34 = 0.0f;
            viewMatrix.M44 = 1.0f;

            targetYAxis = up;

            Quaternion.CreateFromRotationMatrix(ref viewMatrix, out orientation);

            Vector3 offset = target - eye;

            offsetDistance = offset.Length();
        }

        /// <summary>
        /// Builds a perspective projection matrix based on a horizontal field
        /// of view.
        /// </summary>
        /// <param name="fovx">Horizontal field of view in degrees.</param>
        /// <param name="aspect">The viewport's aspect ratio.</param>
        /// <param name="znear">The distance to the near clip plane.</param>
        /// <param name="zfar">The distance to the far clip plane.</param>
        public void Perspective(float fovx, float aspect, float znear, float zfar)
        {
            this.fovx = fovx;
            this.znear = znear;
            this.zfar = zfar;

            float aspectInv = 1.0f / aspect;
            float e = 1.0f / Math.Tan(MathHelper.ToRadians(fovx) / 2.0f);
            float fovy = 2.0f * Math.Atan(aspectInv / e);
            float xScale = 1.0f / Math.Tan(0.5f * fovy);
            float yScale = xScale / aspectInv;

            projMatrix.M11 = xScale;
            projMatrix.M12 = 0.0f;
            projMatrix.M13 = 0.0f;
            projMatrix.M14 = 0.0f;

            projMatrix.M21 = 0.0f;
            projMatrix.M22 = yScale;
            projMatrix.M23 = 0.0f;
            projMatrix.M24 = 0.0f;

            projMatrix.M31 = 0.0f;
            projMatrix.M32 = 0.0f;
            projMatrix.M33 = (zfar + znear) / (znear - zfar);
            projMatrix.M34 = -1.0f;

            projMatrix.M41 = 0.0f;
            projMatrix.M42 = 0.0f;
            projMatrix.M43 = (2.0f * zfar * znear) / (znear - zfar);
            projMatrix.M44 = 0.0f;
        }

        /// <summary>
        /// Rotates the camera. Positive angles specify counter clockwise
        /// rotations when looking down the axis of rotation towards the
        /// origin.
        /// </summary>
        /// <param name="headingDegrees">Y axis rotation in degrees.</param>
        /// <param name="pitchDegrees">X axis rotation in degrees.</param>
        public void Rotate(float headingDegrees, float pitchDegrees)
        {
            this.headingDegrees = -headingDegrees;
            this.pitchDegrees = -pitchDegrees;
        }

        /// <summary>
        /// This method must be called once every frame to update the internal
        /// state of the ThirdPersonCamera object.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public void Update(GameTime gameTime)
        {
            float frameMSSec = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (enableSpringSystem)
            {
                UpdateOrientation(frameMSSec);
                UpdateViewMatrix(frameMSSec);
            }
            else
            {
                UpdateOrientation(1f);
                UpdateViewMatrix();
            }
        }

        public void Update()
        {
            UpdateOrientation(1f);
            UpdateViewMatrix();
        }

        #endregion

        #region Private Methods

        private void UpdateOrientation(float frameMSSec)
        {
            headingDegrees *= frameMSSec;
            pitchDegrees *= frameMSSec;

            if (pitchDegrees + m_PitchLimiter > 90.0f)
                pitchDegrees = 90f - m_PitchLimiter;
            if (pitchDegrees + m_PitchLimiter < -90.0f)
                pitchDegrees = -(90f + m_PitchLimiter);
            m_PitchLimiter = m_PitchLimiter + pitchDegrees;

            float heading = MathHelper.ToRadians(headingDegrees);
            float pitch = MathHelper.ToRadians(pitchDegrees);
            Quaternion rotation = Quaternion.Identity;

            if (heading != 0.0f)
            {
                Quaternion.CreateFromAxisAngle(ref targetYAxis, heading, out rotation);
                Quaternion.Concatenate(ref rotation, ref orientation, out orientation);
            }

            if (pitch != 0.0f)
            {
                Vector3 worldXAxis = Vector3.UnitX;
                Quaternion.CreateFromAxisAngle(ref worldXAxis, pitch, out rotation);
                Quaternion.Concatenate(ref orientation, ref rotation, out orientation);
            }

            headingDegrees = 0f;
            pitchDegrees = 0f;

        }

        private void UpdateViewMatrix()
        {
            Matrix.CreateFromQuaternion(ref orientation, out viewMatrix);

            xAxis.X = viewMatrix.M11;
            xAxis.Y = viewMatrix.M21;
            xAxis.Z = viewMatrix.M31;

            yAxis.X = viewMatrix.M12;
            yAxis.Y = viewMatrix.M22;
            yAxis.Z = viewMatrix.M32;

            zAxis.X = viewMatrix.M13;
            zAxis.Y = viewMatrix.M23;
            zAxis.Z = viewMatrix.M33;

            eye = target + zAxis * offsetDistance;

            viewMatrix.M41 = -Vector3.Dot(xAxis, eye);
            viewMatrix.M42 = -Vector3.Dot(yAxis, eye);
            viewMatrix.M43 = -Vector3.Dot(zAxis, eye);

            Vector3.Negate(ref zAxis, out viewDir);
        }

        private void UpdateViewMatrix(float frameMSSec)
        {
            Matrix.CreateFromQuaternion(ref orientation, out viewMatrix);

            xAxis.X = viewMatrix.M11;
            xAxis.Y = viewMatrix.M21;
            xAxis.Z = viewMatrix.M31;

            yAxis.X = viewMatrix.M12;
            yAxis.Y = viewMatrix.M22;
            yAxis.Z = viewMatrix.M32;

            zAxis.X = viewMatrix.M13;
            zAxis.Y = viewMatrix.M23;
            zAxis.Z = viewMatrix.M33;

            // Calculate the new camera position. The 'idealPosition' is where
            // the camera should be positioned. The camera should be positioned
            // directly behind the target at the required offset distance. What
            // we're doing here is rather than have the camera immediately snap
            // to the 'idealPosition' we slowly move the camera towards the
            // 'idealPosition' using a spring system.
            //
            // References:
            //  Stone, Jonathan, "Third-Person Camera Navigation," Game Programming
            //    Gems 4, Andrew Kirmse, Editor, Charles River Media, Inc., 2004.

            Vector3 idealPosition = target + zAxis * offsetDistance;
            Vector3 displacement = eye - idealPosition;
            Vector3 springAcceleration = (-springConstant * displacement) -
                                         (dampingConstant * velocity);

            velocity += springAcceleration * frameMSSec;
            eye += velocity * frameMSSec;

            // The view matrix is always relative to the camera's current position.
            // Since a spring system is being used here 'eye' will be relative to
            // 'idealPosition'. When the camera is no longer being moved 'eye' will
            // become the same as 'idealPosition'. The local x, y, and z axes that
            // were extracted from the camera's orientation 'orienation' is correct
            // for the 'idealPosition' only. We need to recompute these axes so
            // that they're relative to 'eye'. Once that's done we can use those
            // axes to reconstruct the view matrix.

            zAxis = eye - target;
            zAxis.Normalize();

            Vector3.Negate(ref zAxis, out viewDir);

            Vector3.Cross(ref targetYAxis, ref zAxis, out xAxis);
            xAxis.Normalize();

            Vector3.Cross(ref zAxis, ref xAxis, out yAxis);
            yAxis.Normalize();

            viewMatrix.M11 = xAxis.X;
            viewMatrix.M21 = xAxis.Y;
            viewMatrix.M31 = xAxis.Z;
            Vector3.Dot(ref xAxis, ref eye, out viewMatrix.M41);
            viewMatrix.M41 = -viewMatrix.M41;

            viewMatrix.M12 = yAxis.X;
            viewMatrix.M22 = yAxis.Y;
            viewMatrix.M32 = yAxis.Z;
            Vector3.Dot(ref yAxis, ref eye, out viewMatrix.M42);
            viewMatrix.M42 = -viewMatrix.M42;

            viewMatrix.M13 = zAxis.X;
            viewMatrix.M23 = zAxis.Y;
            viewMatrix.M33 = zAxis.Z;
            Vector3.Dot(ref zAxis, ref eye, out viewMatrix.M43);
            viewMatrix.M43 = -viewMatrix.M43;

            viewMatrix.M14 = 0.0f;
            viewMatrix.M24 = 0.0f;
            viewMatrix.M34 = 0.0f;
            viewMatrix.M44 = 1.0f;
        }

        #endregion

        #region Properties

        public bool EnableSpringSystem
        {
            get { return enableSpringSystem; }
            set { enableSpringSystem = value; }
        }

        public float SpringConstant
        {
            get { return springConstant; }

            // We're using a critically damped spring system where the
            // damping ratio is equal to one.
            //
            // i.e.,
            // Damping Ratio = Damping Constant / (2 * Sqrt(Spring Constant))
            //             1 = Damping Constant / (2 * Sqrt(Spring Constant))

            set
            {
                springConstant = value;
                dampingConstant = 2.0f * (float)Math.Sqrt(springConstant);
            }
        }

        public float DampingConstant
        {
            get { return dampingConstant; }
        }

        public float OffsetDistance
        {
            get { return offsetDistance; }
            set { offsetDistance = value; }
        }

        public Vector3 Position
        {
            get { return eye; }
        }

        public Vector3 Target
        {
            get { return target; }
        }

        public Vector3 TargetYAxis
        {
            get { return targetYAxis; }
        }

        public Vector3 XAxis
        {
            get { return xAxis; }
        }

        public Vector3 YAxis
        {
            get { return yAxis; }
        }

        public Vector3 ZAxis
        {
            get { return zAxis; }
        }

        public Vector3 ViewDirection
        {
            get { return viewDir; }
        }

        public Vector3 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public Matrix ViewMatrix
        {
            get { return viewMatrix; }
        }

        public Matrix ViewProjectionMatrix
        {
            get { return viewMatrix * projMatrix; }
        }

        public Matrix ProjectionMatrix
        {
            get { return projMatrix; }
        }

        public Quaternion Orientation
        {
            get { return orientation; }
        }

        #endregion
    }
}