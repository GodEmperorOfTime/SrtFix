using System.Collections;
using System.Collections.Immutable;

namespace SrtFix.Common;

public class Parser
{

  public static async Task<Subtitles> ParseAsync(string file, CancellationToken cancellation = default)
  {
    using var stream = new StreamReader(file); 
    return await ParseAsync(stream, cancellation); // musi byt vyawaitovant
  }

  public static async Task<Subtitles> ParseAsync(StreamReader reader, CancellationToken cancellation = default)
  {
    var parser = new Impl();
    string? line;
    while ((line = await reader.ReadLineAsync(cancellation)) is not null)
    {
      parser.Next(line);
    }
    parser.Finish();
    return new(parser.Subtitles);
  }

  class Impl
  {

    enum State
    {
      Number,
      Timing,
      Text,
    }

    public ImmutableList<Subtitle> Subtitles { get; private set; } = [];

    State _state = State.Number;
    int _lineNr = 0;
    Subtitle _subtitle = Subtitle.Default;    

    public void Next(string line)
    {
      _lineNr++;
      try
      {
        _state = _state switch
        {
          State.Number => Number(line),
          State.Timing => Timing(line),
          State.Text => Text(line),
          _ => throw new InvalidOperationException()
        };
      }
      catch (InnerParseException)
      {
        throw new ParseException(_lineNr, line);
      }
    }

    public void Finish()
    {
      if (_state == State.Text && _lineNr >= 3)
      {
        ZaraditSubtitle();
      }
    }

    State Number(string line)
    {
      if (int.TryParse(line, out var nr))
      {
        _subtitle = _subtitle with { Nr = nr };
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

    State Timing(string line)
    {
      if (TimingParser.TryParse(line, out var timing))
      {
        _subtitle = _subtitle with { Timing = timing };
        return State.Text;
      }
      else
      {
        throw new InnerParseException();
      }
    }

    State Text(string line)
    {
      if (string.IsNullOrWhiteSpace(line))
      {
        ZaraditSubtitle();
        return State.Number;
      }
      else
      {
        _subtitle = _subtitle with { Text = _subtitle.Text.Add(line.Trim()) };
        return State.Text;
      }
    }

    private void ZaraditSubtitle()
    {
      Subtitles = Subtitles.Add(_subtitle);
      _subtitle = Subtitle.Default;
    }
  }

  class InnerParseException() : Exception
  {

  }
}

public class ParseException : Exception
{
  public ParseException(int rowNr, string line)
  {
    RowNr = rowNr;
    Line = line;
  }

  public int RowNr { get; }
  public string Line { get; }
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
