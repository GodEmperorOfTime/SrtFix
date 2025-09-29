using SrtFix.Common;
using System.CommandLine;

namespace SrtFix;

class ArgsRunner
{

  public static async Task ParseAndRunAsync(
    IReadOnlyList<string> args, CancellationToken cancellationToken = default)
  {
    RootCommand rootCommand = new("App for fixing SRT subtitles");
    Option<double> shiftOption = new("--shift")
    {
      Description = "Shift all timestamps with given number of seconds"
    };
    rootCommand.Options.Add(shiftOption);
    Option<double> scaleOption = new("--scale")
    {
      Description = "Multiplies all timestamps with given factor"
    };
    rootCommand.Options.Add(scaleOption);
    Argument<FileInfo> fileArgument = new("file")
    {
      Description = "SRT file to process (fix)"
    };
    rootCommand.Arguments.Add(fileArgument);
    rootCommand.SetAction((result, cancelToken) =>
    {
      var transformations = new List<ITransformation>();
      var shiftValue = result.GetValue(shiftOption);
      if(shiftValue != 0.0)
      {
        transformations.Add(new ShiftTransformation(TimeSpan.FromSeconds(shiftValue)));
      }
      var scaleValue = result.GetValue(scaleOption);
      if (scaleValue > 0.0)
      {
        transformations.Add(new ScaleTransformation(scaleValue));
      }
      var file = result.GetRequiredValue(fileArgument);
      return Executer.ExecuteAsync(file, transformations, cancellationToken);
    });
    await rootCommand
      .Parse(args)
      .InvokeAsync(cancellationToken: cancellationToken);
  }

}
