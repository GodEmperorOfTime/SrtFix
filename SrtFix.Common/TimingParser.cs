using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace SrtFix.Common;

static partial class TimingParser
{

  [GeneratedRegex("""
    ^
    \s*
    (?<sHour>\d+)
    \:
    (?<sMin>\d+)
    \:
    (?<sSec>\d+(\,|\.)\d+)
    \s*\-+\>\s*
    (?<eHour>\d+)
    \:
    (?<eMin>\d+)
    \:
    (?<eSec>\d+(\,|\.)\d+)
    \s*
    $
    """, RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace)]
  private static partial Regex Rex();

  public static bool TryParse(string s, out Timing result)
  {
    var m = Rex().Match(s);
    bool success;
    (success, result) = m.Success
      && TryExtractTimestamp(m, "sHour", "sMin", "sSec", out var start)
      && TryExtractTimestamp(m, "eHour", "eMin", "eSec", out var end)
      ? (true, new Timing(start, end))
      : (false, default);
    return success;
  }

  static bool TryExtractTimestamp(
    Match m, string hourGroup, string minGroup, string secGroup, out TimeSpan result)
  {
    bool success;
    (success, result) = 
         TryParseInt(m, hourGroup, out var hour)
      && TryParseInt(m, minGroup, out var min)
      && TryParseDouble(m, secGroup, out var sec)
      ? (true, TimeSpan.FromHours(hour) + TimeSpan.FromMinutes(min) + TimeSpan.FromSeconds(sec))
      : (false, default);
    return success;
  }

  static bool TryParseDouble(Match m, string groupName, out double result)
  {
     
    var s = m.Groups[groupName].Value;
    bool success;
    (success, result) = 
         double.TryParse(s, FormatProviders.Czech, out var r)
      || double.TryParse(s, FormatProviders.Invariant, out r)
      ? (r >= 0.0, r)
      : (false, default);
    return success;
  }

  static bool TryParseInt(Match m, string groupName, out int result)
  {
    var s = m.Groups[groupName].Value;
    bool success;
    (success, result) = int.TryParse(s, out var r) && r >= 0
      ? (true, r)
      : (false, 0);
    return success;
  }

}
