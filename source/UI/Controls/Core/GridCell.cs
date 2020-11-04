// <copyright file="GridCell.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.Core.Weak;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Core
{
	public abstract class GridCell<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn> : FixedTemplateContentControl<Panel, FrameworkElement>
		where TGridCellPresenter : GridCellPresenter<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellPanel : GridCellPanel<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellCollection : GridCellCollection<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCell : GridCell<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellSplitter : GridCellSplitter<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellColumnController : GridCellColumnController<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellColumn : GridCellColumn<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
	{
		private System.WeakReference<TGridCellColumn> _weakColumn;
		private IDisposable _weakEventDisposer;

		static GridCell()
		{
			UIElementUtils.OverrideFocusable<GridCell<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>>(false);
			ControlUtils.OverrideIsTabStop<GridCell<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>>(false);
			FrameworkElementUtils.OverrideVisualStyle<GridCell<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>>(null);
		}

		internal TGridCellColumn Column
		{
			get
			{
				if (_weakColumn == null)
					return null;

				return _weakColumn.TryGetTarget(out var column) ? column : null;
			}
			set
			{
				if (ReferenceEquals(Column, value))
					return;

				if (value != null)
				{
					_weakColumn = new System.WeakReference<TGridCellColumn>(value);
					_weakEventDisposer = _weakEventDisposer.DisposeExchange(this.CreateWeakEventListener((t, o, e) => t.OnColumnWidthChanged(o, e), h => value.WidthChanged += h, h => value.WidthChanged -= h));
				}
				else
				{
					_weakColumn = null;
					_weakEventDisposer = _weakEventDisposer.DisposeExchange();
				}
			}
		}

		private void OnColumnWidthChanged(object sender, EventArgs e)
		{
			InvalidateMeasure();

			(VisualParent as TGridCellPanel)?.InvalidateMeasure();
		}
	}
}