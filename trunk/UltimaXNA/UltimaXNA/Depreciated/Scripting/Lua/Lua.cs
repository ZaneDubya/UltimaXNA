/*
LuaInterface License
--------------------

LuaInterface is licensed under the terms of the MIT license reproduced below.
This mean that LuaInterface is free software and can be used for both academic and
commercial purposes at absolutely no cost.

===============================================================================

Copyright (C) 2003-2005 Fabio Mascarenhas de Queiroz.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

===============================================================================
*/


namespace UltimaXNA.Scripting 
{    
	using System;
	using System.IO;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Reflection;
    using System.Threading;
    using Lua511;
    using System.Text;

	/*
	 * Main class of LuaInterface
	 * Object-oriented wrapper to Lua API
	 *
	 * Author: Fabio Mascarenhas
	 * Version: 1.0
	 * 
	 * // steffenj: important changes in Lua class:
	 * - removed all Open*Lib() functions 
	 * - all libs automatically open in the Lua class constructor (just assign nil to unwanted libs)
	 * */
	public class Lua : IDisposable
	{

		static string init_luanet =
			"local metatable = {}									\n"+
			"local import_type = luanet.import_type							\n"+
			"local load_assembly = luanet.load_assembly						\n"+
			"											\n"+
			"-- Lookup a .NET identifier component.							\n"+
			"function metatable:__index(key) -- key is e.g. \"Form\"				\n"+
			"    -- Get the fully-qualified name, e.g. \"System.Windows.Forms.Form\"		\n"+
			"    local fqn = ((rawget(self,\".fqn\") and rawget(self,\".fqn\") ..			\n"+
			"		\".\") or \"\") .. key							\n"+
			"											\n"+
			"    -- Try to find either a luanet function or a CLR type				\n"+
			"    local obj = rawget(luanet,key) or import_type(fqn)					\n"+
			"											\n"+
			"    -- If key is neither a luanet function or a CLR type, then it is simply		\n"+
			"    -- an identifier component.							\n"+
			"    if obj == nil then									\n"+
			"		-- It might be an assembly, so we load it too.				\n"+
			"        load_assembly(fqn)								\n"+
			"        obj = { [\".fqn\"] = fqn }							\n"+
			"        setmetatable(obj, metatable)							\n"+
			"    end										\n"+
			"											\n"+
			"    -- Cache this lookup								\n"+
			"    rawset(self, key, obj)								\n"+
			"    return obj										\n"+
			"end											\n"+
			"											\n"+
			"-- A non-type has been called; e.g. foo = System.Foo()					\n"+
			"function metatable:__call(...)								\n"+
			"    error(\"No such type: \" .. rawget(self,\".fqn\"), 2)				\n"+
			"end											\n"+
			"											\n"+
			"-- This is the root of the .NET namespace						\n"+
			"luanet[\".fqn\"] = false								\n"+
			"setmetatable(luanet, metatable)							\n"+
			"											\n"+
			"-- Preload the mscorlib assembly							\n"+
			"luanet.load_assembly(\"mscorlib\")							\n";

        IntPtr state;
		ObjectTranslator translator;
        LuaCSFunction panicCallback;

        /// <summary>
        /// Used to ensure multiple .net threads all get serialized by this single lock for access to the lua stack/objects
        /// </summary>
        object luaLock = new object();

//#if DEBUG
        StringBuilder sb = new StringBuilder();
        public string ExecutedCode { get { return sb.ToString(); } }
//#endif

