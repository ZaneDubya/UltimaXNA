using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace UltimaXNA.ParticleEngine.PositionProviders
{
	public class Point : PositionProvider
	{
		public Point()
		{
			// we have no parameters
		}

		public Point(Point toCopy)
		{
		}

		public override Vector3 GetPosition(Vector3 emitterPosition, Vector3 particleMovementDirection)
		{
			return emitterPosition;
		}

		public override PositionProvider Clone()
		{
			return new Point(this);
		}

		public override EmitterShape Shape
		{
			get { return EmitterShape.Point; }
		}
	}
}
