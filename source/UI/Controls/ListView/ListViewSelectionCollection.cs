// <copyright file="ListViewSelectionCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
	public sealed class ListViewSelectionCollection : SelectionCollectionBase<ListViewItem>
	{
		internal ListViewSelectionCollection(ListViewSelectorController selectorController) : base(selectorController)
		{
		}
	}
}