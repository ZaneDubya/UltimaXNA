using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using LuaInterface;

namespace UltimaXNA.ParticleEngine
{
	public enum EmitterState
	{
		Creating,
		Emitting,
		WaitingForEmitted,
		Finished
	}

	public abstract class Emitter
	{
		protected ParticleEffect _particleEffect;
		public ParticleEffect ParticleEffect
		{
			get { return _particleEffect; }
			set {
				if (_particleEffect == null) {
					_particleEffect = value;
					State = EmitterState.Emitting;

					SelfStartPosition = _particleEffect.Position + _parameters.SelfStartPositionOffset;
				}
			}
		}

		protected EmitterEmitter _parentEmitter;
		public EmitterEmitter ParentEmitter
		{
			get { return _parentEmitter; }
			set { _parentEmitter = value; }
		}

		private EmitterParameters _parameters;

		protected float _selfAge;

		protected Vector3 SelfStartPosition;
		protected Vector3 _totalMovement;
		protected Vector3 _selfPositionCache;

		// it might be, that our emitter only emits very few packets. if we round up all the time we
		// might be emitting to fast. thus, this variable is used to track these objects, that
		// should only be partly emitted yet
		protected float _emitCountLeftovers;

		private EmitterState _state;
		public EmitterState State
		{
			get { return _state; }
			protected set
			{
				if (_state == value)
				{
					return;
				}

				_state = value;

				if (_state == EmitterState.Finished)
				{
					if (_particleEffect != null)
					{
						_particleEffect.DisposeEmitter(this);
					}

					if (_parentEmitter != null)
					{
						_parentEmitter.OnSubEmitterFinished(this);
					}
				}

				// lua event
				if (_parameters.LuaOnStateChanged != null)
				{
					_parameters.LuaOnStateChanged.Call(this, _state);
				}
			}
		}

		// movement parameters
		private Vector3 _selfMovementDirectionStart;
		private Vector3 _selfMovementDirectionEnd;
		private float _selfVelocityStart;
		private float _selfVelocityEnd;
		private float _selfLifetime;

		protected PositionProvider _positionProvider;

		protected static Random Random = new Random();

		public Emitter(EmitterParameters param)
		{
			_state = EmitterState.Creating;

			SetParameters(param);

			_emitCountLeftovers = 0.999f; // we do not want to wait for our first emitted object... :)
		}

		public void Dispose()
		{
			// lua event
			if (_parameters.LuaOnDisposed != null)
			{
				_parameters.LuaOnDisposed.Call(this);
			}
		}

		// sets the parameters variables in our shader program
		public abstract void UpdateShaderParams();

		public abstract void Draw(GameTime gameTime, GraphicsDevice device);

		protected abstract void DoEmit(float elapsedSeconds, float selfAgePercent);
		protected abstract void OnBeforeEmit(float elapsedSeconds, float selfAgePercent);
		protected abstract void OnAfterEmit(float elapsedSeconds, float selfAgePercent);

		protected abstract void OnBeforeSelfMove(float elapsedSeconds, float selfAgePercent);
		protected abstract void OnAfterSelfMove(float elapsedSeconds, float selfAgePercent, Vector3 positionDiff);

		public virtual void SetParameters(EmitterParameters param)
		{
			bool isFirst = _parameters == null;
			_parameters = param;

			_selfLifetime = param.SelfLifetime;

			_selfMovementDirectionStart = param.SelfMovementDirectionStart.XYZ;

			if (param.SelfChangeMovementDirection)
			{
				_selfMovementDirectionEnd = _parameters.SelfMovementDirectionEnd.XYZ;
			}
			else
			{
				_selfMovementDirectionEnd = _selfMovementDirectionStart;
			}

			_selfVelocityStart = param.SelfVelocityStart;
			if (param.SelfChangeVelocity)
			{
				_selfVelocityEnd = param.SelfVelocityEnd;
			}
			else
			{
				_selfVelocityEnd = _selfVelocityStart;
			}

			_positionProvider = _parameters.PositionProvider;
		}

		public void Update(GameTime gameTime)
		{
			float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
			_selfAge += elapsedSeconds;

			float selfAgePercent = _selfAge / _selfLifetime;

			_selfPositionCache = SelfStartPosition + _totalMovement;

			OnBeforeEmit(elapsedSeconds, selfAgePercent);

			if (_state == EmitterState.Emitting)
			{
				DoEmit(elapsedSeconds, selfAgePercent);

				// emitter movement
				OnBeforeSelfMove(elapsedSeconds, selfAgePercent);
				Vector3 positionDiff = DoSelfMovement(elapsedSeconds, selfAgePercent);
				OnAfterSelfMove(elapsedSeconds, selfAgePercent, positionDiff);
			}

			OnAfterEmit(elapsedSeconds, selfAgePercent);
		}

		// this does some movement and returns the position difference to the last position
		private Vector3 DoSelfMovement(float elapsedSeconds, float selfAgePercent)
		{
			if (_parameters.SelfMove)
			{
				Vector3 curMovementDirection = _selfMovementDirectionStart;

				if (_parameters.SelfChangeMovementDirection)
				{
					curMovementDirection.X = MathHelper.Lerp(curMovementDirection.X, _selfMovementDirectionEnd.X, selfAgePercent) * elapsedSeconds;
					curMovementDirection.Y = MathHelper.Lerp(curMovementDirection.Y, _selfMovementDirectionEnd.Y, selfAgePercent) * elapsedSeconds;
					curMovementDirection.Z = MathHelper.Lerp(curMovementDirection.Z, _selfMovementDirectionEnd.Z, selfAgePercent) * elapsedSeconds;

					curMovementDirection.Normalize();
				}

				float curVelocity = _selfVelocityStart;
				if (_parameters.SelfChangeVelocity)
				{
					curVelocity += (_selfVelocityEnd - _selfVelocityStart) * selfAgePercent;
				}

				Vector3 movement = (curMovementDirection * curVelocity) * elapsedSeconds;
				_totalMovement += movement;

				return movement;
			}

			return Vector3.Zero;
		}

		public void OnParentMovement(Vector3 movement)
		{
			_totalMovement += movement;
		}
	}
}
