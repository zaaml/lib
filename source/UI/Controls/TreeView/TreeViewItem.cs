﻿// <copyright file="TreeViewItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Interfaces;
using Zaaml.UI.Controls.Primitives;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;
using Zaaml.UI.Controls.TreeView.Data;
using Zaaml.UI.Panels.Core;
using Zaaml.UI.Utils;

namespace Zaaml.UI.Controls.TreeView
{
	[ContentProperty(nameof(ItemCollection))]
	[TemplateContractType(typeof(TreeViewItemTemplateContract))]
	public partial class TreeViewItem : IconContentControl, IContextPopupTarget, MouseHoverVisualStateFlickeringReducer<TreeViewItem>.IClient, ISelectableIconContentItem
	{
		public static readonly DependencyProperty IsSelectedProperty = DPM.Register<bool, TreeViewItem>
			("IsSelected", i => i.OnIsSelectedPropertyChangedPrivate, i => i.OnCoerceSelection);

		public static readonly DependencyProperty IsSelectableProperty = DPM.Register<bool, TreeViewItem>
			("IsSelectable", true, i => i.OnIsSelectablePropertyChangedPrivate);

		private static readonly DependencyPropertyKey ItemCollectionPropertyKey = DPM.RegisterReadOnly<TreeViewItemCollection, TreeViewItem>
			("ItemCollectionPrivate");

		public static readonly DependencyProperty SourceCollectionProperty = DPM.Register<IEnumerable, TreeViewItem>
			("SourceCollection", i => i.OnSourceChangedPrivate);

		private static readonly DependencyPropertyKey HasItemsPropertyKey = DPM.RegisterReadOnly<bool, TreeViewItem>
			("HasItems", i => i.OnHasItemsChangedPrivate);

		public static readonly DependencyProperty IsExpandedProperty = DPM.Register<bool, TreeViewItem>
			("IsExpanded", false, t => t.OnIsExpandedPropertyChangedPrivate, t => t.CoerceIsExpandedProperty);

		public static readonly DependencyProperty LevelDistanceProperty = DPM.Register<double, TreeViewItem>
			("LevelDistance", 20, t => t.OnLevelDistanceChanged);

		private static readonly DependencyPropertyKey ActualLevelPaddingPropertyKey = DPM.RegisterReadOnly<Thickness, TreeViewItem>
			("ActualLevelPadding");

		public static readonly DependencyProperty ActualLevelPaddingProperty = ActualLevelPaddingPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey ActualLevelPropertyKey = DPM.RegisterReadOnly<int, TreeViewItem>
			("ActualLevel", 0);

		public static readonly DependencyProperty ActualLevelProperty = ActualLevelPropertyKey.DependencyProperty;

		public static readonly DependencyProperty HasItemsProperty = HasItemsPropertyKey.DependencyProperty;

		// ReSharper disable once StaticMemberInGenericType
		public static readonly DependencyProperty ItemCollectionProperty = ItemCollectionPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey TreeViewControlPropertyKey = DPM.RegisterReadOnly<TreeViewControl, TreeViewItem>
			("TreeViewControl", default, d => d.OnTreeViewControlPropertyChangedPrivate);

		public static readonly DependencyProperty GlyphProperty = DPM.Register<GlyphBase, TreeViewItem>
			("Glyph", i => i.LogicalChildMentor.OnLogicalChildPropertyChanged);

		public static readonly DependencyProperty ValueProperty = DPM.Register<object, TreeViewItem>
			("Value", d => d.OnValuePropertyChangedPrivate);

		public static readonly DependencyProperty CommandProperty = DPM.Register<ICommand, TreeViewItem>
			("Command", d => d.OnCommandChanged);

		public static readonly DependencyProperty CommandParameterProperty = DPM.Register<object, TreeViewItem>
			("CommandParameter", d => d.OnCommandParameterChanged);

		public static readonly DependencyProperty CommandTargetProperty = DPM.Register<DependencyObject, TreeViewItem>
			("CommandTarget", d => d.OnCommandTargetChanged);

		public static readonly DependencyProperty TreeViewControlProperty = TreeViewControlPropertyKey.DependencyProperty;

		private uint _packedValue;

		private TreeViewItemData _treeViewItemData;

		public event EventHandler IsSelectedChanged;

		static TreeViewItem()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<TreeViewItem>();
		}

