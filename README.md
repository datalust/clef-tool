# **C**ompact **L**og **E**vent **F**ormat Tool [![Build status](https://ci.appveyor.com/api/projects/status/ybr08j4h302yaw09?svg=true)](https://ci.appveyor.com/project/datalust/clef-tool) [![Download](https://img.shields.io/badge/download-releases-blue.svg)](https://github.com/datalust/clef-tool/releases)

The `clef` command-line tool reads and processes the newline-delimited JSON streams produced by [_Serilog.Formatting.Compact_](https://github.com/serilog/serilog-formatting-compact) and other sources.

### What does CLEF look like?

CLEF is a very simple, compact JSON event format with standardized fields for timestamps, messages, levels and so-on.

```json
{"@t":"2017-05-09T01:23:45.67890Z","@mt":"Starting up","MachineName":"web-53a889fe"}
```

### Reading CLEF files

The default action, given a CLEF file, will be to pretty-print it in text format to the console.

```
> clef -i log-20170509.clef
[2017-05-09T01:23:45.67890Z INF] Starting up
[2017-05-09T01:23:45.96950Z INF] Checking for updates to version 123.4
...
```

The tool also accepts events on STDIN:

```
> cat log-20170509.clef | clef
...
```

### Filtering

Expressions using the [_Serilog.Filters.Expressions_](https://github.com/serilog/serilog-filters-expressions) syntax can be specified to filter the stream:

```
> clef -i log-20170509.clef --filter="Version > 100"
[2017-05-09T01:23:45.96950Z INF] Checking for updates to version 123.4
```

### Formats

Output will be plain text unless another format is specified.

Write the output in JSON format using `--format-json`:

```
> clef -i log-20170509.clef --format-json
{"@t":"2017-05-09T01:23:45.67890Z","@mt":"Starting up"}
...
```

Control the output text format using `--format-template`:

```
> clef -i log-20170509.clef --format-template="{Message}{NewLine}"
Starting up
...
```

### Outputs

Output will be written to STDOUT unless another destination is specified.

Write output to a file with `-o`:

```
> clef -i log-20170509.clef -o log-20170509.txt
```

Send the output to [Seq](https://getseq.net) by specifying a server URL and optional API key:

```
> clef -i log-20170509.clef --out-seq="https://seq.example.com" --out-seq-apikey="1234567890"
```

### Enrichment

Events can be enriched with additional properties by specifying them using the `-p` switch:

```
> clef -i log-20170509.clef -p CustomerId=C123 -p Environment=Support [...]
```

