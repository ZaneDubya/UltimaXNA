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
using System.Reflection;
using Lua511;

namespace UltimaXNA.Scripting
{
	/*
	 * Type checking and conversion functions.
	 * 
	 * Author: Fabio Mascarenhas
	 * Version: 1.0
	 */
	class CheckType
	{
		private ObjectTranslator translator;

		ExtractValue extractNetObject;
		Dictionary<long, ExtractValue> extractValues = new Dictionary<long, ExtractValue>();
		
		public CheckType(ObjectTranslator translator) 
		{
            this.translator = translator;

            extractValues.Add(typeof(object).TypeHandle.Value.ToInt64(), new ExtractValue(getAsObject));
            extractValues.Add(typeof(sbyte).TypeHandle.Value.ToInt64(), new ExtractValue(getAsSbyte));
            extractValues.Add(typeof(byte).TypeHandle.Value.ToInt64(), new ExtractValue(getAsByte));
            extractValues.Add(typeof(short).TypeHandle.Value.ToInt64(), new ExtractValue(getAsShort));
            extractValues.Add(typeof(ushort).TypeHandle.Value.ToInt64(), new ExtractValue(getAsUshort));
            extractValues.Add(typeof(int).TypeHandle.Value.ToInt64(), new ExtractValue(getAsInt));
            extractValues.Add(typeof(uint).TypeHandle.Value.ToInt64(), new ExtractValue(getAsUint));
            extractValues.Add(typeof(long).TypeHandle.Value.ToInt64(), new ExtractValue(getAsLong));
            extractValues.Add(typeof(ulong).TypeHandle.Value.ToInt64(), new ExtractValue(getAsUlong));
            extractValues.Add(typeof(double).TypeHandle.Value.ToInt64(), new ExtractValue(getAsDouble));
            extractValues.Add(typeof(char).TypeHandle.Value.ToInt64(), new ExtractValue(getAsChar));
            extractValues.Add(typeof(float).TypeHandle.Value.ToInt64(), new ExtractValue(getAsFloat));
            extractValues.Add(typeof(decimal).TypeHandle.Value.ToInt64(), new ExtractValue(getAsDecimal));
            extractValues.Add(typeof(bool).TypeHandle.Value.ToInt64(), new ExtractValue(getAsBoolean));
            extractValues.Add(typeof(string).TypeHandle.Value.ToInt64(), new ExtractValue(getAsString));
            extractValues.Add(typeof(LuaFunction).TypeHandle.Value.ToInt64(), new ExtractValue(getAsFunction));
            extractValues.Add(typeof(LuaTable).TypeHandle.Value.ToInt64(), new ExtractValue(getAsTable));
            extractValues.Add(typeof(LuaUserData).TypeHandle.Value.ToInt64(), new ExtractValue(getAsUserdata));

            extractNetObject = new ExtractValue(getAsNetObject);		
		}

		/*
		 * Checks if the value at Lua stack index stackPos matches paramType,
		 * returning a conversion function if it does and null otherwise.
		 */
        internal ExtractValue getExtractor(IReflect paramType)
        {
            return getExtractor(paramType.UnderlyingSystemType);
        }
		internal ExtractValue getExtractor(Type paramType) 
		{
			if(paramType.IsByRef) paramType=paramType.GetElementType();
			
			long runtimeHandleValue = paramType.TypeHandle.Value.ToInt64();

            if(extractValues.ContainsKey(runtimeHandleValue))
	            return extractValues[runtimeHandleValue];
            else
				return extractNetObject;
		}

