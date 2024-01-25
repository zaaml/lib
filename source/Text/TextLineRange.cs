// <copyright file="TextLineRange.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	public readonly struct TextLineRange
	{
		public TextLineRange(TextLine startLine, TextLine endLine)
		{
			if (ReferenceEquals(startLine.TextSource, endLine.TextSource) == false)
				throw new ArgumentException("Different sources");

			if (startLine.Index > endLine.Index)
				throw new ArgumentException("Invalid lines order");

			TextSource = startLine.TextSource;
			StartLineIndex = startLine.Index;
			EndLineIndex = endLine.Index;
		}

		internal TextLineRange(int startLineIndex, int endLineIndex, TextSource textSource)
		{
			TextSource = textSource;
			StartLineIndex = startLineIndex;
			EndLineIndex = endLineIndex;
		}

		private TextLineRange(int index)
		{
			StartLineIndex = EndLineIndex = index;
		}

		public TextSource TextSource { get; }

		public int StartLineIndex { get; }

		public int EndLineIndex { get; }

		public int LineCount => IsEmpty ? 0 : EndLineIndex - StartLineIndex + 1;

		public bool IsEmpty => StartLineIndex == -1;

		public static readonly TextLineRange Empty = new(-1);

		public TextLine StartLine => IsEmpty ? TextLine.Empty : new TextLine(StartLineIndex, TextSource);

		public TextLine EndLine => IsEmpty ? TextLine.Empty : new TextLine(EndLineIndex, TextSource);
	}
}