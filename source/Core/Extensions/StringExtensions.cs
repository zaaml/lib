// <copyright file="StringExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using Zaaml.Core.Utils;

namespace Zaaml.Core.Extensions
{
	internal static class StringExtensions
	{
		private static CompareInfo GetCompareInfo(StringComparison comparisonType)
		{
			switch (comparisonType)
			{
				case StringComparison.CurrentCulture:
				case StringComparison.CurrentCultureIgnoreCase:
					return CultureInfo.CurrentCulture.CompareInfo;

				case StringComparison.InvariantCulture:
				case StringComparison.InvariantCultureIgnoreCase:
					return CultureInfo.InvariantCulture.CompareInfo;

				case StringComparison.Ordinal:
				case StringComparison.OrdinalIgnoreCase:
					return CultureInfo.InvariantCulture.CompareInfo;

				default:
					throw new ArgumentException();
			}
		}

		public static IEnumerable<string> GetLines(this string str, bool skipEmptyLines = false)
		{
			return StringUtils.GetLines(str, skipEmptyLines);
		}

		public static bool IsNullOrEmpty(this string str)
		{
			return string.IsNullOrEmpty(str);
		}

		public static bool IsNullOrWhiteSpace(this string str)
		{
			return string.IsNullOrWhiteSpace(str);
		}

		public static string Left(this string str, int index)
		{
			return str.Substring(0, Math.Min(index, str.Length));
		}

		public static string LeftOf(this string str, string value)
		{
			return str.LeftOf(value, StringComparison.CurrentCulture);
		}

		public static string LeftOf(this string str, string value, StringComparison comparisonType)
		{
			var index = str.IndexOf(value, comparisonType);

			return index != -1 ? Left(str, index) : string.Empty;
		}

		public static string LeftOf(this string str, char value)
		{
			return str.LeftOf(value, StringComparison.CurrentCulture);
		}

		public static string LeftOf(this string str, char value, StringComparison comparisonType)
		{
			var compareInfo = GetCompareInfo(comparisonType);
			var index = compareInfo.IndexOf(str, value);

			return index != -1 ? Left(str, index) : string.Empty;
		}

		public static IEnumerable<string> Lines(this string text, NewLineKind newLineKind = NewLineKind.Environment)
		{
			return StringUtils.Lines(text, newLineKind);
		}

		public static string Quote(this string str, QuoteKind quote)
		{
			return StringUtils.Quote(str, quote);
		}

		public static string Quote(this string str)
		{
			return StringUtils.Quote(str);
		}

		public static string RemoveFromEnd(this string s, string suffix, StringComparison comparison = StringComparison.CurrentCulture)
		{
			return StringUtils.RemoveFromEnd(s, suffix, comparison);
		}

		public static string RemoveFromStart(string s, string prefix, StringComparison comparison = StringComparison.CurrentCulture)
		{
			return StringUtils.RemoveFromStart(s, prefix, comparison);
		}

		public static string Right(this string str, int index)
		{
			return str.Substring(index);
		}

		public static string RightOf(this string str, string value)
		{
			return str.RightOf(value, StringComparison.CurrentCulture);
		}

		public static string RightOf(this string str, string value, StringComparison comparisonType)
		{
			var index = str.LastIndexOf(value, comparisonType);

			return index != -1 ? Right(str, index + value.Length) : string.Empty;
		}

		public static string RightOf(this string str, char value)
		{
			return str.RightOf(value, StringComparison.CurrentCulture);
		}

		public static string RightOf(this string str, char value, StringComparison comparisonType)
		{
			var compareInfo = GetCompareInfo(comparisonType);
			var index = compareInfo.LastIndexOf(str, value);

			return index != -1 ? Right(str, index + 1) : string.Empty;
		}

		public static string StripSpaces(this string str)
		{
			return StringUtils.StripSpaces(str);
		}

		public static string Truncate(this string value, int maxLength)
		{
			return StringUtils.Truncate(value, maxLength);
		}
	}
}