namespace SrtFix.Common;

static class FormatProviders
{
  public static readonly IFormatProvider Czech = System.Globalization.CultureInfo.GetCultureInfo("cs-Cz");
  public static readonly IFormatProvider Invariant = System.Globalization.CultureInfo.InvariantCulture;
}
