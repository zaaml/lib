// <copyright file="TextSpan.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using Zaaml.Core;

// ReSharper disable ReplaceSliceWithRangeIndexer

namespace Zaaml.Text
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
	public readonly struct TextSpan
	{
		public static TextSpan Empty => new(true);

		internal readonly object TextObject;
		internal readonly int Offset;
		public readonly int StartIndex;
		public readonly int Length;

		public TextSpan(TextSource textSource)
		{
			TextObject = textSource;
			StartIndex = 0;
			Offset = 0;
			Length = textSource.Length;
		}

		public TextSpan(TextSource textSource, int start)
		{
			TextObject = textSource;
			StartIndex = start;
			Offset = 0;
			Length = textSource.Length - start;
		}

		public TextSpan(TextSource textSource, int start, int length)
		{
			TextObject = textSource;
			StartIndex = start;
			Offset = 0;
			Length = length;
		}

		public TextSpan(string textString)
		{
			TextObject = textString;
			StartIndex = 0;
			Offset = 0;
			Length = textString.Length;
		}

		public TextSpan(string textString, int start)
		{
			TextObject = textString;
			StartIndex = start;
			Offset = 0;
			Length = textString.Length - start;
		}

		public TextSpan(string textString, int start, int length)
		{
			TextObject = textString;
			StartIndex = start;
			Offset = 0;
			Length = length;
		}

		internal TextSpan(char[] textArray, int arrayOffset = 0)
		{
			TextObject = textArray;
			StartIndex = 0;
			Offset = arrayOffset;
			Length = textArray.Length;
		}

		internal TextSpan(char[] textArray, int start, int arrayOffset = 0)
		{
			TextObject = textArray;
			StartIndex = start;
			Offset = arrayOffset;
			Length = textArray.Length - start;
		}

		internal TextSpan(char[] textArray, int start, int length, int arrayOffset = 0)
		{
			TextObject = textArray;
			StartIndex = start;
			Offset = arrayOffset;
			Length = length;
		}

		public TextSpan(int start)
		{
			TextObject = default;
			StartIndex = start;
			Offset = 0;
			Length = 0;
		}

		public TextSpan(int start, int length)
		{
			TextObject = default;
			StartIndex = start;
			Offset = 0;
			Length = length;
		}

		private TextSpan(bool empty)
		{
			Debug.Assert(empty);

			TextObject = default;
			StartIndex = -1;
			Offset = 0;
			Length = 0;
		}

		private TextSpan(object textObject, int start, int length, int offset)
		{
			TextObject = textObject;
			StartIndex = start;
			Offset = offset;
			Length = length;
		}

		public string GetText(int start)
		{
			Verify();

			return TextObject switch
			{
				TextSource textSource => textSource.GetText(ActualStartIndex + start, Length - start),
				string textString => textString.Substring(ActualStartIndex + start, Length - start),
				char[] textArray => new string(textArray, ActualStartIndex + start, Length - start),
				_ => default
			};
		}

		private int ActualStartIndex => StartIndex + Offset;

		public string GetText()
		{
			Verify();

			return TextObject switch
			{
				TextSource textSource => textSource.GetText(ActualStartIndex, Length),
				string textString => textString.Substring(ActualStartIndex, Length),
				char[] textArray => new string(textArray, ActualStartIndex, Length),
				_ => default
			};
		}

		public string GetText(int start, int length)
		{
			Verify();

			return TextObject switch
			{
				TextSource textSource => textSource.GetText(ActualStartIndex + start, length),
				string textString => textString.Substring(ActualStartIndex + start, length),
				char[] textArray => new string(textArray, ActualStartIndex + start, length),
				_ => default
			};
		}

		internal string GetTextInternal(int start, int length)
		{
			return TextObject switch
			{
				TextSource textSource => textSource.GetText(ActualStartIndex + start, length),
				string textString => textString.Substring(ActualStartIndex + start, length),
				char[] textArray => new string(textArray, ActualStartIndex + start, length),
				_ => default
			};
		}

		public TextSpan Slice(int start, int length)
		{
			if (start < 0 || start + length > Length)
				throw new ArgumentOutOfRangeException();

			return new TextSpan(TextObject, StartIndex + start, length, Offset);
		}

		public TextSpan Slice(int start)
		{
			if (start < 0 || start > Length)
				throw new ArgumentOutOfRangeException();

			return new TextSpan(TextObject, StartIndex + start, Length - start, Offset);
		}

		public bool IsEmpty => StartIndex == -1;

		private void Verify()
		{
			if (StartIndex == -1)
				throw new InvalidOperationException("Empty text span.");
		}

		public ReadOnlyMemory<char> AsMemory()
		{
			Verify();

			return TextObject switch
			{
				TextSource textSource => textSource.GetTextMemory(ActualStartIndex, Length),
				string textString => textString.AsMemory().Slice(ActualStartIndex, Length),
				char[] textArray => textArray.AsMemory(ActualStartIndex, Length),
				_ => default
			};
		}

		public ReadOnlyMemory<char> AsMemory(int start)
		{
			return TextObject switch
			{
				TextSource textSource => textSource.GetTextMemory(ActualStartIndex + start, Length - start),
				string textString => textString.AsMemory().Slice(ActualStartIndex + start, Length - start),
				char[] textArray => textArray.AsMemory().Slice(ActualStartIndex + start, Length - start),
				_ => default
			};
		}

		public ReadOnlyMemory<char> AsMemory(int start, int length)
		{
			return TextObject switch
			{
				TextSource textSource => textSource.GetTextMemory(ActualStartIndex + start, length),
				string textString => textString.AsMemory().Slice(ActualStartIndex + start, length),
				char[] textArray => textArray.AsMemory().Slice(ActualStartIndex + start, length),
				_ => default
			};
		}

		public char GetChar(int offset)
		{
			if (offset < 0 || offset >= Length)
				throw new ArgumentOutOfRangeException();

			return TextObject switch
			{
				TextSource textSource => textSource.GetChar(ActualStartIndex + offset),
				string textString => textString[ActualStartIndex + offset],
				char[] textArray => textArray[ActualStartIndex + offset],
				_ => default
			};
		}

		public char this[int index] => GetChar(index);

		public static implicit operator TextSpan(string textString)
		{
			return new TextSpan(textString);
		}

		public static implicit operator string(TextSpan textSourceSpan)
		{
			return textSourceSpan.GetText();
		}

		public override string ToString()
		{
			return GetText();
		}

		public int EndIndex => StartIndex + Length;

		public TextPoint Start => new(TextObject, StartIndex, Offset);

		public TextPoint End => new(TextObject, EndIndex, Offset);

		public TextPoint At(int position)
		{
			if (position < 0 || position > Length)
				throw new ArgumentOutOfRangeException();

			return new TextPoint(TextObject, StartIndex + position, Offset);
		}

		private void VerifySource(TextPoint other)
		{
			if (EqualSource(other) == false)
				throw new InvalidOperationException();
		}

		private void VerifySource(TextSpan other)
		{
			if (EqualSource(other) == false)
				throw new InvalidOperationException();
		}

		public bool Contains(TextSpan span)
		{
			if (IsEmpty)
				return false;

			VerifySource(span);

			return StartIndex <= span.StartIndex && span.EndIndex <= EndIndex;
		}

		public bool Contains(TextPoint textPoint)
		{
			if (IsEmpty)
				return false;

			VerifySource(textPoint);

			var pointIndex = textPoint.Index;

			return pointIndex >= StartIndex && pointIndex < EndIndex;
		}

		public static bool operator ==(TextSpan first, TextSpan second)
		{
			return first.Equals(second);
		}

		public static bool operator !=(TextSpan first, TextSpan second)
		{
			return first.Equals(second) == false;
		}

		internal Interval<int> Interval => IsEmpty ? Interval<int>.Empty : new Interval<int>(StartIndex, IntervalEndPoint.Closed, EndIndex, IntervalEndPoint.Open);

		public bool JoinsWith(TextSpan span)
		{
			var maxStart = StartIndex > span.StartIndex ? StartIndex : span.StartIndex;
			var minEnd = EndIndex < span.EndIndex ? EndIndex : span.EndIndex;

			return maxStart == minEnd;
		}

		public TextSpan Junction(TextSpan span)
		{
			var maxStart = StartIndex > span.StartIndex ? StartIndex : span.StartIndex;
			var minEnd = EndIndex < span.EndIndex ? EndIndex : span.EndIndex;

			if (maxStart > minEnd)
			{
				var spanEnd = EndIndex < span.EndIndex ? End : span.End;
				var end = StartIndex > span.StartIndex ? Start : span.Start;

				return FromBounds(spanEnd, end);
			}

			return Empty;
		}

		public TextSpan Bounds(TextSpan span)
		{
			var minStart = StartIndex < span.StartIndex ? StartIndex : span.StartIndex;
			var maxEnd = EndIndex > span.EndIndex ? EndIndex : span.EndIndex;

			return FromBounds(minStart, maxEnd);
		}

		public TextSpan Join(TextSpan span)
		{
			var maxStart = StartIndex > span.StartIndex ? StartIndex : span.StartIndex;
			var minEnd = EndIndex < span.EndIndex ? EndIndex : span.EndIndex;

			if (maxStart == minEnd)
			{
				var joinStart = StartIndex < span.StartIndex ? StartIndex : span.StartIndex;
				var joinEnd = EndIndex > span.EndIndex ? EndIndex : span.EndIndex;

				return FromBounds(joinStart, joinEnd);
			}

			return Empty;
		}

		public static TextSpan FromBounds(int start, int end)
		{
			return new TextSpan(start, end - start);
		}

		public static TextSpan FromBounds(string text, int start, int end)
		{
			return new TextSpan(text, start, end - start);
		}

		public static TextSpan FromBounds(TextSource textSource, int start, int end)
		{
			return new TextSpan(textSource, start, end - start);
		}

		private static TextSpan FromBounds(object textObject, int start, int end, int offset)
		{
			return new TextSpan(textObject, start, end - start, offset);
		}

		public bool IntersectsWith(TextSpan span)
		{
			return span.StartIndex < EndIndex && span.EndIndex > StartIndex;
		}

		public TextSpan Intersection(TextSpan span)
		{
			var start = StartIndex > span.StartIndex ? StartIndex : span.StartIndex;
			var end = EndIndex < span.EndIndex ? EndIndex : span.EndIndex;

			return start <= end ? FromBounds(TextObject, start, end, Offset) : Empty;
		}

		public bool Equals(TextSpan other)
		{
			return StartIndex == other.StartIndex && Length == other.Length && Offset == other.Offset && EqualSource(other);
		}

		private bool EqualSource(TextSpan other)
		{
			return Equals(TextObject, other.TextObject);
		}

		private bool EqualSource(TextPoint other)
		{
			return Equals(TextObject, other.TextObject);
		}

		public override bool Equals(object obj)
		{
			return obj is TextSpan other && Equals(other);
		}

		private string DebuggerDisplay
		{
			get
			{
				if (IsEmpty)
					return "(Empty)";

				if (TextObject is TextSource)
					return $"[{StartIndex};{EndIndex}) - {GetText()}";

				return $"[{StartIndex};{EndIndex})";
			}
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = StartIndex.GetHashCode();

				hashCode = TextObject switch
				{
					TextSource textSource => (hashCode * 397) ^ textSource.GetHashCode(),
					string textString => (hashCode * 397) ^ textString.GetHashCode(),
					char[] textArray => (hashCode * 397) ^ textArray.GetHashCode(),
					_ => (hashCode * 397) ^ Length.GetHashCode()
				};

				return hashCode;
			}
		}

		public TextSpanTag<TTag> WithTag<TTag>(TTag tag)
		{
			return TextSpanTag.Create(this, tag);
		}
	}
}