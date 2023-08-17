using System.Text;
using Microsoft.Extensions.Primitives;

namespace Cocona.Docs;

public class DocsBuilder
{
  private StringBuilder StringBuilder { get; init; } = new();

  public DocsBuilder()
  {
    StringBuilder.Append(@"
      <script src=""https://cdn.tailwindcss.com""></script>
      <style>@font-face { font-family: 'fira-code'; src: url('woff2/FiraCode-Light.woff2') format('woff2'), url(""woff/FiraCode-Light.woff"") }</style>
      <body class=""container bg-slate-950 mx-auto text-slate-100 font-mono py-20"">
    ");
  }

  public DocsBuilder Header(string title)
  {
    StringBuilder.Append(
      $"<h1 class=\"text-3xl font-bold pb-4 border-b-2 border-slate-700\">{title}</h1>");
    return this;
  }

  public DocsBuilder Command(string name = "")
  {
    StringBuilder.Append($"<h1 class=\"text-xl font-bold mt-16 mb-8\">{name}</h1>");
    return this;
  }

  public DocsBuilder Description(string description)
  {
    if (description.Length == 0) return this;

    StringBuilder.Append($@"
      <h2 class=""px-4 w-fit bg-blue-500 mb-4"">Description</h2>
      <p class=""mb-8"">{description}</p>
    ");
    return this;
  }

  public DocsBuilder Arguments(List<(string name, string type, string description)> arguments)
  {
    if (!arguments.Any()) return this;

    StringBuilder.Append("<h2 class=\"px-4 w-fit bg-green-500 mb-4\">Arguments</h2>");
    foreach (var (name, type, description) in arguments)
    {
      StringBuilder.Append($@"
        <div class=""flex items-center mb-2"">
          <span class=""font-['fira-code'] mr-2"">&#10148;</span>
          <span class=""font-bold px-2 bg-slate-200 text-slate-800"">{name}</span>
          <span class=""ml-4 text-slate-400 text-sm"">{type}</span>
        </div>
        <div class=""flex items-center mb-4 ml-4"">
          <span class=""ml-2"">{description}</span>
        </div>
      ");
    }

    StringBuilder.Append("<div class=\"mb-8\"></div>");

    return this;
  }

  public DocsBuilder Options(
    List<(string name, string description, string type, string defaultValue)> options)
  {
    if (options.Count == 0) return this;

    StringBuilder.Append("<h2 class=\"px-4 w-fit bg-slate-500 mb-4\">Options</h2>");
    foreach (var (name, description, type, defaultValue) in options)
    {
      StringBuilder.Append($@"
        <div class=""flex items-center mb-4"">
          <span class=""font-['fira-code'] mr-2"">&#10148;</span>
          <span class=""font-bold px-2 bg-slate-200 text-slate-800"">{name}</span>
          {(type.Length == 0 ? "" : $"<span class=\"ml-4 text-slate-400 text-sm\">&lt;{type}&gt;</span>")}
        </div>
        {(defaultValue.Length == 0 ? "" : $"<p class=\"mb-2 ml-6 text-slate-400 text-sm\">{defaultValue}</p>")}
        <div class=""flex items-center mb-4 ml-6"">
          <span>{description}</span>
        </div>
      ");
    }

    StringBuilder.Append("<div class=\"mb-8\"></div>");

    return this;
  }

  public DocsBuilder Divider()
  {
    StringBuilder.Append("<div class=\"border-b-2 border-slate-700 mt-16\"></div>");
    return this;
  }

  public string Build()
  {
    StringBuilder.Append("</body>");

    return StringBuilder.ToString();
  }
}
