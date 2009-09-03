﻿/*
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

namespace UltimaXNA.Scripting
{
    /// <summary>
    /// Base class to provide consistent disposal flow across lua objects. Uses code provided by Yves Duhoux and suggestions by Hans Schmeidenbacher and Qingrui Li 
    /// </summary>
    public abstract class LuaBase
    {
        private bool _Disposed;
        protected int _Reference;
        protected Lua _Interpreter;

        ~LuaBase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposeManagedResources)
        {
            if (!_Disposed)
            {
                if (disposeManagedResources)
                {
                    if (_Reference != 0)
                        _Interpreter.dispose(_Reference);
                }

                _Disposed = true;
            }
        }

        public override bool Equals(object o)
        {
            if (o is LuaBase)
            {
                LuaBase l = (LuaBase)o;
                return _Interpreter.compareRef(l._Reference, _Reference);
            }
            else return false;
        }

        public override int GetHashCode()
        {
            return _Reference;
        }
    }
}
