// <copyright file="ListGridViewColumnCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.ListView
{
	public sealed class ListGridViewColumnCollection : InheritanceContextDependencyObjectCollection<ListGridViewColumn>
	{
		internal ListGridViewColumnCollection(ListGridView listGridView)
		{
			ListGridView = listGridView;
		}

		public ListGridView ListGridView { get; }
	}
}