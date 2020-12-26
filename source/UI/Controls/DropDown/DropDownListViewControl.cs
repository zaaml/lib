// <copyright file="DropDownListViewControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
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
using ListViewItem = Zaaml.UI.Controls.ListView.ListViewItem;

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

		public static readonly DependencyProperty SelectionItemTemplateProperty = DPM.Register<DataTemplate, DropDownListViewControl>
			("SelectionItemTemplate", d => d.OnSelectionItemTemplatePropertyChangedPrivate);

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

		private DefaultListViewItemTextFilter DefaultFilter { get; set; }

		private DropDownListViewSelectionPresenter DefaultSelectionPresenter => _defaultSelectionPresenter ??= CreteDefaultSelectionPresenter();

		protected override FrameworkElement EditorCore => FilterTextBox;

		private FilterTextBox FilterTextBox => TemplateContract.FilterTextBox;

		protected override ItemCollectionBase<ListViewControl, ListViewItem> ItemCollection => ListViewControl.ItemCollection;

		protected override ListViewControl ItemsControl => ListViewControl;

		public ListViewControl ListViewControl
		{
			get => (ListViewControl) GetValue(ListViewControlProperty);
			set => SetValue(ListViewControlProperty, value);
		}

		protected override ScrollViewControl ScrollView => ListViewControl?.ScrollViewInternal;

		public DataTemplate SelectionItemTemplate
		{
			get => (DataTemplate) GetValue(SelectionItemTemplateProperty);
			set => SetValue(SelectionItemTemplateProperty, value);
		}

		public DropDownListViewSelectionPresenter SelectionPresenter
		{
			get => (DropDownListViewSelectionPresenter) GetValue(SelectionPresenterProperty);
			set => SetValue(SelectionPresenterProperty, value);
		}

		protected override FrameworkElement SelectionPresenterCore => SelectionPresenter;

		private DropDownListViewTemplateContract TemplateContract => (DropDownListViewTemplateContract) TemplateContractInternal;

		private DropDownListViewSelectionPresenter CreteDefaultSelectionPresenter()
		{
			return new DropDownListViewSelectionPresenter
			{
				IsDefault = true,
				DropDownListViewControl = this,
				ItemContentTemplate = SelectionItemTemplate,
				ContentMode = SelectionItemContentMode.None
			};
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

		private void OnListViewChanged(ListViewControl oldListViewControl, ListViewControl newListViewControl)
		{
			if (oldListViewControl != null)
			{
				oldListViewControl.ItemsDefaultFilter = DefaultFilter = null;
				oldListViewControl.ItemMouseButtonUp -= OnListViewItemMouseButtonUp;
				oldListViewControl.ItemClickMode = ClickMode.Release;
				oldListViewControl.FocusItemOnMouseHover = false;
				oldListViewControl.SelectItemOnFocus = true;
				oldListViewControl.PreserveMinSize = false;
				oldListViewControl.DefaultBringIntoViewMode = BringIntoViewMode.Default;

				ClearValue(ItemFilterProperty);
			}

			if (newListViewControl != null)
			{
				newListViewControl.ItemsDefaultFilter = DefaultFilter = new DefaultListViewItemTextFilter(newListViewControl);
				newListViewControl.ItemMouseButtonUp += OnListViewItemMouseButtonUp;
				newListViewControl.ItemClickMode = ClickMode.Release;
				newListViewControl.FocusItemOnMouseHover = true;
				newListViewControl.SelectItemOnFocus = false;
				newListViewControl.PreserveMinSize = true;
				newListViewControl.DefaultBringIntoViewMode = BringIntoViewMode.Top;

				this.BindProperties(ItemFilterProperty, newListViewControl, ListViewControl.ItemsFilterProperty, targetNullValue: DefaultFilter);
			}

			LogicalChildMentor.OnLogicalChildPropertyChanged(oldListViewControl, newListViewControl);

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

		private void OnSelectionItemTemplatePropertyChangedPrivate()
		{
			if (_defaultSelectionPresenter != null)
				_defaultSelectionPresenter.ItemContentTemplate = SelectionItemTemplate;
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