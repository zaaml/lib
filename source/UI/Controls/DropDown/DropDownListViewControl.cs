// <copyright file="DropDownListViewControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
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
	[ContentProperty(nameof(ListView))]
	public class DropDownListViewControl : DropDownEditableSelectorBase<ListViewControl, ListViewItem>
	{
		public static readonly DependencyProperty ListViewProperty = DPM.Register<ListViewControl, DropDownListViewControl>
			("ListView", d => d.OnListViewChanged);

		private static readonly DependencyProperty ItemFilterProperty = DPM.Register<IListViewItemFilter, DropDownListViewControl>
			("ItemFilter", default, d => d.OnItemFilterPropertyChangedPrivate);

		private readonly Binding _itemFilterBinding;

		static DropDownListViewControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<DropDownListViewControl>();
		}

		public DropDownListViewControl()
		{
			this.OverrideStyleKey<DropDownListViewControl>();

			BindSelectedIcon(new Binding("ListView.SelectedItem.Icon") {Source = this});
			BindSelectedContent(new Binding("ListView.SelectedItem.Content") {Source = this});

			_itemFilterBinding = new Binding {Path = new PropertyPath(TextProperty), Source = this, Mode = BindingMode.TwoWay};
		}

		protected override bool AutoPreserveText => ListView?.ItemsFilter == null;

		protected override FrameworkElement Editor => TemplateContract.FilterTextBox;

		protected override ItemCollectionBase<ListViewControl, ListViewItem> ItemCollection => ListView.Items;

		protected override ListViewControl ItemsControl => ListView;

		public ListViewControl ListView
		{
			get => (ListViewControl) GetValue(ListViewProperty);
			set => SetValue(ListViewProperty, value);
		}

		protected override ScrollViewControl ScrollView => ListView?.ScrollViewInternal;

		private DropDownListViewTemplateContract TemplateContract => (DropDownListViewTemplateContract) TemplateContractInternal;

		private protected override void ForceFilterUpdate()
		{
			base.ForceFilterUpdate();

			if (ListView?.ItemsFilter is ItemTextFilter filter)
				filter.ForceUpdate();
		}

		private protected override TimeSpan GetFilterDelay()
		{
			if (ListView?.ItemsFilter is ItemTextFilter filter)
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
				newListViewControl.MouseButtonSelectionOptions = MouseButtonSelectionOptions.LeftButtonDown | MouseButtonSelectionOptions.LeftButtonUp;
				newListViewControl.FocusItemOnMouseHover = true;
				newListViewControl.SelectItemOnFocus = false;
				newListViewControl.PreserveMinSize = true;
				newListViewControl.DefaultBringIntoViewMode = BringIntoViewMode.Top;

				this.BindProperties(ItemFilterProperty, newListViewControl, ListViewControl.ItemsFilterProperty);
			}

			OnItemsControlChanged(oldListViewControl, newListViewControl);
		}

		private void OnListViewItemMouseButtonUp(object sender, ListViewItemMouseButtonEventArgs e)
		{
			if (e.MouseEventArgs.ChangedButton == MouseButton.Left)
			{
				CommitSelection();

				e.MouseEventArgs.Handled = true;
			}
		}
	}

	public class DropDownListViewTemplateContract : DropDownItemsControlTemplateContract
	{
		[TemplateContractPart(Required = false)]
		public FilterTextBox FilterTextBox { get; private set; }
	}
}