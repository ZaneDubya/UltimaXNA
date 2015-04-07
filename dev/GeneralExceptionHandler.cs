using System;
using UltimaXNA.Diagnostics.Tracing;

namespace UltimaXNA.Diagnostics
{
    public class GeneralExceptionHandler
    {
        private static GeneralExceptionHandler _instance;

        public static GeneralExceptionHandler Instance
        {
            get { return _instance ?? (_instance = new GeneralExceptionHandler()); }
            set { _instance = value; }
        }

        public void OnError(Exception e)
        {
            Tracer.Error(e);
            OnErrorOverride(e);
        }
        
        protected virtual void OnErrorOverride(Exception e)
        {
        }
    }
}