		public Lua() 
		{
			state = LuaDLL.luaL_newstate();	// steffenj: Lua 5.1.1 API change (lua_open is gone)
			//LuaDLL.luaopen_base(luaState);	// steffenj: luaopen_* no longer used
			LuaDLL.luaL_openlibs(state);		// steffenj: Lua 5.1.1 API change (luaopen_base is gone, just open all libs right here)
		    LuaDLL.lua_pushstring(state, "LUAINTERFACE LOADED");
            LuaDLL.lua_pushboolean(state, true);
            LuaDLL.lua_settable(state, (int) LuaIndexes.LUA_REGISTRYINDEX);
			LuaDLL.lua_newtable(state);
			LuaDLL.lua_setglobal(state, "luanet");
            LuaDLL.lua_pushvalue(state, (int)LuaIndexes.LUA_GLOBALSINDEX);
			LuaDLL.lua_getglobal(state, "luanet");
			LuaDLL.lua_pushstring(state, "getmetatable");
			LuaDLL.lua_getglobal(state, "getmetatable");
			LuaDLL.lua_settable(state, -3);
            LuaDLL.lua_replace(state, (int)LuaIndexes.LUA_GLOBALSINDEX);
			translator=new ObjectTranslator(this,state);
            LuaDLL.lua_replace(state, (int)LuaIndexes.LUA_GLOBALSINDEX);
			LuaDLL.luaL_dostring(state, Lua.init_luanet);	// steffenj: lua_dostring renamed to luaL_dostring

            // We need to keep this in a managed reference so the delegate doesn't get garbage collected
            panicCallback = new LuaCSFunction(PanicCallback);
            LuaDLL.lua_atpanic(state, panicCallback);

            //LuaDLL.lua_atlock(luaState, lockCallback = new LuaCSFunction(LockCallback));
            //LuaDLL.lua_atunlock(luaState, unlockCallback = new LuaCSFunction(UnlockCallback));
        }

        private bool _StatePassed;

    	/*
    	 * CAUTION: LuaInterface.Lua instances can't share the same lua state! 
    	 */
    	public Lua(Int64 luaState)
    	{
    		IntPtr lState = new IntPtr(luaState);
    		LuaDLL.lua_pushstring(lState, "LUAINTERFACE LOADED");
            LuaDLL.lua_gettable(lState, (int)LuaIndexes.LUA_REGISTRYINDEX);
    		
            if(LuaDLL.lua_toboolean(lState,-1)) 
            {
        		LuaDLL.lua_settop(lState,-2);
        		throw new LuaException("There is already a LuaInterface.Lua instance associated with this Lua state");
    		} 
            else 
            {
        		LuaDLL.lua_settop(lState,-2);
        		LuaDLL.lua_pushstring(lState, "LUAINTERFACE LOADED");
        		LuaDLL.lua_pushboolean(lState, true);
                LuaDLL.lua_settable(lState, (int)LuaIndexes.LUA_REGISTRYINDEX);
        		this.state=lState;
                LuaDLL.lua_pushvalue(lState, (int)LuaIndexes.LUA_GLOBALSINDEX);
				LuaDLL.lua_getglobal(lState, "luanet");
				LuaDLL.lua_pushstring(lState, "getmetatable");
				LuaDLL.lua_getglobal(lState, "getmetatable");
				LuaDLL.lua_settable(lState, -3);
                LuaDLL.lua_replace(lState, (int)LuaIndexes.LUA_GLOBALSINDEX);
				translator=new ObjectTranslator(this, this.state);
                LuaDLL.lua_replace(lState, (int)LuaIndexes.LUA_GLOBALSINDEX);
				LuaDLL.luaL_dostring(lState, Lua.init_luanet);	// steffenj: lua_dostring renamed to luaL_dostring
    		}
                
            _StatePassed = true;
    	}

        /// <summary>
        /// Called for each lua_lock call 
        /// </summary>
        /// <param name="luaState"></param>
        /// Not yet used
        int LockCallback(IntPtr luaState)
        {
            // Monitor.Enter(luaLock);

            return 0;
        }

        /// <summary>
        /// Called for each lua_unlock call 
        /// </summary>
        /// <param name="luaState"></param>
        /// Not yet used
        int UnlockCallback(IntPtr luaState)
        {
            // Monitor.Exit(luaLock);

            return 0;
        }

        public void Close()
        {
            if (_StatePassed)
                return;

            if (state != IntPtr.Zero)
                LuaDLL.lua_close(state);
            //luaState = IntPtr.Zero; <- suggested by Christopher Cebulski http://luaforge.net/forum/forum.php?thread_id=44593&forum_id=146
        }

        static int PanicCallback(IntPtr luaState)
        {
            // string desc = LuaDLL.lua_tostring(luaState, 1);
            string reason = String.Format("unprotected error in call to Lua API ({0})", LuaDLL.lua_tostring(luaState, -1));

           //        lua_tostring(L, -1);

            throw new LuaException(reason);
        }



