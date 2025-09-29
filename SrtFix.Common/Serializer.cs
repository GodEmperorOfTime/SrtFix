using System.Text;

namespace SrtFix.Common;

public class Serializer
{

  public async Task<string> ToStringAsync(Subtitles content)
  {
    var b = new StringBuilder();
    using var writer = new StringWriter(b);
    await WriteAsync(writer, content);
    return b.ToString();
  }

  public async Task WriteAsync(TextWriter writer, Subtitles content)
  {
    foreach (var subtitle in content.Sanitize())
    {
      await writer.WriteLineAsync(subtitle.Nr.ToString());
      var timingStr = SerializeTiming(subtitle.Timing);
      await writer.WriteLineAsync(timingStr);
      foreach (var text in subtitle.Text) 
      { 
        await writer.WriteLineAsync(text);
      }
      await writer.WriteLineAsync();
    }
  }

  public async Task WriteToFileAsync(string fileName, Subtitles result)
  {
    using var file = File.Open(fileName, FileMode.Create);
    using var writer = new StreamWriter(file);
    await WriteAsync(writer, result);
  }

  private string SerializeTiming(Timing timing)
  {
    var start = SerilalizeTimeStamp(timing.Start);
    var end = SerilalizeTimeStamp(timing.End);
    return $"{start} --> {end}";
  }

  private string SerilalizeTimeStamp(TimeSpan v)
  {
    var h = (int)Math.Floor(v.TotalHours);
    var m = v.Minutes;
    var s = (double)v.Seconds + (double)v.Milliseconds / 1000.0;
    return string.Format(FormatProviders.Czech, "{0:D2}:{1:D2}:{2:00.000}", h, m, s);
  }
}
