using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;

namespace Datalust.ClefTool.Tests.Support
{
    public class CollectingSink : ILogEventSink
    {
        readonly object _sync = new object();
        readonly List<LogEvent> _events = new List<LogEvent>();

        public void Emit(LogEvent logEvent)
        {
            lock (_sync)
                _events.Add(logEvent);
        }

        public LogEvent[] Events
        {
            get
            {
                lock (_sync)
                    return _events.ToArray();
            }
        }
    }
}