        /// <summary>
        /// Assuming we have a Lua error string sitting on the stack, throw a C# exception out to the user's app
        /// </summary>
        void ThrowExceptionFromError(int oldTop)
        {
            object err = translator.getObject(state, -1);
            LuaDLL.lua_settop(state, oldTop);

            // If the 'error' on the stack is an actual C# exception, just rethrow it.  Otherwise the value must have started
            // as a true Lua error and is best interpreted as a string - wrap it in a LuaException and rethrow.
            Exception thrown = err as Exception;

            if (thrown == null)
            {
                if (err == null)
                    err = "Unknown Lua Error";

                thrown = new LuaException(err.ToString());
            }

            throw thrown;
        }



        /// <summary>
        /// Convert C# exceptions into Lua errors
        /// </summary>
        /// <returns>num of things on stack</returns>
        /// <param name="e">null for no pending exception</param>
        internal int SetPendingException(Exception e)
        {
            Exception caughtExcept = e;

            if (caughtExcept != null)
            {
                translator.throwError(state, caughtExcept);
                LuaDLL.lua_pushnil(state);

                return 1;
            }
            else
                return 0;
        }

        /// <summary>
        /// CP: Submitted by Paul Moore
        /// </summary>
        /// <param name="chunk"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public LuaFunction LoadString(string chunk, string name)
        {
            int oldTop = LuaDLL.lua_gettop(state);
            if (LuaDLL.luaL_loadbuffer(state, chunk, name) != 0)
                ThrowExceptionFromError(oldTop);
            return translator.getFunction(state, -1);
        }

        /// <summary>
        /// CP: Submitted by Paul Moore
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public virtual LuaFunction LoadFile(string fileName)
        {
            int oldTop = LuaDLL.lua_gettop(state);
            if (LuaDLL.luaL_loadfile(state, fileName) != 0)
                ThrowExceptionFromError(oldTop);

            return translator.getFunction(state, -1);
        }

        /*
         * Excutes a Lua chunk and returns all the chunk's return
         * values in an array
         */
        public virtual object[] DoString(byte[] chunk)
        {
            return DoString(Encoding.ASCII.GetString(chunk));
        }

		/*
		 * Excutes a Lua chunk and returns all the chunk's return
		 * values in an array
		 */
		public virtual object[] DoString(string chunk) 
		{
            return DoString(chunk, "chunk");
		}

        /// <summary>
        /// Executes a Lua chnk and returns all the chunk's return values in an array.
        /// </summary>
        /// <param name="chunk">Chunk to execute</param>
        /// <param name="chunkName">Name to associate with the chunk</param>
        /// <returns></returns>
        public virtual object[] DoString(string chunk, string chunkName)
        {
            sb.AppendLine(chunk);

            int oldTop = LuaDLL.lua_gettop(state);

            if (LuaDLL.luaL_loadbuffer(state, chunk, chunkName) == 0)
            {
                if (LuaDLL.lua_pcall(state, 0, -1, 0) == 0)
                    return translator.popValues(state, oldTop);
                else
                    ThrowExceptionFromError(oldTop);
            }
            else
                ThrowExceptionFromError(oldTop);

            return null;            // Never reached - keeps compiler happy
        }

		/*
		 * Excutes a Lua file and returns all the chunk's return
		 * values in an array
		 */
        public virtual object[] DoFile(string fileName)
        {
            sb.AppendLine(File.ReadAllText(fileName));

			int oldTop=LuaDLL.lua_gettop(state);

			if(LuaDLL.luaL_loadfile(state,fileName)==0) 
			{
                if (LuaDLL.lua_pcall(state, 0, -1, 0) == 0)
                    return translator.popValues(state, oldTop);
                else
                    ThrowExceptionFromError(oldTop);
			} 
			else
                ThrowExceptionFromError(oldTop);

            return null;            // Never reached - keeps compiler happy
		}


