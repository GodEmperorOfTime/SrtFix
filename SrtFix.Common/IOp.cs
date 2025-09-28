namespace SrtFix.Common;

public interface IOp
{

  Subtitle Transform(Subtitle subtitle);

}

public class ShiftOp(TimeSpan shift) : IOp
{

  readonly TimeSpan _shift = shift;

  public Subtitle Transform(Subtitle subtitle) => subtitle with { Timing = subtitle.Timing.Add(_shift) };

}

//public class UnitOperace : IOperace
//{
//  public Subtitle Transform(Subtitle subtitle) => subtitle;
//}

public class StretchOp : IOp
{

  readonly double _koef;

  public StretchOp(double koef)
  {
    _koef = koef;
  }

  public Subtitle Transform(Subtitle subtitle) => subtitle with { Timing = subtitle.Timing.Multiply(_koef) };
}

public static class OpExtensions
{

  //class KompozitniOp(IReadOnlyCollection<IOp> ops) : IOp
  //{

  //  readonly IReadOnlyCollection<IOp> _ops = ops ?? throw new ArgumentNullException(nameof(ops));

  //  public Subtitle Transform(Subtitle subtitle) => _ops.Aggregate(subtitle, (s, o) => o.Transform(s));
  //}

  public static Subtitles Transform(this Subtitles subtitles, params List<IOp> ops)
  {
    Subtitle transform(SubtitleNr s)
    {
      return ops.Aggregate(s.WithoutNr, (s, o) => o.Transform(s));
    }
    return new(subtitles.Select(transform));
  }

}