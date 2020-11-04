// <copyright file="StringUtil.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Zaaml.Core.Utils
{
  internal enum QuoteKind
  {
    Single,
    Double
  }

  internal enum NewLineKind
  {
    Environment,
    LineFeed,
    CarriageReturn,
    CarriageReturnLineFeed,
    LineFeedCarriageReturn
  }

  internal static class StringUtils
  {
    #region Static Fields and Constants

    private const int Space = 32;

    public static readonly string SingleQuote = "'";
    public static readonly string DoubleQuote = "\"";
    public static readonly string CarriageReturn = "\r";
    public static readonly string LineFeed = "\n";
    // ReSharper disable once InconsistentNaming
    public static readonly string CRLF = "\r\n";
    // ReSharper disable once InconsistentNaming
    public static readonly string LFCR = "\n\r";

    #endregion

    #region  Methods

    public static string GetNewLine(NewLineKind newLineKind)
    {
      switch (newLineKind)
      {
        case NewLineKind.Environment:
          return Environment.NewLine;
        case NewLineKind.LineFeed:
          return LineFeed;
        case NewLineKind.CarriageReturn:
          return CarriageReturn;
        case NewLineKind.CarriageReturnLineFeed:
          return CRLF;
        case NewLineKind.LineFeedCarriageReturn:
          return LFCR;
        default:
          throw new ArgumentOutOfRangeException(nameof(newLineKind));
      }
    }

    public static string Left(string str, int index)
    {
      return str.Substring(0, Math.Min(index, str.Length));
    }

    public static string LeftOf(string str, string value)
    {
      var index = str.IndexOf(value, StringComparison.CurrentCulture);

      return index != -1 ? Left(str, index) : string.Empty;
    }

    public static string LeftOf(string str, char value)
    {
      var index = str.IndexOf(value);

      return index != -1 ? Left(str, index) : string.Empty;
    }

    public static IEnumerable<string> Lines(string text, NewLineKind newLineKind = NewLineKind.Environment)
    {
      var current = 0;
      int next;

      while ((next = text.IndexOf(GetNewLine(newLineKind), current + 1, StringComparison.CurrentCulture)) != -1)
      {
        var start = current == 0 ? 0 : current + 1;

        yield return text.Substring(start, next - start);

        current = next;
      }
      yield return text.Substring(current + 1);
    }

    public static string Quote(string str, QuoteKind quote)
    {
      switch (quote)
      {
        case QuoteKind.Single:
          return string.Concat(SingleQuote, str, SingleQuote);
        case QuoteKind.Double:
          return string.Concat(DoubleQuote, str, DoubleQuote);
        default:
          throw new ArgumentOutOfRangeException(nameof(quote));
      }
    }

    public static string Quote(string str)
    {
      return Quote(str, QuoteKind.Double);
    }

    public static string RemoveFromEnd(string s, string suffix, StringComparison comparison = StringComparison.CurrentCulture)
    {
      return s.EndsWith(suffix, comparison) ? s.Substring(0, s.Length - suffix.Length) : s;
    }

    public static string RemoveFromStart(string s, string prefix, StringComparison comparison = StringComparison.CurrentCulture)
    {
      return s.StartsWith(prefix, comparison) ? s.Substring(prefix.Length, s.Length - prefix.Length) : s;
    }

    public static string Right(string str, int index)
    {
      return str.Substring(index);
    }

    public static string RightOf(string str, string value)
    {
      var index = str.LastIndexOf(value, StringComparison.CurrentCulture);
      return index != -1 ? Right(str, index + value.Length) : string.Empty;
    }

    public static string RightOf(string str, char value)
    {
      var index = str.LastIndexOf(value);
      return index != -1 ? Right(str, index + 1) : string.Empty;
    }

    public static string StripSpaces(string str)
    {
      var length = str.Length;
      if (length == 0)
        return string.Empty;

      var nonSpaceStart = 0;

      while (str[nonSpaceStart] == Space)
        if (++nonSpaceStart == length)
          return " ";

      StringBuilder sb = null;
      int i;
      for (i = nonSpaceStart; i < length; ++i)
      {
        if (str[i] != Space) continue;

        var j = i + 1;
        while (j < length && str[j] == Space)
          ++j;

        if (j == length)
          return sb?.Append(str, nonSpaceStart, i - nonSpaceStart).ToString() ?? str.Substring(nonSpaceStart, i - nonSpaceStart);

        if (j == i + 1) continue;

        sb ??= new StringBuilder(length);

        sb.Append(str, nonSpaceStart, i - nonSpaceStart + 1);
        nonSpaceStart = j;
        i = j - 1;
      }

      if (sb == null)
        return nonSpaceStart == 0 ? str : str.Substring(nonSpaceStart, length - nonSpaceStart);

      return i > nonSpaceStart ? sb.Append(str, nonSpaceStart, i - nonSpaceStart).ToString() : sb.ToString();
    }

    public static string Truncate(string value, int maxLength)
    {
      return string.IsNullOrEmpty(value) ? value : value.Substring(0, Math.Min(value.Length, maxLength));
    }

    internal static bool IsEnclosed(string value, char startChar, char endChar)
    {
      return string.IsNullOrEmpty(value) == false && value[0] == startChar && value[value.Length - 1] == endChar;
    }

    public static IEnumerable<string> GetLines(string str, bool skipEmptyLines = false)
    {
	    using var sr = new StringReader(str);
	    string line;

	    while ((line = sr.ReadLine()) != null)
	    {
		    if (skipEmptyLines && string.IsNullOrWhiteSpace(line))
			    continue;

		    yield return line;
	    }
    }

    #endregion
  }
}