		/*
		 * Indexer for global variables from the LuaInterpreter
		 * Supports navigation of tables by using . operator
		 */
		public object this[string fullPath]
		{
			get 
			{
				object returnValue=null;
				int oldTop=LuaDLL.lua_gettop(state);
				string[] path=fullPath.Split(new char[] { '.' });
				LuaDLL.lua_getglobal(state,path[0]);
				returnValue=translator.getObject(state,-1);
				if(path.Length>1) 
				{
					string[] remainingPath=new string[path.Length-1];
					Array.Copy(path,1,remainingPath,0,path.Length-1);
					returnValue=getObject(remainingPath);
				}
				LuaDLL.lua_settop(state,oldTop);
				return returnValue;
			}
			set 
			{
				int oldTop=LuaDLL.lua_gettop(state);
				string[] path=fullPath.Split(new char[] { '.' });
				if(path.Length==1) 
				{
					translator.push(state,value);
					LuaDLL.lua_setglobal(state,fullPath);
				} 
				else 
				{
					LuaDLL.lua_getglobal(state,path[0]);
					string[] remainingPath=new string[path.Length-1];
					Array.Copy(path,1,remainingPath,0,path.Length-1);
					setObject(remainingPath,value);
				}
				LuaDLL.lua_settop(state,oldTop);
			}
		}
		/*
		 * Navigates a table in the top of the stack, returning
		 * the value of the specified field
		 */
		internal object getObject(string[] remainingPath) 
		{
			object returnValue=null;
			for(int i=0;i<remainingPath.Length;i++) 
			{
				LuaDLL.lua_pushstring(state,remainingPath[i]);
				LuaDLL.lua_gettable(state,-2);
				returnValue=translator.getObject(state,-1);
				if(returnValue==null) break;	
			}
			return returnValue;    
		}
		/*
		 * Gets a numeric global variable
		 */
		public double GetNumber(string fullPath) 
		{
			return (double)this[fullPath];
		}
		/*
		 * Gets a string global variable
		 */
		public string GetString(string fullPath) 
		{
			return (string)this[fullPath];
		}
		/*
		 * Gets a table global variable
		 */
		public LuaTable GetTable(string fullPath) 
		{
			return (LuaTable)this[fullPath];
		}
		/*
		 * Gets a table global variable as an object implementing
		 * the interfaceType interface
		 */
		public object GetTable(Type interfaceType, string fullPath) 
		{
			return CodeGeneration.Instance.GetClassInstance(interfaceType,GetTable(fullPath));
		}
		/*
		 * Gets a function global variable
		 */
		public LuaFunction GetFunction(string fullPath) 
		{
            object obj=this[fullPath];
			return (obj is LuaCSFunction ? new LuaFunction((LuaCSFunction)obj,this) : (LuaFunction)obj);
		}
		/*
		 * Gets a function global variable as a delegate of
		 * type delegateType
		 */
		public Delegate GetFunction(Type delegateType,string fullPath) 
		{
			return CodeGeneration.Instance.GetDelegate(delegateType,GetFunction(fullPath));
		}
		/*
		 * Calls the object as a function with the provided arguments,
		 * returning the function's returned values inside an array
		 */
		internal object[] callFunction(object function,object[] args) 
		{
            return callFunction(function, args, null);
		}


		/*
		 * Calls the object as a function with the provided arguments and
		 * casting returned values to the types in returnTypes before returning
		 * them in an array
		 */
		internal object[] callFunction(object function,object[] args,Type[] returnTypes) 
		{
			int nArgs=0;
			int oldTop=LuaDLL.lua_gettop(state);
			if(!LuaDLL.lua_checkstack(state,args.Length+6))
                throw new LuaException("Lua stack overflow");
			translator.push(state,function);
			if(args!=null) 
			{
				nArgs=args.Length;
				for(int i=0;i<args.Length;i++) 
				{
					translator.push(state,args[i]);
				}
			}
            int error = LuaDLL.lua_pcall(state, nArgs, -1, 0);
            if (error != 0)
                ThrowExceptionFromError(oldTop);

