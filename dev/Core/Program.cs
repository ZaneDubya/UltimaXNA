using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Diagnostics;
using System.Windows.Forms;

namespace UltimaXNA.Core
{
    class Program
    {
        #region EntryPoint
        [STAThread]
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Logger.Debug("Waiting for resolution dialog");
            UltimaXNA.WinForms.ResolutionSet ResDial;
            ResDial = new UltimaXNA.WinForms.ResolutionSet();
            DialogResult dr = ResDial.ShowDialog();

            if (dr == DialogResult.Cancel)
            {
                ResDial.Close();
            }
            else if (dr == DialogResult.OK)
            {
                using (UltimaEngine engine = new UltimaEngine(ResDial.getWidth(), ResDial.getHeight()))
                {
                    ResDial.Close();
                    Logger.Debug("Starting engine with resolution {0} x {1}", ResDial.getWidth(), ResDial.getHeight());
                    engine.Run();
                }
            }
        }
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Diagnostics.Logger.Fatal(e.ExceptionObject);
        }
        #endregion
    }
}
