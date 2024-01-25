// <copyright file="TemplatedGridViewCellGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using Zaaml.UI.Controls.Core.GridView;

namespace Zaaml.UI.Controls.Core
{
	internal sealed class TemplatedGridViewCellGenerator<TGridCell> : GridViewCellGenerator<TGridCell> where TGridCell : GridViewCell, new()
	{
		private readonly Stack<TGridCell> _cellsPool = new();
		private DataTemplate _cellTemplate;

		public TemplatedGridViewCellGenerator(GridViewCellGenerator<TGridCell> generator)
		{
			Generator = generator;
		}

		public GridViewCellGenerator<TGridCell> Generator { get; }

		public DataTemplate CellTemplate
		{
			get => _cellTemplate;
			set
			{
				if (ReferenceEquals(_cellTemplate, value))
					return;

				Generator.OnGeneratorChangingInternal();

				_cellTemplate = value;
				_cellsPool.Clear();

				Generator.OnGeneratorChangedInternal();
			}
		}

		protected override TGridCell CreateCell()
		{
			if (_cellsPool.Count > 0)
				return _cellsPool.Pop();

			var itemTemplate = CellTemplate;

			if (itemTemplate == null)
				return new TGridCell();

			return (TGridCell)itemTemplate.LoadContent();
		}

		protected override void DisposeCell(TGridCell item)
		{
			_cellsPool.Push(item);
		}
	}
}