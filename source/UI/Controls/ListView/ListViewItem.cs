// <copyright file="ListViewItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Input;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.ListView.Data;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;
using Zaaml.UI.Panels.Core;
using Zaaml.UI.Utils;

namespace Zaaml.UI.Controls.ListView
{
	public partial class ListViewItem : IconContentControl, ISelectable, ISelectableEx, MouseHoverVisualStateFlickeringReducer<ListViewItem>.IClient, IContextPopupTarget
	{
		public static readonly DependencyProperty IsSelectedProperty = DPM.Register<bool, ListViewItem>
			("IsSelected", i => i.OnIsSelectedPropertyChangedPrivate, i => i.OnCoerceSelection);

		private static readonly DependencyPropertyKey ListViewPropertyKey = DPM.RegisterReadOnly<ListViewControl, ListViewItem>
			("ListView", default, d => d.OnListViewPropertyChangedPrivate);

		public static readonly DependencyProperty ListViewProperty = ListViewPropertyKey.DependencyProperty;

		private ListViewItemData _listViewItemData;

		static ListViewItem()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ListViewItem>();
		}

		public ListViewItem()
		{
			this.OverrideStyleKey<ListViewItem>();
		}

		internal bool ActualCanSelect => CanSelect && ListView?.CanSelectItemInternal(this) != false;

		internal Rect ArrangeRect { get; private set; }

		protected virtual bool CanSelect => true;

		private bool FocusOnMouseHover => ListView?.FocusItemOnMouseHover ?? false;

		private bool IsActuallyFocused => IsFocused;

		private bool IsFocusedVisualState { get; set; }

		private bool IsMouseOverVisualState { get; set; }

		public bool IsSelected
		{
			get => (bool) GetValue(IsSelectedProperty);
			set => SetValue(IsSelectedProperty, value);
		}

		protected virtual bool IsValid => this.HasValidationError() == false;

		public ListViewControl ListView
		{
			get => (ListViewControl) GetValue(ListViewProperty);
			internal set => this.SetReadOnlyValue(ListViewPropertyKey, value);
		}

		internal ListViewItemData ListViewItemData
		{
			get => _listViewItemData;
			set
			{
				try
				{
					if (ReferenceEquals(_listViewItemData, value))
						return;

					if (_listViewItemData != null)
						_listViewItemData.ListViewItem = null;

					_listViewItemData = value;

					if (_listViewItemData != null)
						_listViewItemData.ListViewItem = this;
				}
				finally
				{
					SyncListNodeState();
				}
			}
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

			ListView?.OnItemGotFocusInternal(this);
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
				ListView?.Select(this);

			OnIsSelectedChanged();

			UpdateVisualState(true);
		}

		protected virtual void OnListViewChanged(ListViewControl oldListView, ListViewControl newListView)
		{
		}

		internal virtual void OnListViewChangedInternal(ListViewControl oldListView, ListViewControl newListView)
		{
			OnListViewChanged(oldListView, newListView);
		}

		private void OnListViewPropertyChangedPrivate(ListViewControl oldListView, ListViewControl newListView)
		{
			OnListViewChangedInternal(oldListView, newListView);
		}

		protected override void OnLostFocus(RoutedEventArgs e)
		{
			base.OnLostFocus(e);

			ListView?.OnItemLostFocusInternal(this);
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);

			ListView?.OnItemMouseEnter(this, e);
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			ListView?.OnItemMouseLeave(this, e);

			base.OnMouseLeave(e);
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			if (e.Handled)
				return;

			ListView?.OnItemMouseButton(this, e);
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			if (e.Handled)
				return;

			ListView?.OnItemMouseButton(this, e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			ListView?.OnItemMouseMove(this, e);
		}

		protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
		{
			if (e.Handled)
				return;

			ListView?.OnItemMouseButton(this, e);
		}

		protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
		{
			if (e.Handled)
				return;

			ListView?.OnItemMouseButton(this, e);
		}

		internal void SelectInternal()
		{
			SetIsSelectedInternal(true);
		}

		internal void SetIsSelectedInternal(bool value)
		{
			this.SetCurrentValueInternal(IsSelectedProperty, value ? KnownBoxes.BoolTrue : KnownBoxes.BoolFalse);
		}

		private protected virtual void SyncListNodeState()
		{
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

		void MouseHoverVisualStateFlickeringReducer<ListViewItem>.IClient.UpdateVisualStateOnArrange(bool? isMouseOver)
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
	}
}