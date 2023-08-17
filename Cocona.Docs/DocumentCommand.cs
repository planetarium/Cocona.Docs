using System.Collections.Immutable;
using System.Text;
using Cocona.Command;
using Cocona.Command.BuiltIn;

namespace Cocona.Docs;

public class DocumentCommand
{
  [PrimaryCommand]
  public void Generate([FromService] ICoconaCommandProvider commandProvider,
    [Argument(Name = "OUT_DIR", Description = "Path to output directory.")]
string outDir)
  {
    var collection = commandProvider.GetCommandCollection();
    var appName = AppDomain.CurrentDomain.FriendlyName;
    var rendered = GenerateDocs(collection, title: appName);

    File.WriteAllText(Path.Combine(outDir, "index.html"), rendered);
  }

  private static string GenerateDocs(CommandCollection commandCollection, string title = "")
  {
    var builder = new DocsBuilder()
      .Header($"{title} CLI");

    void AppendCommand(CommandDescriptor command, string parent = "")
    {
      var commandName = command.Name switch
      {
        _ when command.IsPrimaryCommand => parent,
        _ when parent.Length == 0 => command.Name,
        _ => $"{parent} {command.Name}"
      };
      var subcommands = command.SubCommands?.All ?? Array.Empty<CommandDescriptor>();
      var isEmpty = command.Description.Length == 0 &&
                    command.Arguments.Count == 0 &&
                    command.Options.Count == 0;

      if (!isEmpty)
      {
        builder
          .Command(commandName)
          .Description(command.Description)
          .Arguments(command.Arguments.Select(argument => (
            argument.Name,
            argument.ArgumentType.Name,
            argument.Description)).ToList())
          .Options(command.Options.Select(option => (
            option.Name,
            option.Description,
            option.OptionType.Name,
            option.DefaultValue.HasValue ? option.DefaultValue.ToString() : "")).ToList());
      }

      foreach (var subcommand in subcommands.OrderBy(subcommand => !subcommand.IsPrimaryCommand))
      {
        AppendCommand(subcommand, commandName);
      }

      if (parent.Length == 0 && (subcommands.Count > 0 || !isEmpty))
      {
        builder.Divider();
      }
    }

    foreach (var command in commandCollection.All)
    {
      AppendCommand(command);
    }

    return builder.Build();
  }
}
