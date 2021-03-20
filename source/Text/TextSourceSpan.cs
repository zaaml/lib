// <copyright file="TextSourceSpan.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal readonly struct TextSourceSpan
	{
		public static TextSourceSpan Empty => new(null, 0, 0);

		public TextSourceSpan(TextSource textSource)
		{
			TextSource = textSource;
			Start = 0;
			Length = textSource.Length;
		}

		public TextSourceSpan(TextSource textSource, int start)
		{
			TextSource = textSource;
			Start = start;
			Length = textSource.Length - start;
		}

		public TextSourceSpan(TextSource textSource, int start, int length)
		{
			TextSource = textSource;
			Start = start;
			Length = length;
		}

		private TextSource TextSource { get; }

		private int Start { get; }

		private int Length { get; }

		public string GetText(int start)
		{
			return TextSource.GetText(Start + start, Length - start);
		}

		public string GetText()
		{
			return TextSource.GetText(Start, Length);
		}

		public string GetText(int start, int length)
		{
			return TextSource.GetText(Start + start, length);
		}

		public TextSourceSpan Slice(int start, int length)
		{
			if (start < 0 || start + length > Length)
				throw new ArgumentOutOfRangeException();

			return new TextSourceSpan(TextSource, Start + start, length);
		}

		public TextSourceSpan Slice(int start)
		{
			if (start < 0 || start > Length)
				throw new ArgumentOutOfRangeException();

			return new TextSourceSpan(TextSource, Start + start, Length - start);
		}

		public bool IsEmpty => TextSource == null;

		public ReadOnlyMemory<char> AsMemory()
		{
			return TextSource.GetTextMemory(Start, Length);
		}

		public ReadOnlyMemory<char> AsMemory(int start)
		{
			return TextSource.GetTextMemory(Start + start, Length - start);
		}

		public ReadOnlyMemory<char> AsMemory(int start, int length)
		{
			return TextSource.GetTextMemory(Start + start, length);
		}

		public int GetChar(int pointer)
		{
			return TextSource.GetChar(Start + pointer);
		}
	}
}