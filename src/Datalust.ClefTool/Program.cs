using Autofac;
using Datalust.ClefTool.Cli;
using Datalust.ClefTool.Cli.Commands;

namespace Datalust.ClefTool
{
    static class Program
    {
        public static int Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<CommandLineHost>();
            builder.RegisterTypes(typeof(PipeCommand), typeof(HelpCommand), typeof(VersionCommand))
                .As<Command>()
                .WithMetadataFrom<CommandAttribute>();

            using var container = builder.Build();
            var clh = container.Resolve<CommandLineHost>();
            return clh.Run(args);
        }
    }
}