            if(returnTypes != null)
			    return translator.popValues(state,oldTop,returnTypes);
            else
                return translator.popValues(state, oldTop);
		}
		/*
		 * Navigates a table to set the value of one of its fields
		 */
		internal void setObject(string[] remainingPath, object val) 
		{
			for(int i=0; i<remainingPath.Length-1;i++) 
			{
				LuaDLL.lua_pushstring(state,remainingPath[i]);
				LuaDLL.lua_gettable(state,-2);
			}
			LuaDLL.lua_pushstring(state,remainingPath[remainingPath.Length-1]);
			translator.push(state,val);
			LuaDLL.lua_settable(state,-3);
		}
		/*
		 * Creates a new table as a global variable or as a field
		 * inside an existing table
		 */
		public void NewTable(string fullPath) 
		{
			string[] path=fullPath.Split(new char[] { '.' });
			int oldTop=LuaDLL.lua_gettop(state);
			if(path.Length==1) 
			{
				LuaDLL.lua_newtable(state);
				LuaDLL.lua_setglobal(state,fullPath);
			} 
			else 
			{
				LuaDLL.lua_getglobal(state,path[0]);
				for(int i=1; i<path.Length-1;i++) 
				{
					LuaDLL.lua_pushstring(state,path[i]);
					LuaDLL.lua_gettable(state,-2);
				}
				LuaDLL.lua_pushstring(state,path[path.Length-1]);
				LuaDLL.lua_newtable(state);
				LuaDLL.lua_settable(state,-3);
			}
			LuaDLL.lua_settop(state,oldTop);
		}

		public ListDictionary GetTableDict(LuaTable table)
		{
			ListDictionary dict = new ListDictionary();

			int oldTop = LuaDLL.lua_gettop(state);
			translator.push(state, table);
			LuaDLL.lua_pushnil(state);
			while (LuaDLL.lua_next(state, -2) != 0) 
			{
				dict[translator.getObject(state, -2)] = translator.getObject(state, -1);
				LuaDLL.lua_settop(state, -2);
			}
			LuaDLL.lua_settop(state, oldTop);

			return dict;
		}

		/*
		 * Lets go of a previously allocated reference to a table, function
		 * or userdata
		 */

      #region lua debug functions

      /// <summary>
      /// lua hook calback delegate
      /// </summary>
      /// <author>Reinhard Ostermeier</author>
      private LuaHookFunction hookCallback = null;

      /// <summary>
      /// Activates the debug hook
      /// </summary>
      /// <param name="mask">Mask</param>
      /// <param name="count">Count</param>
      /// <returns>see lua docs. -1 if hook is already set</returns>
      /// <author>Reinhard Ostermeier</author>
      public int SetDebugHook(EventMasks mask, int count)
      {
         if (hookCallback == null)
         {
            hookCallback = new LuaHookFunction(DebugHookCallback);
            return LuaDLL.lua_sethook(state, hookCallback, (int)mask, count);
         }
         return -1;
      }

      /// <summary>
      /// Removes the debug hook
      /// </summary>
      /// <returns>see lua docs</returns>
      /// <author>Reinhard Ostermeier</author>
      public int RemoveDebugHook()
      {
         hookCallback = null;
         return LuaDLL.lua_sethook(state, null, 0, 0);
      }

      /// <summary>
      /// Gets the hook mask.
      /// </summary>
      /// <returns>hook mask</returns>
      /// <author>Reinhard Ostermeier</author>
      public EventMasks GetHookMask()
      {
         return (EventMasks)LuaDLL.lua_gethookmask(state);
      }

      /// <summary>
      /// Gets the hook count
      /// </summary>
      /// <returns>see lua docs</returns>
      /// <author>Reinhard Ostermeier</author>
      public int GetHookCount()
      {
         return LuaDLL.lua_gethookcount(state);
      }

      /// <summary>
      /// Gets the stack entry on a given level
      /// </summary>
      /// <param name="level">level</param>
      /// <param name="luaDebug">lua debug structure</param>
      /// <returns>Returns true if level was allowed, false if level was invalid.</returns>
      /// <author>Reinhard Ostermeier</author>
      public bool GetStack(int level, out LuaDebug luaDebug)
      {
         luaDebug = new LuaDebug();
         IntPtr ld = System.Runtime.InteropServices.Marshal.AllocHGlobal(System.Runtime.InteropServices.Marshal.SizeOf(luaDebug));
         System.Runtime.InteropServices.Marshal.StructureToPtr(luaDebug, ld, false);
         try
         {
            return LuaDLL.lua_getstack(state, level, ld) != 0;
         }
         finally
         {
            luaDebug = (LuaDebug)System.Runtime.InteropServices.Marshal.PtrToStructure(ld, typeof(LuaDebug));
            System.Runtime.InteropServices.Marshal.FreeHGlobal(ld);
         }
      }

