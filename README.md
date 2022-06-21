# Cocona.Docs

A [`Cocona`][Cocona] extension to generate CLI [`Cocona`][Cocona] App's documents easily.

[Cocona]: https://github.com/mayuki/Cocona

## Build

```
$ dotnet build
```

## Examples

Just add **one line** to use this extension.

```csharp
[HasSubCommands(typeof(Cocona.Docs.DocumentCommand), "docs")]
```

And you can run like the below command on your project:

```
$ dotnet run -- docs <OUT_DIR>
```
