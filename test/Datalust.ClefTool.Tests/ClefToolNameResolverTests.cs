using System;
using System.IO;
using System.Linq;
using Datalust.ClefTool.Syntax;
using Serilog.Events;
using Serilog.Templates;
using Xunit;

namespace Datalust.ClefTool.Tests;

public class ClefToolNameResolverTests
{
    [Fact]
    public void NewlineFunctionEvaluatesToNewlineInTemplates()
    {
        var template = new ExpressionTemplate("a{newline()}b", nameResolver: new ClefToolNameResolver());
        var output = new StringWriter();
        var evt = new LogEvent(DateTimeOffset.Now, LogEventLevel.Debug, null, MessageTemplate.Empty,
            Enumerable.Empty<LogEventProperty>());
        template.Format(evt, output);
        var result = output.ToString();
        Assert.Equal($"a{Environment.NewLine}b", result);
    }
}

