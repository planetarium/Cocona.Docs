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
      <link rel=""stylesheet"" href=""https://cdn.jsdelivr.net/npm/bootstrap-icons@1.8.0/font/bootstrap-icons.css""/>
      <style>@font-face { font-family: ""fira-code""; src: url(""woff2/FiraCode-Light.woff2"") format(""woff2""), url(""woff/FiraCode-Light.woff""); }</style>

      <body class=""bg-slate-950 font-mono"">
    ");
  }

  public DocsBuilder Header(string title)
  {
    StringBuilder.Append($@"
      <header class=""sticky top-0 text-white px-8 h-16 z-41 bg-slate-950 border-b border-slate-700 flex gap-4 items-center"">
        <span class=""text-white text-4xl top-5 left-4 cursor-pointer md:hidden"" onclick=""toggle()"">
          <i class=""bi bi-list""></i>
        </span>
        <h1 class=""text-2xl font-bold"">{title} CLI</h1>
      </header>
    ");

    return this;
  }

  public DocsBuilder Body(Action body)
  {
    StringBuilder.Append(@"
      <div class=""container mx-auto px-4 py-16 text-white transition-[padding] md:pl-[19rem]"">
        <ul class=""flex flex-col gap-16"">
    ");

    body();

    StringBuilder.Append(@"
        </ul>
      </div>
    ");
    return this;
  }

  public DocsBuilder Command(string id, Action command)
  {
    StringBuilder.Append($"<li id=\"{id}\" class=\"flex flex-col gap-6 scroll-mt-24\">");

    command();

    StringBuilder.Append("</li>\n");
    return this;
  }

  public DocsBuilder CommandHeader(string title, string description = "")
  {
    StringBuilder.Append($@"
      <div class=""flex flex-col gap-2"">
        <h1 class=""text-xl font-bold"">{title}</h1>
    ");

    if (description.Length > 0)
    {
      StringBuilder.Append($"<p class=\"text-slate-400\">{description}</p>");
    }

    StringBuilder.Append("</div>");

    return this;
  }

  public DocsBuilder Arguments(List<(string name, string type, string description)> arguments)
  {
    if (arguments.Count == 0) return this;

    StringBuilder.Append(@"
      <h2 class=""px-4 w-fit bg-blue-500"">Arguments</h2>
      <ul class=""flex flex-col gap-4"">
    ");
    foreach (var (name, type, description) in arguments)
    {
      StringBuilder.Append($@"
        <li class=""flex flex-col gap-2"">
          <div class=""flex items-center gap-2"">
            <span class=""font-['fira-code']"">&#10148;</span>
            <span class=""px-2 bg-slate-200 font-bold text-slate-800"">{name}</span>
            <span class=""text-slate-400 text-sm"">{type}</span>
          </div>
          <div class=""ml-6"">{description}</div>
        </li>
      ");
    }

    StringBuilder.Append("</ul>\n");

    return this;
  }

  public DocsBuilder Options(
    IList<(string name, string payload, string defaultValue, string description)> options)
  {
    if (!options.Any()) return this;

    StringBuilder.Append(@"
      <h2 class=""px-4 w-fit bg-slate-500"">Options</h2>
      <ul class=""flex flex-col gap-4"">
    ");
    foreach (var (name, payload, defaultValue, description) in options)
    {
      var defaultValueString = defaultValue.Length > 0 ? $"Default Value : {defaultValue}" : "";
      StringBuilder.Append($@"
        <li class=""flex flex-col gap-2"">
          <div class=""flex items-center gap-2"">
            <span class=""font-['fira-code']"">&#10148;</span>
            <span class=""px-2 bg-slate-200 font-bold text-slate-800"">{name}</span>
            <span class=""text-slate-400 text-sm"">&lt;{payload}&gt;</span>
          </div>
          <div class=""ml-6 text-sm text-slate-400"">{defaultValueString}</div>
          <div class=""ml-6"">{description}</div>
      ");
      StringBuilder.Append("</li>\n");
    }

    StringBuilder.Append("</ul>\n");
    return this;
  }

  public DocsBuilder Divider()
  {
    StringBuilder.Append("<li class=\"border-b border-slate-700\"></li>\n");

    return this;
  }

  public DocsBuilder SideBar(IEnumerable<IEnumerable<(string id, string name)>> menuGroups)
  {
    StringBuilder.Append(@"
      <aside class=""sidebar fixed top-16 left-0 z-40 h-[calc(100vh-4rem)] w-72 bg-slate-950 border-r border-slate-700 overflow-y-auto translate-x-[-100%] transition md:translate-x-0"">
        <ul class=""flex flex-col gap-4 p-8 text-white text-sm"">
    ");

    foreach (var menus in menuGroups)
    {
      foreach (var (id, name) in menus)
      {
        StringBuilder.Append(
          $"<li><a href=\"#{id}\" class=\"block\" onclick=\"toggle()\">{name}</a></li>\n");
      }

      StringBuilder.Append("<li class=\"border-b border-slate-700\"></li>");
    }

    StringBuilder.Append(@"
        </ul>
      </aside>
    ");
    return this;
  }

  public string Build()
  {
    StringBuilder.Append(@" 
      </body>

      <script type=""text/javascript"">
        function toggle() {
          document.querySelector("".sidebar"").classList.toggle(""translate-x-[-100%]"");
        }
      </script>
    ");

    return StringBuilder.ToString();
  }
}
