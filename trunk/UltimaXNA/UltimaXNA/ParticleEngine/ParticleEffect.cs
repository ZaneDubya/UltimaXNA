using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LuaInterface;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.ParticleEngine
{
	public class ParticleEffect
	{
		private List<Emitter> _emitters;

		// if we add or dispose some emitters while updating, we are attempting to change the list we are iterating - thus these buffers
		private List<Emitter> _emitterBuffer;
		private List<Emitter> _disposedBuffer;

		private Vector3 _position;
		public Vector3 Position
		{
			get { return _position; }
			set
			{
				Vector3 positionDiff = value - _position;
				_position = value;

				OnSelfMovement(positionDiff);
			}
		}

		public LuaFunction LuaOnDisposed;

		public ParticleEffect(Vector3 position)
		{
			_emitters = new List<Emitter>();
			_emitterBuffer = new List<Emitter>();
			_disposedBuffer = new List<Emitter>();

			_position = position;
		}

		public void UpdateShaderParams()
		{
			foreach (Emitter emitter in _emitters)
			{
				emitter.UpdateShaderParams();
			}

			foreach (Emitter emitter in _emitterBuffer)
			{
				emitter.UpdateShaderParams();
			}
		}

		public void AddEmitter(Emitter emitter)
		{
			_emitterBuffer.Add(emitter);
			emitter.ParticleEffect = this;
		}

		public void DisposeEmitter(Emitter emitter)
		{
			if (_emitters.Contains(emitter))
			{
				_disposedBuffer.Add(emitter);
			}
		}

		private void MoveEmitterBuffer()
		{
			if (_emitterBuffer.Count > 0)
			{
				_emitters.AddRange(_emitterBuffer);
				_emitterBuffer.Clear();
			}
		}

		private void DisposeEmitters()
		{
			foreach (Emitter emitter in _disposedBuffer)
			{
				emitter.Dispose();
				_emitters.Remove(emitter);
			}

			_disposedBuffer.Clear();

			if (_emitters.Count == 0)
			{
				ParticleEngine.DisposeEffect(this);
			}
		}

		public void Dispose()
		{
			if (LuaOnDisposed != null)
			{
				LuaOnDisposed.Call(this);
			}
		}

		public void Update(GameTime gameTime)
		{
			foreach (Emitter em in _emitters)
			{
				em.Update(gameTime);
			}
			MoveEmitterBuffer();
			DisposeEmitters();
		}

		public void Draw(GameTime gameTime, GraphicsDevice graphics)
		{
			foreach (Emitter em in _emitters)
			{
				em.Draw(gameTime, graphics);
			}
		}

		private void OnSelfMovement(Vector3 positionDiff)
		{
			foreach (Emitter em in _emitters)
			{
				em.OnParentMovement(positionDiff);
			}

			foreach (Emitter em in _emitterBuffer)
			{
				em.OnParentMovement(positionDiff);
			}
		}
	}
}
