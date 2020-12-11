// <copyright file="TableViewItemPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.UI.Panels;
using Zaaml.UI.Panels.Interfaces;
using Panel = Zaaml.UI.Panels.Core.Panel;

namespace Zaaml.UI.Controls.TableView
{
	public class TableViewItemPanel : Panel, IStackPanelAdvanced
	{
		private TableViewItem _item;

		internal TableViewItem Item
		{
			get => _item;
			set
			{
				if (ReferenceEquals(_item, value))
					return;

				_item = value;

				InvalidateMeasure();
			}
		}

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			var tableViewPanel = Item?.GetVisualParent() as TableViewPanel;

			if (tableViewPanel == null)
				return StackPanelLayout.Arrange(this, finalSize);

			var orientation = tableViewPanel.ItemsPresenter?.TableViewControl?.Orientation.Rotate() ?? Orientation.Horizontal;
			var offset = new OrientedPoint(orientation);
			var finalOriented = finalSize.AsOriented(orientation);
			var spacing = Item.TableViewControl?.ElementSpacing ?? 0;

			for (var index = 0; index < Children.Count; index++)
			{
				var child = Children[index];
				var definition = tableViewPanel.GetDefinition(index);
				var size = new OrientedSize
				{
					Direct = definition.DesiredSize,
					Indirect = finalOriented.Indirect
				};

				var rect = new Rect(offset.Point, size.Size);

				child.Arrange(rect);
				offset.Direct += size.Direct + spacing;
			}

			return finalSize;
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			var tableViewPanel = Item?.GetVisualParent() as TableViewPanel;

			if (tableViewPanel == null)
				return StackPanelLayout.Measure(this, availableSize);

			var orientation = tableViewPanel.ItemsPresenter?.TableViewControl?.Orientation.Rotate() ?? Orientation.Horizontal;
			var orientedResult = new OrientedSize(orientation);
			var orientedAvailable = new OrientedSize(orientation, availableSize);
			var starAsAuto = orientedAvailable.Direct.IsPositiveInfinity();

			for (var index = 0; index < Children.Count; index++)
			{
				var child = Children[index];
				var definition = tableViewPanel.GetDefinition(index);
				var childConstraint = new OrientedSize(orientation, XamlConstants.InfiniteSize);

				if (definition.Length.IsStar && starAsAuto == false)
					continue;

				if (definition.Length.IsAbsolute)
					childConstraint = childConstraint.ChangeDirect(definition.Length.Value);

				child.Measure(childConstraint.Size);

				var childDesired = new OrientedSize(orientation, child.DesiredSize);

				if (definition.Length.IsAbsolute)
					childDesired = childDesired.ChangeDirect(childConstraint.Direct);
				else
					definition.DesiredSize = Math.Max(definition.DesiredSize, childDesired.Direct);

				orientedResult = orientedResult.StackSize(childDesired.Size);
			}

			if (starAsAuto)
				return orientedResult.Size;

			if (tableViewPanel.CurrentMeasurePass == TableViewPanel.MeasurePass.Star)
			{
				for (var index = 0; index < Children.Count; index++)
				{
					var child = Children[index];
					var definition = tableViewPanel.GetDefinition(index);

					if (definition.Length.IsStar == false)
						continue;

					var childConstraint = new OrientedSize(orientation, XamlConstants.InfiniteSize).ChangeDirect(definition.DesiredSize);

					child.Measure(childConstraint.Size);

					var childDesired = new OrientedSize(orientation, child.DesiredSize);

					orientedResult = orientedResult.StackSize(childDesired.Size);
				}
			}

			orientedResult.Direct = orientedResult.Direct.Clamp(0, orientedAvailable.Direct);

			return orientedResult.Size;
		}

		public Orientation Orientation => Orientation.Horizontal;

		double IStackPanelAdvanced.Spacing => Item?.TableViewControl?.ElementSpacing ?? 0.0;
	}
}