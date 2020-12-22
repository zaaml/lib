// <copyright file="TabViewItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.CommandCore;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Interfaces;
using Zaaml.UI.Controls.Menu;
using Zaaml.UI.Converters;
using ContentPresenter = System.Windows.Controls.ContentPresenter;

namespace Zaaml.UI.Controls.TabView
{
	public partial class TabViewItem : HeaderedIconContentControl, ISelectableHeaderedIconContentItem
	{
		private static readonly DependencyPropertyKey ActualCloseButtonVisibilityPropertyKey = DPM.RegisterReadOnly<Visibility, TabViewItem>
			("ActualCloseButtonVisibility", Visibility.Collapsed);

		public static readonly DependencyProperty ActualCloseButtonVisibilityProperty = ActualCloseButtonVisibilityPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey ActualPinButtonVisibilityPropertyKey = DPM.RegisterReadOnly<Visibility, TabViewItem>
			("ActualPinButtonVisibility", Visibility.Collapsed);

		public static readonly DependencyProperty ActualPinButtonVisibilityProperty = ActualPinButtonVisibilityPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey ActualCloseCommandPropertyKey = DPM.RegisterReadOnly<ICommand, TabViewItem>
			("ActualCloseCommand");

		private static readonly DependencyPropertyKey ActualCloseCommandParameterPropertyKey = DPM.RegisterReadOnly<object, TabViewItem>
			("ActualCloseCommandParameter");

		public static readonly DependencyProperty ActualCloseCommandParameterProperty = ActualCloseCommandParameterPropertyKey.DependencyProperty;

		public static readonly DependencyProperty ActualCloseCommandProperty = ActualCloseCommandPropertyKey.DependencyProperty;

		public static readonly DependencyProperty CloseCommandProperty = DPM.Register<ICommand, TabViewItem>
			("CloseCommand", t => t.OnCloseCommandChanged);

		public static readonly DependencyProperty CloseCommandParameterProperty = DPM.Register<object, TabViewItem>
			("CloseCommandParameter", t => t.OnCloseCommandParameterChanged);

		public static readonly DependencyProperty CloseButtonAutoVisibilityProperty = DPM.Register<Visibility, TabViewItem>
			("CloseButtonAutoVisibility", Visibility.Collapsed, t => t.UpdateCloseButtonVisibility);

		public static readonly DependencyProperty CloseButtonVisibilityProperty = DPM.Register<ElementVisibility, TabViewItem>
			("CloseButtonVisibility", ElementVisibility.Inherit, t => t.UpdateCloseButtonVisibility);

		public static readonly DependencyProperty PinButtonVisibilityProperty = DPM.Register<ElementVisibility, TabViewItem>
			("PinButtonVisibility", ElementVisibility.Inherit, t => t.UpdatePinButtonVisibility);

		public static readonly DependencyProperty PinButtonAutoVisibilityProperty = DPM.Register<Visibility, TabViewItem>
			("PinButtonAutoVisibility", Visibility.Collapsed, t => t.UpdatePinButtonVisibility);

		internal static readonly DependencyProperty DisplayIndexProperty = DPM.Register<int, TabViewItem>
			("DisplayIndex", -1, t => t.OnDisplayIndexChanged);

		public static readonly DependencyProperty IsSelectedProperty = DPM.Register<bool, TabViewItem>
			("IsSelected", false, t => t.OnIsSelectedChanged);

		private static readonly DependencyPropertyKey TabViewControlPropertyKey = DPM.RegisterReadOnly<TabViewControl, TabViewItem>
			("TabViewControl", t => t.OnTabViewControlChanged);

		public static readonly DependencyProperty IsPinnedProperty = DPM.Register<bool, TabViewItem>
			("IsPinned", t => t.OnIsPinnedChanged);

		public static readonly DependencyProperty ValueProperty = DPM.Register<object, TabViewItem>
			("Value", default, d => d.OnValuePropertyChangedPrivate);

		public static readonly DependencyProperty TabViewControlProperty = TabViewControlPropertyKey.DependencyProperty;

		private MenuItem _menuItem;

		public event EventHandler IsSelectedChanged;

		static TabViewItem()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<TabViewItem>();
		}

		public TabViewItem()
		{
			this.OverrideStyleKey<TabViewItem>();

			ContentHost.SetBinding(ContentPresenter.ContentTemplateProperty, new Binding {Path = new PropertyPath(ContentTemplateProperty), Source = this});
			ContentHost.SetBinding(ContentPresenter.ContentTemplateSelectorProperty, new Binding {Path = new PropertyPath(ContentTemplateSelectorProperty), Source = this});
			ContentHost.SetBinding(ContentPresenter.ContentStringFormatProperty, new Binding {Path = new PropertyPath(ContentStringFormatProperty), Source = this});

			SelectCommand = new RelayCommand(OnSelectCommandExecute);

			UpdateActualCloseCommandParameter();
		}

