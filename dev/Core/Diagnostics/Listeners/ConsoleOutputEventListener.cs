using System;

namespace UltimaXNA.Core.Diagnostics.Listeners
{
    public class ConsoleOutputEventListener : AEventListener
    {
        public ConsoleOutputEventListener()
        {

        }

        public override void OnEventWritten(EventLevel level, string message)
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
            Console.WriteLine(message);
            ConsoleManager.PopColor();
        }
    }
}