      /// <summary>
      /// Gets info (see lua docs)
      /// </summary>
      /// <param name="what">what (see lua docs)</param>
      /// <param name="luaDebug">lua debug structure</param>
      /// <returns>see lua docs</returns>
      /// <author>Reinhard Ostermeier</author>
      public int GetInfo(String what, ref LuaDebug luaDebug)
      {
         IntPtr ld = System.Runtime.InteropServices.Marshal.AllocHGlobal(System.Runtime.InteropServices.Marshal.SizeOf(luaDebug));
         System.Runtime.InteropServices.Marshal.StructureToPtr(luaDebug, ld, false);
         try
         {
            return LuaDLL.lua_getinfo(state, what, ld);
         }
         finally
         {
            luaDebug = (LuaDebug)System.Runtime.InteropServices.Marshal.PtrToStructure(ld, typeof(LuaDebug));
            System.Runtime.InteropServices.Marshal.FreeHGlobal(ld);
         }
      }

      /// <summary>
      /// Gets local (see lua docs)
      /// </summary>
      /// <param name="luaDebug">lua debug structure</param>
      /// <param name="n">see lua docs</param>
      /// <returns>see lua docs</returns>
      /// <author>Reinhard Ostermeier</author>
      public String GetLocal(LuaDebug luaDebug, int n)
      {
         IntPtr ld = System.Runtime.InteropServices.Marshal.AllocHGlobal(System.Runtime.InteropServices.Marshal.SizeOf(luaDebug));
         System.Runtime.InteropServices.Marshal.StructureToPtr(luaDebug, ld, false);
         try
         {
            return LuaDLL.lua_getlocal(state, ld, n);
         }
         finally
         {
            System.Runtime.InteropServices.Marshal.FreeHGlobal(ld);
         }
      }

      /// <summary>
      /// Sets local (see lua docs)
      /// </summary>
      /// <param name="luaDebug">lua debug structure</param>
      /// <param name="n">see lua docs</param>
      /// <returns>see lua docs</returns>
      /// <author>Reinhard Ostermeier</author>
      public String SetLocal(LuaDebug luaDebug, int n)
      {
         IntPtr ld = System.Runtime.InteropServices.Marshal.AllocHGlobal(System.Runtime.InteropServices.Marshal.SizeOf(luaDebug));
         System.Runtime.InteropServices.Marshal.StructureToPtr(luaDebug, ld, false);
         try
         {
            return LuaDLL.lua_setlocal(state, ld, n);
         }
         finally
         {
            System.Runtime.InteropServices.Marshal.FreeHGlobal(ld);
         }
      }

      /// <summary>
      /// Gets up value (see lua docs)
      /// </summary>
      /// <param name="funcindex">see lua docs</param>
      /// <param name="n">see lua docs</param>
      /// <returns>see lua docs</returns>
      /// <author>Reinhard Ostermeier</author>
      public String GetUpValue(int funcindex, int n)
      {
         return LuaDLL.lua_getupvalue(state, funcindex, n);
      }

      /// <summary>
      /// Sets up value (see lua docs)
      /// </summary>
      /// <param name="funcindex">see lua docs</param>
      /// <param name="n">see lua docs</param>
      /// <returns>see lua docs</returns>
      /// <author>Reinhard Ostermeier</author>
      public String SetUpValue(int funcindex, int n)
      {
         return LuaDLL.lua_setupvalue(state, funcindex, n);
      }

