// <copyright file="ListViewItemGridCellElement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
	public class ListViewItemGridCellElement : GridElement
	{
		static ListViewItemGridCellElement()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ListViewItemGridCellElement>();
		}

		public ListViewItemGridCellElement()
		{
			this.OverrideStyleKey<ListViewItemGridCellElement>();
		}
	}
}