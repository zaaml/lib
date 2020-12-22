// <copyright file="DropDownListViewControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using Zaaml.Core;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.PropertyCore.Extensions;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Editors.Text;
using Zaaml.UI.Controls.ListView;
using Zaaml.UI.Controls.ScrollView;

namespace Zaaml.UI.Controls.DropDown
{
	[TemplateContractType(typeof(DropDownListViewTemplateContract))]
	[ContentProperty(nameof(ListViewControl))]
	public class DropDownListViewControl : DropDownEditableSelectorBase<ListViewControl, ListViewItem>
	{
		public static readonly DependencyProperty ListViewControlProperty = DPM.Register<ListViewControl, DropDownListViewControl>
			("ListViewControl", d => d.OnListViewChanged);

		private static readonly DependencyProperty ItemFilterProperty = DPM.Register<IListViewItemFilter, DropDownListViewControl>
			("ItemFilter", d => d.OnItemFilterPropertyChangedPrivate);

		public static readonly DependencyProperty SelectionPresenterProperty = DPM.Register<DropDownListViewSelectionPresenter, DropDownListViewControl>
			("SelectionPresenter", d => d.OnSelectionPresenterPropertyChangedPrivate);

		public static readonly DependencyProperty ItemSelectionTemplateProperty = DPM.Register<DataTemplate, DropDownListViewControl>
			("ItemSelectionTemplate", d => d.OnItemSelectionTemplate);

		private static readonly DependencyPropertyKey ActualSelectionPresenterPropertyKey = DPM.RegisterReadOnly<DropDownListViewSelectionPresenter, DropDownListViewControl>
			("ActualSelectionPresenter");

		public static readonly DependencyProperty ActualSelectionPresenterProperty = ActualSelectionPresenterPropertyKey.DependencyProperty;

		private readonly Binding _itemFilterBinding;
		private DropDownListViewSelectionPresenter _defaultSelectionPresenter;

		internal event EventHandler<ValueChangedEventArgs<ListViewControl>> ListViewControlChanged;

		static DropDownListViewControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<DropDownListViewControl>();
		}

		public DropDownListViewControl()
		{
			this.OverrideStyleKey<DropDownListViewControl>();

			_itemFilterBinding = new Binding {Path = new PropertyPath(TextProperty), Source = this, Mode = BindingMode.TwoWay};
		}

		public DropDownListViewSelectionPresenter ActualSelectionPresenter
		{
			get => (DropDownListViewSelectionPresenter) GetValue(ActualSelectionPresenterProperty);
			private set => this.SetReadOnlyValue(ActualSelectionPresenterPropertyKey, value);
		}

		protected override bool AutoPreserveText => ListViewControl?.ItemsFilter == null;

		private DropDownListViewSelectionPresenter DefaultSelectionPresenter => _defaultSelectionPresenter ??= CreteDefaultSelectionPresenter();

		protected override FrameworkElement EditorCore => FilterTextBox;

		private FilterTextBox FilterTextBox => TemplateContract.FilterTextBox;

		protected override ItemCollectionBase<ListViewControl, ListViewItem> ItemCollection => ListViewControl.Items;

		protected override ListViewControl ItemsControl => ListViewControl;

		public DataTemplate ItemSelectionTemplate
		{
			get => (DataTemplate) GetValue(ItemSelectionTemplateProperty);
			set => SetValue(ItemSelectionTemplateProperty, value);
		}

		public ListViewControl ListViewControl
		{
			get => (ListViewControl) GetValue(ListViewControlProperty);
			set => SetValue(ListViewControlProperty, value);
		}

		protected override ScrollViewControl ScrollView => ListViewControl?.ScrollViewInternal;

		public DropDownListViewSelectionPresenter SelectionPresenter
		{
			get => (DropDownListViewSelectionPresenter) GetValue(SelectionPresenterProperty);
			set => SetValue(SelectionPresenterProperty, value);
		}

		protected override FrameworkElement SelectionPresenterCore => SelectionPresenter;

		private DropDownListViewTemplateContract TemplateContract => (DropDownListViewTemplateContract) TemplateContractInternal;

		private DropDownListViewSelectionPresenter CreteDefaultSelectionPresenter()
		{
			return new DropDownListViewSelectionPresenter {DropDownListViewControl = this, ItemContentTemplate = ItemSelectionTemplate};
		}

		private protected override void ForceFilterUpdate()
		{
			base.ForceFilterUpdate();

			if (ListViewControl?.ItemsFilter is ItemTextFilter filter)
				filter.ForceUpdate();
		}