      /// <summary>
      /// Delegate that is called on lua hook callback
      /// </summary>
      /// <param name="luaState">lua state</param>
      /// <param name="luaDebug">Pointer to LuaDebug (lua_debug) structure</param>
      /// <author>Reinhard Ostermeier</author>
      private void DebugHookCallback(IntPtr luaState, IntPtr luaDebug)
      {
         try
         {
            LuaDebug ld = (LuaDebug)System.Runtime.InteropServices.Marshal.PtrToStructure(luaDebug, typeof(LuaDebug));
            EventHandler<DebugHookEventArgs> temp = DebugHook;
            if (temp != null)
            {
               temp(this, new DebugHookEventArgs(ld));
            }
         }
         catch (Exception ex)
         {
            OnHookException(new HookExceptionEventArgs(ex));
         }
      }

      /// <summary>
      /// Event that is raised when an exception occures during a hook call.
      /// </summary>
      /// <author>Reinhard Ostermeier</author>
      public event EventHandler<HookExceptionEventArgs> HookException;
      private void OnHookException(HookExceptionEventArgs e)
      {
         EventHandler<HookExceptionEventArgs> temp = HookException;
         if (temp != null)
         {
            temp(this, e);
         }
      }

      /// <summary>
      /// Event when lua hook callback is called
      /// </summary>
      /// <remarks>
      /// Is only raised if SetDebugHook is called before.
      /// </remarks>
      /// <author>Reinhard Ostermeier</author>
      public event EventHandler<DebugHookEventArgs> DebugHook;

      /// <summary>
      /// Pops a value from the lua stack.
      /// </summary>
      /// <returns>Returns the top value from the lua stack.</returns>
      /// <author>Reinhard Ostermeier</author>
      public object Pop()
      {
         int top = Lua511.LuaDLL.lua_gettop(state);
         return translator.popValues(state, top - 1)[0];
      }

      /// <summary>
      /// Pushes a value onto the lua stack.
      /// </summary>
      /// <param name="value">Value to push.</param>
      /// <author>Reinhard Ostermeier</author>
      public void Push(object value)
      {
         translator.push(state, value);
      }

      #endregion

		internal void dispose(int reference) 
		{
            if (state != IntPtr.Zero) //Fix submitted by Qingrui Li
                LuaDLL.lua_unref(state,reference);
		}
		/*
		 * Gets a field of the table corresponding to the provided reference
		 * using rawget (do not use metatables)
		 */
		internal object rawGetObject(int reference,string field) 
		{
			int oldTop=LuaDLL.lua_gettop(state);
			LuaDLL.lua_getref(state,reference);
			LuaDLL.lua_pushstring(state,field);
			LuaDLL.lua_rawget(state,-2);
			object obj=translator.getObject(state,-1);
			LuaDLL.lua_settop(state,oldTop);
			return obj;
		}
		/*
		 * Gets a field of the table or userdata corresponding to the provided reference
		 */
		internal object getObject(int reference,string field) 
		{
			int oldTop=LuaDLL.lua_gettop(state);
			LuaDLL.lua_getref(state,reference);
			object returnValue=getObject(field.Split(new char[] {'.'}));
			LuaDLL.lua_settop(state,oldTop);
			return returnValue;
		}
		/*
		 * Gets a numeric field of the table or userdata corresponding the the provided reference
		 */
		internal object getObject(int reference,object field) 
		{
			int oldTop=LuaDLL.lua_gettop(state);
			LuaDLL.lua_getref(state,reference);
			translator.push(state,field);
			LuaDLL.lua_gettable(state,-2);
			object returnValue=translator.getObject(state,-1);
			LuaDLL.lua_settop(state,oldTop);
			return returnValue;
		}
		/*
		 * Sets a field of the table or userdata corresponding the the provided reference
		 * to the provided value
		 */
		internal void setObject(int reference, string field, object val) 
		{
			int oldTop=LuaDLL.lua_gettop(state);
			LuaDLL.lua_getref(state,reference);
			setObject(field.Split(new char[] {'.'}),val);
			LuaDLL.lua_settop(state,oldTop);
		}
		/*
		 * Sets a numeric field of the table or userdata corresponding the the provided reference
		 * to the provided value
		 */
		internal void setObject(int reference, object field, object val) 
		{
			int oldTop=LuaDLL.lua_gettop(state);
			LuaDLL.lua_getref(state,reference);
			translator.push(state,field);
			translator.push(state,val);
			LuaDLL.lua_settable(state,-3);
			LuaDLL.lua_settop(state,oldTop);
		}

