// <copyright file="TextSourceSpan.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using Zaaml.Core;

// ReSharper disable ReplaceSliceWithRangeIndexer

namespace Zaaml.Text
{
	public readonly struct TextSpan
	{
		public static TextSpan Empty => new(true);

		public TextSpan(TextSource textSource)
		{
			TextSource = textSource;
			TextString = default;
			StartIndex = 0;
			Length = textSource.Length;
		}

		public TextSpan(TextSource textSource, int start)
		{
			TextSource = textSource;
			TextString = default;
			StartIndex = start;
			Length = textSource.Length - start;
		}

		public TextSpan(TextSource textSource, int start, int length)
		{
			TextSource = textSource;
			TextString = default;
			StartIndex = start;
			Length = length;
		}

		public TextSpan(string textString)
		{
			TextSource = default;
			TextString = textString;
			StartIndex = 0;
			Length = textString.Length;
		}

		public TextSpan(string textString, int start)
		{
			TextSource = default;
			TextString = textString;
			StartIndex = start;
			Length = textString.Length - start;
		}

		public TextSpan(string textString, int start, int length)
		{
			TextSource = default;
			TextString = textString;
			StartIndex = start;
			Length = length;
		}

		public TextSpan(int start)
		{
			TextSource = default;
			TextString = default;
			StartIndex = start;
			Length = 0;
		}

		public TextSpan(int start, int length)
		{
			TextSource = default;
			TextString = default;
			StartIndex = start;
			Length = length;
		}

		private TextSpan(bool empty)
		{
			Debug.Assert(empty);

			TextSource = default;
			TextString = default;
			StartIndex = -1;
			Length = 0;
		}

		private TextSpan(TextSource textSource, string textString, int start, int length)
		{
			TextSource = textSource;
			TextString = textString;
			StartIndex = start;
			Length = length;
		}

		internal TextSource TextSource { get; }

		internal string TextString { get; }

		public int StartIndex { get; }

		public int Length { get; }

		public string GetText(int start)
		{
			Verify();

			return TextSource != null ? TextSource.GetText(StartIndex + start, Length - start) : TextString.Substring(start);
		}

		public string GetText()
		{
			Verify();

			return TextSource != null ? TextSource.GetText(StartIndex, Length) : TextString;
		}

		public string GetText(int start, int length)
		{
			Verify();

			return TextSource != null ? TextSource.GetText(StartIndex + start, length) : TextString.Substring(start, length);
		}

		public TextSpan Slice(int start, int length)
		{
			if (start < 0 || start + length > Length)
				throw new ArgumentOutOfRangeException();

			return new TextSpan(TextSource, TextString, start, length);
		}

		public TextSpan Slice(int start)
		{
			if (start < 0 || start > Length)
				throw new ArgumentOutOfRangeException();

			return new TextSpan(TextSource, TextString, start, Length - start);
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

			return TextSource?.GetTextMemory(StartIndex, Length) ?? TextString.AsMemory().Slice(StartIndex);
		}

		public ReadOnlyMemory<char> AsMemory(int start)
		{
			return TextSource?.GetTextMemory(StartIndex + start, Length - start) ?? TextString.AsMemory().Slice(StartIndex + start);
		}

		public ReadOnlyMemory<char> AsMemory(int start, int length)
		{
			return TextSource?.GetTextMemory(StartIndex + start, length) ?? TextString.AsMemory().Slice(StartIndex + start, length);
		}

		public char GetChar(int offset)
		{
			if (offset < 0 || offset >= Length)
				throw new ArgumentOutOfRangeException();

			return TextSource?.GetChar(StartIndex + offset) ?? TextString[StartIndex + offset];
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

		public TextPoint Start => new(TextSource, TextString, StartIndex);

		public TextPoint End => new(TextSource, TextString, EndIndex);

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

		internal TextSpan IntersectWith(TextSpan textSpan)
		{
			VerifySource(textSpan);

			var interval = Interval;
			var textInterval = textSpan.Interval;
			var intersection = Core.Interval.Intersect(interval, textInterval).Normalize(IntervalEndPoint.Closed, IntervalEndPoint.Open);

			if (intersection.IsEmpty)
				return Empty;

			return new TextSpan(TextSource, TextString, intersection.Minimum, intersection.Maximum - intersection.Minimum);
		}

		public bool Equals(TextSpan other)
		{
			return StartIndex == other.StartIndex && Length == other.Length && EqualSource(other);
		}

		private bool EqualSource(TextSpan other)
		{
			return ReferenceEquals(TextSource, other.TextSource) && string.Equals(TextString, other.TextString, StringComparison.Ordinal);
		}

		private bool EqualSource(TextPoint other)
		{
			return ReferenceEquals(TextSource, other.TextSource) && string.Equals(TextString, other.TextString, StringComparison.Ordinal);
		}

		public override bool Equals(object obj)
		{
			return obj is TextSpan other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(StartIndex, Length, TextSource, TextString);
		}
	}
}