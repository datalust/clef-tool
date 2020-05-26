using System;
using System.Globalization;
using System.IO;
using Datalust.ClefTool.Cli.Features;
using Datalust.ClefTool.Pipe;
using Serilog;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Compact.Reader;
using Serilog.Formatting.Display;

// ReSharper disable UnusedType.Global

namespace Datalust.ClefTool.Cli.Commands
{
    [Command("pipe", "Process CLEF-formatted events")]
    class PipeCommand : Command
    {
        readonly EnrichFeature _enrichFeature;
        readonly FileInputFeature _fileInputFeature;
        readonly FilterFeature _filterFeature;
        readonly JsonFormatFeature _jsonFormatFeature;
        readonly FileOutputFeature _fileOutputFeature;
        readonly TemplateFormatFeature _templateFormatFeature;
        readonly SeqOutputFeature _seqOutputFeature;
        readonly InvalidDataHandlingFeature _invalidDataHandlingFeature;

        const string DefaultOutputTemplate = "[{Timestamp:o} {Level:u3}] {Message} {Properties}{NewLine}{Exception}";

        public PipeCommand()
        {
            _fileInputFeature = Enable<FileInputFeature>();
            _fileOutputFeature = Enable<FileOutputFeature>();
            _enrichFeature = Enable<EnrichFeature>();
            _filterFeature = Enable<FilterFeature>();
            _jsonFormatFeature = Enable<JsonFormatFeature>();
            _templateFormatFeature = Enable<TemplateFormatFeature>();
            _seqOutputFeature = Enable<SeqOutputFeature>();
            _invalidDataHandlingFeature = Enable<InvalidDataHandlingFeature>();
        }

        protected override int Run()
        {
            try
            {
                var failed = false;
                SelfLog.Enable(m =>
                {
                    Console.Error.WriteLine(m);
                    failed = true;
                });

                var levelSwitch = new LoggingLevelSwitch(LevelAlias.Minimum);
                var configuration = new LoggerConfiguration()
                    .MinimumLevel.ControlledBy(levelSwitch);

                foreach (var property in _enrichFeature.Properties)
                {
                    configuration.Enrich.WithProperty(property.Key, property.Value);
                }

                if (_filterFeature.Filter != null)
                {
                    configuration.Filter.ByIncludingOnly(_filterFeature.Filter);
                }

                if (_seqOutputFeature.SeqUrl != null)
                {
                    configuration.WriteTo.Seq(
                        _seqOutputFeature.SeqUrl,
                        apiKey: _seqOutputFeature.SeqApiKey,
                        compact: true,
                        batchPostingLimit: _seqOutputFeature.BatchPostingLimit,
                        eventBodyLimitBytes: _seqOutputFeature.EventBodyLimitBytes,
                        controlLevelSwitch: levelSwitch);
                }
                else if (_jsonFormatFeature.UseJsonFormat)
                {
                    if (_fileOutputFeature.OutputFilename != null)
                    {
                        configuration.AuditTo.File(new CompactJsonFormatter(), _fileOutputFeature.OutputFilename);
                    }
                    else
                    {
                        configuration.WriteTo.Console(new CompactJsonFormatter());
                    }
                }
                else
                {
                    var template = _templateFormatFeature.OutputTemplate ?? DefaultOutputTemplate;
                    if (_fileOutputFeature.OutputFilename != null)
                    {
                        // This will differ slightly from the console output until `{Message:l}` becomes available
                        configuration.AuditTo.File(
                            new MessageTemplateTextFormatter(template, CultureInfo.InvariantCulture),
                            _fileOutputFeature.OutputFilename);
                    }
                    else
                    {
                        configuration.WriteTo.Console(outputTemplate: template);
                    }
                }

                using var logger = configuration.CreateLogger();
                using var inputFile = _fileInputFeature.InputFilename != null
                    ? new StreamReader(File.Open(_fileInputFeature.InputFilename, FileMode.Open, FileAccess.Read,
                        FileShare.ReadWrite))
                    : null;
                using var reader = new LogEventReader(inputFile ?? Console.In);
                EventPipe.PipeEvents(reader, logger, _invalidDataHandlingFeature.InvalidDataHandling);

                return failed ? 1 : 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return -1;
            }
        }
    }
}
