// <copyright file="ResizeAdorner.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Artboard
{
	public class ArtboardResizeAdorner : ArtboardAdorner
	{
		static ArtboardResizeAdorner()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ArtboardResizeAdorner>();
		}

		public ArtboardResizeAdorner()
		{
			this.OverrideStyleKey<ArtboardResizeAdorner>();
		}
	}
}