		internal ExtractValue checkType(IntPtr luaState,int stackPos,Type paramType) 
		{
            LuaTypes luatype = LuaDLL.lua_type(luaState, stackPos);

			if(paramType.IsByRef) paramType=paramType.GetElementType();

            Type underlyingType = Nullable.GetUnderlyingType(paramType);
            if (underlyingType != null)
            {
                paramType = underlyingType;     // Silently convert nullable types to their non null requics
            }

			long runtimeHandleValue = paramType.TypeHandle.Value.ToInt64();
			
			if (paramType.Equals(typeof(object)))
				return extractValues[runtimeHandleValue];            

            //CP: Added support for generic parameters
            if (paramType.IsGenericParameter)
            {
                if (luatype == LuaTypes.LUA_TBOOLEAN)
                    return extractValues[typeof(bool).TypeHandle.Value.ToInt64()];
                else if (luatype == LuaTypes.LUA_TSTRING)
                    return extractValues[typeof(string).TypeHandle.Value.ToInt64()];
                else if (luatype == LuaTypes.LUA_TTABLE)
                    return extractValues[typeof(LuaTable).TypeHandle.Value.ToInt64()];
                else if (luatype == LuaTypes.LUA_TUSERDATA)
                    return extractValues[typeof(object).TypeHandle.Value.ToInt64()];
                else if (luatype == LuaTypes.LUA_TFUNCTION)
                    return extractValues[typeof(LuaFunction).TypeHandle.Value.ToInt64()];
                else if (luatype == LuaTypes.LUA_TNUMBER)
                    return extractValues[typeof(double).TypeHandle.Value.ToInt64()];
                else
                {
                    ;//an unsupported type was encountered
                }
            }

            if (LuaDLL.lua_isnumber(luaState, stackPos))
                return extractValues[runtimeHandleValue];

            if (paramType == typeof(bool))
            {
                if (LuaDLL.lua_isboolean(luaState, stackPos))
					return extractValues[runtimeHandleValue];
            }
            else if (paramType == typeof(string))
            {
                if (LuaDLL.lua_isstring(luaState, stackPos))
					return extractValues[runtimeHandleValue];
                else if (luatype == LuaTypes.LUA_TNIL)
					return extractNetObject; // kevinh - silently convert nil to a null string pointer
            }
            else if (paramType == typeof(LuaTable))
            {
                if (luatype == LuaTypes.LUA_TTABLE)
					return extractValues[runtimeHandleValue];
            }
            else if (paramType == typeof(LuaUserData))
            {
                if (luatype == LuaTypes.LUA_TUSERDATA)
					return extractValues[runtimeHandleValue];
            }
            else if (paramType == typeof(LuaFunction))
            {
                if (luatype == LuaTypes.LUA_TFUNCTION)
					return extractValues[runtimeHandleValue];
            }
            else if (typeof(Delegate).IsAssignableFrom(paramType) && luatype == LuaTypes.LUA_TFUNCTION)
            {
                return new ExtractValue(new DelegateGenerator(translator, paramType).extractGenerated);
            }
            else if (paramType.IsInterface && luatype == LuaTypes.LUA_TTABLE)
            {
                return new ExtractValue(new ClassGenerator(translator, paramType).extractGenerated);
            }
            else if ((paramType.IsInterface || paramType.IsClass) && luatype == LuaTypes.LUA_TNIL)
            {
                // kevinh - allow nil to be silently converted to null - extractNetObject will return null when the item ain't found
                return extractNetObject;
            }
            else if (LuaDLL.lua_type(luaState, stackPos) == LuaTypes.LUA_TTABLE)
            {
                if (LuaDLL.luaL_getmetafield(luaState, stackPos, "__index"))
                {
                    object obj = translator.getNetObject(luaState, -1);
                    LuaDLL.lua_settop(luaState, -2);
                    if (obj != null && paramType.IsAssignableFrom(obj.GetType()))
						return extractNetObject;
                }
                else
					return null;
            }
            else
            {
                object obj = translator.getNetObject(luaState, stackPos);
                if (obj != null && paramType.IsAssignableFrom(obj.GetType()))
					return extractNetObject;
            }

            return null;
		}

