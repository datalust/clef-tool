using System;
using System.IO;
using Datalust.ClefTool.Pipe;
using Datalust.ClefTool.Tests.Support;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact.Reader;
using Xunit;

namespace Datalust.ClefTool.Tests.Pipe
{
    public class EventPipeTests
    {
        static readonly string ClefThreeEvents =
            "{\"@t\":\"2017-04-20T04:24:47.0251719Z\",\"@mt\":\"Loop {Counter}\",\"Counter\":0}" + Environment.NewLine +
            "{\"@t\":\"2017-04-20T04:24:47.0371689Z\",\"@mt\":\"Loop {Counter}\",\"Counter\":1}" + Environment.NewLine +
            "{\"@t\":\"2017-04-20T04:24:47.0371689Z\",\"@mt\":\"Loop {Counter}\",\"Counter\":2}" + Environment.NewLine;

        static readonly string ClefTwoValidOneInvalid =
            "{\"@t\":\"2017-04-20T04:24:47.0251719Z\",\"@mt\":\"Loop {Counter}\",\"Counter\":0}" + Environment.NewLine +
            "Hello, world!" + Environment.NewLine +
            "{\"@t\":\"2017-04-20T04:24:47.0371689Z\",\"@mt\":\"Loop {Counter}\",\"Counter\":2}" + Environment.NewLine;

        [Fact]
        public void EventsAreCopiedFromSourceToDestination()
        {
            var output = PipeEvents(ClefThreeEvents, InvalidDataHandling.Fail);
            Assert.Equal(3, output.Length);
        }

        [Fact]
        public void InFailModeInvalidJsonThrows()
        {
            Assert.Throws<JsonReaderException>(() => PipeEvents(ClefTwoValidOneInvalid, InvalidDataHandling.Fail));
        }

        [Fact]
        public void InIgnoreModeInvalidJsonIsDropped()
        {
            var output = PipeEvents(ClefTwoValidOneInvalid, InvalidDataHandling.Ignore);
            Assert.Equal(2, output.Length);
        }

        [Fact]
        public void InReportModeInvalidJsonIsReported()
        {
            var output = PipeEvents(ClefTwoValidOneInvalid, InvalidDataHandling.Report);
            Assert.Equal(3, output.Length);
        }

        static LogEvent[] PipeEvents(string input, InvalidDataHandling invalidDataHandling)
        {
            var output = new CollectingSink();
            using (var source = new LogEventReader(new StringReader(input)))
            using (var destination = new LoggerConfiguration()
                .MinimumLevel.Is(LevelAlias.Minimum)
                .WriteTo.Sink(output)
                .CreateLogger())
            {
                EventPipe.PipeEvents(source, destination, invalidDataHandling);
            }

            return output.Events;
        }
    }
}
