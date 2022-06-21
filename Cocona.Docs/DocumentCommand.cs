using System.Collections.Immutable;
using System.Text;
using Cocona.Command;

namespace Cocona.Docs;
public class DocumentCommand
{
    [PrimaryCommand]
    public void Generate([FromService] ICoconaCommandProvider commandProvider, [Argument] string outDir)
    {
        var collection = commandProvider.GetCommandCollection();
        var rendered = GenerateDocs(collection);
        foreach (KeyValuePair<string, string> pair in rendered)
        {
            File.WriteAllText(Path.Combine(outDir, pair.Key == "" ? "index" : pair.Key), pair.Value);
        }
    }

    private ImmutableDictionary<string, string> GenerateDocs(CommandCollection commandCollection, string prefix = "")
    {
        var subcommandsDictionary = ImmutableDictionary<string, string>.Empty;
        StringBuilder stringBuilder = new StringBuilder();
        foreach (var command in commandCollection.All)
        {
            if (!command.IsPrimaryCommand && command.SubCommands is CommandCollection { })
            {
                subcommandsDictionary = subcommandsDictionary
                    .AddRange(GenerateDocs(command.SubCommands, prefix + (prefix != "" ? "." : "") + command.Name));
                continue;
            }

            stringBuilder.Append($"<h2>{command.Name}</h2><br/>");
            stringBuilder.Append($"<p>{command.Description}</p>");

            if (command.Options.Count > 0)
            {
                stringBuilder.Append("<h3>Options</h3>");
                stringBuilder.Append("<dd>");
                foreach (var option in command.Options)
                {
                    string defaultValueString = option.DefaultValue.HasValue ? $" {option.DefaultValue}" : "";
                    stringBuilder.Append($"<dt>");

                    if (option.ShortName.Count > 0)
                    {
                        foreach (char shortName in option.ShortName)
                        {
                            stringBuilder.Append($"<code>-{shortName}</code>");
                        }

                        stringBuilder.Append(", ");
                    }

                    stringBuilder.Append($"<code>--{option.Name} {ResolveTypeToString(option.OptionType)}{defaultValueString}</code></dt>");
                    stringBuilder.Append($"<dd>{option.Description}</dd>");
                }

                stringBuilder.Append("</dd>");
            }
            
            if (command.Arguments.Count > 0)
            {
                stringBuilder.Append("<h3>Arguments</h3>");
                stringBuilder.Append("<dd>");
                foreach (var argument in command.Arguments)
                {
                    string defaultValueString = argument.DefaultValue.HasValue ? $" {argument.DefaultValue}" : "";
                    stringBuilder.Append($"<dt><code>{argument.Name}</code> [{ResolveTypeToString(argument.ArgumentType)}]{defaultValueString}</dt>");
                    stringBuilder.Append($"<dd>{argument.Description}</dd>");
                }
                stringBuilder.Append("</dd>");
            }
        }

        if (subcommandsDictionary.Count > 0)
        {
            stringBuilder.Append("<h2>Subcommands</h2>");
            stringBuilder.Append("<ul>");
            foreach (KeyValuePair<string, string> pair in subcommandsDictionary)
            {
                stringBuilder.Append($"<li><a href=\"./{pair.Key}\">{pair.Key}</a></li>");
            }

            stringBuilder.Append("</ul>");
        }
        
        subcommandsDictionary = subcommandsDictionary.Add(prefix, (subcommandsDictionary.TryGetValue(prefix, out var value) ? value : "") + stringBuilder);

        return subcommandsDictionary;
    }

    private string ResolveTypeToString(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return type.GetGenericArguments()[0].Name + "?";
        }
        else
        {
            return type.Name;
        }
    }
}
