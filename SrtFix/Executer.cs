using SrtFix.Common;

namespace SrtFix;

class Executer
{

  public static async Task ExecuteAsync(
    FileInfo file, List<ITransformation> transfomations, 
    CancellationToken cancellationToken)
  {
    if (!file.Exists)
    {
      Console.WriteLine($"File `{file}` doesn't exist :(");
    }
    else
    {
      Console.WriteLine($"Transformation applied to file `{file}`:");
      EchoTransformation(transfomations);
      string origFile = GetOrigFile(file);
      var original = await Parser.ParseAsync(origFile, cancellationToken);
      var result = original.Transform(transfomations);
      Console.WriteLine("Result preview:");
      EchoSubtitlesPreview(result);

      var ser = new Serializer();
      await ser.WriteToFileAsync(file.FullName, result);

    }
  }

  private static void EchoTransformation(List<ITransformation> transfomations)
  {
    var a = Math.Max(10, transfomations.Max(t => t.Name.Length));
    foreach (var t in transfomations) 
    {
      var name = t.Name.PadRight(a, ' ');
      Console.WriteLine($"  {name} | {t.Description}");
    }
  }

  private static string GetOrigFile(FileInfo file)
  {
    var dir = file.DirectoryName;
    var origFileName = $"{file.FullName}.orig";
    var origFile = string.IsNullOrWhiteSpace(dir) 
      ? origFileName 
      : Path.Combine(dir, origFileName);
    if (!File.Exists(origFile))
    {
      file.CopyTo(origFile);
    }
    return origFile;
  }

  private static void EchoSubtitlesPreview(Subtitles result)
  {
    const int COUNT = 10;
    if (result.Count == 0)
    {
      Console.WriteLine("  No subtitles :(");
    }
    else if (result.Count <= COUNT * 2)
    {
      EchoSubtitles(result);
    }
    else
    {
      EchoSubtitles(result.Take(COUNT));
      Console.WriteLine("  ...");
      EchoSubtitles(result.TakeLast(COUNT));
    }
  }

  static void EchoSubtitles(IEnumerable<SubtitleNr> subtitles)
  {
    foreach (var subtitle in subtitles)
    {
      EchoSubtitle(subtitle);
    }
  }

  static void EchoSubtitle(SubtitleNr subtitle)
  {
    var text = subtitle.Text.FirstOrDefault() ?? string.Empty;
    if(text.Length > 40)
    {
      text = text[..40] + "...";
    }
    Console.WriteLine(
      $"  {subtitle.Nr,-4} | {subtitle.Timing.Start} | {subtitle.Timing.End} | {text}");
  }

}