		public TreeViewItem()
		{
			this.OverrideStyleKey<TreeViewItem>();
		}

		public ICommand Command
		{
			get => (ICommand) GetValue(CommandProperty);
			set => SetValue(CommandProperty, value);
		}

		internal bool ActualCanCollapse => HasItems && CanCollapse;

		internal bool ActualCanExpand => HasItems && CanExpand;

		internal bool ActualCanSelect => CanSelect && TreeViewControl?.CanSelectItemInternal(this) != false;

		public int ActualLevel
		{
			get => (int) GetValue(ActualLevelProperty);
			private set => this.SetReadOnlyValue(ActualLevelPropertyKey, value);
		}

		public Thickness ActualLevelPadding
		{
			get => (Thickness) GetValue(ActualLevelPaddingProperty);
			private set => this.SetReadOnlyValue(ActualLevelPaddingPropertyKey, value);
		}

		internal Rect ArrangeRect { get; private set; }

		protected virtual bool CanCollapse => true;

		protected virtual bool CanExpand => true;

		protected virtual bool CanSelect => IsSelectable;

		private bool CoerceIsExpanded
		{
			get => PackedDefinition.CoerceIsExpanded.GetValue(_packedValue);
			set => PackedDefinition.CoerceIsExpanded.SetValue(ref _packedValue, value);
		}

		public object CommandParameter
		{
			get => GetValue(CommandParameterProperty);
			set => SetValue(CommandParameterProperty, value);
		}

		public DependencyObject CommandTarget
		{
			get => (DependencyObject) GetValue(CommandTargetProperty);
			set => SetValue(CommandTargetProperty, value);
		}

		private bool FocusOnMouseHover => TreeViewControl?.FocusItemOnMouseHover ?? false;

		public GlyphBase Glyph
		{
			get => (GlyphBase) GetValue(GlyphProperty);
			set => SetValue(GlyphProperty, value);
		}


		private TreeViewItemGlyphPresenter GlyphPresenter => TemplateContract.GlyphPresenter;

		public bool HasItems
		{
			get => (bool) GetValue(HasItemsProperty);
			internal set => this.SetReadOnlyValue(HasItemsPropertyKey, value);
		}

		protected virtual bool IsActuallyFocused => IsFocused;

		public bool IsExpanded
		{
			get => (bool) GetValue(IsExpandedProperty);
			set => SetValue(IsExpandedProperty, value);
		}

		private bool IsFocusedVisualState { get; set; }

		private bool IsMouseOverVisualState { get; set; }

		public bool IsSelectable
		{
			get => (bool) GetValue(IsSelectableProperty);
			set => SetValue(IsSelectableProperty, value);
		}

		public bool IsSelected
		{
			get => (bool) GetValue(IsSelectedProperty);
			set => SetValue(IsSelectedProperty, value);
		}

		protected virtual bool IsValid => this.HasValidationError() == false;

		public TreeViewItemCollection ItemCollection => this.GetValueOrCreate(ItemCollectionPropertyKey, CreateItemCollectionPrivate);

		private int Level => TreeViewItemData?.ActualLevel ?? 0;

		public double LevelDistance
		{
			get => (double) GetValue(LevelDistanceProperty);
			set => SetValue(LevelDistanceProperty, value);
		}

		public TreeViewItem ParentItem => TreeViewItemData?.ActualParent?.TreeViewItem;

		public IEnumerable SourceCollection
		{
			get => (IEnumerable) GetValue(SourceCollectionProperty);
			set => SetValue(SourceCollectionProperty, value);
		}

		protected IEnumerable SourceCore
		{
			get => ItemCollection.SourceCollectionInternal;
			set => ItemCollection.SourceCollectionInternal = value;
		}

		private bool SuspendPushIsExpanded
		{
			get => PackedDefinition.SuspendPushIsExpanded.GetValue(_packedValue);
			set => PackedDefinition.SuspendPushIsExpanded.SetValue(ref _packedValue, value);
		}

		private TreeViewItemTemplateContract TemplateContract => (TreeViewItemTemplateContract) TemplateContractInternal;

		public TreeViewControl TreeViewControl
		{
			get => (TreeViewControl) GetValue(TreeViewControlProperty);
			internal set => this.SetReadOnlyValue(TreeViewControlPropertyKey, value);
		}

