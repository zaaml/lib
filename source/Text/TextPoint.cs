// <copyright file="TextPoint.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;

namespace Zaaml.Text
{
	public readonly struct TextPoint
	{
		public static TextPoint Empty => new(true);

		public TextPoint(TextSource textSource)
		{
			TextSource = textSource;
			TextString = default;
			Index = 0;
		}

		public TextPoint(TextSource textSource, int start)
		{
			TextSource = textSource;
			TextString = default;
			Index = start;
		}

		public TextPoint(string textString)
		{
			TextSource = default;
			TextString = textString;
			Index = 0;
		}

		public TextPoint(string textString, int start)
		{
			TextSource = default;
			TextString = textString;
			Index = start;
		}

		public TextPoint(int start)
		{
			TextSource = default;
			TextString = default;
			Index = start;
		}

		private TextPoint(bool empty)
		{
			Debug.Assert(empty);

			TextSource = default;
			TextString = default;
			Index = -1;
		}

		internal TextPoint(TextSource textSource, string textString, int start)
		{
			TextSource = textSource;
			TextString = textString;
			Index = start;
		}

		internal TextSource TextSource { get; }

		internal string TextString { get; }

		public int Index { get; }

		public bool IsEmpty => Index == -1;

		public char Char => GetChar(0);

		public char GetChar(int offset)
		{
			if (IsEmpty)
				return '\0';

			return TextSource?.GetChar(Index + offset) ?? TextString[Index + offset];
		}

		public char this[int offset] => GetChar(offset);

		public override string ToString()
		{
			return Index.ToString();
		}

		private bool EqualSource(TextPoint other)
		{
			return ReferenceEquals(TextSource, other.TextSource) && string.Equals(TextString, other.TextString, StringComparison.Ordinal);
		}

		private void VerifySource(TextPoint other)
		{
			if (EqualSource(other) == false)
				throw new InvalidOperationException();
		}

		public bool Equals(TextPoint other)
		{
			return Index == other.Index && EqualSource(other);
		}

		public override bool Equals(object obj)
		{
			return obj is TextPoint other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Index, TextSource, TextString);
		}

		public static implicit operator int(TextPoint textPoint)
		{
			return textPoint.Index;
		}

		public static implicit operator char(TextPoint textPoint)
		{
			return textPoint.Char;
		}

		public static TextPoint operator +(TextPoint textPoint, int offset)
		{
			return new TextPoint(textPoint.TextSource, textPoint.TextString, textPoint.Index + offset);
		}

		public static bool operator ==(TextPoint first, TextPoint second)
		{
			return first.Equals(second);
		}

		public static bool operator >(TextPoint first, TextPoint second)
		{
			first.VerifySource(second);

			return first.Index > second.Index;
		}

		public static bool operator >=(TextPoint first, TextPoint second)
		{
			first.VerifySource(second);

			return first.Index >= second.Index;
		}

		public static bool operator !=(TextPoint first, TextPoint second)
		{
			return first.Equals(second) == false;
		}

		public static bool operator <(TextPoint first, TextPoint second)
		{
			first.VerifySource(second);

			return first.Index < second.Index;
		}

		public static bool operator <=(TextPoint first, TextPoint second)
		{
			first.VerifySource(second);

			return first.Index <= second.Index;
		}

		public static TextPoint operator -(TextPoint textPoint, int offset)
		{
			return new TextPoint(textPoint.TextSource, textPoint.TextString, textPoint.Index - offset);
		}

		public int CompareTo(TextPoint other)
		{
			VerifySource(other);

			return Index.CompareTo(other.Index);
		}

		public static TextPoint operator ++(TextPoint textPoint)
		{
			return new TextPoint(textPoint.TextSource, textPoint.TextString, textPoint.Index + 1);
		}

		public static TextPoint operator --(TextPoint textPoint)
		{
			return new TextPoint(textPoint.TextSource, textPoint.TextString, textPoint.Index - 1);
		}

		public static int operator -(TextPoint first, TextPoint second)
		{
			first.VerifySource(second);

			return first.Index - second.Index;
		}
	}
}