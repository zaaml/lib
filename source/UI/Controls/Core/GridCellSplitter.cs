// <copyright file="GridCellSplitter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Panels.Core;
using Zaaml.UI.Utils;

namespace Zaaml.UI.Controls.Core
{
	public abstract class GridCellSplitter<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn> : FixedTemplateControl<Panel>
		where TGridCellPresenter : GridCellPresenter<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellPanel : GridCellPanel<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellCollection : GridCellCollection<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCell : GridCell<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellSplitter : GridCellSplitter<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellColumnController : GridCellColumnController<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
		where TGridCellColumn : GridCellColumn<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>
	{
		private static readonly Brush Brush = new SolidColorBrush(Colors.Transparent);
		private bool _dragging;
		private double _leftWidth;
		private Point _origin;
		private double _rightWidth;

		protected GridCellSplitter()
		{
			Cursor = Cursors.SizeWE;

			System.Windows.Controls.Panel.SetZIndex(this, 1);
		}

		static GridCellSplitter()
		{
			Brush.Freeze();

			UIElementUtils.OverrideFocusable<GridCellSplitter<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>>(false);
			ControlUtils.OverrideIsTabStop<GridCellSplitter<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>>(false);
			FrameworkElementUtils.OverrideVisualStyle<GridCellSplitter<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>>(null);
		}

		private GridCellPanel<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn> Panel =>
			VisualParent as GridCellPanel<TGridCellPresenter, TGridCellPanel, TGridCellCollection, TGridCell, TGridCellSplitter, TGridCellColumnController, TGridCellColumn>;

		protected override void ApplyTemplateOverride()
		{
			base.ApplyTemplateOverride();

			TemplateRoot.Background = Brush;
			TemplateRoot.Cursor = Cursors.SizeWE;
			TemplateRoot.Width = 6;
			TemplateRoot.Margin = new Thickness(0, -1, 0, -1);
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			var finalRect = new Rect(new Size(6, finalSize.Height));

			finalRect.Offset(-3, 0);

			TemplateRoot.Arrange(finalRect);

			return finalSize;
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			base.MeasureOverride(availableSize);

			return new Size(0, 0);
		}

		private void Normalize()
		{
			var panel = Panel;

			if (panel == null)
				return;

			var width = 0.0;
			var totalWidth = panel.ActualWidth;

			foreach (var cell in panel.Children.OfType<TGridCell>())
				width += cell.Column.Width;

			foreach (var cell in panel.Children.OfType<TGridCell>())
				cell.Column.Width = totalWidth * cell.Column.Width / width;
		}

		protected override void OnLostMouseCapture(MouseEventArgs e)
		{
			base.OnLostMouseCapture(e);

			_dragging = false;
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			var panel = Panel;

			if (panel == null)
				return;

			Normalize();

			var childIndex = panel.Children.IndexOf(this);

			_leftWidth = ((TGridCell) panel.Children[childIndex - 1]).Column.Width;
			_rightWidth = ((TGridCell) panel.Children[childIndex + 1]).Column.Width;

			_dragging = CaptureMouse();

			if (_dragging == false)
				return;

			_origin = e.GetPosition(panel);
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonUp(e);

			if (_dragging)
				ReleaseMouseCapture();

			_dragging = false;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (_dragging == false)
				return;

			var panel = Panel;

			if (panel == null)
				return;

			var location = e.GetPosition(panel);
			var delta = location - _origin;
			var childIndex = panel.Children.IndexOf(this);
			var leftColumn = ((TGridCell) panel.Children[childIndex - 1]).Column;
			var rightColumn = ((TGridCell) panel.Children[childIndex + 1]).Column;

			var left = Math.Max(40, _leftWidth + delta.X);
			var right = Math.Max(40, _rightWidth - delta.X);

			if (left < right)
			{
				leftColumn.Width = left;
				rightColumn.Width = (_leftWidth + _rightWidth) - left;
			}
			else
			{
				rightColumn.Width = right;
				leftColumn.Width = (_leftWidth + _rightWidth) - right;
			}
		}
	}
}