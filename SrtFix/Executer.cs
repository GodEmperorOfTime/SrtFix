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
      string origFile = GetOrigFile(file);
      var original = await Parser.ParseAsync(origFile, cancellationToken);
      var result = original.Transform(transfomations);
      var ser = new Serializer();
      await ser.WriteToFileAsync(file.FullName, result);

      Console.WriteLine($"Transformation applied to file `{file}`:");
      EchoTransformation(transfomations);
      Console.WriteLine();
      Console.WriteLine("Result preview:");
      EchoSubtitlesPreview(result);
    }
  }

  private static void EchoTransformation(List<ITransformation> transfomations)
  {
    if (transfomations.Count == 0)
    {
      Console.WriteLine("  No transformations");
    }
    else
    {
      var a = Math.Max(10, transfomations.Max(t => t.Name.Length));
      foreach (var t in transfomations) 
      {
        var name = t.Name.PadRight(a, ' ');
        Console.WriteLine($"  {name} | {t.Description}");
      }
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
      Console.WriteLine("   ...");
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
    const int MAX_LENGTH = 40;
    var text = subtitle.Text.FirstOrDefault() ?? string.Empty;
    if (text.Length > MAX_LENGTH)
    {
      text = text[..MAX_LENGTH] + "...";
    }
    var s = FormatTimstamp(subtitle.Timing.Start);
    var e = FormatTimstamp(subtitle.Timing.End);
    Console.WriteLine(
      $"  {subtitle.Nr,4} | {s} | {e} | {text}");
  }

  private static string FormatTimstamp(TimeSpan v)
  {
    var hours = (int)Math.Floor(v.TotalHours);
    return $"{hours:D2}:{v.Minutes:D2}:{v.Seconds:D2}:{v.Milliseconds:D3}";
    
  }
}
