using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LuaInterface;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.ParticleEngine
{
	public class LuaUtil
	{
		public LuaUtil()
		{
		}

		public Vector3 Vector3(float x, float y, float z)
		{
			return new Vector3(x, y, z);
		}

		public string Test()
		{
			return "utilll";
		}

		public EmitterEmitterParameters EmitterEmitterParameters()
		{
			return ParticleEngine.CreateEmitterEmitterParameters();
		}

		public ParticleEmitterParameters ParticleEmitterParameters()
		{
			return ParticleEngine.CreateParticleEmitterParameters();
		}

		public ParticleEmitter ParticleEmitter(ParticleEmitterParameters param)
		{
			return ParticleEngine.CreateParticleEmitter(param);
		}

		public EmitterEmitter EmitterEmitter(EmitterEmitterParameters param)
		{
			return ParticleEngine.CreateEmitterEmitter(param);
		}

		public ParticleEffect ParticleEffect(float x, float y, float z)
		{
			return ParticleEngine.CreateEffect(new Vector3(x, y, z));
		}

		public ParticleEffect ParticleEffect(Vector3 position)
		{
			return ParticleEngine.CreateEffect(position);
		}

		public void DelayedCall(double millis, LuaFunction func, LuaTable args)
		{
			try
			{
				int delay = (int)millis;
				ParticleEngine.AddDelayedLuaCall(delay, func, args);
			}
			catch (Exception e)
			{
				// TODO: Logging facility
				Console.Error.WriteLine("Error queueing delayed lua call");
				Console.Error.WriteLine(e.Message);
			}
		}

		public Texture2D GetTextureFromFile(string filename)
		{
			return ParticleEngine.GetTexture(filename);
		}

		public void Log(string msg)
		{
			Console.Error.WriteLine(msg);
		}
	}
}