		internal TreeViewItemData TreeViewItemData
		{
			get => _treeViewItemData;
			set
			{
				try
				{
					if (ReferenceEquals(_treeViewItemData, value))
						return;

					if (_treeViewItemData != null)
						_treeViewItemData.TreeViewItem = null;

					_treeViewItemData = value;

					if (_treeViewItemData != null)
						_treeViewItemData.TreeViewItem = this;
				}
				finally
				{
					SyncTreeNodeState();
				}
			}
		}

		public object Value
		{
			get => GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		private void CleanGlyphPresenter()
		{
			if (GlyphPresenter == null)
				return;

			GlyphPresenter.ContentTemplate = null;
			GlyphPresenter.Content = null;
		}

		private bool CoerceIsExpandedProperty(bool value)
		{
			return SuspendPushIsExpanded == false ? value : CoerceIsExpanded;
		}

		public bool Collapse()
		{
			if (ActualCanCollapse == false)
				return false;

			this.SetCurrentValueInternal(IsExpandedProperty, KnownBoxes.BoolFalse);

			return IsExpanded == false;
		}

		private TreeViewItemCollection CreateItemCollectionPrivate()
		{
			return new TreeViewItemCollection(this);
		}

		public bool Expand()
		{
			if (ActualCanExpand == false)
				return false;

			this.SetCurrentValueInternal(IsExpandedProperty, KnownBoxes.BoolTrue);

			return IsExpanded;
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			SyncTreeNodeState();

			return base.MeasureOverride(availableSize);
		}

		protected virtual void OnClick()
		{
			RaiseClickEvent();
			TreeViewControl?.OnItemClick(this);
		}

		private object OnCoerceSelection(object arg)
		{
			var isSelected = (bool) arg;

			if (isSelected && ActualCanSelect == false)
				return KnownBoxes.BoolFalse;

			return arg;
		}

		private protected override void OnDependencyPropertyChangedInternal(DependencyPropertyChangedEventArgs args)
		{
			base.OnDependencyPropertyChangedInternal(args);

			if (args.Property == GlyphProperty)
				UpdateGlyphPresenter();
		}

		protected override void OnGotFocus(RoutedEventArgs e)
		{
			base.OnGotFocus(e);

			TreeViewControl?.OnItemGotFocusInternal(this);
		}

		protected virtual void OnHasItemsChanged()
		{
		}

		internal virtual void OnHasItemsChangedInternal()
		{
			OnHasItemsChanged();
		}

		private void OnHasItemsChangedPrivate()
		{
			OnHasItemsChangedInternal();
		}

		private void OnIsExpandedPropertyChangedPrivate(bool oldIsExpanded, bool newIsExpanded)
		{
			TreeViewControl?.OnItemIsExpandedChangedInternal(this);

			PushIsExpanded(newIsExpanded);

			ItemCollection.IsExpanded = newIsExpanded;

			if (IsExpanded)
				RaiseExpandedEvent();
			else
				RaiseCollapsedEvent();
		}

		private void OnIsSelectablePropertyChangedPrivate(bool oldValue, bool newValue)
		{
			if (newValue == false && IsSelected)
				SetIsSelectedInternal(false);
		}

		protected virtual void OnIsSelectedChanged()
		{
			var selected = IsSelected;

			if (selected)
				RaiseSelectedEvent();
			else
				RaiseUnselectedEvent();

			if (selected == IsSelected)
				IsSelectedChanged?.Invoke(this, EventArgs.Empty);
		}

		private void OnIsSelectedPropertyChangedPrivate()
		{
			var selected = IsSelected;

			if (selected)
				TreeViewControl?.Select(this);
			else
				TreeViewControl?.Unselect(this);

			OnIsSelectedChanged();

			UpdateVisualState(true);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			TreeViewControl?.OnItemKeyDown(this, e);
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);

			TreeViewControl?.OnItemKeyUp(this, e);
		}

		private void OnLevelDistanceChanged()
		{
			UpdateActualLevelPadding();
		}

		protected override void OnLostFocus(RoutedEventArgs e)
		{
			base.OnLostFocus(e);

			TreeViewControl?.OnItemLostFocusInternal(this);
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);

			TreeViewControl?.OnItemMouseEnter(this, e);
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			TreeViewControl?.OnItemMouseLeave(this, e);

			base.OnMouseLeave(e);
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			if (e.Handled)
				return;

