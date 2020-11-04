// <copyright file="TreeViewItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;
using Zaaml.UI.Controls.TreeView.Data;
using Zaaml.UI.Panels.Core;
using Zaaml.UI.Utils;

namespace Zaaml.UI.Controls.TreeView
{
	[ContentProperty(nameof(Items))]
	public partial class TreeViewItem : IconContentControl, ISelectable, ISelectableEx, IContextPopupTarget, MouseHoverVisualStateFlickeringReducer<TreeViewItem>.IClient
	{
		public static readonly DependencyProperty IsSelectedProperty = DPM.Register<bool, TreeViewItem>
			("IsSelected", i => i.OnIsSelectedPropertyChangedPrivate, i => i.OnCoerceSelection);

		private static readonly DependencyPropertyKey ItemsPropertyKey = DPM.RegisterReadOnly<TreeViewItemCollection, TreeViewItem>
			("ItemsInt");

		public static readonly DependencyProperty ItemsSourceProperty = DPM.Register<IEnumerable, TreeViewItem>
			("ItemsSource", i => i.OnItemsSourceChangedPrivate);

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
		public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey TreeViewPropertyKey = DPM.RegisterReadOnly<TreeViewControl, TreeViewItem>
			("TreeView", default, d => d.OnTreeViewPropertyChangedPrivate);

		public static readonly DependencyProperty TreeViewProperty = TreeViewPropertyKey.DependencyProperty;

		private uint _packedValue;

		private TreeViewItemData _treeViewItemData;

		static TreeViewItem()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<TreeViewItem>();
		}

		public TreeViewItem()
		{
			this.OverrideStyleKey<TreeViewItem>();
		}

		internal bool ActualCanCollapse => HasItems && CanCollapse;

		internal bool ActualCanExpand => HasItems && CanExpand;

		internal bool ActualCanSelect => CanSelect && TreeView?.CanSelectItemInternal(this) != false;

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

		protected virtual bool CanSelect => true;

		private bool CoerceIsExpanded
		{
			get => PackedDefinition.CoerceIsExpanded.GetValue(_packedValue);
			set => PackedDefinition.CoerceIsExpanded.SetValue(ref _packedValue, value);
		}

		private bool FocusOnMouseHover => TreeView?.FocusItemOnMouseHover ?? false;

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

		public bool IsSelected
		{
			get => (bool) GetValue(IsSelectedProperty);
			set => SetValue(IsSelectedProperty, value);
		}

		protected virtual bool IsValid => this.HasValidationError() == false;

		public TreeViewItemCollection Items => this.GetValueOrCreate(ItemsPropertyKey, CreateItemCollectionPrivate);

		public IEnumerable ItemsSource
		{
			get => (IEnumerable) GetValue(ItemsSourceProperty);
			set => SetValue(ItemsSourceProperty, value);
		}

		protected IEnumerable ItemsSourceCore
		{
			get => Items.SourceInternal;
			set => Items.SourceInternal = value;
		}

		private int Level => TreeViewItemData?.ActualLevel ?? 0;

		public double LevelDistance
		{
			get => (double) GetValue(LevelDistanceProperty);
			set => SetValue(LevelDistanceProperty, value);
		}

		public TreeViewItem ParentItem => TreeViewItemData?.ActualParent?.TreeViewItem;

		private bool SuspendPushIsExpanded
		{
			get => PackedDefinition.SuspendPushIsExpanded.GetValue(_packedValue);
			set => PackedDefinition.SuspendPushIsExpanded.SetValue(ref _packedValue, value);
		}

		public TreeViewControl TreeView
		{
			get => (TreeViewControl) GetValue(TreeViewProperty);
			internal set => this.SetReadOnlyValue(TreeViewPropertyKey, value);
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

		private object OnCoerceSelection(object arg)
		{
			var isSelected = (bool) arg;

			if (isSelected && CanSelect == false)
				return KnownBoxes.BoolFalse;

			return arg;
		}

		protected override void OnGotFocus(RoutedEventArgs e)
		{
			base.OnGotFocus(e);

			TreeView?.OnItemGotFocusInternal(this);
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
			TreeView?.OnItemIsExpandedChangedInternal(this);

			PushIsExpanded(newIsExpanded);

			Items.IsExpanded = newIsExpanded;

			if (IsExpanded)
				RaiseExpandedEvent();
			else
				RaiseCollapsedEvent();
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
				TreeView?.Select(this);

			OnIsSelectedChanged();

			UpdateVisualState(true);
		}

		private void OnItemsSourceChangedPrivate(IEnumerable oldSource, IEnumerable newSource)
		{
			ItemsSourceCore = newSource;
		}

		private void OnLevelDistanceChanged()
		{
			UpdateActualLevelPadding();
		}

		protected override void OnLostFocus(RoutedEventArgs e)
		{
			base.OnLostFocus(e);

			TreeView?.OnItemLostFocusInternal(this);
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);

			TreeView?.OnItemMouseEnter(this, e);
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			TreeView?.OnItemMouseLeave(this, e);

			base.OnMouseLeave(e);
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			if (e.Handled)
				return;

			TreeView?.OnItemMouseButton(this, e);
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			if (e.Handled)
				return;

			TreeView?.OnItemMouseButton(this, e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			TreeView?.OnItemMouseMove(this, e);
		}

		protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
		{
			if (e.Handled)
				return;

			TreeView?.OnItemMouseButton(this, e);
		}

		protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
		{
			if (e.Handled)
				return;

			TreeView?.OnItemMouseButton(this, e);
		}

		protected virtual void OnTreeViewChanged(TreeViewControl oldTreeView, TreeViewControl newTreeView)
		{
		}

		internal virtual void OnTreeViewChangedInternal(TreeViewControl oldTreeView, TreeViewControl newTreeView)
		{
			OnTreeViewChanged(oldTreeView, newTreeView);
		}

		private void OnTreeViewPropertyChangedPrivate(TreeViewControl oldTreeView, TreeViewControl newTreeView)
		{
			OnTreeViewChangedInternal(oldTreeView, newTreeView);
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

		private void UpdateActualLevelPadding()
		{
			var level = Level;

			ActualLevel = level;
			ActualLevelPadding = new Thickness(LevelDistance * level, 0, 0, 0);
		}

		internal void UpdateHasItemsInternal()
		{
			var hasItems = Items.ActualCountInternal > 0;

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

		public event EventHandler IsSelectedChanged;

		bool ISelectable.IsSelected
		{
			get => IsSelected;
			set => SetIsSelectedInternal(value);
		}

		bool ISelectableEx.CanSelect => CanSelect;

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