using System;
using UltimaXNA.Diagnostics.Tracing;
using UltimaXNA.Core.Diagnostics;

namespace UltimaXNA.Core
{
    class EntryPoint
    {
        [STAThread]
        static void Main(string[] args)
        {
            new ApplicationBootstrapper().Initialize();
        }
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Tracer.Critical((Exception)e.ExceptionObject);
        }
    }
}
