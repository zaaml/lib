// <copyright file="TextLineTag.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	public readonly struct TextLineTag<TTag>
	{
		public readonly TextLine Line;
		public readonly TTag Tag;

		public TextLineTag(TextLine line, TTag tag)
		{
			Line = line;
			Tag = tag;
		}

		public static TextLineTag<TTag> Empty => new(TextLine.Empty, default);

		public bool IsEmpty => Line.IsEmpty;
	}

	public static class TextLineTag
	{
		public static TextLineTag<TTag> Create<TTag>(TextLine line, TTag tag)
		{
			return new TextLineTag<TTag>(line, tag);
		}
	}
}