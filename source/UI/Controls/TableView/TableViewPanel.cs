// <copyright file="TableViewPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore;
using Zaaml.UI.Panels;
using Zaaml.UI.Panels.Core;
using Zaaml.UI.Panels.Interfaces;

namespace Zaaml.UI.Controls.TableView
{
	public class TableViewPanel : ItemsPanel<TableViewItem>, IStackPanelAdvanced
	{
		internal MeasurePass CurrentMeasurePass { get; set; }

		private List<TableViewDefinition> Definitions { get; } = new List<TableViewDefinition>();

		private Stack<TableViewDefinition> DefinitionsPool { get; } = new Stack<TableViewDefinition>();

		internal TableViewItemsPresenter ItemsPresenter { get; set; }

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			return StackPanelLayout.Arrange(this, finalSize);
		}

		internal TableViewDefinition GetDefinition(int index)
		{
			while (index >= Definitions.Count)
				Definitions.Add(new TableViewDefinition());

			return Definitions[index];
		}

		private TableViewDefinition GetDefinition()
		{
			return DefinitionsPool.Count > 0
				? DefinitionsPool.Pop()
				: new TableViewDefinition
				{
					IsImplicit = true
				};
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			var tableViewControl = ItemsPresenter?.TableViewControl;

			if (tableViewControl == null)
				return StackPanelLayout.Measure(this, availableSize);

			foreach (var definition in Definitions)
			{
				if (definition.IsImplicit)
					DefinitionsPool.Push(definition);
			}

			Definitions.Clear();
			Definitions.AddRange(tableViewControl.Definitions);

			foreach (var tableViewItem in Children.OfType<TableViewItem>())
				while (Definitions.Count < tableViewItem.Elements.Count)
					Definitions.Add(GetDefinition());

			CurrentMeasurePass = MeasurePass.Fixed;

			var orientation = tableViewControl.Orientation;
			var orientedResult = new OrientedSize(orientation);
			var elementSpacing = tableViewControl.ElementSpacing;
			var definitionsSpacing = elementSpacing * (Definitions.Count - 1);
			var orientedAvailable = new OrientedSize(orientation, availableSize);

			orientedAvailable.Indirect -= definitionsSpacing;

			var constraint = orientedAvailable.ChangeDirect(double.PositiveInfinity).Size;
			var defaultDefinitionLength = tableViewControl.DefinitionLength;

			for (var index = 0; index < Definitions.Count; index++)
			{
				var definition = GetDefinition(index);

				if (definition.IsImplicit)
					definition.Length = defaultDefinitionLength;

				definition.DesiredSize = definition.Length.IsAbsolute ? definition.Length.Value : 0;
			}

			for (var index = 0; index < Children.Count; index++)
			{
				var child = Children[index];

				if (child is TableViewItem tableViewItem)
					tableViewItem.FixedMeasure(constraint);
				else
					child.Measure(constraint);

				orientedResult = orientedResult.StackSize(child.DesiredSize);
			}

			var fixedIndirect = 0.0;

			foreach (var definition in Definitions)
				fixedIndirect += definition.DesiredSize;

			var starLengthValue = 0.0;

			for (var index = 0; index < Definitions.Count; index++)
			{
				var definition = Definitions[index];

				if (definition.Length.IsStar)
					starLengthValue += definition.Length.Value;
			}

			var starAvailable = Math.Max(0, orientedAvailable.Indirect - fixedIndirect);
			var starLength = starAvailable / starLengthValue;

			CurrentMeasurePass = MeasurePass.Star;

			for (var index = 0; index < Definitions.Count; index++)
			{
				var definition = Definitions[index];

				if (definition.Length.IsStar)
					definition.DesiredSize = definition.Length.Value * starLength;
			}

			orientedResult = new OrientedSize(orientation);

			for (var index = 0; index < Children.Count; index++)
			{
				var child = Children[index];

				if (child is TableViewItem tableViewItem)
					tableViewItem.StarMeasure(constraint);
				else
					child.Measure(constraint);

				orientedResult = orientedResult.StackSize(child.DesiredSize);
			}

			orientedResult.Indirect = fixedIndirect + definitionsSpacing + starAvailable;

			CurrentMeasurePass = MeasurePass.Undefined;

			return orientedResult.Size;
		}

		Orientation IOrientedPanel.Orientation => Orientation.Vertical;

		double IStackPanelAdvanced.Spacing => ItemsPresenter?.TableViewControl?.ItemSpacing ?? 0;

		internal enum MeasurePass
		{
			Undefined,
			Fixed,
			Star
		}
	}
}