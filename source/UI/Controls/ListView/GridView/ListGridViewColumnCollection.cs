// <copyright file="ListGridViewColumnCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.ListView
{
	public sealed class ListGridViewColumnCollection : GridViewColumnCollection<ListGridViewColumn>
	{
		internal ListGridViewColumnCollection(ListGridView listGridView)
		{
			ListGridView = listGridView;
		}

		public ListGridView ListGridView { get; }

		protected override void OnItemAdded(ListGridViewColumn column)
		{
			base.OnItemAdded(column);

			if (column.View != null)
				throw new InvalidOperationException();

			column.View = ListGridView;
		}

		protected override void OnItemRemoved(ListGridViewColumn column)
		{
			if (ReferenceEquals(column.View, ListGridView) == false)
				throw new InvalidOperationException();

			column.View = null;

			base.OnItemRemoved(column);
		}
	}
}