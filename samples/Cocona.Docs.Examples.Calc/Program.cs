using Cocona;

[HasSubCommands(typeof(CalculateCommand), "calculate")]
[HasSubCommands(typeof(Cocona.Docs.DocumentCommand), "docs")]
public class Program
{
  public static void Main(string[] args)
  {
    CoconaApp.Run<Program>(args);
  }

  [Command]
  public void Add(
    [Argument(Name = "X", Description = "The first operand.")]
    int x,
    [Argument(Name = "Y", Description = "The second operand.")]
    int y
  )
  {
    Console.Out.WriteLine(x + y);
  }
}
