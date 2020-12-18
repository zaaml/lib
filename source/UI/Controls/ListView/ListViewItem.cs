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
using Zaaml.PresentationCore.Utils;
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

		private static readonly DependencyPropertyKey ListViewControlPropertyKey = DPM.RegisterReadOnly<ListViewControl, ListViewItem>
			("ListViewControl", default, d => d.OnListViewControlPropertyChangedPrivate);

		//private static readonly DependencyPropertyKey ActualCheckBoxVisibilityPropertyKey = DPM.RegisterReadOnly<Visibility, ListViewItem>
		//	("ActualCheckBoxVisibility", Visibility.Collapsed);

		//public static readonly DependencyProperty ActualCheckBoxVisibilityProperty = ActualCheckBoxVisibilityPropertyKey.DependencyProperty;

		//public static readonly DependencyProperty CheckBoxVisibilityProperty = DPM.Register<ElementVisibility, ListViewItem>
		//	("CheckBoxVisibility", ElementVisibility.Inherit, d => d.OnCheckBoxVisibilityPropertyChangedPrivate);

		public static readonly DependencyProperty ListViewControlProperty = ListViewControlPropertyKey.DependencyProperty;

		private ListViewItemData _listViewItemData;

		static ListViewItem()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ListViewItem>();
		}

		public ListViewItem()
		{
			this.OverrideStyleKey<ListViewItem>();
		}

		internal bool ActualCanSelect => CanSelect && ListViewControl?.CanSelectItemInternal(this) != false;

		//public Visibility ActualCheckBoxVisibility
		//{
		//	get => (Visibility) GetValue(ActualCheckBoxVisibilityProperty);
		//	private set => this.SetReadOnlyValue(ActualCheckBoxVisibilityPropertyKey, value);
		//}

		internal Rect ArrangeRect { get; private set; }

		protected virtual bool CanSelect => true;

		//public ElementVisibility CheckBoxVisibility
		//{
		//	get => (ElementVisibility) GetValue(CheckBoxVisibilityProperty);
		//	set => SetValue(CheckBoxVisibilityProperty, value);
		//}

		private bool FocusOnMouseHover => ListViewControl?.FocusItemOnMouseHover ?? false;

		private bool IsActuallyFocused => IsFocused;

		private bool IsFocusedVisualState { get; set; }

		private bool IsMouseOverVisualState { get; set; }

		public bool IsSelected
		{
			get => (bool) GetValue(IsSelectedProperty);
			set => SetValue(IsSelectedProperty, value);
		}

		protected virtual bool IsValid => this.HasValidationError() == false;

		public ListViewControl ListViewControl
		{
			get => (ListViewControl) GetValue(ListViewControlProperty);
			internal set => this.SetReadOnlyValue(ListViewControlPropertyKey, value);
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

		//private void OnCheckBoxVisibilityPropertyChangedPrivate(ElementVisibility oldValue, ElementVisibility newValue)
		//{
		//	UpdateCheckBoxVisibility();
		//}

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

			ListViewControl?.OnItemGotFocusInternal(this);
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
				ListViewControl?.Select(this);

			OnIsSelectedChanged();

			UpdateVisualState(true);
		}

		protected virtual void OnListViewControlChanged(ListViewControl oldListView, ListViewControl newListView)
		{
		}

		internal virtual void OnListViewControlChangedInternal(ListViewControl oldListView, ListViewControl newListView)
		{
			OnListViewControlChanged(oldListView, newListView);
		}

		private void OnListViewControlPropertyChangedPrivate(ListViewControl oldListView, ListViewControl newListView)
		{
			//UpdateCheckBoxVisibility();

			OnListViewControlChangedInternal(oldListView, newListView);
		}

		protected override void OnLostFocus(RoutedEventArgs e)
		{
			base.OnLostFocus(e);

			ListViewControl?.OnItemLostFocusInternal(this);
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);

			ListViewControl?.OnItemMouseEnter(this, e);
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			ListViewControl?.OnItemMouseLeave(this, e);

			base.OnMouseLeave(e);
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			if (e.Handled)
				return;

			ListViewControl?.OnItemMouseButton(this, e);
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			if (e.Handled)
				return;

			ListViewControl?.OnItemMouseButton(this, e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			ListViewControl?.OnItemMouseMove(this, e);
		}

		protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
		{
			if (e.Handled)
				return;

			ListViewControl?.OnItemMouseButton(this, e);
		}

		protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
		{
			if (e.Handled)
				return;

			ListViewControl?.OnItemMouseButton(this, e);
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

		//private void UpdateCheckBoxVisibility()
		//{
		//	var inheritVisibility = ListViewControl != null ? (Visibility?) VisibilityUtils.EvaluateElementVisibility(ListViewControl.ItemsCheckBoxVisibility, Visibility.Collapsed) : null;

		//	ActualCheckBoxVisibility = VisibilityUtils.EvaluateElementVisibility(CheckBoxVisibility, Visibility.Collapsed, inheritVisibility);
		//}

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