			TreeViewControl?.OnItemMouseButton(this, e);
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			if (e.Handled)
				return;

			TreeViewControl?.OnItemMouseButton(this, e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			TreeViewControl?.OnItemMouseMove(this, e);
		}

		protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
		{
			if (e.Handled)
				return;

			TreeViewControl?.OnItemMouseButton(this, e);
		}

		protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
		{
			if (e.Handled)
				return;

			TreeViewControl?.OnItemMouseButton(this, e);
		}

		private void OnSourceChangedPrivate(IEnumerable oldSource, IEnumerable newSource)
		{
			SourceCore = newSource;
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			UpdateGlyphPresenter();
		}

		protected override void OnTemplateContractDetaching()
		{
			CleanGlyphPresenter();

			base.OnTemplateContractDetaching();
		}

		protected virtual void OnTreeViewControlChanged(TreeViewControl oldTreeView, TreeViewControl newTreeView)
		{
		}

		internal virtual void OnTreeViewControlChangedInternal(TreeViewControl oldTreeView, TreeViewControl newTreeView)
		{
			OnTreeViewControlChanged(oldTreeView, newTreeView);
		}

		private void OnTreeViewControlPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (e.Property == TreeViewControl.ItemGlyphKindProperty)
				UpdateGlyphPresenter();
			else if (e.Property == TreeViewControl.ItemGlyphTemplateProperty)
				UpdateGlyphPresenter();
		}

		private void OnTreeViewControlPropertyChangedPrivate(TreeViewControl oldTreeView, TreeViewControl newTreeView)
		{
			if (ReferenceEquals(oldTreeView, newTreeView))
				return;

			if (oldTreeView != null)
				oldTreeView.DependencyPropertyChangedInternal -= OnTreeViewControlPropertyChanged;

			if (newTreeView != null)
				newTreeView.DependencyPropertyChangedInternal += OnTreeViewControlPropertyChanged;

			OnTreeViewControlChangedInternal(oldTreeView, newTreeView);
		}

		private void OnValuePropertyChangedPrivate(object oldValue, object newValue)
		{
		}

		private void PushIsExpanded(bool value)
		{
			if (SuspendPushIsExpanded)
				return;

			if (TreeViewItemData != null)
				TreeViewItemData.IsExpanded = value;
		}

		internal void SelectInternal()
		{
			SetIsSelectedInternal(true);
		}

		internal void SetIsSelectedInternal(bool value)
		{
			this.SetCurrentValueInternal(IsSelectedProperty, value ? KnownBoxes.BoolTrue : KnownBoxes.BoolFalse);
		}

		private void SyncTreeNodeState()
		{
			UpdateIsExpanded();
			UpdateActualLevelPadding();
			UpdateHasItemsInternal();
		}

		public override string ToString()
		{
#if DEBUG
			//return (TreeNode?.ToString() ?? base.ToString()) + $"  ({UniqueId})";

			if (ReferenceEquals(TreeViewItemData?.Data, this))
				return base.ToString();

			return TreeViewItemData?.ToString() ?? base.ToString();
#else
			return base.ToString();
#endif
		}

		internal void UnselectInternal()
		{
			SetIsSelectedInternal(false);
		}

		private void UpdateActualLevelPadding()
		{
			var level = Level;

			ActualLevel = level;
			ActualLevelPadding = new Thickness(LevelDistance * level, 0, 0, 0);
		}

		private void UpdateGlyphPresenter()
		{
			if (GlyphPresenter == null)
				return;

			if (Glyph != null)
			{
				GlyphPresenter.ContentTemplate = null;
				GlyphPresenter.Content = Glyph;
			}
			else if (TreeViewControl != null)
			{
				if (TreeViewControl.ItemGlyphKind == TreeViewGlyphKind.Check)
				{
					GlyphPresenter.ContentTemplate = null;
					GlyphPresenter.Content = new TreeViewCheckGlyph(this);
				}
				else
				{
					GlyphPresenter.ContentTemplate = TreeViewControl.ItemGlyphTemplate;
					GlyphPresenter.Content = null;
				}
			}
			else
			{
				GlyphPresenter.ContentTemplate = null;
				GlyphPresenter.Content = null;
			}
		}

		internal void UpdateHasItemsInternal()
		{
			var hasItems = ItemCollection.ActualCountInternal > 0;

			if (TreeViewItemData != null)
			{
				if (TreeViewItemData.IsLoaded == false)
					TreeViewItemData.LoadNodes();

				hasItems = TreeViewItemData.FlatCount > 0;
			}

			if (HasItems != hasItems)
				HasItems = hasItems;
		}

		private void UpdateIsExpanded()
		{
			if (TreeViewItemData == null)
				return;

			try
			{
				SuspendPushIsExpanded = true;
				CoerceIsExpanded = TreeViewItemData.IsExpanded;

				if (IsExpanded != TreeViewItemData.IsExpanded)
					this.SetCurrentValueInternal(IsExpandedProperty, TreeViewItemData.IsExpanded);
			}
			finally
			{
				SuspendPushIsExpanded = false;
			}
		}

		internal void UpdateIsExpandedInternal()
		{
			UpdateIsExpanded();
		}

		protected override void UpdateVisualState(bool useTransitions)
		{
			UpdateVisualStateImpl(useTransitions);
		}

		private void UpdateVisualStateImpl(bool useTransitions, bool? actualIsMouseOver = null, bool? actualIsFocused = null)
		{
			var isMouseOver = actualIsMouseOver ?? IsMouseOver;
			var isFocused = actualIsFocused ?? IsActuallyFocused;

			IsFocusedVisualState = isFocused;
			IsMouseOverVisualState = isMouseOver;

			// Common states
			if (!IsEnabled)
				GotoVisualState(Content is Control ? CommonVisualStates.Normal : CommonVisualStates.Disabled, useTransitions);
			else if (isMouseOver)
				GotoVisualState(CommonVisualStates.MouseOver, useTransitions);
			else
				GotoVisualState(CommonVisualStates.Normal, useTransitions);

			// Selection states
			if (IsSelected)
			{
				if (isFocused)
					GotoVisualState(CommonVisualStates.Selected, useTransitions);
				else
				{
					if (GotoVisualState(CommonVisualStates.SelectedUnfocused, useTransitions) == false)
						GotoVisualState(CommonVisualStates.Selected, useTransitions);
				}
			}
			else
				GotoVisualState(CommonVisualStates.Unselected, useTransitions);

			// Focus states
			if (isFocused)
				GotoVisualState(CommonVisualStates.Focused, useTransitions);
			else
				GotoVisualState(CommonVisualStates.Unfocused, useTransitions);

			// Expansion states
			if (IsExpanded)
				GotoVisualState(CommonVisualStates.Expanded, useTransitions);
			else
				GotoVisualState(CommonVisualStates.Collapsed, useTransitions);

			// Validation states
			if (IsValid)
				GotoVisualState(CommonVisualStates.Valid, useTransitions);
			else
			{
				if (isFocused)
					GotoVisualState(CommonVisualStates.InvalidFocused, useTransitions);
				else
					GotoVisualState(CommonVisualStates.InvalidUnfocused, useTransitions);
			}
		}

		void IContextPopupTarget.OnContextPopupControlOpened(IContextPopupControl popupControl)
		{
			SelectInternal();
		}

		DependencyProperty ISelectableItem.ValueProperty => ValueProperty;

		DependencyProperty ISelectableItem.SelectionProperty => IsSelectedProperty;

		Rect ILayoutInformation.ArrangeRect
		{
			get => ArrangeRect;
			set => ArrangeRect = value;
		}

		void MouseHoverVisualStateFlickeringReducer<TreeViewItem>.IClient.UpdateVisualStateOnArrange(bool? isMouseOver)
		{
			if (isMouseOver == null)
			{
				UpdateVisualStateImpl(true);

				return;
			}

			var newMouse = isMouseOver.Value;
			var newFocus = FocusOnMouseHover ? isMouseOver : IsActuallyFocused;

			if (IsMouseOverVisualState == newMouse && newFocus == IsFocusedVisualState)
				return;

			UpdateVisualStateImpl(true, newMouse, newFocus);
		}

		private static class PackedDefinition
		{
			public static readonly PackedBoolItemDefinition SuspendPushIsExpanded;
			public static readonly PackedBoolItemDefinition CoerceIsExpanded;

			static PackedDefinition()
			{
				var allocator = new PackedValueAllocator();

				SuspendPushIsExpanded = allocator.AllocateBoolItem();
				CoerceIsExpanded = allocator.AllocateBoolItem();
			}
		}
	}
}