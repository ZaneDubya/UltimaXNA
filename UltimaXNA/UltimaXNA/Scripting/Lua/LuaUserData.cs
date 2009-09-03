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
    public class LuaUserData : LuaBase
    {
        //internal int _Reference;
        //private Lua _Interpreter;
        public LuaUserData(int reference, Lua interpreter)
        {
            _Reference = reference;
            _Interpreter = interpreter;
        }
        //~LuaUserData()
        //{
        //    if (_Reference != 0)
        //        _Interpreter.dispose(_Reference);
        //}
        /*
         * Indexer for string fields of the userdata
         */
        public object this[string field]
        {
            get
            {
                return _Interpreter.getObject(_Reference, field);
            }
            set
            {
                _Interpreter.setObject(_Reference, field, value);
            }
        }
        /*
         * Indexer for numeric fields of the userdata
         */
        public object this[object field]
        {
            get
            {
                return _Interpreter.getObject(_Reference, field);
            }
            set
            {
                _Interpreter.setObject(_Reference, field, value);
            }
        }
        /*
         * Calls the userdata and returns its return values inside
         * an array
         */
        public object[] Call(params object[] args)
        {
            return _Interpreter.callFunction(this, args);
        }
        /*
         * Pushes the userdata into the Lua stack
         */
        internal void push(IntPtr luaState)
        {
            LuaDLL.lua_getref(luaState, _Reference);
        }
        public override string ToString()
        {
            return "userdata";
        }
        //public override bool Equals(object o)
        //{
        //    if (o is LuaUserData)
        //    {
        //        LuaUserData l = (LuaUserData)o;
        //        return _Interpreter.compareRef(l._Reference, _Reference);
        //    }
        //    else return false;
        //}
        //public override int GetHashCode()
        //{
        //    return _Reference;
        //}
    }
}
