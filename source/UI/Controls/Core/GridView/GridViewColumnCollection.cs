// <copyright file="GridViewColumnCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Core.GridView
{
	public class GridViewColumnCollection<TGridColumn> : InheritanceContextDependencyObjectCollection<TGridColumn>
		where TGridColumn : GridViewColumn
	{
		public event EventHandler<GridViewColumnEventArgs<TGridColumn>> ColumnAdded;
		public event EventHandler<GridViewColumnEventArgs<TGridColumn>> ColumnRemoved;

		protected override void OnItemAdded(TGridColumn column)
		{
			base.OnItemAdded(column);

			ColumnAdded?.Invoke(this, new GridViewColumnEventArgs<TGridColumn>(column));
		}

		protected override void OnItemRemoved(TGridColumn column)
		{
			ColumnRemoved?.Invoke(this, new GridViewColumnEventArgs<TGridColumn>(column));

			base.OnItemRemoved(column);
		}
	}
}