		/*
		 * Registers an object's method as a Lua function (global or table field)
		 * The method may have any signature
		 */
        public LuaFunction RegisterFunction(string path, object target, MethodBase function /*MethodInfo function*/)  //CP: Fix for struct constructor by Alexander Kappner (link: http://luaforge.net/forum/forum.php?thread_id=2859&forum_id=145)
		{
            // We leave nothing on the stack when we are done
            int oldTop = LuaDLL.lua_gettop(state);
            
			LuaMethodWrapper wrapper=new LuaMethodWrapper(translator,target,function.DeclaringType,function);
			translator.push(state,new LuaCSFunction(wrapper.call));

			this[path]=translator.getObject(state,-1);
            LuaFunction f = GetFunction(path);

            LuaDLL.lua_settop(state, oldTop);

            return f;
		}


		/*
		 * Compares the two values referenced by ref1 and ref2 for equality
		 */
		internal bool compareRef(int ref1, int ref2) 
		{
			int top=LuaDLL.lua_gettop(state);
			LuaDLL.lua_getref(state,ref1);
			LuaDLL.lua_getref(state,ref2);
            int equal=LuaDLL.lua_equal(state,-1,-2);
			LuaDLL.lua_settop(state,top);
			return (equal!=0);
		}
        
        internal void pushCSFunction(LuaCSFunction function)
        {
            translator.pushFunction(state,function);
        }

        #region IDisposable Members

        public virtual void Dispose()
        {
            if (translator != null)
            {
                translator.pendingEvents.Dispose();
                translator = null;
            }
        
            this.Close();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }

        #endregion
   }

   /// <summary>
   /// Event codes for lua hook function
   /// </summary>
   /// <remarks>
   /// Do not change any of the values because they must match the lua values
   /// </remarks>
   /// <author>Reinhard Ostermeier</author>
   public enum EventCodes
   {
      LUA_HOOKCALL = 0,
      LUA_HOOKRET = 1,
      LUA_HOOKLINE = 2,
      LUA_HOOKCOUNT = 3,
      LUA_HOOKTAILRET = 4,
   }

   /// <summary>
   /// Event masks for lua hook callback
   /// </summary>
   /// <remarks>
   /// Do not change any of the values because they must match the lua values
   /// </remarks>
   /// <author>Reinhard Ostermeier</author>
   [Flags]
   public enum EventMasks
   {
      LUA_MASKCALL = (1 << EventCodes.LUA_HOOKCALL),
      LUA_MASKRET = (1 << EventCodes.LUA_HOOKRET),
      LUA_MASKLINE = (1 << EventCodes.LUA_HOOKLINE),
      LUA_MASKCOUNT = (1 << EventCodes.LUA_HOOKCOUNT),
      LUA_MASKALL = Int32.MaxValue,
   }

   /// <summary>
   /// Structure for lua debug information
   /// </summary>
   /// <remarks>
   /// Do not change this struct because it must match the lua structure lua_debug
   /// </remarks>
   /// <author>Reinhard Ostermeier</author>
   [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
   public struct LuaDebug
   {
      public EventCodes eventCode;
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)]
      public String name;
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)]
      public String namewhat;
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)]
      public String what;
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPStr)]
      public String source;
      public int currentline;
      public int nups;
      public int linedefined;
      public int lastlinedefined;
      [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 60/*LUA_IDSIZE*/)]
      public String shortsrc;
      public int i_ci;
   }

   /// <summary>
   /// Event args for hook callback event
   /// </summary>
   /// <author>Reinhard Ostermeier</author>
   public class DebugHookEventArgs : EventArgs
   {
      private readonly LuaDebug luaDebug;

      public DebugHookEventArgs(LuaDebug luaDebug)
      {
         this.luaDebug = luaDebug;
      }

      public LuaDebug LuaDebug
      {
         get { return luaDebug; }
      }
   }

   public class HookExceptionEventArgs : EventArgs
   {
      private readonly Exception m_Exception;
      public Exception Exception
      {
         get { return m_Exception; }
      }

      public HookExceptionEventArgs(Exception ex)
      {
         m_Exception = ex;
      }
   }
}