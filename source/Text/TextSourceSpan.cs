// <copyright file="TextSourceSpan.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

// ReSharper disable ReplaceSliceWithRangeIndexer

namespace Zaaml.Text
{
	public readonly struct TextSourceSpan
	{
		public static TextSourceSpan Empty => new(default, default, -1, 0);

		public TextSourceSpan(TextSource textSource)
		{
			TextSource = textSource;
			TextMemory = default;
			Start = 0;
			Length = textSource.Length;
		}

		public TextSourceSpan(TextSource textSource, int start)
		{
			TextSource = textSource;
			TextMemory = default;
			Start = start;
			Length = textSource.Length - start;
		}

		public TextSourceSpan(TextSource textSource, int start, int length)
		{
			TextSource = textSource;
			TextMemory = default;
			Start = start;
			Length = length;
		}

		public TextSourceSpan(ReadOnlyMemory<char> textMemory)
		{
			TextSource = default;
			TextMemory = textMemory;
			Start = 0;
			Length = textMemory.Length;
		}

		public TextSourceSpan(ReadOnlyMemory<char> textMemory, int start)
		{
			TextSource = default;
			TextMemory = textMemory;
			Start = start;
			Length = textMemory.Length - start;
		}

		public TextSourceSpan(ReadOnlyMemory<char> textMemory, int start, int length)
		{
			TextSource = default;
			TextMemory = textMemory;
			Start = start;
			Length = length;
		}

		private TextSourceSpan(TextSource textSource, ReadOnlyMemory<char> textMemory, int start, int length)
		{
			TextSource = textSource;
			TextMemory = textMemory;
			Start = start;
			Length = length;
		}

		private TextSource TextSource { get; }

		private ReadOnlyMemory<char> TextMemory { get; }

		private int Start { get; }

		public int Length { get; }

		public string GetText(int start)
		{
			Verify();

#if NETCOREAPP
			return TextSource != null ? TextSource.GetText(Start + start, Length - start) : new string(TextMemory.Slice(start).Span);
#else
			return TextSource != null ? TextSource.GetText(Start + start, Length - start) : TextMemory.Slice(start).Span.ToString();
#endif
		}

		public string GetText()
		{
			Verify();

#if NETCOREAPP
			return TextSource != null ? TextSource.GetText(Start, Length) : new string(TextMemory.Span);
#else
			return TextSource != null ? TextSource.GetText(Start, Length) : TextMemory.Span.ToString();
#endif
		}

		public string GetText(int start, int length)
		{
			Verify();

#if NETCOREAPP
			return TextSource != null ? TextSource.GetText(Start + start, length) : new string(TextMemory.Slice(start, length).Span);
#else
			return TextSource != null ? TextSource.GetText(Start + start, length) : TextMemory.Slice(start, length).Span.ToString();
#endif
		}

		public TextSourceSpan Slice(int start, int length)
		{
			if (start < 0 || start + length > Length)
				throw new ArgumentOutOfRangeException();

			return new TextSourceSpan(TextSource, TextMemory, Start + start, length);
		}

		public TextSourceSpan Slice(int start)
		{
			if (start < 0 || start > Length)
				throw new ArgumentOutOfRangeException();

			return new TextSourceSpan(TextSource, TextMemory, Start + start, Length - start);
		}

		public bool IsEmpty => Start == -1;

		private void Verify()
		{
			if (Start == -1)
				throw new InvalidOperationException("Empty text span.");
		}

		public ReadOnlyMemory<char> AsMemory()
		{
			Verify();

			return TextSource?.GetTextMemory(Start, Length) ?? TextMemory;
		}

		public ReadOnlyMemory<char> AsMemory(int start)
		{
			return TextSource?.GetTextMemory(Start + start, Length - start) ?? TextMemory.Slice(start);
		}

		public ReadOnlyMemory<char> AsMemory(int start, int length)
		{
			return TextSource?.GetTextMemory(Start + start, length) ?? TextMemory.Slice(start, length);
		}

		public char GetChar(int offset)
		{
			if (offset < 0 || offset >= Length)
				throw new ArgumentOutOfRangeException();

			return TextSource?.GetChar(Start + offset) ?? TextMemory.Span[Start + offset];
		}

		public char this[int index] => GetChar(index);

		public override string ToString()
		{
			return GetText();
		}
	}
}