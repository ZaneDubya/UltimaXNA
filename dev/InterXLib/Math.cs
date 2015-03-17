using Microsoft.Xna.Framework;
using SystemMath = System.Math;

namespace InterXLib
{
    public static class Math
    {
        public static float PI = (float)SystemMath.PI;
        public static float Cos(float radians)
        {
            return (float)SystemMath.Cos(radians);
        }

        public static float Sin(float radians)
        {
            return (float)SystemMath.Sin(radians);
        }

        public static float CosDeg(float degrees)
        {
            return (float)SystemMath.Cos(degrees / 180f * PI);
        }

        public static float SinDeg(float degrees)
        {
            return (float)SystemMath.Sin(degrees / 180f * PI);
        }

        public static float ExponentialLerp(float low, float high, float t)
        {
            float value = (float)SystemMath.Pow(t, 3) * (high - low) + low;
            return value;
        }

        public static Vector3 V3FromTheta(double theta)
        {
            return new Vector3((float)SystemMath.Cos(theta), -(float)SystemMath.Sin(theta), 0f);
        }

        public static float Abs(float value)
        {
            return SystemMath.Abs(value);
        }

        public static int Abs(int value)
        {
            return SystemMath.Abs(value);
        }

        public static float Atan(float value)
        {
            return (float)SystemMath.Atan(value);
        }

        public static float Tan(float value)
        {
            return (float)SystemMath.Tan(value);
        }

        public static float Round(float value)
        {
            return (float)SystemMath.Round(value);
        }

        public static float Sqrt(float value)
        {
            return (float)SystemMath.Sqrt(value);
        }

        public static int Min(int a, int b)
        {
            return Math.Min(a, b);
        }

        public static float Pow(float x, float y)
        {
            return (float)SystemMath.Pow(x, y);
        }
    }
}
