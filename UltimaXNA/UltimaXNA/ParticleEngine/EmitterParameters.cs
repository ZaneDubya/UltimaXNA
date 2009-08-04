using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LuaInterface;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.ParticleEngine
{
	public abstract class EmitterParameters
	{
		// how fast do we emit objects?
		public bool ChangeEmitRate {
			get { return EmitRateEnd != EmitRateStart; }
		}
		public RandomizedValue EmitRateStart;
		public RandomizedValue EmitRateEnd;

		// how many emitted objects can be active at the same time?
		public int MaxEmittedCount;

		// when do we die? use a negative value to prevent this emitter from dying automatically
		public RandomizedValue SelfLifetime;

		// this value is added to the given start position
		public Vector3 SelfStartPositionOffset;

		// movement direction of the emitter itself
		public bool SelfMove
		{
			get { return SelfMovementDirectionStart.IsValid; }
		}
		public bool SelfChangeMovementDirection
		{
			get { return SelfMovementDirectionStart.IsValid && SelfMovementDirectionEnd.IsValid && SelfMovementDirectionStart != SelfMovementDirectionEnd; }
		}
		public RandomizedVector3 SelfMovementDirectionStart;
		public RandomizedVector3 SelfMovementDirectionEnd;

		// movement velocity
		public bool SelfChangeVelocity
		{
			get { return SelfVelocityStart.IsValid && SelfVelocityEnd.IsValid && SelfVelocityStart != SelfVelocityEnd; }
		}
		public RandomizedValue SelfVelocityStart;
		public RandomizedValue SelfVelocityEnd;

		private PositionProvider _positionProvider;
		public PositionProvider PositionProvider
		{
			get { return _positionProvider.Clone(); }
			set { _positionProvider = value; }
		}

		// should the emitted objects move with us? only applies to particles
		public bool EmittedMoveWithEmitter;

		// lua events
		public LuaFunction LuaOnStateChanged;
		public LuaFunction LuaOnDisposed;

		// set the default values
		public EmitterParameters() {
			EmitRateStart = 500;
			EmitRateEnd = 500;
			MaxEmittedCount = 10000;
			SelfLifetime = 13f;
			SelfStartPositionOffset = new Vector3(0, 0, 0);
			SelfMovementDirectionStart = new Vector3(0f, 1f, 0f);
			SelfMovementDirectionEnd = new Vector3(0f, 1f, 0f);
			SelfVelocityStart = 15;
			SelfVelocityEnd = 2;
			this.EmittedMoveWithEmitter = false;
			this._positionProvider = new PositionProviders.Point();
		}

		// copy ctor
		public EmitterParameters(EmitterParameters toCopy) {
			this.EmitRateStart = toCopy.EmitRateStart;
			this.EmitRateEnd = toCopy.EmitRateEnd;

			this.MaxEmittedCount = toCopy.MaxEmittedCount;

			this.SelfLifetime = toCopy.SelfLifetime;
			this.SelfStartPositionOffset = toCopy.SelfStartPositionOffset;

			this.SelfMovementDirectionStart = toCopy.SelfMovementDirectionStart;
			this.SelfMovementDirectionEnd = toCopy.SelfMovementDirectionEnd;

			this.SelfVelocityStart = toCopy.SelfVelocityStart;
			this.SelfVelocityEnd = toCopy.SelfVelocityEnd;

			this.EmittedMoveWithEmitter = toCopy.EmittedMoveWithEmitter;

			this._positionProvider = toCopy._positionProvider.Clone();

			this.LuaOnDisposed = toCopy.LuaOnDisposed;
			this.LuaOnStateChanged = toCopy.LuaOnStateChanged;
		}
	}

	public class ParticleEmitterParameters : EmitterParameters {
		// the shader we are using to calculate and render the particles
		public String ShaderName;

		public Matrix WorldViewProjection;

		// how long should the emitted objects life? We can not use random values here due to gpu/cpu sync
		public float EmittedLifetime;

		// movement of the emitted objects
		public bool EmittedDoMove
		{
			get { return EmittedMovementDirectionStart.IsValid && EmittedVelocityStart.IsValid; }
		}
		public bool EmittedChangeMovementDirection
		{
			get { return EmittedMovementDirectionStart.IsValid && EmittedMovementDirectionEnd.IsValid && EmittedMovementDirectionStart != EmittedMovementDirectionEnd; }
		}
		public RandomizedVector3 EmittedMovementDirectionStart;
		public RandomizedVector3 EmittedMovementDirectionEnd;

		// velocity of the emitted objects
		public bool EmittedChangeVelocity {
			get { return EmittedVelocityStart.IsValid && EmittedVelocityEnd.IsValid && EmittedVelocityStart != EmittedVelocityEnd; }
		}
		public RandomizedValue EmittedVelocityStart;
		public RandomizedValue EmittedVelocityEnd;
		

		// the color settings of the emitted particles (in case we emit particles)
		public bool EmittedUseColor
		{
			get { return EmittedColorStart.IsValid; }
		}
		public bool EmittedChangeColor
		{
			get { return EmittedColorStart.IsValid && EmittedColorEnd.IsValid && EmittedColorStart != EmittedColorEnd; }
		}
		public bool EmittedUseAlpha
		{
			get { return EmittedColorStart.IsAlphaValid; }
		}
		public bool EmittedChangeAlpha
		{
			get { return EmittedColorStart.IsAlphaValid && EmittedColorEnd.IsAlphaValid && EmittedColorStart.A != EmittedColorEnd.A; }
		}
		public RandomizedColor EmittedColorStart;
		public RandomizedColor EmittedColorEnd;

		// transparency
		public RandomizedValue EmittedAlphaStart
		{
			get { return EmittedColorStart.A; }
			set { EmittedColorStart.A = value; }
		}

		public float EmittedAlphaEnd
		{
			get { return EmittedColorEnd.A; }
			set { EmittedColorEnd.A = value; }
		}

		// the texture we are using for the particles
		public bool EmittedUseTexture {
			get { return EmittedEffectTexture != null; }
		}
		public Texture2D EmittedEffectTexture;

		// the size of our emitted particles
		public bool EmittedChangeSize {
			get { return EmittedSizeStart.IsValid && EmittedSizeEnd.IsValid && EmittedSizeStart != EmittedSizeEnd; }
		}
		public RandomizedValue EmittedSizeStart;
		public RandomizedValue EmittedSizeEnd;

		public ParticleEmitterParameters()
		{
			EmittedLifetime = 4f;
			EmittedMovementDirectionStart = new RandomizedVector3(new Vector3(1f, 1f, 0f), new Vector3(-1f, -1f, 0f));
			EmittedMovementDirectionEnd = RandomizedVector3.Invalid;
			EmittedVelocityStart = new RandomizedValue(8, 11);
			EmittedVelocityEnd = new RandomizedValue(3, 5);
			EmittedColorStart = new RandomizedColor(0f, 1f, 0f);
			EmittedColorEnd = new RandomizedColor(0f, 0f, 1f);
			EmittedAlphaStart = new RandomizedValue(1f, 1f);
			EmittedAlphaEnd = new RandomizedValue(1f, 1f);
			EmittedEffectTexture = null;
			EmittedSizeStart = new RandomizedValue(1f, 1f);
			EmittedSizeEnd = RandomizedValue.Invalid;
		}

		public ParticleEmitterParameters(ParticleEmitterParameters toCopy) : base(toCopy)
		{
			this.ShaderName = toCopy.ShaderName;
			this.WorldViewProjection = toCopy.WorldViewProjection;

			this.EmittedLifetime = toCopy.EmittedLifetime;
			this.EmittedMovementDirectionStart = toCopy.EmittedMovementDirectionStart;
			this.EmittedMovementDirectionEnd = toCopy.EmittedMovementDirectionEnd;
			this.EmittedVelocityStart = toCopy.EmittedVelocityStart;
			this.EmittedVelocityEnd = toCopy.EmittedVelocityEnd;
			this.EmittedColorStart = toCopy.EmittedColorStart;
			this.EmittedColorEnd = toCopy.EmittedColorEnd;
			this.EmittedEffectTexture = toCopy.EmittedEffectTexture;
			this.EmittedSizeStart = toCopy.EmittedSizeStart;
			this.EmittedSizeEnd = toCopy.EmittedSizeEnd;
		}
	}

	public enum EmittedType
	{
		Emitter,
		Particle
	}

	public class EmitterEmitterParameters : EmitterParameters {
		// what shall our new emitter emit?
		public EmittedType EmittedType;

		// if the emitted object is an emitter itself, these are it's parameters
		public EmitterParameters EmittedParameters;

		// lua events
		public LuaFunction LuaOnBeforeEmitterEmitted;
		public LuaFunction LuaOnAfterEmitterEmitted;

		public EmitterEmitterParameters() {
			EmittedParameters = null;
			EmittedType = EmittedType.Particle;
		}

		public EmitterEmitterParameters(EmitterEmitterParameters toCopy)
		{
			if (toCopy.EmittedParameters != null)
			{
				switch (toCopy.EmittedType)
				{
					case EmittedType.Emitter:
						this.EmittedParameters = new EmitterEmitterParameters((EmitterEmitterParameters)toCopy.EmittedParameters);
						break;
					case EmittedType.Particle:
						this.EmittedParameters = new ParticleEmitterParameters((ParticleEmitterParameters)toCopy.EmittedParameters);
						break;
				}
			}
			this.EmittedType = toCopy.EmittedType;

			this.LuaOnAfterEmitterEmitted = toCopy.LuaOnAfterEmitterEmitted;
			this.LuaOnBeforeEmitterEmitted = toCopy.LuaOnBeforeEmitterEmitted;
		}
	}
}
