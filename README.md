# **C**ompact **L**og **E**vent **F**ormat Tool [![Build status](https://ci.appveyor.com/api/projects/status/ybr08j4h302yaw09?svg=true)](https://ci.appveyor.com/project/datalust/clef-tool) [![Download](https://img.shields.io/badge/download-releases-blue.svg)](https://github.com/datalust/clef-tool/releases)

The `clef` command-line tool reads and processes the newline-delimited JSON streams produced by [_Serilog.Formatting.Compact_](https://github.com/serilog/serilog-formatting-compact) and other sources.

### What does CLEF look like?

[CLEF](https://clef-json.org) is a very simple, compact JSON event format with standardized fields for timestamps, messages, levels and so-on.

```json
{"@t":"2022-05-09T01:23:45.67890Z","@mt":"Starting up","MachineName":"web-53a889fe"}
```

### Getting started

[Binary releases](https://github.com/datalust/clef-tool/releases) can be downloaded directly from this project.

Or, if you have `dotnet` installed, `clef` can be installed as a global tool using:

```
dotnet tool install --global Datalust.ClefTool
```

And run with:

```
dotnet clef --help
```

### Reading CLEF files

The default action, given a CLEF file, will be to pretty-print it in text format to the console.

```
> clef -i log-20220509.clef
[2022-05-09T01:23:45.67890Z INF] Starting up
[2022-05-09T01:23:45.96950Z INF] Checking for updates to version 123.4
...
```

The tool also accepts events on STDIN:

```
> cat log-20220509.clef | clef
...
```

### Filtering

Expressions using the [_Serilog.Expressions_](https://github.com/serilog/serilog-expressions) syntax can be specified to filter the stream:

```
> clef -i log-20220509.clef --filter="Version > 100"
[2022-05-09T01:23:45.96950Z INF] Checking for updates to version 123.4
```

### Formats

Output will be plain text unless another format is specified.

Write the output in JSON format using `--format-json`:

```
> clef -i log-20220509.clef --format-json
{"@t":"2022-05-09T01:23:45.67890Z","@mt":"Starting up"}
...
```

Control the output text format using `--format-template`:

```
> clef -i log-20220509.clef --format-template="{@m}`n"
Starting up
...
```

Escaping of embedded newlines is shell-dependent; PowerShell <code>`n</code> syntax is shown.

### Outputs

Output will be written to STDOUT unless another destination is specified.

Write output to a file with `-o`:

```
> clef -i log-20220509.clef -o log-20220509.txt
```

Send the output to [Seq](https://getseq.net) by specifying a server URL and optional API key:

```
> clef -i log-20220509.clef --out-seq="https://seq.example.com" --out-seq-apikey="1234567890"
```

### Enrichment

Events can be enriched with additional properties by specifying them using the `-p` switch:

```
> clef -i log-20220509.clef -p CustomerId=C123 -p Environment=Support [...]
```