		public Visibility ActualCloseButtonVisibility
		{
			get => (Visibility) GetValue(ActualCloseButtonVisibilityProperty);
			private set => this.SetReadOnlyValue(ActualCloseButtonVisibilityPropertyKey, value);
		}

		public ICommand ActualCloseCommand
		{
			get => (ICommand) GetValue(ActualCloseCommandProperty);
			private set => this.SetReadOnlyValue(ActualCloseCommandPropertyKey, value);
		}

		public object ActualCloseCommandParameter
		{
			get => GetValue(ActualCloseCommandParameterProperty);
			private set => this.SetReadOnlyValue(ActualCloseCommandParameterPropertyKey, value);
		}

		public Visibility ActualPinButtonVisibility
		{
			get => (Visibility) GetValue(ActualPinButtonVisibilityProperty);
			private set => this.SetReadOnlyValue(ActualPinButtonVisibilityPropertyKey, value);
		}

		public Visibility CloseButtonAutoVisibility
		{
			get => (Visibility) GetValue(CloseButtonAutoVisibilityProperty);
			set => SetValue(CloseButtonAutoVisibilityProperty, value);
		}

		public ElementVisibility CloseButtonVisibility
		{
			get => (ElementVisibility) GetValue(CloseButtonVisibilityProperty);
			set => SetValue(CloseButtonVisibilityProperty, value);
		}

		public ICommand CloseCommand
		{
			get => (ICommand) GetValue(CloseCommandProperty);
			set => SetValue(CloseCommandProperty, value);
		}

		public object CloseCommandParameter
		{
			get => GetValue(CloseCommandParameterProperty);
			set => SetValue(CloseCommandParameterProperty, value);
		}

		internal ContentPresenter ContentHost { get; } = new ContentPresenter();

		internal int DisplayIndex
		{
			get => (int) GetValue(DisplayIndexProperty);
			set => SetValue(DisplayIndexProperty, value);
		}

		public bool IsPinned
		{
			get => (bool) GetValue(IsPinnedProperty);
			set => SetValue(IsPinnedProperty, value);
		}

		public bool IsSelected
		{
			get => (bool) GetValue(IsSelectedProperty);
			set => SetValue(IsSelectedProperty, value);
		}

		protected override IEnumerator LogicalChildren => Content != null ? EnumeratorUtils.Concat(Content, base.LogicalChildren) : base.LogicalChildren;

		public Visibility PinButtonAutoVisibility
		{
			get => (Visibility) GetValue(PinButtonAutoVisibilityProperty);
			set => SetValue(PinButtonAutoVisibilityProperty, value);
		}

		public ElementVisibility PinButtonVisibility
		{
			get => (ElementVisibility) GetValue(PinButtonVisibilityProperty);
			set => SetValue(PinButtonVisibilityProperty, value);
		}

		public ICommand SelectCommand { get; }

		public TabViewControl TabViewControl
		{
			get => this.GetValue<TabViewControl>(TabViewControlProperty);
			internal set => this.SetReadOnlyValue(TabViewControlPropertyKey, value);
		}

		public object Value
		{
			get => GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		private void ActivatePrivate()
		{
			SetIsSelectedPrivate(true);

			TabViewControl?.Activate(this);

			OnActivated();
		}

		internal void AttachMenuItem(MenuItem menuItem)
		{
			if (_menuItem != null)
				DetachMenuItem(_menuItem);

			_menuItem = menuItem;
			_menuItem.BindProperties(HeaderedMenuItem.HeaderProperty, this, HeaderProperty, converter: VisualCloneConverter.Instance);
			_menuItem.BindProperties(HeaderedMenuItem.HeaderTemplateProperty, this, HeaderTemplateProperty);
			_menuItem.BindProperties(HeaderedMenuItem.HeaderTemplateSelectorProperty, this, HeaderTemplateSelectorProperty);
			_menuItem.BindProperties(HeaderedMenuItem.HeaderStringFormatProperty, this, HeaderStringFormatProperty);
			_menuItem.BindProperties(HeaderedMenuItem.IconProperty, this, IconProperty);
		}

		internal void DetachMenuItem(MenuItem menuItem)
		{
			if (ReferenceEquals(_menuItem, menuItem) == false)
				return;

			_menuItem.ClearValue(HeaderedMenuItem.HeaderProperty);
			_menuItem.ClearValue(HeaderedMenuItem.HeaderTemplateProperty);
			_menuItem.ClearValue(HeaderedMenuItem.HeaderTemplateSelectorProperty);
			_menuItem.ClearValue(HeaderedMenuItem.HeaderStringFormatProperty);
			_menuItem.ClearValue(HeaderedMenuItem.IconProperty);
			_menuItem = null;
		}

