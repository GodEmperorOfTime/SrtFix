using SrtFix.Common;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.IO;
using System.Threading.Tasks;

namespace SrtFix;

internal class Class2
{

  public static async Task RunAsync(
    IReadOnlyList<string> args, CancellationToken cancellationToken = default)
  {
    RootCommand rootCommand = new("App for fixing SRT subtitles");
    Option<double> shiftOption = new("--shift")
    {
      Description = "Pocet sekund o kolik posunout cele titulky"
    };
    rootCommand.Options.Add(shiftOption);
    Option<double> stretchOption = new("--stretch")
    {
      Description = "Koeficient, kterym znasobit vsechny casy"
    };
    rootCommand.Options.Add(stretchOption);
    Argument<FileInfo> fileArgument = new("file")
    {
      Description = "Soubor s titulkama, ktery zpracovavat"
    };
    rootCommand.Arguments.Add(fileArgument);
    rootCommand.SetAction((result, cancelToken) =>
    {
      var ops = new List<IOp>();
      var shiftValue = result.GetValue(shiftOption);
      if(shiftValue != 0.0)
      {
        ops.Add(new ShiftOp(TimeSpan.FromSeconds(shiftValue)));
      }
      var stretchValue = result.GetValue(stretchOption);
      if (stretchValue > 0.0)
      {
        ops.Add(new StretchOp(stretchValue));
      }
      var file = result.GetRequiredValue(fileArgument);
      return Class1.AaaaAsync(file, ops, cancellationToken);
    });
    await rootCommand
      .Parse(args)
      .InvokeAsync(cancellationToken: cancellationToken);
  }

  //static Task RunAsync(ParseResult result, CancellationToken cancellationToken)
  //{ 
  //}

}
