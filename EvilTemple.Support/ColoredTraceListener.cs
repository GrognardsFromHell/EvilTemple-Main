using System;
using System.Diagnostics;

namespace EvilTemple.Support
{
    public class ColoredTraceListener : ConsoleTraceListener
    {
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            WriteEventType(eventType);
            WriteLine(data);
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
        {
            WriteEventType(eventType);
            WriteLine(string.Join(" ", data));
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            WriteEventType(eventType);
            WriteLine(id);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            WriteEventType(eventType);
            WriteLine(message);
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            WriteEventType(eventType);
            WriteLine(string.Format(format, args));
        }

        public override void TraceTransfer(TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId)
        {
            WriteLine(message);
        }

        private void WriteEventType(TraceEventType eventType)
        {
            ConsoleColor? color = null;

            var text = eventType.ToString();

            switch (eventType)
            {
                case TraceEventType.Critical:
                    color = ConsoleColor.DarkRed;
                    break;
                case TraceEventType.Error:
                    color = ConsoleColor.DarkRed;
                    break;
                case TraceEventType.Warning:
                    color = ConsoleColor.Yellow;
                    break;
                case TraceEventType.Information:
                    color = ConsoleColor.White;
                    text = "Debug";
                    break;
                case TraceEventType.Verbose:
                    color = ConsoleColor.White;
                    break;
                default:
                    break;
            }

            if (color.HasValue)
                Console.ForegroundColor = color.Value;

            Write("[" + text + "] ");

            if (color.HasValue)
                Console.ResetColor();
        }
    }
}