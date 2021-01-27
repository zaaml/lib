// <copyright file="GridHeaderElement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Core
{
	public class GridHeaderElement : Control
	{
		static GridHeaderElement()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<GridHeaderElement>();
		}

		protected GridHeaderElement()
		{
			this.OverrideStyleKey<GridHeaderElement>();
		}
	}
}