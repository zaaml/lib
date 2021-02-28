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
using Zaaml.UI.Panels.Flexible;

namespace Zaaml.UI.Controls.Core
{
	public class GridCellSplitter : FixedTemplateControl<Panel>
	{
		private static readonly Brush Brush = new SolidColorBrush(Colors.Transparent);
		private bool _dragging;
		private double _leftWidth;
		private Point _origin;
		private double _rightWidth;

		static GridCellSplitter()
		{
			Brush.Freeze();

			UIElementUtils.OverrideFocusable<GridCellSplitter>(false);
			ControlUtils.OverrideIsTabStop<GridCellSplitter>(false);
			FrameworkElementUtils.OverrideVisualStyle<GridCellSplitter>(null);
		}

		public GridCellSplitter()
		{
			Cursor = Cursors.SizeWE;

			System.Windows.Controls.Panel.SetZIndex(this, 1);
		}

		private GridCellsPanel Panel => VisualParent as GridCellsPanel;

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

			foreach (var cell in panel.Children.OfType<GridCell>())
				width += cell.ColumnInternal.ActualWidth;

			foreach (var cell in panel.Children.OfType<GridCell>())
				cell.ColumnInternal.Width = new FlexLength(totalWidth * cell.ColumnInternal.ActualWidth / width, FlexLengthUnitType.Star);
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

			_leftWidth = ((GridCell) panel.Children[childIndex - 1]).ColumnInternal.ActualWidth;
			_rightWidth = ((GridCell) panel.Children[childIndex + 1]).ColumnInternal.ActualWidth;

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
			var leftColumn = ((GridCell) panel.Children[childIndex - 1]).ColumnInternal;
			var rightColumn = ((GridCell) panel.Children[childIndex + 1]).ColumnInternal;

			var left = Math.Max(40, _leftWidth + delta.X);
			var right = Math.Max(40, _rightWidth - delta.X);

			if (left < right)
			{
				leftColumn.ActualWidth = left;
				rightColumn.ActualWidth = (_leftWidth + _rightWidth) - left;
			}
			else
			{
				rightColumn.ActualWidth = right;
				leftColumn.ActualWidth = (_leftWidth + _rightWidth) - right;
			}
		}
	}
}