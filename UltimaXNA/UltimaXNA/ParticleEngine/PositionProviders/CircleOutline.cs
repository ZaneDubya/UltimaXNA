using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace UltimaXNA.ParticleEngine.PositionProviders
{
	public class CircleOutline : PositionProvider
	{
		private float _radius;
		private bool _moveAwayFromRadius;
		private Matrix _rotation;
		private bool _doRotate;

		public CircleOutline(float radius)
			: this(radius, false, 0, 0)
		{
		}

		public CircleOutline(float radius, bool moveAwayFromCenter)
			: this(radius, moveAwayFromCenter, 0, 0)
		{
		}

		public CircleOutline(float radius, bool moveAwayFromCenter, float rotationX, float rotationY)
		{
			_radius = radius;
			_moveAwayFromRadius = moveAwayFromCenter;
			_doRotate = rotationX != 0 || rotationY != 0;
			if (_doRotate)
			{
				_rotation = Matrix.CreateRotationX(rotationX) * Matrix.CreateRotationY(rotationY);
			}
		}

		public CircleOutline(CircleOutline toCopy)
		{
			_radius = toCopy._radius;
			_moveAwayFromRadius = toCopy._moveAwayFromRadius;
			_doRotate = toCopy._doRotate;
			_rotation = toCopy._rotation;
		}

		public override Vector3 GetPosition(Vector3 emitterPosition, Vector3 particleMovementDirection)
		{
			Vector3 ret;
			if (_moveAwayFromRadius)
			{
				// the position on the circle is determined through the velocity vector
				ret = particleMovementDirection;
				ret.Z = 0;
			}
			else
			{
				ret = new Vector3(-1 + 2 * (float)_random.NextDouble(), -1 + 2 * (float)_random.NextDouble(), 0);
			}

			ret.Normalize();
			ret *= _radius;

			if (_doRotate)
			{
				ret = Vector3.Transform(ret, _rotation);
			}

			ret += emitterPosition;

			return ret;

		}

		public override PositionProvider Clone()
		{
			return new CircleOutline(this);
		}

		public override EmitterShape Shape
		{
			get { return EmitterShape.CircleOutline; }
		}
	}
}
