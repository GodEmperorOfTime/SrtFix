using SrtFix.Common;
using System.CommandLine;

namespace SrtFix;

class ArgsRunner
{

  public static async Task ParseAndRunAsync(
    IReadOnlyList<string> args, CancellationToken cancellationToken = default)
  {
    RootCommand rootCommand = new("App for fixing SRT subtitles");
    Option<double> shiftOption = new("--move", "-m")
    {
      Description = "Move (shift) all timestamps by the given number of seconds"
    };
    rootCommand.Options.Add(shiftOption);
    Option<double> scaleOption = new("--scale", "-s")
    {
      Description = "Scales (multiplies) all timestamps using the specified factor"
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
      var moveValue = result.GetValue(shiftOption);
      if(moveValue != 0.0)
      {
        transformations.Add(new MoveTransformation(TimeSpan.FromSeconds(moveValue)));
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
