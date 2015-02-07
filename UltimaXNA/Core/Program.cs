using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA
{
    class Program
    {
        #region EntryPoint
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
        #endregion
    }
}
