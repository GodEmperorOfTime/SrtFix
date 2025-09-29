namespace SrtFix.Common;

public interface ITransformation
{

  Subtitle Transform(Subtitle subtitle);

}

public class ShiftTransformation(TimeSpan shift) : ITransformation
{

  readonly TimeSpan _shift = shift;

  public Subtitle Transform(Subtitle subtitle) => subtitle with { Timing = subtitle.Timing.Add(_shift) };

}

public class ScaleTransformation(double factor) : ITransformation
{

  readonly double _factor = factor;

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