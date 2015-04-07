using System;

namespace CCC.Core.Diagnostics.Tracing
{
    public interface ITracer
    {
        void Critical(Exception ex);
        void Critical(Exception ex, string message, params object[] args);
        void Error(Exception ex);
        void Error(Exception ex, string message, params object[] args);
        void Warn(Exception ex);
        void Warn(Exception ex, string message, params object[] args);
        void Warn(string message, params object[] args);
        void Verbose(string message, params object[] args);
        void Info(string message, params object[] args);
    }
}