using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.ParticleEngine
{
	public struct Particle
	{
		public Vector3 Position;
		public float CreationTime;

		public Vector3 MovementDirectionStart;
		public Vector3 MovementDirectionEnd;
		public float VelocityStart;
		public float VelocityEnd;
		public Vector4 ColorStart;
		public Vector4 ColorEnd;
		public float SizeStart;
		public float SizeEnd;

		public static int SizeInBytes = sizeof(float) * 22;

		public static readonly VertexElement[] VertexElements =
		{
			new VertexElement(0, 0, VertexElementFormat.Vector3,
				VertexElementMethod.Default,
				VertexElementUsage.Position, 0),
			new VertexElement(0, sizeof(float) * 3, VertexElementFormat.Single,
				VertexElementMethod.Default,
				VertexElementUsage.PointSize, 0),
			new VertexElement(0, sizeof(float) * 4, VertexElementFormat.Vector3,
				VertexElementMethod.Default,
				VertexElementUsage.Normal, 0),
				new VertexElement(0, sizeof(float) * 7, VertexElementFormat.Vector3,
				VertexElementMethod.Default,
				VertexElementUsage.Normal, 1),
			new VertexElement(0, sizeof(float) * 10, VertexElementFormat.Single,
				VertexElementMethod.Default,
				VertexElementUsage.PointSize, 1),
			new VertexElement(0, sizeof(float) * 11, VertexElementFormat.Single,
				VertexElementMethod.Default,
				VertexElementUsage.PointSize, 2),
			new VertexElement(0, sizeof(float) * 12, VertexElementFormat.Vector4,
				VertexElementMethod.Default,
				VertexElementUsage.Color, 0),
			new VertexElement(0, sizeof(float) * 16, VertexElementFormat.Vector4,
				VertexElementMethod.Default,
				VertexElementUsage.Color, 1),
			new VertexElement(0, sizeof(float) * 20, VertexElementFormat.Single,
				VertexElementMethod.Default,
				VertexElementUsage.PointSize, 3),
			new VertexElement(0, sizeof(float) * 21, VertexElementFormat.Single,
				VertexElementMethod.Default,
				VertexElementUsage.PointSize, 4)
		};
	}
}
