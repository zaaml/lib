// <copyright file="PopupBar.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	public class PopupBar : PopupBarBase
	{
		static PopupBar()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<PopupBar>();
		}

		public PopupBar()
		{
			this.OverrideStyleKey<PopupBar>();
		}
	}
}