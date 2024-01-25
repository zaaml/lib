// <copyright file="GridViewController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using Zaaml.UI.Controls.ScrollView;

namespace Zaaml.UI.Controls.Core.GridView
{
	public abstract class GridViewController
	{
		private GridViewColumnController _columnController;
		private AxisScrollInfo _scrollInfo;

		protected GridViewController(FrameworkElement owner)
		{
			Owner = owner;
			Owner.LayoutUpdated += OnLayoutUpdated;
		}

		protected abstract IEnumerable<GridViewCellsPresenter> CellsPresenters { get; }

		internal IEnumerable<GridViewCellsPresenter> CellsPresentersInternal => CellsPresenters;

		public GridViewColumnController ColumnController => _columnController ??= CreateColumnController();

		protected abstract GridViewCellsPresenter HeaderCellsPresenter { get; }

		internal GridViewCellsPresenter HeaderCellsPresenterInternal => HeaderCellsPresenter;

		private GridViewCellsPanel LastMeasurePanel { get; set; }

		internal long LayoutVersion { get; private set; }

		public FrameworkElement Owner { get; }

		internal AxisScrollInfo ScrollInfo
		{
			get => _scrollInfo;
			set
			{
				if (_scrollInfo.IsCloseTo(value))
					return;

				_scrollInfo = value;

				InvalidateCellsPresentersArrange();
			}
		}

		protected abstract GridViewColumnController CreateColumnController();

		internal void InvalidateCellsPresentersArrange()
		{
			HeaderCellsPresenter?.CellsPanelInternal?.InvalidateArrange();

			foreach (var cellsPresenter in CellsPresenters)
				cellsPresenter.CellsPanelInternal?.InvalidateArrange();
		}

		internal void InvalidateCellsPresentersMeasure()
		{
			HeaderCellsPresenter?.CellsPanelInternal?.InvalidateMeasure();

			foreach (var cellsPresenter in CellsPresenters)
				cellsPresenter.CellsPanelInternal?.InvalidateMeasure();
		}

		internal void InvalidateMeasuredPanels()
		{
			HeaderCellsPresenter?.CellsPanelInternal?.InvalidatePanelMeasureInternal();

			var panel = LastMeasurePanel;

			while (panel != null)
			{
				panel.InvalidatePanelMeasureInternal();

				panel = panel.PrevCellsPanel;
			}
		}

		private void OnLayoutUpdated(object sender, EventArgs e)
		{
			LayoutVersion++;

			var panel = LastMeasurePanel;

			while (panel != null)
			{
				var nextPanel = panel.PrevCellsPanel;

				panel.PrevCellsPanel = null;

				panel = nextPanel;
			}

			LastMeasurePanel = null;
		}

		internal void OnPanelMeasured(GridViewCellsPanel gridCellsPanel)
		{
			if (gridCellsPanel.MeasureLayoutVersion == LayoutVersion)
				return;

			gridCellsPanel.PrevCellsPanel = LastMeasurePanel;
			gridCellsPanel.MeasureLayoutVersion = LayoutVersion;

			LastMeasurePanel = gridCellsPanel;
		}
	}
}