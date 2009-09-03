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
    using System.Reflection;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Lua511;

    /*
     * Cached method
     */
    struct MethodCache
    {
        public MethodBase cachedMethod;
        // List or arguments
        public object[] args;
        // Positions of out parameters
        public int[] outList;
        // Types of parameters
        public MethodArgs[] argTypes;
    }

    /*
     * Parameter information
     */
    struct MethodArgs
    {
        // Position of parameter
        public int index;
        // Type-conversion function
        public ExtractValue extractValue;
    }

    /*
     * Argument extraction with type-conversion function
     */
    delegate object ExtractValue(IntPtr luaState, int stackPos);

    /*
     * Wrapper class for methods/constructors accessed from Lua.
     * 
     * Author: Fabio Mascarenhas
     * Version: 1.0
     */
    class LuaMethodWrapper
    {
        private ObjectTranslator _Translator;
        private MethodBase _Method;
        private MethodCache _LastCalledMethod = new MethodCache();
        private string _MethodName;
        private MemberInfo[] _Members;
        private IReflect _TargetType;
        private ExtractValue _ExtractTarget;
        private object _Target;
        private BindingFlags _BindingType;

        /*
         * Constructs the wrapper for a known MethodBase instance
         */
        public LuaMethodWrapper(ObjectTranslator translator, object target, IReflect targetType, MethodBase method)
        {
            _Translator = translator;
            _Target = target;
            _TargetType = targetType;
            if (targetType != null)
                _ExtractTarget = translator.typeChecker.getExtractor(targetType);
            _Method = method;
            _MethodName = method.Name;

            if (method.IsStatic)
            { _BindingType = BindingFlags.Static; }
            else
            { _BindingType = BindingFlags.Instance; }
        }
        /*
         * Constructs the wrapper for a known method name
         */
        public LuaMethodWrapper(ObjectTranslator translator, IReflect targetType, string methodName, BindingFlags bindingType)
        {
            _Translator = translator;
            _MethodName = methodName;
            _TargetType = targetType;
            
            if (targetType != null)
                _ExtractTarget = translator.typeChecker.getExtractor(targetType);

            _BindingType = bindingType;
            
            //CP: Removed NonPublic binding search and added IgnoreCase
            _Members = targetType.UnderlyingSystemType.GetMember(methodName, MemberTypes.Method, bindingType | BindingFlags.Public | BindingFlags.IgnoreCase/*|BindingFlags.NonPublic*/);
        }


        /// <summary>
        /// Convert C# exceptions into Lua errors
        /// </summary>
        /// <returns>num of things on stack</returns>
        /// <param name="e">null for no pending exception</param>
        int SetPendingException(Exception e)
        {
            return _Translator.interpreter.SetPendingException(e);
        }


        /*
         * Calls the method. Receives the arguments from the Lua stack
         * and returns values in it.
         */
        public int call(IntPtr luaState)
        {
            MethodBase methodToCall = _Method;
            object targetObject = _Target;
            bool failedCall = true;
            int nReturnValues = 0;

            if (!LuaDLL.lua_checkstack(luaState, 5))
                throw new LuaException("Lua stack overflow");

            bool isStatic = (_BindingType & BindingFlags.Static) == BindingFlags.Static;

            SetPendingException(null);

            if (methodToCall == null) // Method from name
            {
                if (isStatic)
                    targetObject = null;
                else
                    targetObject = _ExtractTarget(luaState, 1);

                //LuaDLL.lua_remove(luaState,1); // Pops the receiver
                if (_LastCalledMethod.cachedMethod != null) // Cached?
                {
                    int numStackToSkip = isStatic ? 0 : 1; // If this is an instance invoe we will have an extra arg on the stack for the targetObject
                    int numArgsPassed = LuaDLL.lua_gettop(luaState) - numStackToSkip;

                    if (numArgsPassed == _LastCalledMethod.argTypes.Length) // No. of args match?
                    {
                        if (!LuaDLL.lua_checkstack(luaState, _LastCalledMethod.outList.Length + 6))
                            throw new LuaException("Lua stack overflow");

                        try
                        {
                            for (int i = 0; i < _LastCalledMethod.argTypes.Length; i++)
                            {
                                _LastCalledMethod.args[_LastCalledMethod.argTypes[i].index] =
                                    _LastCalledMethod.argTypes[i].extractValue(luaState, i + 1 + numStackToSkip);

                                if (_LastCalledMethod.args[_LastCalledMethod.argTypes[i].index] == null &&
                                    !LuaDLL.lua_isnil(luaState, i + 1 + numStackToSkip))
                                {
                                    throw new LuaException("argument number " + (i + 1) + " is invalid");
                                }
                            }
                            if ((_BindingType & BindingFlags.Static) == BindingFlags.Static)
                            {
                                _Translator.push(luaState, _LastCalledMethod.cachedMethod.Invoke(null, _LastCalledMethod.args));
                            }
                            else
                            {
                                if (_LastCalledMethod.cachedMethod.IsConstructor)
                                    _Translator.push(luaState, ((ConstructorInfo)_LastCalledMethod.cachedMethod).Invoke(_LastCalledMethod.args));
                                else
                                    _Translator.push(luaState, _LastCalledMethod.cachedMethod.Invoke(targetObject, _LastCalledMethod.args));
                            }
                            failedCall = false;
                        }
                        catch (TargetInvocationException e)
                        {
                            // Failure of method invocation
                            return SetPendingException(e.GetBaseException());
                        }
                        catch (Exception e)
                        {
                            if (_Members.Length == 1) // Is the method overloaded?
                                // No, throw error
                                return SetPendingException(e);
                        }
                    }
                }

                // Cache miss
                if (failedCall)
                {
                    // System.Diagnostics.Debug.WriteLine("cache miss on " + methodName);

                    // If we are running an instance variable, we can now pop the targetObject from the stack
                    if (!isStatic)
                    {
                        if (targetObject == null)
                        {
                            _Translator.throwError(luaState, String.Format("instance method '{0}' requires a non null target object", _MethodName));
                            LuaDLL.lua_pushnil(luaState);
                            return 1;
                        }

                        LuaDLL.lua_remove(luaState, 1); // Pops the receiver
                    }

                    bool hasMatch = false;
                    string candidateName = null;

                    foreach (MemberInfo member in _Members)
                    {
                        candidateName = member.ReflectedType.Name + "." + member.Name;

                        MethodBase m = (MethodInfo)member;

                        bool isMethod = _Translator.matchParameters(luaState, m, ref _LastCalledMethod);
                        if (isMethod)
                        {
                            hasMatch = true;
                            break;
                        }
                    }
                    if (!hasMatch)
                    {
                        string msg = (candidateName == null)
                            ? "invalid arguments to method call"
                            : ("invalid arguments to method: " + candidateName);

                        _Translator.throwError(luaState, msg);
                        LuaDLL.lua_pushnil(luaState);
                        return 1;
                    }
                }
            }
            else // Method from MethodBase instance 
            {
                if (methodToCall.ContainsGenericParameters)
                {
                    bool isMethod = _Translator.matchParameters(luaState, methodToCall, ref _LastCalledMethod);

                    if (methodToCall.IsGenericMethodDefinition)
                    {
                        //need to make a concrete type of the generic method definition
                        List<Type> typeArgs = new List<Type>();

                        foreach (object arg in _LastCalledMethod.args)
                            typeArgs.Add(arg.GetType());
                    
                        MethodInfo concreteMethod = (methodToCall as MethodInfo).MakeGenericMethod(typeArgs.ToArray());
                        
                        _Translator.push(luaState, concreteMethod.Invoke(targetObject, _LastCalledMethod.args));
                        failedCall = false;
                    }
                    else if(methodToCall.ContainsGenericParameters)
                    {
                        _Translator.throwError(luaState, "unable to invoke method on generic class as the current method is an open generic method");
                        LuaDLL.lua_pushnil(luaState);
                        return 1;
                    }
                }
                else
                {
                    if (!methodToCall.IsStatic && !methodToCall.IsConstructor && targetObject == null)
                    {
                        targetObject = _ExtractTarget(luaState, 1);
                        LuaDLL.lua_remove(luaState, 1); // Pops the receiver
                    }

                    if (!_Translator.matchParameters(luaState, methodToCall, ref _LastCalledMethod))
                    {
                        _Translator.throwError(luaState, "invalid arguments to method call");
                        LuaDLL.lua_pushnil(luaState);
                        return 1;
                    }
                }
            }

            if (failedCall)
            {
                if (!LuaDLL.lua_checkstack(luaState, _LastCalledMethod.outList.Length + 6))
                    throw new LuaException("Lua stack overflow");
                try
                {
                    if (isStatic)
                    {
                        _Translator.push(luaState, _LastCalledMethod.cachedMethod.Invoke(null, _LastCalledMethod.args));
                    }
                    else
                    {
                        if (_LastCalledMethod.cachedMethod.IsConstructor)
                            _Translator.push(luaState, ((ConstructorInfo)_LastCalledMethod.cachedMethod).Invoke(_LastCalledMethod.args));
                        else
                            _Translator.push(luaState, _LastCalledMethod.cachedMethod.Invoke(targetObject, _LastCalledMethod.args));
                    }
                }
                catch (TargetInvocationException e)
                {
                    return SetPendingException(e.GetBaseException());
                }
                catch (Exception e)
                {
                    return SetPendingException(e);
                }
            }

            // Pushes out and ref return values
            for (int index = 0; index < _LastCalledMethod.outList.Length; index++)
            {
                nReturnValues++;
                //for(int i=0;i<lastCalledMethod.outList.Length;i++)
                _Translator.push(luaState, _LastCalledMethod.args[_LastCalledMethod.outList[index]]);
            }
            return nReturnValues < 1 ? 1 : nReturnValues;
        }
    }




    /// <summary>
    /// We keep track of what delegates we have auto attached to an event - to allow us to cleanly exit a LuaInterface session
    /// </summary>
    class EventHandlerContainer : IDisposable
    {
        Dictionary<Delegate, RegisterEventHandler> dict = new Dictionary<Delegate, RegisterEventHandler>();

        public void Add(Delegate handler, RegisterEventHandler eventInfo)
        {
            dict.Add(handler, eventInfo);
        }

        public void Remove(Delegate handler)
        {
            bool found = dict.Remove(handler);
            Debug.Assert(found);
        }

        /// <summary>
        /// Remove any still registered handlers
        /// </summary>
        public void Dispose()
        {
            foreach (KeyValuePair<Delegate, RegisterEventHandler> pair in dict)
            {
                pair.Value.RemovePending(pair.Key);
            }

            dict.Clear();
        }
    }


    /*
     * Wrapper class for events that does registration/deregistration
     * of event handlers.
     * 
     * Author: Fabio Mascarenhas
     * Version: 1.0
     */
    class RegisterEventHandler
    {
        object target;
        EventInfo eventInfo;
        EventHandlerContainer pendingEvents;

        public RegisterEventHandler(EventHandlerContainer pendingEvents, object target, EventInfo eventInfo)
        {
            this.target = target;
            this.eventInfo = eventInfo;
            this.pendingEvents = pendingEvents;
        }


        /*
         * Adds a new event handler
         */
        public Delegate Add(LuaFunction function)
        {
            //CP: Fix by Ben Bryant for event handling with one parameter
            //link: http://luaforge.net/forum/message.php?msg_id=9266
            Delegate handlerDelegate = CodeGeneration.Instance.GetDelegate(eventInfo.EventHandlerType, function);
            eventInfo.AddEventHandler(target, handlerDelegate);
            pendingEvents.Add(handlerDelegate, this);

            return handlerDelegate;


            //MethodInfo mi = eventInfo.EventHandlerType.GetMethod("Invoke");
            //ParameterInfo[] pi = mi.GetParameters();
            //LuaEventHandler handler=CodeGeneration.Instance.GetEvent(pi[1].ParameterType,function);

            //Delegate handlerDelegate=Delegate.CreateDelegate(eventInfo.EventHandlerType,handler,"HandleEvent");
            //eventInfo.AddEventHandler(target,handlerDelegate);
            //pendingEvents.Add(handlerDelegate, this);

            //return handlerDelegate;
        }

        /*
         * Removes an existing event handler
         */
        public void Remove(Delegate handlerDelegate)
        {
            RemovePending(handlerDelegate);
            pendingEvents.Remove(handlerDelegate);
        }

        /*
         * Removes an existing event handler (without updating the pending handlers list)
         */
        internal void RemovePending(Delegate handlerDelegate)
        {
            eventInfo.RemoveEventHandler(target, handlerDelegate);
        }
    }

    /*
     * Base wrapper class for Lua function event handlers.
     * Subclasses that do actual event handling are created
     * at runtime.
     * 
     * Author: Fabio Mascarenhas
     * Version: 1.0
     */
    public class LuaEventHandler
    {
        public LuaFunction handler = null;

        // CP: Fix provided by Ben Bryant for delegates with one param
        // link: http://luaforge.net/forum/message.php?msg_id=9318
        public void handleEvent(object[] args)
        {
            handler.Call(args);
        }
        //public void handleEvent(object sender,object data) 
        //{
        //    handler.call(new object[] { sender,data },new Type[0]);
        //}
    }

    /*
     * Wrapper class for Lua functions as delegates
     * Subclasses with correct signatures are created
     * at runtime.
     * 
     * Author: Fabio Mascarenhas
     * Version: 1.0
     */
    public class LuaDelegate
    {
        public Type[] returnTypes;
        public LuaFunction function;
        public LuaDelegate()
        {
            function = null;
            returnTypes = null;
        }
        public object callFunction(object[] args, object[] inArgs, int[] outArgs)
        {
            // args is the return array of arguments, inArgs is the actual array
            // of arguments passed to the function (with in parameters only), outArgs
            // has the positions of out parameters
            object returnValue;
            int iRefArgs;
            object[] returnValues = function.call(inArgs, returnTypes);
            if (returnTypes[0] == typeof(void))
            {
                returnValue = null;
                iRefArgs = 0;
            }
            else
            {
                returnValue = returnValues[0];
                iRefArgs = 1;
            }
            // Sets the value of out and ref parameters (from
            // the values returned by the Lua function).
            for (int i = 0; i < outArgs.Length; i++)
            {
                args[outArgs[i]] = returnValues[iRefArgs];
                iRefArgs++;
            }
            return returnValue;
        }
    }

    /*
     * Static helper methods for Lua tables acting as CLR objects.
     * 
     * Author: Fabio Mascarenhas
     * Version: 1.0
     */
    public class LuaClassHelper
    {
        /*
         *  Gets the function called name from the provided table,
         * returning null if it does not exist
         */
        public static LuaFunction getTableFunction(LuaTable luaTable, string name)
        {
            object funcObj = luaTable.rawget(name);
            if (funcObj is LuaFunction)
                return (LuaFunction)funcObj;
            else
                return null;
        }
        /*
         * Calls the provided function with the provided parameters
         */
        public static object callFunction(LuaFunction function, object[] args, Type[] returnTypes, object[] inArgs, int[] outArgs)
        {
            // args is the return array of arguments, inArgs is the actual array
            // of arguments passed to the function (with in parameters only), outArgs
            // has the positions of out parameters
            object returnValue;
            int iRefArgs;
            object[] returnValues = function.call(inArgs, returnTypes);
            if (returnTypes[0] == typeof(void))
            {
                returnValue = null;
                iRefArgs = 0;
            }
            else
            {
                returnValue = returnValues[0];
                iRefArgs = 1;
            }
            for (int i = 0; i < outArgs.Length; i++)
            {
                args[outArgs[i]] = returnValues[iRefArgs];
                iRefArgs++;
            }
            return returnValue;
        }
    }
}
