# **C**ompact **L**og **E**vent **F**ormat Tool

The `clef` command-line tool reads and processes the newline-delimited JSON streams produced by [_Serilog.Formatting.Compact_](https://github.com/serilog/serilog-formatting-compact) and other sources.

### Reading CLEF files

The default action, given a CLEF file, will be to pretty-print it in text format to the console.

```shell
> clef -i log-20170509.clef
2017-05-09T01:23:45.67890Z [INF] Starting up
2017-05-09T01:23:45.96950Z [INF] Checking for updates to version 123.4
...
```

The tool also accepts events on STDIN:

```shell
> cat log-20170509.clef | clef
...
```

### Filtering

Expressions using the [_Serilog.Filters.Expressions_](https://github.com/serilog/serilog-filters-expressions) syntax can be specified to filter the stream:

```shell
> clef -i log-20170509.clef --filter="Version > 100"
2017-05-09T01:23:45.96950Z [INF] Checking for updates to version 123.4
```

### Formats

Output will be plain text unless another format is specified.

Write the output in CLEF format using `--format-clef`:

```shell
> clef -i log-20170509.clef --format-clef
{"@t":"2017-05-09T01:23:45.67890Z","@mt":"Starting up"}
...
```

Control the output text format using `--format-template`:

```shell
> clef -i log-20170509.clef --format-template="{Message}{NewLine}"
Starting up
...
```

### Outputs

Output will be written to STDOUT unless another destination is specified.

Write output to a file with `-o`:

```shell
> clef -i log-20170509.clef -o log-20170509.txt
```

Send the output to [Seq](https://getseq.net) by specifying a server URL and optional API key:

```
> clef -i log-20170509.clef --out-seq="https://seq.example.com" --out-seq-apikey="1234567890"
```
