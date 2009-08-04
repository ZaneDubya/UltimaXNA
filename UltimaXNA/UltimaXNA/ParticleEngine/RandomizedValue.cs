using System;

using Microsoft.Xna.Framework;

namespace UltimaXNA.ParticleEngine
{
	public struct RandomizedValue
	{
		public static RandomizedValue Invalid;

		static RandomizedValue() {
			Invalid.MinValue = null;
			Invalid.MaxValue = null;
		}

		private static Random _random = new Random();

		public float? MinValue;
		public float? MaxValue;

		public bool IsValid
		{
			get { return MinValue != null; }
		}

		public float Random
		{
			get
			{
				if (!IsValid)
				{
					return 0f;
				}

				if (MaxValue == null || MinValue == MaxValue)
				{
					return MinValue.Value;
				}

				return (MinValue.Value + (MaxValue.Value - MinValue.Value) * (float)_random.NextDouble());
			}
		}

		public RandomizedValue(float minVal, float maxVal) {
			MinValue = minVal;
			MaxValue = maxVal;
		}

		public RandomizedValue(float value) {
			MinValue = value;
			MaxValue = null;
		}

		public static implicit operator float(RandomizedValue val) {
			return val.Random;
		}

		public static implicit operator RandomizedValue(float val)
		{
			return new RandomizedValue(val);
		}

		public static bool operator ==(RandomizedValue first, RandomizedValue second)
		{
			return first.MinValue == second.MinValue &&
				first.MaxValue == second.MaxValue;
		}

		public static bool operator !=(RandomizedValue first, RandomizedValue second) {
			return first.MinValue != second.MinValue ||
				first.MaxValue != second.MaxValue;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is RandomizedValue)
			{
				return this == (RandomizedValue)obj;
			} else {
				return base.Equals(obj);
			}
		}
	}

	public struct RandomizedColor
	{
		public static RandomizedColor Invalid;

		static RandomizedColor()
		{
			Invalid.R = RandomizedValue.Invalid;
			Invalid.G = RandomizedValue.Invalid;
			Invalid.B = RandomizedValue.Invalid;
			Invalid.A = RandomizedValue.Invalid;
		}

		public RandomizedValue R;
		public RandomizedValue G;
		public RandomizedValue B;
		public RandomizedValue A;

		public Vector3 RGB
		{
			get { return new Vector3(R, G, B); }
		}

		public Vector4 RGBA
		{
			get { return new Vector4(R, G, B, A); }
		}

		public bool IsValid
		{
			get { return R.IsValid && G.IsValid && B.IsValid; }
		}

		public bool IsAlphaValid
		{
			get { return A.IsValid; }
		}

		public RandomizedColor(float r, float g, float b, float a)
		{
			R = new RandomizedValue(r);
			G = new RandomizedValue(g);
			B = new RandomizedValue(b);
			A = new RandomizedValue(a);
		}

		public RandomizedColor(float r, float g, float b)
		{
			R = new RandomizedValue(r);
			G = new RandomizedValue(g);
			B = new RandomizedValue(b);
			A = new RandomizedValue(1);
		}

		public RandomizedColor(float rMin, float gMin, float bMin, float aMin, float rMax, float gMax, float bMax, float aMax)
		{
			R = new RandomizedValue(rMin, rMax);
			G = new RandomizedValue(gMin, gMax);
			B = new RandomizedValue(bMin, bMax);
			A = new RandomizedValue(aMin, aMax);
		}

		public RandomizedColor(float rMin, float gMin, float bMin, float rMax, float gMax, float bMax)
		{
			R = new RandomizedValue(rMin, rMax);
			G = new RandomizedValue(gMin, gMax);
			B = new RandomizedValue(bMin, bMax);
			A = new RandomizedValue(1);
		}

		public static bool operator ==(RandomizedColor first, RandomizedColor second)
		{
			return first.R == second.R &&
				first.G == second.G &&
				first.B == second.B &&
				first.A == second.A;
		}

		public static bool operator !=(RandomizedColor first, RandomizedColor second)
		{
			return first.R != second.R ||
				first.G != second.G ||
				first.B != second.B ||
				first.A != second.A;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is RandomizedColor)
			{
				return this == (RandomizedColor)obj;
			}
			else
			{
				return base.Equals(obj);
			}
		}
	}

	public struct RandomizedVector3
	{
		public static RandomizedVector3 Invalid;
		static RandomizedVector3()
		{
			Invalid.X = RandomizedValue.Invalid;
			Invalid.Y = RandomizedValue.Invalid;
			Invalid.Z = RandomizedValue.Invalid;
		}

		// direction of the vector
		public RandomizedValue X;
		public RandomizedValue Y;
		public RandomizedValue Z;

		public Vector3 DirectionMin
		{
			set
			{
				X.MinValue = value.X;
				Y.MinValue = value.Y;
				Z.MinValue = value.Z;
			}
		}

		public Vector3 DirectionMax
		{
			set
			{
				X.MaxValue = value.X;
				Y.MaxValue = value.Y;
				Z.MaxValue = value.Z;
			}
		}

		public bool IsValid
		{
			get { return X.IsValid && Y.IsValid && Z.IsValid; }
		}


		public Vector3 XYZ
		{
			get {
				// apply random direction
				Vector3 ret = new Vector3(X, Y, Z);
				ret.Normalize();

				return ret; 
			}
		}

		public RandomizedVector3(Vector3 directionMin, Vector3 directionMax)
		{
			X = new RandomizedValue(directionMin.X, directionMax.X);
			Y = new RandomizedValue(directionMin.Y, directionMax.Y);
			Z = new RandomizedValue(directionMin.Z, directionMax.Z);
		}

		public RandomizedVector3(Vector3 direction)
		{
			X = new RandomizedValue(direction.X);
			Y = new RandomizedValue(direction.Y);
			Z = new RandomizedValue(direction.Z);
		}

		public static bool operator ==(RandomizedVector3 first, RandomizedVector3 second)
		{
			return first.X == second.X &&
				first.Y == second.Y &&
				first.Z == second.Z;
		}

		public static bool operator !=(RandomizedVector3 first, RandomizedVector3 second)
		{
			return first.X != second.X ||
				first.Y != second.Y ||
				first.Z != second.Z;
		}

		public static implicit operator RandomizedVector3(Vector3 vec)
		{
			return new RandomizedVector3(vec);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is RandomizedVector3)
			{
				return this == (RandomizedVector3)obj;
			}
			else
			{
				return base.Equals(obj);
			}
		}
	}
}
