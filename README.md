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

### Outputs

Specify an output file with `-o`:

```shell
> clef -i log-20170509.clef -o log-20170509.txt
```

Write the output itself in CLEF format using `--out-clef`:

```shell
> clef -i log-20170509.clef --out-clef
{"@t":"2017-05-09T01:23:45.67890Z","@mt":"Starting up"}
...
```

Send the output to [Seq](https://getseq.net) by specifying a server URL and optional API key:

```
> clef -i log-20170509.clef --out-seq="https://seq.example.com" --out-seq-apikey="1234567890"
```

Control the output text format using `--out-template`:

```shell
> clef -i log-20170509.clef --out-template="{Message}"
Starting up
...
```