		private protected override TimeSpan GetFilterDelay()
		{
			if (ListViewControl?.ItemsFilter is ItemTextFilter filter)
				return filter.Delay;

			return base.GetFilterDelay();
		}

		private protected override FocusNavigator<ListViewItem> GetFocusNavigator(ListViewControl control)
		{
			return control?.FocusNavigator;
		}

		private protected override SelectorController<ListViewItem> GetSelectorController(ListViewControl control)
		{
			return control?.SelectorController;
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			if (ActualSelectionPresenter == null)
				UpdateActualSelectionPresenter();

			return base.MeasureOverride(availableSize);
		}

		private void OnActualSelectionPresenterPropertyChangedPrivate(DropDownListViewSelectionPresenter oldValue, DropDownListViewSelectionPresenter newValue)
		{
		}

		private void OnItemFilterPropertyChangedPrivate(IListViewItemFilter oldValue, IListViewItemFilter newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			if (oldValue is ItemTextFilter oldItemFilter)
			{
				if (ReferenceEquals(oldItemFilter.ReadLocalBinding(ItemTextFilter.FilterTextProperty), _itemFilterBinding))
					oldItemFilter.ClearValue(ItemTextFilter.FilterTextProperty);
			}

			if (newValue is ItemTextFilter newItemFilter)
			{
				if (newItemFilter.HasLocalValue(ItemTextFilter.FilterTextProperty) == false)
					newItemFilter.SetBinding(ItemTextFilter.FilterTextProperty, _itemFilterBinding);
			}
		}

		private void OnItemSelectionTemplate()
		{
			if (_defaultSelectionPresenter != null)
				_defaultSelectionPresenter.ItemContentTemplate = ItemSelectionTemplate;
		}

		private void OnListViewChanged(ListViewControl oldListViewControl, ListViewControl newListViewControl)
		{
			if (oldListViewControl != null)
			{
				oldListViewControl.ItemMouseButtonUp -= OnListViewItemMouseButtonUp;
				oldListViewControl.MouseButtonSelectionOptions = MouseButtonSelectionOptions.LeftButtonDown;
				oldListViewControl.FocusItemOnMouseHover = false;
				oldListViewControl.SelectItemOnFocus = true;
				oldListViewControl.PreserveMinSize = false;
				oldListViewControl.DefaultBringIntoViewMode = BringIntoViewMode.Default;

				ClearValue(ItemFilterProperty);
			}

			if (newListViewControl != null)
			{
				newListViewControl.ItemMouseButtonUp += OnListViewItemMouseButtonUp;
				newListViewControl.MouseButtonSelectionOptions = MouseButtonSelectionOptions.LeftButtonUp;
				newListViewControl.FocusItemOnMouseHover = true;
				newListViewControl.SelectItemOnFocus = false;
				newListViewControl.PreserveMinSize = true;
				newListViewControl.DefaultBringIntoViewMode = BringIntoViewMode.Top;

				this.BindProperties(ItemFilterProperty, newListViewControl, ListViewControl.ItemsFilterProperty);
			}

			OnItemsControlChanged(oldListViewControl, newListViewControl);

			ListViewControlChanged?.Invoke(this, new ValueChangedEventArgs<ListViewControl>(oldListViewControl, newListViewControl));
		}

		private void OnListViewItemMouseButtonUp(object sender, ListViewItemMouseButtonEventArgs e)
		{
			if (e.MouseEventArgs.ChangedButton == MouseButton.Left)
			{
				if (ListViewControl.SelectionMode == ListViewSelectionMode.Single)
					CommitSelection();

				e.MouseEventArgs.Handled = true;
			}
		}

		private void OnSelectionPresenterPropertyChangedPrivate(DropDownListViewSelectionPresenter oldValue, DropDownListViewSelectionPresenter newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			if (oldValue != null)
				oldValue.DropDownListViewControl = null;

			if (newValue != null)
				newValue.DropDownListViewControl = this;

			UpdateActualSelectionPresenter();
		}

		private void UpdateActualSelectionPresenter()
		{
			ActualSelectionPresenter = SelectionPresenter ?? DefaultSelectionPresenter;
		}
	}

	public class DropDownListViewTemplateContract : DropDownItemsControlTemplateContract
	{
		[TemplateContractPart(Required = false)]
		public FilterTextBox FilterTextBox { get; [UsedImplicitly] private set; }
	}
}