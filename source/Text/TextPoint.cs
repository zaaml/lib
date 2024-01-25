// <copyright file="TextPoint.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Zaaml.Text
{
	public readonly struct TextPoint
	{
		public static TextPoint Empty => new(true);

		internal readonly object TextObject;
		internal readonly int Offset;
		public readonly int Index;

		public TextPoint(TextSource textSource)
		{
			TextObject = textSource;
			Offset = 0;
			Index = 0;
		}

		public TextPoint(TextSource textSource, int start)
		{
			TextObject = textSource;
			Offset = 0;
			Index = start;
		}

		public TextPoint(string textString)
		{
			TextObject = textString;
			Offset = 0;
			Index = 0;
		}

		public TextPoint(string textString, int start)
		{
			TextObject = textString;
			Offset = 0;
			Index = start;
		}

		internal TextPoint(char[] textArray, int offset = 0)
		{
			TextObject = textArray;
			Offset = offset;
			Index = 0;
		}

		internal TextPoint(char[] textArray, int start, int offset = 0)
		{
			TextObject = textArray;
			Offset = offset;
			Index = start;
		}

		public TextPoint(int start)
		{
			TextObject = default;
			Offset = 0;
			Index = start;
		}

		private TextPoint(bool empty)
		{
			Debug.Assert(empty);

			TextObject = default;
			Offset = 0;
			Index = -1;
		}

		internal TextPoint(object textObject, int start, int offset)
		{
			TextObject = textObject;
			Offset = offset;
			Index = start;
		}

		private int ActualIndex => Index + Offset;

		public bool IsEmpty => Index == -1;

		public char Char => GetChar(0);

		public TextPoint FindNext(char c)
		{
			var nextIndex = -1;

			if (TextObject is string textString)
				nextIndex = textString.IndexOf(c, ActualIndex);
			else if (TextObject is TextSource textSource)
				nextIndex = textSource.IndexOf(c, ActualIndex);
			else if (TextObject is char[] textArray)
				throw new NotImplementedException();

			return nextIndex == -1 ? Empty : new TextPoint(TextObject, nextIndex, Offset);
		}

		public char CharOrDefault => TryGetChar(0, out var ch) ? ch : default;

		public char GetChar(int offset)
		{
			if (IsEmpty)
				throw new IndexOutOfRangeException();

			if (Index >= TextLength)
				throw new IndexOutOfRangeException();

			return GetCharSafe(offset);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private char GetCharSafe(int offset)
		{
			return TextObject switch
			{
				TextSource textSource => textSource.GetChar(ActualIndex + offset),
				string textString => textString[ActualIndex + offset],
				char[] textArray => textArray[ActualIndex + offset],
				_ => default
			};
		}

		public bool TryGetChar(int offset, out char ch)
		{
			if (IsEmpty)
			{
				ch = default;

				return false;
			}

			if (ActualIndex >= TextLength)
			{
				ch = default;

				return false;
			}

			ch = GetCharSafe(offset);

			return true;
		}

		private int TextLength =>
			TextObject switch
			{
				TextSource textSource => textSource.Length,
				string textString => textString.Length,
				char[] textArray => textArray.Length,
				_ => 0
			};

		public char this[int offset] => GetChar(offset);

		public override string ToString()
		{
			return Index.ToString();
		}

		public TextPoint Prev => this - 1;

		public TextPoint Next => this + 1;


		public bool HasNext =>
			TextObject switch
			{
				TextSource textSource => ActualIndex < textSource.Length,
				string textString => ActualIndex < textString.Length,
				char[] textArray => ActualIndex < textArray.Length,
				_ => false
			};

		private bool EqualSource(TextPoint other)
		{
			return Equals(TextObject, other.TextObject);
		}

		private void VerifySource(TextPoint other)
		{
			if (EqualSource(other) == false)
				throw new InvalidOperationException();
		}

		public bool Equals(TextPoint other)
		{
			return Index == other.Index && Offset == other.Offset && EqualSource(other);
		}

		public override bool Equals(object obj)
		{
			return obj is TextPoint other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = Index.GetHashCode();

				hashCode = (hashCode * 397) ^ Offset.GetHashCode();
				hashCode = TextObject switch
				{
					TextSource textSource => (hashCode * 397) ^ textSource.GetHashCode(),
					string textString => (hashCode * 397) ^ textString.GetHashCode(),
					char[] textArray => (hashCode * 397) ^ textArray.GetHashCode(),
					_ => hashCode
				};

				return hashCode;
			}
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
			return new TextPoint(textPoint.TextObject, textPoint.Index + offset, textPoint.Offset);
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
			return new TextPoint(textPoint.TextObject, textPoint.Index - offset, textPoint.Offset);
		}

		public int CompareTo(TextPoint other)
		{
			VerifySource(other);

			return Index.CompareTo(other.Index);
		}

		public static TextPoint operator ++(TextPoint textPoint)
		{
			return new TextPoint(textPoint.TextObject, textPoint.Index + 1, textPoint.Offset);
		}

		public static TextPoint operator --(TextPoint textPoint)
		{
			return new TextPoint(textPoint.TextObject, textPoint.Index - 1, textPoint.Offset);
		}

		public static int operator -(TextPoint first, TextPoint second)
		{
			first.VerifySource(second);

			return first.Index - second.Index;
		}

		public TextPointTag<TTag> WithTag<TTag>(TTag tag)
		{
			return TextPointTag.Create(this, tag);
		}
	}
}