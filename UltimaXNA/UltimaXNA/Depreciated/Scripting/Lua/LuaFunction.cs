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

using System;
using System.Collections.Generic;
using System.Text;
using Lua511;

namespace UltimaXNA.Scripting
{
    public class LuaFunction : LuaBase
    {
        //private Lua interpreter;
        internal LuaCSFunction function;
        //internal int reference;

        public LuaFunction(int reference, Lua interpreter)
        {
            _Reference = reference;
            this.function = null;
            _Interpreter = interpreter;
        }

        public LuaFunction(LuaCSFunction function, Lua interpreter)
        {
            _Reference = 0;
            this.function = function;
            _Interpreter = interpreter;
        }

        //~LuaFunction()
        //{
        //    if (reference != 0)
        //        interpreter.dispose(reference);
        //}

        //bool disposed = false;
        //~LuaFunction()
        //{
        //    Dispose(false);
        //}

        //public void Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}

        //public virtual void Dispose(bool disposeManagedResources)
        //{
        //    if (!this.disposed)
        //    {
        //        if (disposeManagedResources)
        //        {
        //            if (_Reference != 0) 
        //                _Interpreter.dispose(_Reference);
        //        }

        //        disposed = true;
        //    }
        //}


        /*
         * Calls the function casting return values to the types
         * in returnTypes
         */
        internal object[] call(object[] args, Type[] returnTypes)
        {
            return _Interpreter.callFunction(this, args, returnTypes);
        }
        /*
         * Calls the function and returns its return values inside
         * an array
         */
        public object[] Call(params object[] args)
        {
            return _Interpreter.callFunction(this, args);
        }
        /*
         * Pushes the function into the Lua stack
         */
        internal void push(IntPtr luaState)
        {
            if (_Reference != 0)
                LuaDLL.lua_getref(luaState, _Reference);
            else
                _Interpreter.pushCSFunction(function);
        }
        public override string ToString()
        {
            return "function";
        }
        public override bool Equals(object o)
        {
            if (o is LuaFunction)
            {
                LuaFunction l = (LuaFunction)o;
                if (this._Reference != 0 && l._Reference != 0)
                    return _Interpreter.compareRef(l._Reference, this._Reference);
                else
                    return this.function == l.function;
            }
            else return false;
        }
        public override int GetHashCode()
        {
            if (_Reference != 0)
                return _Reference;
            else
                return function.GetHashCode();
        }
    }

}
