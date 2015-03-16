using Microsoft.Xna.Framework;

namespace InterXLib.View
{
    public class Projections
    {
        private ThirdPersonCamera m_Camera;
        public ThirdPersonCamera Camera
        {
            get { return m_Camera; }
        }

        public void Initialize()
        {
            m_Camera = new ThirdPersonCamera();
        }

        public Matrix ProjectionScreen
        {
            get
            {
                return Matrix.CreateOrthographicOffCenter(0f,
                  Settings.Resolution.X,
                  Settings.Resolution.Y,
                  0f, -20000f, 20000f);
            }
        }

        public Matrix ProjectionWorld
        {
            get
            {
                return m_Camera.ProjectionMatrix;
            }
        }

        public Matrix ViewWorld
        {
            get
            {
                return m_Camera.ViewMatrix;
            }
        }
    }
}
