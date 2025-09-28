using SrtFix.Common;

namespace SrtFix;

class Executer
{

  public static async Task ExecuteAsync(
    FileInfo file, List<IOp> ops, CancellationToken cancellationToken)
  {
    var dir = file.DirectoryName!;

    var fileName = file.FullName;
    var origFileName = $"{fileName}.orig";
    var origFile = Path.Combine(dir, origFileName);
    if(!File.Exists(origFile))
    {
      file.CopyTo(origFile);
    }
    var original = await Parser.ParseAsync(origFile, cancellationToken);
    var result = original.Transform(ops);
    var ser = new Serializer();
    await ser.WriteToFileAsync(fileName, result);

    foreach(var s in result.Take(10))
    {
      EchoSubtitle(s);
    }
    Console.WriteLine('…');
    foreach (var s in result.TakeLast(10))
    {
      EchoSubtitle(s);
    }
  }

  static void EchoSubtitle(SubtitleNr subtitle)
  {
    var text = subtitle.Text.FirstOrDefault() ?? string.Empty;
    if(text.Length > 40)
    {
      text = text.Substring(0, 40) + '…';
    }
    Console.WriteLine(
      $"{subtitle.Nr} | {subtitle.Timing.Start} | {subtitle.Timing.End} | {text}");
  }

}
