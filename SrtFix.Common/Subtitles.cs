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
}

public record SubtitleNr(int Nr, Timing Timing, ImmutableList<string> Text)
{
  public static SubtitleNr Default { get; } = new(0, Timing.Default, []);
}

public record Subtitle(Timing Timing, ImmutableList<string> Text);

public record Timing(TimeSpan Start, TimeSpan End)
{
  public static Timing Default { get; } = new(TimeSpan.Zero, TimeSpan.Zero);
}
