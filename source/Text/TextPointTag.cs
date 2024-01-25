// <copyright file="TextPointValue.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	public readonly struct TextPointTag<TTag>
	{
		public readonly TextPoint Point;
		public readonly TTag Tag;

		public TextPointTag(TextPoint point, TTag tag)
		{
			Point = point;
			Tag = tag;
		}

		public static TextPointTag<TTag> Empty => new(TextPoint.Empty, default);

		public bool IsEmpty => Point.IsEmpty;
	}

	public static class TextPointTag
	{
		public static TextPointTag<TTag> Create<TTag>(TextPoint point, TTag tag)
		{
			return new TextPointTag<TTag>(point, tag);
		}
	}
}