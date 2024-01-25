// <copyright file="Alignment.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore
{
	public struct Alignment
	{
		public static readonly Alignment LeftTop = new() { Horizontal = HorizontalAlignment.Left, Vertical = VerticalAlignment.Top };
		public static readonly Alignment CenterTop = new() { Horizontal = HorizontalAlignment.Center, Vertical = VerticalAlignment.Top };
		public static readonly Alignment RightTop = new() { Horizontal = HorizontalAlignment.Right, Vertical = VerticalAlignment.Top };
		public static readonly Alignment RightCenter = new() { Horizontal = HorizontalAlignment.Right, Vertical = VerticalAlignment.Center };
		public static readonly Alignment RightBottom = new() { Horizontal = HorizontalAlignment.Right, Vertical = VerticalAlignment.Bottom };
		public static readonly Alignment CenterBottom = new() { Horizontal = HorizontalAlignment.Center, Vertical = VerticalAlignment.Bottom };
		public static readonly Alignment LeftBottom = new() { Horizontal = HorizontalAlignment.Left, Vertical = VerticalAlignment.Bottom };
		public static readonly Alignment LeftCenter = new() { Horizontal = HorizontalAlignment.Left, Vertical = VerticalAlignment.Center };
		public static readonly Alignment CenterCenter = new() { Horizontal = HorizontalAlignment.Center, Vertical = VerticalAlignment.Center };

		public HorizontalAlignment Horizontal;
		public VerticalAlignment Vertical;
	}
}