		/*
		 * The following functions return the value in the Lua stack
		 * index stackPos as the desired type if it can, or null
		 * otherwise.
		 */
		private object getAsSbyte(IntPtr luaState,int stackPos) 
		{
			sbyte retVal=(sbyte)LuaDLL.lua_tonumber(luaState,stackPos);
			if(retVal==0 && !LuaDLL.lua_isnumber(luaState,stackPos)) return null;
			return retVal;
		}
		private object getAsByte(IntPtr luaState,int stackPos) 
		{
			byte retVal=(byte)LuaDLL.lua_tonumber(luaState,stackPos);
			if(retVal==0 && !LuaDLL.lua_isnumber(luaState,stackPos)) return null;
			return retVal;
		}
		private object getAsShort(IntPtr luaState,int stackPos) 
		{
			short retVal=(short)LuaDLL.lua_tonumber(luaState,stackPos);
			if(retVal==0 && !LuaDLL.lua_isnumber(luaState,stackPos)) return null;
			return retVal;
		}
		private object getAsUshort(IntPtr luaState,int stackPos) 
		{
			ushort retVal=(ushort)LuaDLL.lua_tonumber(luaState,stackPos);
			if(retVal==0 && !LuaDLL.lua_isnumber(luaState,stackPos)) return null;
			return retVal;
		}
		private object getAsInt(IntPtr luaState,int stackPos) 
		{
			int retVal=(int)LuaDLL.lua_tonumber(luaState,stackPos);
			if(retVal==0 && !LuaDLL.lua_isnumber(luaState,stackPos)) return null;
			return retVal;
		}
		private object getAsUint(IntPtr luaState,int stackPos) 
		{
			uint retVal=(uint)LuaDLL.lua_tonumber(luaState,stackPos);
			if(retVal==0 && !LuaDLL.lua_isnumber(luaState,stackPos)) return null;
			return retVal;
		}
		private object getAsLong(IntPtr luaState,int stackPos) 
		{
			long retVal=(long)LuaDLL.lua_tonumber(luaState,stackPos);
			if(retVal==0 && !LuaDLL.lua_isnumber(luaState,stackPos)) return null;
			return retVal;
		}
		private object getAsUlong(IntPtr luaState,int stackPos) 
		{
			ulong retVal=(ulong)LuaDLL.lua_tonumber(luaState,stackPos);
			if(retVal==0 && !LuaDLL.lua_isnumber(luaState,stackPos)) return null;
			return retVal;
		}
		private object getAsDouble(IntPtr luaState,int stackPos) 
		{
			double retVal=LuaDLL.lua_tonumber(luaState,stackPos);
			if(retVal==0 && !LuaDLL.lua_isnumber(luaState,stackPos)) return null;
			return retVal;
		}
		private object getAsChar(IntPtr luaState,int stackPos) 
		{
			char retVal=(char)LuaDLL.lua_tonumber(luaState,stackPos);
			if(retVal==0 && !LuaDLL.lua_isnumber(luaState,stackPos)) return null;
			return retVal;
		}
		private object getAsFloat(IntPtr luaState,int stackPos) 
		{
			float retVal=(float)LuaDLL.lua_tonumber(luaState,stackPos);
			if(retVal==0 && !LuaDLL.lua_isnumber(luaState,stackPos)) return null;
			return retVal;
		}
		private object getAsDecimal(IntPtr luaState,int stackPos) 
		{
			decimal retVal=(decimal)LuaDLL.lua_tonumber(luaState,stackPos);
			if(retVal==0 && !LuaDLL.lua_isnumber(luaState,stackPos)) return null;
			return retVal;
		}
		private object getAsBoolean(IntPtr luaState,int stackPos) 
		{
			return LuaDLL.lua_toboolean(luaState,stackPos);
		}
		private object getAsString(IntPtr luaState,int stackPos) 
		{
			string retVal=LuaDLL.lua_tostring(luaState,stackPos);
			if(retVal=="" && !LuaDLL.lua_isstring(luaState,stackPos)) return null;
			return retVal;
		}
		private object getAsTable(IntPtr luaState,int stackPos) 
		{
			return translator.getTable(luaState,stackPos);
		}
		private object getAsFunction(IntPtr luaState,int stackPos) 
		{
			return translator.getFunction(luaState,stackPos);
		}
		private object getAsUserdata(IntPtr luaState,int stackPos) 
		{
			return translator.getUserData(luaState,stackPos);
		}
		public object getAsObject(IntPtr luaState,int stackPos) 
		{
			if(LuaDLL.lua_type(luaState,stackPos)==LuaTypes.LUA_TTABLE) 
			{
				if(LuaDLL.luaL_getmetafield(luaState,stackPos,"__index")) 
				{
					if(LuaDLL.luaL_checkmetatable(luaState,-1)) 
					{
						LuaDLL.lua_insert(luaState,stackPos);
						LuaDLL.lua_remove(luaState,stackPos+1);
					} 
					else 
					{
						LuaDLL.lua_settop(luaState,-2);
					}
				}
			}
			object obj=translator.getObject(luaState,stackPos);
			return obj;
		}
		public object getAsNetObject(IntPtr luaState,int stackPos) 
		{
			object obj=translator.getNetObject(luaState,stackPos);
			if(obj==null && LuaDLL.lua_type(luaState,stackPos)==LuaTypes.LUA_TTABLE) 
			{
				if(LuaDLL.luaL_getmetafield(luaState,stackPos,"__index")) 
				{
					if(LuaDLL.luaL_checkmetatable(luaState,-1)) 
					{
						LuaDLL.lua_insert(luaState,stackPos);
						LuaDLL.lua_remove(luaState,stackPos+1);
						obj=translator.getNetObject(luaState,stackPos);
					} 
					else 
					{
						LuaDLL.lua_settop(luaState,-2);
					}
				}
			}
			return obj;
		}
	}
}
