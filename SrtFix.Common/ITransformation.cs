namespace SrtFix.Common;

public interface ITransformation
{
  string Name { get; }
  string Description { get; }
  Subtitle Transform(Subtitle subtitle);
}

public class MoveTransformation(TimeSpan shift) : ITransformation
{

  readonly TimeSpan _shift = shift;

  public string Name => "move";
  public string Description => $"Moves (shifts) all timestamps by {_shift.TotalSeconds:N} sec";

  public Subtitle Transform(Subtitle subtitle) => subtitle with { Timing = subtitle.Timing.Add(_shift) };

}

public class ScaleTransformation(double factor) : ITransformation
{

  readonly double _factor = factor;

  public string Name => "scale";
  public string Description => $"Scales all timestamps by a factor of {_factor:N6}";

  public Subtitle Transform(Subtitle subtitle) => subtitle with { Timing = subtitle.Timing.Multiply(_factor) };
}

public static class TransformationExtensions
{

  public static Subtitles Transform(
    this Subtitles subtitles, params List<ITransformation> transformations)
  {
    Subtitle transform(SubtitleNr s)
    {
      return transformations.Aggregate(s.WithoutNr, (s, o) => o.Transform(s));
    }
    return new(subtitles.Select(transform));
  }

}