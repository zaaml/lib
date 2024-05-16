// <copyright file="TableViewPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Panels;
using Zaaml.UI.Panels.Core;
using Zaaml.UI.Panels.Interfaces;

namespace Zaaml.UI.Controls.TableView
{
	public class TableViewPanel : ItemsPanel<TableViewItem>, IStackPanelAdvanced
	{
		private TableViewItem _footerItem;
		private TableViewItem _headerItem;
		private double _itemIndirect;

		internal MeasurePass CurrentMeasurePass { get; set; }

		private List<TableViewDefinition> Definitions { get; } = [];

		private Stack<TableViewDefinition> DefinitionsPool { get; } = [];

		internal TableViewItem FooterItem
		{
			get => _footerItem;
			set
			{
				if (ReferenceEquals(_footerItem, value))
					return;

				if (_footerItem != null && value != null)
				{
					LogicalChildMentor.DetachLogical(_footerItem);

					Children[Children.Count - 1] = value;

					LogicalChildMentor.AttachLogical(_footerItem);

					_footerItem = value;
				}
				else
				{
					if (_footerItem != null)
					{
						Children.RemoveAt(Children.Count - 1);

						LogicalChildMentor.AttachLogical(_footerItem);
					}

					_footerItem = value;

					LogicalChildMentor.DetachLogical(_footerItem);

					if (_footerItem != null)
						Children.Add(_footerItem);
				}
			}
		}

		internal TableViewItem HeaderItem
		{
			get => _headerItem;
			set
			{
				if (ReferenceEquals(_headerItem, value))
					return;

				if (_headerItem != null && value != null)
				{
					LogicalChildMentor.DetachLogical(_headerItem);

					Children[0] = value;

					LogicalChildMentor.AttachLogical(_headerItem);

					_headerItem = value;
				}
				else
				{
					if (_headerItem != null)
					{
						Children.RemoveAt(0);

						LogicalChildMentor.AttachLogical(_headerItem);
					}

					_headerItem = value;

					LogicalChildMentor.DetachLogical(_headerItem);

					if (_headerItem != null)
						Children.Insert(0, _headerItem);
				}
			}
		}

		internal void SetItemAvailableIndirectSize(TableViewItemPanel itemPanel, double indirect)
		{
			_itemIndirect = Math.Min(_itemIndirect, indirect);
		}

		internal TableViewItemsPresenter ItemsPresenter { get; set; }

		internal Orientation Orientation  => ItemsPresenter?.TableViewControl?.Orientation.Rotate() ?? Orientation.Horizontal;

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			return StackPanelLayout.Arrange(this, finalSize);
		}

		internal override ItemHostCollection<TableViewItem> CreateHostCollectionInternal()
		{
			return new TableViewPanelHostCollection(this);
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

		private void InitDefinitions(TableViewControl tableViewControl)
		{
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

			var defaultDefinitionLength = tableViewControl.DefinitionLength;

			for (var index = 0; index < Definitions.Count; index++)
			{
				var definition = GetDefinition(index);

				if (definition.IsImplicit)
					definition.Length = defaultDefinitionLength;

				definition.DesiredSize = definition.Length.IsAbsolute ? definition.Length.Value : 0;
			}
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			var tableViewControl = ItemsPresenter?.TableViewControl;

			if (tableViewControl == null)
				return StackPanelLayout.Measure(this, availableSize);

			_itemIndirect = int.MaxValue;

			InitDefinitions(tableViewControl);

			var orientation = tableViewControl.Orientation;
			var orientedResult = new OrientedSize(orientation);
			var elementSpacing = tableViewControl.ElementSpacing;
			var definitionsSpacing = elementSpacing * (Definitions.Count - 1);
			var orientedAvailable = new OrientedSize(orientation, availableSize);

			orientedAvailable.Indirect -= definitionsSpacing;

			var constraint = orientedAvailable.ChangeDirect(double.PositiveInfinity).Size;

			// Fixed measure pass
			CurrentMeasurePass = MeasurePass.Fixed;

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

			var actualAvailableIndirect = Math.Min(orientedAvailable.Indirect, _itemIndirect);
			var starAvailable = Math.Max(0, actualAvailableIndirect - fixedIndirect);
			var starLength = starAvailable / starLengthValue;

			// Star measure pass
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

		private sealed class TableViewPanelHostCollection : PanelHostCollectionBase<TableViewItem, TableViewPanel>
		{
			public TableViewPanelHostCollection(TableViewPanel panel) : base(panel)
			{
			}

			protected override UIElementCollectionSpan Children
			{
				get
				{
					var start = Panel.HeaderItem == null ? 0 : 1;
					var length = (Panel.FooterItem == null ? Panel.Children.Count : Panel.Children.Count - 1) - start;

					return new UIElementCollectionSpan(Panel.Children, start, length);
				}
			}
		}
	}
}