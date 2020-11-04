// <copyright file="ListViewSelectorController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
	internal sealed class ListViewSelectorController : SelectorController<ListViewControl, ListViewItem>
	{
		public ListViewSelectorController(ListViewControl listViewControl) : base(listViewControl, new ListViewSelectorAdvisor(listViewControl))
		{
		}
	}
}