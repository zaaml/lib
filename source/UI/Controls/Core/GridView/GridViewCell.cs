// <copyright file="GridViewCell.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using Zaaml.Core;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.PropertyCore.Extensions;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;

namespace Zaaml.UI.Controls.Core.GridView
{
	[ContentProperty(nameof(Content))]
	public abstract class GridViewCell : GridViewElement
	{
		private static readonly DependencyPropertyKey CellIndexPropertyKey = DPM.RegisterReadOnly<int, GridViewCell>
			("CellIndex", -1, d => d.OnCellIndexPropertyChangedPrivate);

		public static readonly DependencyProperty CellIndexProperty = CellIndexPropertyKey.DependencyProperty;

		public static readonly DependencyProperty ContentProperty = DPM.Register<object, GridViewCell>
			("Content", d => d.OnContentPropertyChangedPrivate);

		public static readonly DependencyProperty DisplayContentTemplateProperty = DPM.Register<DataTemplate, GridViewCell>
			("DisplayContentTemplate", d => d.OnDisplayContentTemplatePropertyChangedPrivate);

		public static readonly DependencyProperty EditContentTemplateProperty = DPM.Register<DataTemplate, GridViewCell>
			("EditContentTemplate", d => d.OnEditContentTemplatePropertyChangedPrivate);

		private static readonly DependencyPropertyKey ModePropertyKey = DPM.RegisterReadOnly<GridViewCellMode, GridViewCell>
			("Mode", GridViewCellMode.Display, d => d.OnModePropertyChangedPrivate, d => d.OnCoerceMode);

		public static readonly DependencyProperty EditModeTriggerProperty = DPM.Register<GridViewCellEditModeTrigger, GridViewCell>
			("EditModeTrigger", GridViewCellEditModeTrigger.Focus, d => d.OnEditModeTriggerPropertyChangedPrivate);

		public static readonly DependencyProperty DisplayPaddingProperty = DPM.Register<Thickness, GridViewCell>
			("DisplayPadding", default, d => d.OnDisplayPaddingPropertyChangedPrivate);

		public static readonly DependencyProperty EditPaddingProperty = DPM.Register<Thickness, GridViewCell>
			("EditPadding", default, d => d.OnEditPaddingPropertyChangedPrivate);

		public static readonly DependencyProperty ModeProperty = ModePropertyKey.DependencyProperty;

		private static readonly HashSet<DependencyProperty> BorderProperties = new()
		{
			BackgroundProperty,
			BorderBrushProperty,
			BorderThicknessProperty
		};

		private GridViewCellMode? _viewModeRequest;

		static GridViewCell()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<GridViewCell>();
		}

		protected GridViewCell()
		{
			this.OverrideStyleKey<GridViewCell>();

			DisplayContentPresenter.HorizontalAlignment = HorizontalAlignment.Stretch;
			DisplayContentPresenter.VerticalAlignment = VerticalAlignment.Stretch;
			DisplayContentPresenter.HorizontalContentAlignment = HorizontalAlignment.Stretch;
			DisplayContentPresenter.VerticalContentAlignment = VerticalAlignment.Stretch;

			EditContentPresenter.HorizontalAlignment = HorizontalAlignment.Stretch;
			EditContentPresenter.VerticalAlignment = VerticalAlignment.Stretch;
			EditContentPresenter.HorizontalContentAlignment = HorizontalAlignment.Stretch;
			EditContentPresenter.VerticalContentAlignment = VerticalAlignment.Stretch;
		}

		private IconContentPresenter ActualContentPresenter => Mode == GridViewCellMode.Display ? DisplayContentPresenter : EditContentPresenter;

		internal Size AutoDesiredSize { get; set; }

		public int CellIndex
		{
			get => (int)GetValue(CellIndexProperty);
			internal set => this.SetReadOnlyValue(CellIndexPropertyKey, value);
		}

		protected GridViewCellPresenter CellPresenter => TemplateContract.CellPresenter;

		internal GridViewCellsPanel CellsPanelInternal => CellsPresenterInternal?.CellsPanelInternal;

		internal GridViewCellsPresenter CellsPresenterInternal { get; set; }

		internal GridViewColumnController ColumnControllerInternal => CellsPresenterInternal?.ColumnControllerInternal;

