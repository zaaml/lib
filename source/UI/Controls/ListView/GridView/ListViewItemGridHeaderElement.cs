// <copyright file="ListViewItemGridHeaderElement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
	public class ListViewItemGridHeaderElement : GridElement
	{
		static ListViewItemGridHeaderElement()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ListViewItemGridHeaderElement>();
		}

		public ListViewItemGridHeaderElement()
		{
			this.OverrideStyleKey<ListViewItemGridHeaderElement>();
		}
	}
}