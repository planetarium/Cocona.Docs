using Cocona;

public class CalculateCommand
{
  [Command(Description = "Add two numbers.")]
  public void Add(
    [Argument(Name = "X", Description = "The first operand.")]
    int x,
    [Argument(Name = "Y", Description = "The second operand.")]
    int y
  )
  {
    Console.Out.WriteLine(x + y);
  }

  [Command(Description = "Subtract two numbers.")]
  public void Subtract(
    [Argument(Name = "X", Description = "The first operand.")]
    int x,
    [Argument(Name = "Y", Description = "The second operand.")]
    int y,
    [Option('r', Description = "Reverse operands' order.")]
    bool reverse = false
  )
  {
    var result = reverse ? y - x : x - y;
    Console.Out.WriteLine(result);
  }
}