		public object Content
		{
			get => GetValue(ContentProperty);
			set => SetValue(ContentProperty, value);
		}

		protected GridViewCellDisplayPresenter DisplayContentPresenter { get; } = new();

		public DataTemplate DisplayContentTemplate
		{
			get => (DataTemplate)GetValue(DisplayContentTemplateProperty);
			set => SetValue(DisplayContentTemplateProperty, value);
		}

		public Thickness DisplayPadding
		{
			get => (Thickness)GetValue(DisplayPaddingProperty);
			set => SetValue(DisplayPaddingProperty, value);
		}

		internal GridViewCellDisplayPresenter DisplayPresenterInternal => DisplayContentPresenter;

		protected GridViewCellDisplayPresenter EditContentPresenter { get; } = new();

		public GridViewCellEditModeTrigger EditModeTrigger
		{
			get => (GridViewCellEditModeTrigger)GetValue(EditModeTriggerProperty);
			set => SetValue(EditModeTriggerProperty, value);
		}

		public Thickness EditPadding
		{
			get => (Thickness)GetValue(EditPaddingProperty);
			set => SetValue(EditPaddingProperty, value);
		}

		protected abstract GridViewColumn GridColumnCore { get; }

		internal GridViewColumn GridColumnCoreInternal => GridColumnCore;

		public GridViewCellMode Mode
		{
			get => (GridViewCellMode)GetValue(ModeProperty);
			protected set => this.SetReadOnlyValue(ModePropertyKey, value.Box());
		}

		private IconContentPresenter NonActualContentPresenter => Mode == GridViewCellMode.Display ? EditContentPresenter : DisplayContentPresenter;

		private GridCellTemplateContract TemplateContract => (GridCellTemplateContract)TemplateContractCore;

		private protected virtual void HideEditor()
		{
			Mode = GridViewCellMode.Display;
		}

		private void OnCellIndexPropertyChangedPrivate(int oldValue, int newValue)
		{
		}

		private GridViewCellMode OnCoerceMode(GridViewCellMode mode)
		{
			if (mode == GridViewCellMode.Display)
			{
				if (this.HasKeyboardFocus())
				{
					_viewModeRequest = mode;

					mode = GridViewCellMode.Edit;
				}
				else
					_viewModeRequest = null;
			}
			else
				_viewModeRequest = null;

			return mode;
		}

		private void OnContentPropertyChangedPrivate(object oldValue, object newValue)
		{
			UpdateContentPresenter();
		}

		private void OnDisplayContentTemplatePropertyChangedPrivate(DataTemplate oldValue, DataTemplate newValue)
		{
			DisplayContentPresenter.ContentTemplate = newValue;
		}

		private void OnDisplayPaddingPropertyChangedPrivate(Thickness oldValue, Thickness newValue)
		{
			UpdateDisplayPadding();
		}

		private void OnEditContentTemplatePropertyChangedPrivate(DataTemplate oldValue, DataTemplate newValue)
		{
			EditContentPresenter.ContentTemplate = newValue;
		}

		private void OnEditModeTriggerPropertyChangedPrivate(GridViewCellEditModeTrigger oldValue, GridViewCellEditModeTrigger newValue)
		{
		}

		private void OnEditPaddingPropertyChangedPrivate(Thickness oldValue, Thickness newValue)
		{
			UpdateEditPadding();
		}

		protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			if (Mode == GridViewCellMode.Edit && _viewModeRequest == GridViewCellMode.Display)
				HideEditor();

