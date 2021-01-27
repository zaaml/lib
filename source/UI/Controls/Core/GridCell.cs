// <copyright file="GridCell.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.Core;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.UI.Controls.Core
{
	public abstract class GridCell : TemplateContractControl
	{
		private UIElement _childCore;

		static GridCell()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<GridCell>();
		}

		protected GridCell()
		{
			this.OverrideStyleKey<GridCell>();
		}

		protected Border Border => TemplateContract.Border;

		internal GridCellsPanel CellsPanelInternal => CellsPresenterInternal?.CellsPanelInternal;

		internal GridCellsPresenter CellsPresenterInternal { get; set; }

		protected UIElement ChildCore
		{
			get => _childCore;
			set
			{
				if (ReferenceEquals(_childCore, value))
					return;

				_childCore = value;

				var border = Border;

				if (border != null)
					border.Child = _childCore;

				OnChildCoreChanged();
			}
		}

		protected virtual void OnChildCoreChanged()
		{
		}

		internal GridColumn ColumnInternal => ControllerInternal?.GetColumn(this);

		internal GridController ControllerInternal => CellsPresenterInternal?.ControllerInternal;

		internal int Index { get; set; }

		private GridCellTemplateContract TemplateContract => (GridCellTemplateContract) TemplateContractInternal;

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			if (Border != null && ChildCore != null)
				Border.Child = ChildCore;
		}

		protected override void OnTemplateContractDetaching()
		{
			if (Border != null)
				Border.Child = null;

			base.OnTemplateContractDetaching();
		}

		internal void UpdateStructureInternal(bool force)
		{
			UpdateStructure();
		}

		protected virtual void UpdateStructure()
		{
		}

		protected override void UpdateVisualState(bool useTransitions)
		{
			var isMouseOver = IsMouseOver;

			// Common states
			if (!IsEnabled)
				GotoVisualState(CommonVisualStates.Disabled, useTransitions);
			else if (isMouseOver)
				GotoVisualState(CommonVisualStates.MouseOver, useTransitions);
			else
				GotoVisualState(CommonVisualStates.Normal, useTransitions);
		}
	}

	public abstract class GridCell<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell> : GridCell
		where TGridCellsPresenter : GridCellsPresenter<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellsPanel : GridCellsPanel<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellCollection : GridCellCollection<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCell : GridCell<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
	{
		static GridCell()
		{
			UIElementUtils.OverrideFocusable<GridCell<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>>(false);
			ControlUtils.OverrideIsTabStop<GridCell<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>>(false);
			FrameworkElementUtils.OverrideVisualStyle<GridCell<TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>>(null);
		}
	}

	public abstract class GridCellTemplateContract : TemplateContract
	{
		[TemplateContractPart(Required = false)]
		public Border Border { get; [UsedImplicitly] private set; }
	}
}