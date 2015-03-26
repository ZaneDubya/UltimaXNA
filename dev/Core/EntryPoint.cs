using System;

namespace UltimaXNA.Core
{
    class EntryPoint
    {
        [STAThread]
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            using (UltimaEngine engine = new UltimaEngine(800, 600))
            {
                engine.Run();
            }
        }
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Diagnostics.Logger.Fatal(e.ExceptionObject);
        }
    }
}
