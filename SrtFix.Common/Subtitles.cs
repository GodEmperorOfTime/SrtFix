using System.Collections;
using System.Collections.Immutable;

namespace SrtFix.Common;

public class Subtitles : IReadOnlyCollection<SubtitleNr>
{

  readonly ImmutableList<Subtitle> _items;

  public Subtitles(IEnumerable<Subtitle> items)
  {
    _items = [.. items ?? throw new ArgumentNullException(nameof(items))];
  }

  public int Count => _items.Count;

  public IEnumerator<SubtitleNr> GetEnumerator() => NrItems.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)NrItems).GetEnumerator();

  private IEnumerable<SubtitleNr> NrItems => _items.Select(GetSubtitleNr);

  private SubtitleNr GetSubtitleNr(Subtitle source, int index) => new(index + 1, source.Timing, source.Text);

  public Subtitles Sanitize()
  {
    var newItems =
      from item in _items
      let s = TimesSpanUtils.Max(item.Timing.Start, TimeSpan.Zero)
      let e = TimesSpanUtils.Max(item.Timing.End, TimeSpan.Zero)
      where s < e
      let t = new Timing(s, e)
      orderby t
      select new Subtitle(t, item.Text);
    return new(newItems);
  }
}

static class TimesSpanUtils
{
  public static TimeSpan Max(TimeSpan l, TimeSpan r) => l > r ? l : r;
}

public record SubtitleNr(int Nr, Timing Timing, ImmutableList<string> Text)
{
  public Subtitle WithoutNr => new (Timing, Text);
  public static SubtitleNr Default { get; } = new(0, Timing.Default, []);
}

public record Subtitle(Timing Timing, ImmutableList<string> Text);

public readonly record struct Timing(TimeSpan Start, TimeSpan End) 
  : IComparable<Timing>
{
  public static Timing Default { get; } = new(TimeSpan.Zero, TimeSpan.Zero);

  public Timing Add(TimeSpan value) => new(Start + value, End + value);

  public Timing Multiply(double value) => new(Start * value, End * value);

  public int CompareTo(Timing other)
  {
    int r = this.Start.CompareTo(other.Start);
    return r != 0 ? r : this.End.CompareTo(other.End);
  }
}
