// <copyright file="ListGridViewColumnCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
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

		protected override void OnItemAdded(ListGridViewColumn column)
		{
			base.OnItemAdded(column);

			if (column.ListGridView != null)
				throw new InvalidOperationException();

			column.ListGridView = ListGridView;
		}

		protected override void OnItemRemoved(ListGridViewColumn column)
		{
			if (ReferenceEquals(column.ListGridView, ListGridView) == false)
				throw new InvalidOperationException();

			column.ListGridView = null;

			base.OnItemRemoved(column);
		}
	}
}