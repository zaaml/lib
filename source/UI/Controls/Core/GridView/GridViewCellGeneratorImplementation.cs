// <copyright file="GridCellGeneratorImplementation.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.UI.Controls.Core.GridView
{
	internal class GridViewCellGeneratorImplementation<TGridCell>
		where TGridCell : GridViewCell, new()
	{
		private readonly Stack<TGridCell> _cellsPool = new();
		private readonly List<GridViewCellGeneratorTargetProperty<TGridCell>> _generatorProperties;

		public GridViewCellGeneratorImplementation(params GridViewCellGeneratorTargetProperty<TGridCell>[] properties)
		{
			_generatorProperties = new List<GridViewCellGeneratorTargetProperty<TGridCell>>(properties);
		}

		public TGridCell CreateCell()
		{
			var gridCell = _cellsPool.Count > 0 ? _cellsPool.Pop() : new TGridCell();

			foreach (var generatorProperty in _generatorProperties)
				generatorProperty.SetPropertyValueInternal(gridCell);

			return gridCell;
		}

		public void DisposeCell(TGridCell gridCell)
		{
			foreach (var generatorProperty in _generatorProperties)
				generatorProperty.ClearPropertyValueInternal(gridCell);

			_cellsPool.Push(gridCell);
		}
	}
}