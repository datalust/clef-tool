using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Serilog.Events;
using Serilog.Expressions;

namespace Datalust.ClefTool.Syntax;

public class ClefToolNameResolver : NameResolver
{
    public static LogEventPropertyValue? NewLine()
    {
        return new ScalarValue(Environment.NewLine);
    }

    public override bool TryResolveFunctionName(string name, [NotNullWhen(true)] out MethodInfo? implementation)
    {
        if (nameof(NewLine).Equals(name, StringComparison.OrdinalIgnoreCase))
        {
            implementation = typeof(ClefToolNameResolver).GetMethod(nameof(NewLine), BindingFlags.Public | BindingFlags.Static)!;
            return true;
        }

        return base.TryResolveFunctionName(name, out implementation);
    }
}
