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
    List<(string name, CommandDescriptor command)> GetSubcommands(
      CommandDescriptor command,
      string parentName = "")
    {
      var subcommands = command.SubCommands?.All ?? Array.Empty<CommandDescriptor>();
      var isEmpty = command.Description.Length == 0 &&
                    command.Arguments.Count == 0 &&
                    command.Options.Count == 0;
      if (isEmpty && subcommands.Count == 0)
      {
        return new List<(string name, CommandDescriptor command)>();
      }

      var commandName = command.Name switch
      {
        _ when command.IsPrimaryCommand => parentName,
        _ when parentName.Length == 0 => command.Name,
        _ => $"{parentName} {command.Name}",
      };

      var unfoldedSubcommands = subcommands
        .OrderBy(subcommand => !subcommand.IsPrimaryCommand)
        .SelectMany(subcommand => GetSubcommands(subcommand, commandName))
        .ToList();
      if (isEmpty)
      {
        return unfoldedSubcommands;
      }

      return new[] { (commandName, command) }.Concat(unfoldedSubcommands).ToList();
    }

    var commands = commandCollection.All
      .Select(command => GetSubcommands(command))
      .Where(subcommands => subcommands.Count > 0)
      .ToList();

    var docs = new DocsBuilder();
    return docs.Header(title)
      .Body(() =>
      {
        foreach (var subcommands in commands)
        {
          foreach (var (name, command) in subcommands)
          {
            docs.Command(name.Replace(' ', '_'), () => docs
              .CommandHeader(name, command.Description)
              .Arguments(command.Arguments.Select(argument => (
                argument.Name,
                argument.ArgumentType.Name,
                argument.Description)).ToList())
              .Options(command.Options.Select(option => (
                option.Name,
                option.OptionType.Name,
                option.DefaultValue.Value?.ToString() ?? "",
                option.Description)).ToList()));
          }

          docs.Divider();
        }
      })
      .SideBar(commands.Select(subcommands =>
        subcommands.Select(subcommand =>
          (subcommand.name.Replace(' ', '_'), subcommand.name))))
      .Build();
  }
}
