using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.ParticleEngine
{
	public class ParticleEmitter : Emitter
	{
		private ParticleEmitterParameters _parameters;
		public ParticleEmitterParameters Parameters
		{
			get { return _parameters; }
		}

		private Effect Shader;

		// Shortcuts for often used Effect parameters
		private EffectParameter _shaderTimeParameter;
		private EffectParameter _shaderEmitterLocationDiffParameter;

		private Particle[] _particles;
		// A vertex buffer holding our particles. This contains the same data as
		// the particles array, but copied across to where the GPU can access it.
		private DynamicVertexBuffer _vertexBuffer;

		private VertexDeclaration _vertexDeclaration;

		// see http://creators.xna.com/en-US/sample/particle3d for a detailed description on these values
		private int _firstActiveParticle;
		private int _firstNewParticle;
		private int _firstFreeParticle;
		private int _firstRetiredParticle;

		// Count how many times Draw has been called. This is used to know
		// when it is safe to retire old particles back into the free list.
		private int drawCounter;

		public ParticleEmitter(EmitterParameters param) : base(param)
		{
		}

		protected override void DoEmit(float elapsedSeconds, float selfAgePercent)
		{
			float emitRate = _parameters.EmitRateStart;
			if (_parameters.ChangeEmitRate)
			{
				emitRate = MathHelper.Lerp(_parameters.EmitRateStart, _parameters.EmitRateEnd, selfAgePercent);
			}
			float newParticlesF = emitRate * elapsedSeconds + _emitCountLeftovers;
			int newParticles = (int)newParticlesF;
			_emitCountLeftovers = newParticlesF - newParticles;
			for (int i = 0; i < newParticles; i++)
			{
				AddParticle();
			}

		}

		public override void SetParameters(EmitterParameters param)
		{
			if (!(param is ParticleEmitterParameters))
			{
				throw new ArgumentException("param is not of Type ParticleEmitterParameters", "param");
			}
			else if (param == null)
			{
				throw new ArgumentNullException("param");
			}
			else
			{
				bool firstTime = _parameters == null;
				base.SetParameters(param);

				_parameters = (ParticleEmitterParameters)param;

				if (firstTime)
				{
					_selfPositionCache = _parameters.SelfStartPositionOffset;
				}

				if (firstTime)
				{
					// we nee our own instance of the effect here to be able to keep our parameters on the effect
					Shader = ParticleEngine.Content.Load<Effect>(_parameters.ShaderName).Clone(ParticleEngine.Graphics);
					_shaderTimeParameter = Shader.Parameters["currentTime"];
					_shaderEmitterLocationDiffParameter = Shader.Parameters["emitterLocationDiff"];

					Shader.CurrentTechnique = Shader.Techniques["T0"];

					_particles = new Particle[param.MaxEmittedCount];

					int size = Particle.SizeInBytes * _particles.Length;
					_vertexBuffer = new DynamicVertexBuffer(ParticleEngine.Graphics, size, BufferUsage.WriteOnly | BufferUsage.Points);
				}

				UpdateShaderParams();
			}
		}


		public override void UpdateShaderParams()
		{
			if (Shader == null)
			{
				return;
			}

			Shader.Parameters["worldViewProjection"].SetValue(_parameters.WorldViewProjection);
			Shader.Parameters["moveWithEmitter"].SetValue(_parameters.EmittedMoveWithEmitter);
			Shader.Parameters["lifetime"].SetValue(_parameters.EmittedLifetime);
			Shader.Parameters["useMovement"].SetValue(_parameters.EmittedDoMove);
			Shader.Parameters["changeMovement"].SetValue(_parameters.EmittedChangeMovementDirection);
			Shader.Parameters["changeVelocity"].SetValue(_parameters.EmittedChangeVelocity);
			Shader.Parameters["useColor"].SetValue(_parameters.EmittedUseColor);
			Shader.Parameters["changeColor"].SetValue(_parameters.EmittedChangeColor);
			Shader.Parameters["useAlpha"].SetValue(_parameters.EmittedUseAlpha);
			Shader.Parameters["changeAlpha"].SetValue(_parameters.EmittedChangeAlpha);
			Shader.Parameters["effectTexture"].SetValue(_parameters.EmittedEffectTexture);
			Shader.Parameters["useTexture"].SetValue(_parameters.EmittedUseTexture);
			Shader.Parameters["changeSize"].SetValue(_parameters.EmittedChangeSize);
		}

		/// <summary>
		/// Helper for checking when active particles have reached the end of
		/// their life. It moves old particles from the active area of the queue
		/// to the retired section.
		/// </summary>
		private void RetireActiveParticles()
		{
			float particleDuration = _parameters.EmittedLifetime;

			while (_firstActiveParticle != _firstNewParticle)
			{
				// Is this particle old enough to retire?
				float particleAge = _selfAge - _particles[_firstActiveParticle].CreationTime;

				if (particleAge < particleDuration)
					break;

				// Remember the time at which we retired this particle.
				_particles[_firstActiveParticle].CreationTime = drawCounter;

				// Move the particle from the active to the retired queue.
				_firstActiveParticle++;

				if (_firstActiveParticle >= _particles.Length)
					_firstActiveParticle = 0;
			}
		}


		/// <summary>
		/// Helper for checking when retired particles have been kept around long
		/// enough that we can be sure the GPU is no longer using them. It moves
		/// old particles from the retired area of the queue to the free section.
		/// </summary>
		private void FreeRetiredParticles()
		{
			while (_firstRetiredParticle != _firstActiveParticle)
			{
				// Has this particle been unused long enough that
				// the GPU is sure to be finished with it?
				int age = drawCounter - (int)_particles[_firstRetiredParticle].CreationTime;

				// The GPU is never supposed to get more than 2 frames behind the CPU.
				// We add 1 to that, just to be safe in case of buggy drivers that
				// might bend the rules and let the GPU get further behind.
				if (age < 3)
					break;

				// Move the particle from the retired to the free queue.
				_firstRetiredParticle++;

				if (_firstRetiredParticle >= _particles.Length)
					_firstRetiredParticle = 0;
			}
		}

		/// <summary>
		/// Helper for uploading new particles from our managed
		/// array to the GPU vertex buffer.
		/// </summary>
		void AddNewParticlesToVertexBuffer()
		{
			int stride = Particle.SizeInBytes;

			try
			{
				if (_firstNewParticle < _firstFreeParticle)
				{

					// If the new particles are all in one consecutive range,
					// we can upload them all in a single call.
					_vertexBuffer.SetData<Particle>(_firstNewParticle * stride, _particles,
										 _firstNewParticle,
										 _firstFreeParticle - _firstNewParticle,
										 stride, SetDataOptions.NoOverwrite);
				}
				else
				{
					// If the new particle range wraps past the end of the queue
					// back to the start, we must split them over two upload calls.
					_vertexBuffer.SetData<Particle>(_firstNewParticle * stride, _particles,
										 _firstNewParticle,
										 _particles.Length - _firstNewParticle,
										 stride, SetDataOptions.NoOverwrite);

					if (_firstFreeParticle > 0)
					{
						_vertexBuffer.SetData<Particle>(0, _particles,
											 0, _firstFreeParticle,
											 stride, SetDataOptions.NoOverwrite);
					}
				}

				// Move the particles we just uploaded from the new to the active queue.
				_firstNewParticle = _firstFreeParticle;

			}
			catch (OutOfMemoryException)
			{
				// TODO: Log error
				return;
			}
		}

		/// <summary>
		/// Adds a new particle to the system.
		/// </summary>
		private void AddParticle()
		{
			// Figure out where in the circular queue to allocate the new particle.
			int nextFreeParticle = _firstFreeParticle + 1;

			if (nextFreeParticle >= _particles.Length)
				nextFreeParticle = 0;

			// If there are no free particles, we just have to give up.
			if (nextFreeParticle == _firstRetiredParticle)
				return;

			// Fill in the particle vertex structure.
			_particles[_firstFreeParticle].ColorEnd = _parameters.EmittedColorEnd.RGBA;
			_particles[_firstFreeParticle].ColorStart = _parameters.EmittedColorStart.RGBA;
			_particles[_firstFreeParticle].CreationTime = _selfAge;
			_particles[_firstFreeParticle].SizeStart = _parameters.EmittedSizeStart;
			_particles[_firstFreeParticle].SizeEnd = _parameters.EmittedSizeEnd;
			_particles[_firstFreeParticle].VelocityStart = _parameters.EmittedVelocityStart;
			_particles[_firstFreeParticle].VelocityEnd = _parameters.EmittedVelocityEnd;
			_particles[_firstFreeParticle].MovementDirectionStart = _parameters.EmittedMovementDirectionStart.XYZ;
			_particles[_firstFreeParticle].MovementDirectionEnd = _parameters.EmittedMovementDirectionEnd.XYZ;

			_particles[_firstFreeParticle].Position = _positionProvider.GetPosition(_selfPositionCache, _particles[_firstFreeParticle].MovementDirectionStart);

			_firstFreeParticle = nextFreeParticle;
		}

		protected override void OnBeforeEmit(float elapsedSeconds, float selfAgePercent)
		{
			RetireActiveParticles();
			FreeRetiredParticles();

			// lifetime is over. we are not emitting any more particles, but wait for the emitted to die, too
			if (selfAgePercent > 1.0)
			{
				State = this._firstActiveParticle == _firstFreeParticle ? EmitterState.Finished : EmitterState.WaitingForEmitted;
			}
		}

		protected override void OnAfterEmit(float elapsedSeconds, float selfAgePercent)
		{
			// do nothing
		}

		protected override void OnBeforeSelfMove(float elapsedSeconds, float selfAgePercent)
		{
			// do nothing
		}

		protected override void OnAfterSelfMove(float elapsedSeconds, float selfAgePercent, Vector3 positionDiff)
		{
			// Vector3 totalDiff = SelfStartPosition - _selfPosition;
			_shaderEmitterLocationDiffParameter.SetValue(_totalMovement);
		}

		public override void Draw(GameTime gameTime, GraphicsDevice device)
		{
			if (Shader == null)
			{
				return;
			}
			if (_vertexDeclaration == null || _vertexDeclaration.GraphicsDevice != device)
			{
				_vertexDeclaration = new VertexDeclaration(device, Particle.VertexElements);
			}

			// Restore the vertex buffer contents if the graphics device was lost.
			if (_vertexBuffer.IsContentLost)
			{
				_vertexBuffer.SetData<Particle>(_particles);
			}

			// If there are any particles waiting in the newly added queue,
			// we'd better upload them to the GPU ready for drawing.
			if (_firstNewParticle != _firstFreeParticle)
			{
				AddNewParticlesToVertexBuffer();
			}

			// If there are any active particles, draw them now!
			if (_firstActiveParticle != _firstFreeParticle)
			{
				// Set an effect parameter describing the current time. All the vertex
				// shader particle animation is keyed off this value.
				_shaderTimeParameter.SetValue(_selfAge);

				// Set the particle vertex buffer and vertex declaration.
				device.Vertices[0].SetSource(_vertexBuffer, 0,
											 Particle.SizeInBytes);

				device.VertexDeclaration = _vertexDeclaration;

				// Activate the particle effect.
				Shader.Begin();

				Shader.CurrentTechnique.Passes["P0"].Begin();

				if (_firstActiveParticle < _firstFreeParticle)
				{
					// If the active particles are all in one consecutive range,
					// we can draw them all in a single call.
					device.DrawPrimitives(PrimitiveType.PointList,
										  _firstActiveParticle,
										  _firstFreeParticle - _firstActiveParticle);
				}
				else
				{
					// If the active particle range wraps past the end of the queue
					// back to the start, we must split them over two draw calls.
					device.DrawPrimitives(PrimitiveType.PointList,
										  _firstActiveParticle,
										  _particles.Length - _firstActiveParticle);

					if (_firstFreeParticle > 0)
					{
						device.DrawPrimitives(PrimitiveType.PointList,
											  0,
											  _firstFreeParticle);
					}
				}

				Shader.CurrentTechnique.Passes["P0"].End();

				Shader.End();
			}

			drawCounter++;
		}
	}
}