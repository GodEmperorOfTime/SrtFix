using System.Collections;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace SrtFix.Common;

public class Parser
{

  enum State
  {
    Number,
    Timing,
    Text,
  }

  public static async Task<Subtitles> ParseAsync(string file, CancellationToken cancellation = default)
  {
    using var stream = new StreamReader(file); //File.OpenText(file);
    return await ParseAsync(stream, cancellation);
  }
  public static async Task<Subtitles> ParseAsync(StreamReader reader, CancellationToken cancellation = default)
  {
    var items = ImmutableList<Subtitle>.Empty;
    var subtitle = Subtitle.Default;
    State state = State.Number;
    string? line;
    int rowNr = 0;
    while ((line = await reader.ReadLineAsync(cancellation)) is not null)
    {
      rowNr++;
      try
      {
        state = state switch
        {
          State.Number => Number(line, ref subtitle),
          State.Timing => Timing(line, ref subtitle),
          State.Text => Text(line, ref subtitle),
          _ => throw new InvalidOperationException()
        };
      }
      catch (InnerParseException)
      {
        throw new ParseException(rowNr);
      }
      items = AddIfNewSubtitle(items, state, rowNr, subtitle);
    }
    items = AddIfNewSubtitle(items, state, rowNr, subtitle);
    return new(items);
  }

  static ImmutableList<Subtitle> AddIfNewSubtitle(ImmutableList<Subtitle> items, State state, int rowNr, Subtitle subtitle)
  {
    return state == State.Number && rowNr >= 3 ? items.Add(subtitle) : items;
  }


  static State Number(string line, ref Subtitle subtitle)
  {
    if (int.TryParse(line, out var nr))
    {
      subtitle = subtitle with { Nr = nr };
      return State.Timing;
    }
    else if (string.IsNullOrWhiteSpace(line))
    {
      return State.Number;
    }
    else
    {
      throw new InnerParseException();
    }
  }


  static State Timing(string line, ref Subtitle subtitle)
  {
    if(TimingParser.TryParse(line, out var timing))
    {
      subtitle = subtitle with { Timing = timing };
      return State.Text;
    }
    else
    {
      throw new InnerParseException();
    }
  }


  static State Text(string line, ref Subtitle subtitle)
  {
    if(string.IsNullOrWhiteSpace(line))
    {
      return State.Number;
    }
    else
    {
      subtitle = subtitle with { Text = subtitle.Text.Add(line.Trim()) };
      return State.Text;
    }
  }

  class InnerParseException() : Exception
  {

  }
}

public class ParseException : Exception
{
  public ParseException(int rowNr)
  {
    RowNr = rowNr;
  }

  public int RowNr { get; }
}

public class Subtitles : IReadOnlyCollection<Subtitle>
{

  readonly ImmutableList<Subtitle> _items;

  public Subtitles(ImmutableList<Subtitle> items)
  {
    _items = items ?? throw new ArgumentNullException(nameof(items));
  }

  public int Count => _items.Count;

  public IEnumerator<Subtitle> GetEnumerator() => ((IEnumerable<Subtitle>)_items).GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_items).GetEnumerator();
}

public record Subtitle(int Nr, Timing Timing, ImmutableList<string> Text)
{
  public static Subtitle Default { get; } = new (0, Timing.Default, []);
}

public record Timing (TimeSpan Start, TimeSpan End)
{
  public static Timing Default { get; } = new(TimeSpan.Zero, TimeSpan.Zero);
}
