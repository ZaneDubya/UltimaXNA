using System;

namespace UltimaXNA.Core.Diagnostics.Tracing.Listeners
{
    public class ConsoleOutputEventListener : AEventListener
    {
        public ConsoleOutputEventListener()
        {

        }

        public override void OnEventWritten(EventLevel level, string messag)
        {
            ConsoleColor color = ConsoleColor.Gray;

            switch (level)
            {
                case EventLevel.Info:
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
            Console.WriteLine(messag);
            ConsoleManager.PopColor();
        }
    }
}