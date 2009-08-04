using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using System.Threading;
using LuaInterface;

namespace UltimaXNA.ParticleEngine
{
	public struct DelayedLuaCall
	{
		private int _milliseconds;
		private LuaFunction _function;
		private LuaTable _args;

		public DelayedLuaCall(int millis, LuaFunction func, LuaTable args)
		{
			_milliseconds = millis;
			_function = func;
			_args = args;
		}

		public void Call()
		{
			_function.Call(_args);
		}

		public void Wait()
		{
			Thread.Sleep(_milliseconds);
		}
	}

	public class ParticleEngine
	{
		private static Matrix _defaultWorldViewProjection;
		public static Matrix DefaultWorldViewProjection
		{
			get { return _defaultWorldViewProjection; }
			set
			{
				_defaultWorldViewProjection = value;
				UpdateShaderParams();
			}
		}

		private static String _defaultShaderName;
		public static String DefaultShaderName
		{
			get { return _defaultShaderName; }
			set {
				_defaultShaderName = value;
			}
		}

		private static List<ParticleEffect> _effectList;
		private static List<ParticleEffect> _disposedBuffer;

		private static GraphicsDevice _graphics;
		public static GraphicsDevice Graphics
		{
			get { return _graphics; }
		}

		private static ContentManager _content;
		public static ContentManager Content
		{
			get { return _content; }
		}

		private static string _textureDirectoryPath;
		private static Dictionary<string, Texture2D> _textureCache;

		private static string _scriptDirectoryPath;

		private static Lua _lua;

		private static Mutex _delayedCallListMutex;
		private static List<DelayedLuaCall> _delayedCallList;

		public static void Initialize(Game game, string dataDirectory)
		{
			_defaultWorldViewProjection = Matrix.CreateOrthographicOffCenter(0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height, 0, Int32.MinValue, Int32.MaxValue);
			_defaultShaderName = "Shaders/ParticleEffect";

			_effectList = new List<ParticleEffect>();
			_disposedBuffer = new List<ParticleEffect>();
			_graphics = game.GraphicsDevice;
			_content = game.Content;
			_textureCache = new Dictionary<string, Texture2D>();

			_delayedCallListMutex = new Mutex();
			_delayedCallList = new List<DelayedLuaCall>();

			string texDir = Path.Combine(dataDirectory, "textures");
			if (!Directory.Exists(texDir))
			{
				throw new DirectoryNotFoundException(string.Format("ParticleEngine: TextureDirectory \"{0}\" not found", texDir));
			}
			_textureDirectoryPath = texDir;

			string scriptDir = Path.Combine(dataDirectory, "scripts");
			if (!Directory.Exists(scriptDir))
			{
				throw new DirectoryNotFoundException(string.Format("ParticleEngine: ScriptDirectory \"{0}\" not found", scriptDir));
			}
			_scriptDirectoryPath = scriptDir;

			InitializeLua();
		}

		private static void UpdateShaderParams()
		{
			foreach (ParticleEffect eff in _effectList)
			{
				eff.UpdateShaderParams();
			}
		}

		public static ParticleEffect CreateEffect(Vector3 position)
		{
			ParticleEffect ret = new ParticleEffect(position);
			_effectList.Add(ret);
			return ret;
		}

		public static ParticleEmitterParameters CreateParticleEmitterParameters()
		{
			ParticleEmitterParameters param = new ParticleEmitterParameters();
			param.ShaderName = DefaultShaderName;
			param.WorldViewProjection = DefaultWorldViewProjection;
			return param;
		}

		public static EmitterEmitterParameters CreateEmitterEmitterParameters()
		{
			return new EmitterEmitterParameters();
		}

		public static EmitterParameters CreateEmitterParameters(EmittedType type)
		{
			switch (type)
			{
				case EmittedType.Emitter:
					return CreateEmitterEmitterParameters();
				default:
				case EmittedType.Particle:
					return CreateParticleEmitterParameters();
			}
		}

		public static ParticleEmitter CreateParticleEmitter(ParticleEmitterParameters param)
		{
			return new ParticleEmitter(param);
		}

		public static EmitterEmitter CreateEmitterEmitter(EmitterEmitterParameters param)
		{
			return new EmitterEmitter(param);
		}

		public static Emitter CreateEmitter(EmitterParameters param)
		{
			if (param is EmitterEmitterParameters)
			{
				return CreateEmitterEmitter((EmitterEmitterParameters)param);
			}
			else if (param is ParticleEmitterParameters)
			{
				return CreateParticleEmitter((ParticleEmitterParameters)param);
			}
			else
			{
				throw new ArgumentException("Param is of unknown type", "param");
			}
		}

		public static void Update(GameTime gameTime)
		{
			foreach (ParticleEffect eff in _effectList)
			{
				eff.Update(gameTime);
			}

			DisposeEffects();

			DoDelayedCalls();
		}

		public static void DisposeEffect(ParticleEffect effect)
		{
			_disposedBuffer.Add(effect);
		}

		public static void DisposeEffects()
		{
			foreach (ParticleEffect eff in _disposedBuffer)
			{
				eff.Dispose();
				_effectList.Remove(eff);
			}

			_disposedBuffer.Clear();
		}

