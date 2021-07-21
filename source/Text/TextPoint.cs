// <copyright file="TextPoint.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	public readonly struct TextPoint
	{
		public static readonly TextPoint Empty = new(-1);

		public bool Equals(TextPoint other)
		{
			return Index == other.Index;
		}

		public bool IsEmpty => Index < 0;

		public override bool Equals(object obj)
		{
			return obj is TextPoint other && Equals(other);
		}

		public override int GetHashCode()
		{
			return Index;
		}

		public TextPoint(int index)
		{
			Index = index;
		}

		public int Index { get; }

		public static implicit operator int(TextPoint textPoint)
		{
			return textPoint.Index;
		}

		public static TextPoint operator +(TextPoint textPoint, int offset)
		{
			return new TextPoint(textPoint.Index + offset);
		}

		public static bool operator ==(TextPoint first, TextPoint second)
		{
			return first.Index == second.Index;
		}

		public static bool operator >(TextPoint first, TextPoint second)
		{
			return first.Index > second.Index;
		}

		public static bool operator >=(TextPoint first, TextPoint second)
		{
			return first.Index >= second.Index;
		}

		public static bool operator !=(TextPoint first, TextPoint second)
		{
			return first.Index != second.Index;
		}

		public static bool operator <(TextPoint first, TextPoint second)
		{
			return first.Index < second.Index;
		}

		public static bool operator <=(TextPoint first, TextPoint second)
		{
			return first.Index <= second.Index;
		}

		public static TextPoint operator -(TextPoint textPoint, int offset)
		{
			return new TextPoint(textPoint.Index - offset);
		}

		public int CompareTo(TextPoint other)
		{
			return Index.CompareTo(other.Index);
		}

		public static TextPoint operator ++(TextPoint textPoint)
		{
			return new TextPoint(textPoint.Index + 1);
		}

		public static TextPoint operator --(TextPoint textPoint)
		{
			return new TextPoint(textPoint.Index - 1);
		}

		public override string ToString()
		{
			return Index.ToString();
		}

		public static int operator -(TextPoint first, TextPoint second)
		{
			return first.Index - second.Index;
		}
	}
}