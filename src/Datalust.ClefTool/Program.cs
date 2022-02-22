using System.Reflection;
using Autofac;
using Datalust.ClefTool.Cli;

namespace Datalust.ClefTool
{
    class Program
    {
        public static int Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<CommandLineHost>();
            builder.RegisterAssemblyTypes(typeof(Program).GetTypeInfo().Assembly)
                .As<Command>()
                .WithMetadataFrom<CommandAttribute>();

            using var container = builder.Build();
            var clh = container.Resolve<CommandLineHost>();
            return clh.Run(args);
        }
    }
}
