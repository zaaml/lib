// <copyright file="TextLine.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace Zaaml.Text
{
	public readonly struct TextLine
	{
		public static TextLine Empty => new(true);
		public bool IsEmpty => Index == -1;

		public readonly TextSource TextSource;
		public readonly int Index;

		public TextLine(int index)
		{
			TextSource = default;
			Index = index;
		}

		public TextLine(int index, TextSource textSource)
		{
			TextSource = textSource;
			Index = index;
		}

		private TextLine(bool empty)
		{
			Debug.Assert(empty);

			TextSource = default;
			Index = -1;
		}

		public TextLineTag<TTag> WithTag<TTag>(TTag tag)
		{
			return TextLineTag.Create(this, tag);
		}
	}
}