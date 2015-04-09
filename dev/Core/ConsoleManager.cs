#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

#endregion

namespace UltimaXNA.Core
{
    [SuppressUnmanagedCodeSecurity]
    internal static class ConsoleManager
    {
        private const string Kernel32_DllName = "kernel32.dll";
        private static readonly Stack<ConsoleColor> m_consoleColors = new Stack<ConsoleColor>();

        public static bool HasConsole
        {
            get { return GetConsoleWindow() != IntPtr.Zero; }
        }

        public static void PushColor(ConsoleColor color)
        {
            try
            {
                m_consoleColors.Push(Console.ForegroundColor);
                Console.ForegroundColor = color;
            }
            catch
            {
            }
        }

        public static ConsoleColor PopColor()
        {
            try
            {
                Console.ForegroundColor = m_consoleColors.Pop();
            }
            catch
            {
            }

            return Console.ForegroundColor;
        }

        [DllImport(Kernel32_DllName)]
        private static extern bool AllocConsole();

        [DllImport(Kernel32_DllName)]
        private static extern bool FreeConsole();

        [DllImport(Kernel32_DllName)]
        private static extern IntPtr GetConsoleWindow();

        public static void Show()
        {
            if(!HasConsole)
            {
                AllocConsole();
                InvalidateOutAndError();
            }
        }

        public static void Hide()
        {
            if(HasConsole)
            {
                SetOutAndErrorNull();
                FreeConsole();
            }
        }

        public static void Toggle()
        {
            if(HasConsole)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        private static void InvalidateOutAndError()
        {
            Type type = typeof(Console);

            _FieldInfo output = type.GetField("_out",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            _FieldInfo error = type.GetField("_error",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            _MethodInfo initializeStdOutError = type.GetMethod("InitializeStdOutError",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            Debug.Assert(output != null);
            Debug.Assert(error != null);
            Debug.Assert(initializeStdOutError != null);

            output.SetValue(null, null);
            error.SetValue(null, null);
            initializeStdOutError.Invoke(null, new object[] {true});
        }

        private static void SetOutAndErrorNull()
        {
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);
        }
    }
}