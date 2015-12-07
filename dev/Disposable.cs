using System;
using UltimaXNA.Core.Diagnostics;

namespace UltimaXNA
{
    public static class Disposable
    {
        public static IDisposable Create(Action action)
        {
            return new ActionDisposable(action);
        }

        private class ActionDisposable : IDisposable
        {
            private readonly Action _action;

            public ActionDisposable(Action action)
            {
                Guard.RequireIsNotNull(action, "action");
                _action = action;
            }

            public void Dispose()
            {
                _action();
            }
        }
    }
}