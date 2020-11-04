// <copyright file="GridCellColumnController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;

namespace Zaaml.UI.Controls.Core
{
	public abstract class GridCellColumnController<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellPresenter : GridCellPresenter<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellPanel : GridCellPanel<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellCollection : GridCellCollection<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCell : GridCell<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellSplitter : GridCellSplitter<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellColumnController : GridCellColumnController<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellColumn : GridCellColumn<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
	{
		private List<TGridCellColumn> Columns { get; } = new List<TGridCellColumn>();

		protected abstract TGridCellColumn CreateColumn(int index);

		public TGridCellColumn GetColumn(int index)
		{
			while (Columns.Count <= index)
			{
				var column = CreateColumn(Columns.Count);

				column.Width = Columns.Count > 0 ? Columns.Sum(c => c.Width) / Columns.Count : 1.0;

				Columns.Add(column);
			}

			return Columns[index];
		}

		public double GetFlexWidth(int index)
		{
			return GetColumn(index).Width;
		}
	}
}