			base.OnLostKeyboardFocus(e);
		}

		private void OnModePropertyChangedPrivate(GridViewCellMode oldValue, GridViewCellMode newValue)
		{
			SetContentPresenter();
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);

			if (EditModeTrigger == GridViewCellEditModeTrigger.MouseOver)
				ShowEditor();
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			if (EditModeTrigger == GridViewCellEditModeTrigger.MouseOver)
				HideEditor();

			base.OnMouseLeave(e);
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (BorderProperties.Contains(e.Property))
				CellPresenter?.OnBorderPropertyChanged(e);

			if (e.Property == PaddingProperty)
				UpdatePadding();
			else if (e.Property == VerticalContentAlignmentProperty)
				UpdateVerticalContentAlignment();
			else if (e.Property == HorizontalContentAlignmentProperty)
				UpdateHorizontalContentAlignment();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			CellPresenter.GridCell = this;

			SetContentPresenter();
		}

		protected override void OnTemplateContractDetaching()
		{
			CellPresenter.Child = null;
			CellPresenter.GridCell = null;

			base.OnTemplateContractDetaching();
		}

		private void SetContentPresenter()
		{
			UpdateContentPresenter();

			CellPresenter.Child = ActualContentPresenter;
		}

		private protected virtual void ShowEditor()
		{
			Mode = GridViewCellMode.Edit;
		}

		private void UpdateContentPresenter()
		{
			NonActualContentPresenter.Content = null;
			ActualContentPresenter.Content = Content;
		}

		private void UpdateDisplayPadding()
		{
			var isDefaultValue = this.IsDefaultValue(DisplayPaddingProperty);

			DisplayContentPresenter.Margin = isDefaultValue ? Padding : DisplayPadding;
		}

		private void UpdateEditPadding()
		{
			var isDefaultValue = this.IsDefaultValue(EditPaddingProperty);

			EditContentPresenter.Margin = isDefaultValue ? Padding : EditPadding;
		}

		private void UpdateHorizontalContentAlignment()
		{
			DisplayContentPresenter.HorizontalContentAlignment = HorizontalContentAlignment;
			EditContentPresenter.HorizontalContentAlignment = HorizontalContentAlignment;
		}

		private void UpdatePadding()
		{
			UpdateDisplayPadding();
			UpdateEditPadding();
		}

		protected virtual void UpdateStructure()
		{
		}

		internal void UpdateStructureInternal(bool force)
		{
			UpdateStructure();
		}

		private void UpdateVerticalContentAlignment()
		{
			DisplayContentPresenter.VerticalContentAlignment = VerticalContentAlignment;
			EditContentPresenter.VerticalContentAlignment = VerticalContentAlignment;
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

	public abstract class GridViewCell<TGridColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell> : GridViewCell
		where TGridColumn : GridViewColumn
		where TGridCellsPresenter : GridViewCellsPresenter<TGridColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellsPanel : GridViewCellsPanel<TGridColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCellCollection : GridViewCellCollection<TGridColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
		where TGridCell : GridViewCell<TGridColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>
	{
		private static readonly DependencyPropertyKey GridColumnPropertyKey = DPM.RegisterReadOnly<TGridColumn, GridViewCell<TGridColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>>
			("GridColumn", d => d.OnGridColumnPropertyChangedPrivate);

		public static readonly DependencyProperty GridColumnProperty = GridColumnPropertyKey.DependencyProperty;

		static GridViewCell()
		{
			UIElementUtils.OverrideFocusable<GridViewCell<TGridColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>>(false);
			ControlUtils.OverrideIsTabStop<GridViewCell<TGridColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>>(false);
			FrameworkElementUtils.OverrideVisualStyle<GridViewCell<TGridColumn, TGridCellsPresenter, TGridCellsPanel, TGridCellCollection, TGridCell>>(null);
		}

		public TGridColumn GridColumn
		{
			get => (TGridColumn)GetValue(GridColumnProperty);
			private set => this.SetReadOnlyValue(GridColumnPropertyKey, value);
		}

		protected override GridViewColumn GridColumnCore => GridColumn;

		internal void AttachColumnInternal(TGridColumn gridColumn)
		{
			if (ReferenceEquals(GridColumn, null) == false)
				throw new InvalidOperationException("GridColumn is already attached");

			GridColumn = gridColumn;
		}

		internal void DetachColumnInternal(TGridColumn gridColumn)
		{
			if (ReferenceEquals(GridColumn, gridColumn) == false)
				throw new InvalidOperationException("Invalid GridColumn");

			GridColumn = null;
		}

		private void OnGridColumnPropertyChangedPrivate(TGridColumn oldValue, TGridColumn newValue)
		{
			oldValue?.DetachCellInternal(this);
			newValue?.AttachCellInternal(this);
		}
	}

	public abstract class GridCellTemplateContract : TemplateContract
	{
		[TemplateContractPart(Required = true)]
		public GridViewCellPresenter CellPresenter { get; [UsedImplicitly] private set; }
	}
}