		public static void Draw(GameTime gameTime)
		{
			SetParticleRenderStates(_graphics.RenderState);

			foreach (ParticleEffect eff in _effectList)
			{
				eff.Draw(gameTime, _graphics);
			}

			// Reset a couple of the more unusual renderstates that we changed,
			// so as not to mess up any other subsequent drawing.
			_graphics.RenderState.PointSpriteEnable = false;
			_graphics.RenderState.DepthBufferWriteEnable = true;
		}

		/// <summary>
		/// Helper for setting the renderstates used to draw particles.
		/// </summary>
		private static void SetParticleRenderStates(RenderState renderState)
		{
			// Enable point sprites.
			renderState.PointSpriteEnable = true;
			renderState.PointSizeMax = 256;

			// Set the alpha blend mode.
			renderState.AlphaBlendEnable = true;
			renderState.AlphaBlendOperation = BlendFunction.Add;
			renderState.SourceBlend = Blend.SourceAlpha;
			renderState.DestinationBlend = Blend.InverseSourceAlpha;

			// Set the alpha test mode.
			renderState.AlphaTestEnable = true;
			renderState.AlphaFunction = CompareFunction.Greater;
			renderState.ReferenceAlpha = 0;

			// Enable the depth buffer (so particles will not be visible through
			// solid objects like the ground plane), but disable depth writes
			// (so particles will not obscure other particles).
			renderState.DepthBufferEnable = false;
			renderState.DepthBufferWriteEnable = false;
		}

		public static Texture2D GetTexture(string identifier)
		{
			if (_textureCache.ContainsKey(identifier))
			{
				return _textureCache[identifier];
			}

			Texture2D ret = null;
			try
			{
				ret = _content.Load<Texture2D>(identifier);
			}
			catch (ContentLoadException)
			{
				try
				{
					ret = Texture2D.FromFile(_graphics, Path.Combine(_textureDirectoryPath, identifier));
				} catch (Exception) {
				}
			}

			if (ret != null)
			{
				_textureCache[identifier] = ret;
				return ret;
			}

			// TODO: Log error
			return null;
		}

		private static void InitializeLua()
		{
			_lua = new Lua();

			LuaLoadAssembly("UltimaXNA.exe");

			LuaImportType("RandomizedValue", typeof(RandomizedValue));
			LuaImportType("RandomizedColor", typeof(RandomizedColor));
			LuaImportType("RandomizedVector3", typeof(RandomizedVector3));
			LuaImportType("EmitterShape", typeof(EmitterShape));
			LuaImportType("EmittedType", typeof(EmittedType));
			LuaImportType("PPPoint", typeof(PositionProviders.Point));
			LuaImportType("PPCircle", typeof(PositionProviders.Circle));
			LuaImportType("PPCircleOutline", typeof(PositionProviders.CircleOutline));

			_lua["util"] = new LuaUtil();

			_lua.DoString("luanet.load_assembly = function(assembly) print(\"this function is unsupported\") end");
			_lua.DoString("luanet.import_type = function(assembly) print(\"this function is unsupported\") end");
		}

		private static void LuaImportType(string name, Type t)
		{
			_lua.DoString(string.Format("{0} = luanet.import_type(\"{1}\")", name, t.FullName));
		}

		private static void LuaLoadAssembly(string name) {
			_lua.DoString(string.Format("luanet.load_assembly(\"{0}\")", name));
		}

		public static void LoadEffectFromScript(string scriptName)
		{
			string fullPath = null;
			try
			{
				fullPath = Path.Combine(_scriptDirectoryPath, scriptName);
				if (!fullPath.EndsWith(".lua"))
				{
					fullPath += ".lua";
				}

				_lua.DoFile(fullPath);
			}
			catch (Exception e)
			{
				// TODO: Logging facility
				Console.Error.WriteLine("Error processing lua file {0}:", fullPath);
				Console.Error.WriteLine(e.Message);
			}
		}

		public static void LoadEffectFromString(string script)
		{
			try
			{
				_lua.DoString(script);
			}
			catch (Exception e)
			{
				// TODO: Logging facility
				Console.Error.WriteLine(e.Message);
			}
		}

		// stuff to do delayed calls
		public static void AddDelayedLuaCall(int millis, LuaFunction func, LuaTable args)
		{
			DelayedLuaCall call = new DelayedLuaCall(millis, func, args);
			ThreadPool.QueueUserWorkItem(new WaitCallback(ParticleEngine.InternalWaitForDelayedCall), call);
		}

		// we wait until we want to call it, then add it to our delayed call queue in order to have no problems with multithreading
		public static void InternalWaitForDelayedCall(object state)
		{
			DelayedLuaCall call = (DelayedLuaCall)state;
			call.Wait();


			_delayedCallListMutex.WaitOne();
			_delayedCallList.Add(call);
			_delayedCallListMutex.ReleaseMutex();
		}

		private static void DoDelayedCalls()
		{
			_delayedCallListMutex.WaitOne();

			foreach (DelayedLuaCall c in _delayedCallList)
			{
				try
				{
					c.Call();
				}
				catch (Exception e)
				{
					// TODO: Logging facility
					Console.Error.WriteLine(e.Message);
				}
			}

			_delayedCallList.Clear();

			_delayedCallListMutex.ReleaseMutex();
		}
	}
}