		private Visibility EvaluateHiddenVisibility(Visibility visibility, bool force = false)
		{
#if !SILVERLIGHT
			if (visibility == Visibility.Hidden)
				return force || IsMouseOver || IsSelected ? Visibility.Visible : Visibility.Hidden;
#endif

			return visibility;
		}

		internal virtual void OnActivated()
		{
		}

		private void OnCloseCommandChanged()
		{
			UpdateActualCloseCommand();
		}

		private void OnCloseCommandParameterChanged()
		{
			UpdateActualCloseCommandParameter();
		}

		protected override void OnContentChanged(object oldContent, object newContent)
		{
			base.OnContentChanged(oldContent, newContent);

			if (oldContent != null)
				RemoveLogicalChild(oldContent);

			ContentHost.Content = newContent;

			if (newContent != null)
				AddLogicalChild(newContent);
		}

		private void OnDisplayIndexChanged()
		{
			var tabViewItemsPanel = this.GetParent(VisualTreeEnumerationStrategy.Instance) as TabViewItemsPanel;

			tabViewItemsPanel?.OnDisplayIndexChanged(this);
		}

		protected override void OnGotFocus(RoutedEventArgs e)
		{
			base.OnGotFocus(e);

#if !SILVERLIGHT
			if (e.Handled)
				return;

			e.Handled = true;
#endif

			if (TabViewControl?.IsInitializing == false)
				ActivatePrivate();
		}

		private void OnIsPinnedChanged()
		{
			TabViewControl?.ItemsPresenterInternal?.ItemsHostInternal?.InvalidateMeasure();
		}

		protected virtual void OnIsSelectedChanged()
		{
			var selected = IsSelected;

			if (selected)
				TabViewControl?.SelectItemInternal(this);

			UpdateVisualState(true);
			UpdateButtonsVisibility();

			if (selected)
				RaiseSelectedEvent();
			else
				RaiseUnselectedEvent();

			if (selected == IsSelected)
				RaiseSelectionChanged();
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);

			UpdateButtonsVisibility();
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			UpdateButtonsVisibility();

			base.OnMouseLeave(e);
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			if (e.Handled)
				return;

			e.Handled = true;

			ActivatePrivate();
		}

		private void OnSelectCommandExecute()
		{
			ActivatePrivate();
		}

		private void OnTabViewControlChanged()
		{
			UpdateButtonsVisibility();
			UpdateActualCloseCommand();
			UpdateActualCloseCommandParameter();
		}

		private void OnValuePropertyChangedPrivate(object oldValue, object newValue)
		{
		}

		private void RaiseSelectionChanged()
		{
			IsSelectedChanged?.Invoke(this, EventArgs.Empty);
		}

		internal void SetIsSelectedInternal(bool isSelected)
		{
			SetIsSelectedPrivate(isSelected);
		}

		private void SetIsSelectedPrivate(bool isSelected)
		{
			this.SetCurrentValueInternal(IsSelectedProperty, isSelected ? KnownBoxes.BoolTrue : KnownBoxes.BoolFalse);
		}

		protected virtual void UpdateActualCloseCommand()
		{
			ActualCloseCommand = CloseCommand ?? TabViewControl?.CloseTabCommand;
		}

		private void UpdateActualCloseCommandParameter()
		{
			ActualCloseCommandParameter = CloseCommandParameter ?? this;
		}

		internal void UpdateButtonsVisibility()
		{
			UpdateCloseButtonVisibility();
			UpdatePinButtonVisibility();
		}

		internal void UpdateCloseButtonVisibility()
		{
			var extendedVisibility = CloseButtonVisibility;

			if (extendedVisibility == ElementVisibility.Inherit && TabViewControl != null)
				ActualCloseButtonVisibility = EvaluateHiddenVisibility(VisibilityUtils.EvaluateElementVisibility(TabViewControl.ItemsCloseButtonVisibility, Visibility.Collapsed));
			else
				ActualCloseButtonVisibility = EvaluateHiddenVisibility(VisibilityUtils.EvaluateElementVisibility(extendedVisibility, CloseButtonAutoVisibility));
		}

		internal void UpdatePinButtonVisibility()
		{
			var extendedVisibility = PinButtonVisibility;

			if (extendedVisibility == ElementVisibility.Inherit && TabViewControl != null)
				ActualPinButtonVisibility = EvaluateHiddenVisibility(VisibilityUtils.EvaluateElementVisibility(TabViewControl.ItemsPinButtonVisibility, IsPinned ? Visibility.Visible : Visibility.Collapsed), IsPinned);
			else
				ActualPinButtonVisibility = EvaluateHiddenVisibility(VisibilityUtils.EvaluateElementVisibility(extendedVisibility, IsPinned ? Visibility.Visible : PinButtonAutoVisibility), IsPinned);
		}

		DependencyProperty ISelectableItem.ValueProperty => ValueProperty;

		DependencyProperty ISelectableItem.SelectionProperty => IsSelectedProperty;
	}
}