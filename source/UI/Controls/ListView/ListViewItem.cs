// <copyright file="ListViewItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Interfaces;
using Zaaml.UI.Controls.ListView.Data;
using Zaaml.UI.Controls.Primitives;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;
using Zaaml.UI.Panels.Core;
using Zaaml.UI.Utils;

namespace Zaaml.UI.Controls.ListView
{
	[TemplateContractType(typeof(ListViewItemTemplateContract))]
	public partial class ListViewItem : IconContentControl, MouseHoverVisualStateFlickeringReducer<ListViewItem>.IClient, ISelectableIconContentItem, IContextPopupTarget
	{
		public static readonly DependencyProperty GlyphProperty = DPM.Register<GlyphBase, ListViewItem>
			("Glyph", i => i.LogicalChildMentor.OnLogicalChildPropertyChanged);

		private static readonly DependencyPropertyKey ListViewControlPropertyKey = DPM.RegisterReadOnly<ListViewControl, ListViewItem>
			("ListViewControl", d => d.OnListViewControlPropertyChangedPrivate);

		public static readonly DependencyProperty ValueProperty = DPM.Register<object, ListViewItem>
			("Value", default, d => d.OnValuePropertyChangedPrivate);

		public static readonly DependencyProperty CommandProperty = DPM.Register<ICommand, ListViewItem>
			("Command", d => d.OnCommandChanged);

		public static readonly DependencyProperty CommandParameterProperty = DPM.Register<object, ListViewItem>
			("CommandParameter", d => d.OnCommandParameterChanged);

		public static readonly DependencyProperty CommandTargetProperty = DPM.Register<DependencyObject, ListViewItem>
			("CommandTarget", d => d.OnCommandTargetChanged);

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

		public ICommand Command
		{
			get => (ICommand) GetValue(CommandProperty);
			set => SetValue(CommandProperty, value);
		}

		internal Rect ArrangeRect { get; private set; }

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

		private bool FocusOnMouseHover => ListViewControl?.FocusItemOnMouseHover ?? false;

		public GlyphBase Glyph
		{
			get => (GlyphBase) GetValue(GlyphProperty);
			set => SetValue(GlyphProperty, value);
		}

		private ListViewItemGlyphPresenter GlyphPresenter => TemplateContract.GlyphPresenter;

		private bool IsActuallyFocused => IsFocused;

		private bool IsFocusedVisualState { get; set; }

		private bool IsMouseOverVisualState { get; set; }

		private protected virtual bool IsSelectedState => IsSelected;

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

		private ListViewItemTemplateContract TemplateContract => (ListViewItemTemplateContract) TemplateContractInternal;

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

		protected virtual void OnClick()
		{
			RaiseClickEvent();
			ListViewControl?.OnItemClick(this);
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

			ListViewControl?.OnItemGotFocusInternal(this);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			ListViewControl?.OnItemKeyDown(this, e);
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);

			ListViewControl?.OnItemKeyUp(this, e);
		}

		protected virtual void OnListViewControlChanged(ListViewControl oldListView, ListViewControl newListView)
		{
		}

		internal virtual void OnListViewControlChangedInternal(ListViewControl oldListView, ListViewControl newListView)
		{
			OnListViewControlChanged(oldListView, newListView);
		}

		private void OnListViewControlPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (e.Property == ListViewControl.ItemGlyphKindProperty)
				UpdateGlyphPresenter();
			else if (e.Property == ListViewControl.ItemGlyphTemplateProperty)
				UpdateGlyphPresenter();
		}

		private void OnListViewControlPropertyChangedPrivate(ListViewControl oldListView, ListViewControl newListView)
		{
			if (ReferenceEquals(oldListView, newListView))
				return;

			if (oldListView != null)
				oldListView.DependencyPropertyChangedInternal -= OnListViewControlPropertyChanged;

			if (newListView != null)
				newListView.DependencyPropertyChangedInternal += OnListViewControlPropertyChanged;

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

		private void OnValuePropertyChangedPrivate(object oldValue, object newValue)
		{
			ListViewControl?.OnItemValueChanged(this);
		}

		private protected virtual void SyncListNodeState()
		{
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
			else if (ListViewControl != null)
			{
				if (ListViewControl.ItemGlyphKind == ListViewGlyphKind.Check)
				{
					GlyphPresenter.ContentTemplate = null;
					GlyphPresenter.Content = new ListViewCheckGlyph(this);
				}
				else
				{
					GlyphPresenter.ContentTemplate = ListViewControl.ItemGlyphTemplate;
					GlyphPresenter.Content = null;
				}
			}
			else
			{
				GlyphPresenter.ContentTemplate = null;
				GlyphPresenter.Content = null;
			}
		}

		protected override void UpdateVisualState(bool useTransitions)
		{
			UpdateVisualStateImpl(useTransitions);
		}

		private void UpdateVisualStateImpl(bool useTransitions, bool? actualIsMouseOver = null, bool? actualIsFocused = null)
		{
			var isMouseOver = actualIsMouseOver ?? IsMouseOver;
			var isFocused = actualIsFocused ?? IsActuallyFocused;
			var isSelected = IsSelectedState;

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
			if (isSelected)
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

		DependencyProperty ISelectableItem.ValueProperty => ValueProperty;

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

	public class ListViewItemTemplateContract : IconContentControlTemplateContract
	{
		[TemplateContractPart(Required = true)]
		public ListViewItemGlyphPresenter GlyphPresenter { get; [UsedImplicitly] private set; }
	}
}