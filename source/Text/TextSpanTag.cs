// <copyright file="TextSpanTag.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	public readonly struct TextSpanTag<T>
	{
		public readonly TextSpan Span;
		public readonly T Tag;

		public TextSpanTag(TextSpan span, T tag)
		{
			Span = span;
			Tag = tag;
		}

		public static TextSpanTag<T> Empty => new(TextSpan.Empty, default);

		public bool IsEmpty => Span.IsEmpty;
	}

	public static class TextSpanTag
	{
		public static TextSpanTag<TTag> Create<TTag>(TextSpan span, TTag tag)
		{
			return new TextSpanTag<TTag>(span, tag);
		}
	}
}