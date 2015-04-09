using System;
using System.Diagnostics.Tracing;
using UltimaXNA;

namespace UltimaXNA.Diagnostics.Tracing.Listeners
{
    public class ConsoleOutputEventListener : EventListener
    {
        public ConsoleOutputEventListener()
        {

        }

        protected override void OnEventWritten(EventWrittenEventArgs e)
        {
            ConsoleColor color = ConsoleColor.Gray;

            switch (e.Level)
            {
                case EventLevel.Informational:
                    color = ConsoleColor.White;
                    break;
                case EventLevel.Warning:
                    color = ConsoleColor.Yellow;
                    break;
                case EventLevel.Error:
                case EventLevel.Critical:
                    color = ConsoleColor.Red;
                    break;
            }

            ConsoleManager.PushColor(color);
            Console.WriteLine(e.Payload[0]);
            ConsoleManager.PopColor();
        }
    }
}