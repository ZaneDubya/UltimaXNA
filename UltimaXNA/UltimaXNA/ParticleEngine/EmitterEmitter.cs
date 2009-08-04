using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LuaInterface;
using Microsoft.Xna.Framework;

namespace UltimaXNA.ParticleEngine
{
	public class EmitterEmitter : Emitter
	{
		private EmitterEmitterParameters _parameters;
		public EmitterEmitterParameters Parameters
		{
			get { return _parameters; }
		}

		private List<Emitter> _emittedEmitters;

		public EmitterEmitter(EmitterParameters param)
			: base(param)
		{
			_emittedEmitters = new List<Emitter>();
		}

		public override void SetParameters(EmitterParameters param)
		{
			if (!(param is EmitterEmitterParameters))
			{
				throw new ArgumentException("param is not of Type EmitterEmitterParameters", "param");
			}
			else if (param == null)
			{
				throw new ArgumentNullException("param");
			}
			else
			{
				_parameters = (EmitterEmitterParameters)param;
				base.SetParameters(param);
			}
		}

		protected override void DoEmit(float elapsedSeconds, float selfAgePercent)
		{
			float emitRate = _parameters.EmitRateStart;
			if (_parameters.ChangeEmitRate)
			{
				emitRate = MathHelper.Lerp(_parameters.EmitRateStart, _parameters.EmitRateEnd, selfAgePercent);
			}
			float newParticlesF = emitRate * elapsedSeconds + _emitCountLeftovers;

			// we cannot emit more emitters than we are allowed to, and not less than zero
			newParticlesF = Math.Min(_parameters.MaxEmittedCount - _emittedEmitters.Count, newParticlesF);
			newParticlesF = Math.Max(newParticlesF, 0);

			int newParticles = (int)newParticlesF;
			_emitCountLeftovers = newParticlesF - newParticles;

			for (int i = 0; i < newParticles; i++)
			{
				AddEmitter();
			}
		}

		private void AddEmitter()
		{
			EmitterParameters param;
			switch (_parameters.EmittedType)
			{
				case EmittedType.Emitter:
					param = new EmitterEmitterParameters((EmitterEmitterParameters)_parameters.EmittedParameters);
					break;
				default:
				case EmittedType.Particle:
					param = new ParticleEmitterParameters((ParticleEmitterParameters)_parameters.EmittedParameters);
					break;
			}
			param.SelfStartPositionOffset = this._selfPositionCache;

			if (_parameters.LuaOnBeforeEmitterEmitted != null)
			{
				_parameters.LuaOnBeforeEmitterEmitted.Call(_particleEffect, this, param);
			}

			Emitter newEmitter = ParticleEngine.CreateEmitter(param);
			
			_particleEffect.AddEmitter(newEmitter);
			_emittedEmitters.Add(newEmitter);

			newEmitter.ParentEmitter = this;

			if (_parameters.LuaOnAfterEmitterEmitted != null)
			{
				_parameters.LuaOnAfterEmitterEmitted.Call(_particleEffect, this, newEmitter);
			}
		}

		protected override void OnAfterEmit(float elapsedSeconds, float selfAgePercent)
		{
			// do nothing
		}

		protected override void OnBeforeEmit(float elapsedSeconds, float selfAgePercent)
		{
			// lifetime is over. we are not emitting any more emitters
			if (selfAgePercent > 1.0)
			{
				State = EmitterState.Finished;
			}
		}

		protected override void OnAfterSelfMove(float elapsedSeconds, float selfAgePercent, Vector3 positionDiff)
		{
			// if emitted move with emitter => move them
			if (_parameters.EmittedMoveWithEmitter)
			{
				foreach (Emitter em in _emittedEmitters)
				{
					em.OnParentMovement(positionDiff);
				}
			}
		}

		protected override void OnBeforeSelfMove(float elapsedSeconds, float selfAgePercent)
		{
			// to nothing
		}

		public override void Draw(GameTime gameTime, Microsoft.Xna.Framework.Graphics.GraphicsDevice device)
		{
			// do nothing
		}

		public override void UpdateShaderParams()
		{
			// do nothing
		}

		public void OnSubEmitterFinished(Emitter emitter)
		{
			_emittedEmitters.Remove(emitter);
		}
	}
}
