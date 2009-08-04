using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace UltimaXNA.ParticleEngine
{
	public enum EmitterShape {
		Point,
		CircleOutline,
		Circle,
		Line,
		Cross,
		SphereOutline,
		Sphere,
		BoxOutline,
		Box
	}

	public abstract class PositionProvider
	{
		protected static Random _random = new Random();

		public abstract Vector3 GetPosition(Vector3 emitterPosition, Vector3 particleMovementDirection);

		public abstract PositionProvider Clone();

		public abstract EmitterShape Shape { get; }
	}
}
