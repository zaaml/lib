// <copyright file="TabLayoutViewBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.TabView;

namespace Zaaml.UI.Controls.Docking
{
	public abstract class TabLayoutViewBase<TLayout> : BaseLayoutView<TLayout> where TLayout : TabLayoutBase
	{
		private Panel Host => TemplateContract.Host;

		private DockTabViewControl TabViewControl => TemplateContract.TabViewControl;

		private TabLayoutViewBaseTemplateContract TemplateContract => (TabLayoutViewBaseTemplateContract)TemplateContractInternal;

		protected internal override void ArrangeItems()
		{
		}

		private void AttachItem(DockItem item)
		{
			if (TabViewControl != null)
			{
				item.ActualSelectionScope.Suspend();

				var tabViewItem = item.TabViewItem;

				tabViewItem.Content = item;
				TabViewControl?.ItemCollection.Add(tabViewItem);

				item.ActualSelectionScope.Resume();

				tabViewItem.OnAttachedInternal();
			}
			else
				Host?.Children.Add(item);
		}

		protected override TemplateContract CreateTemplateContract()
		{
			return new TabLayoutViewBaseTemplateContract();
		}

		private void DetachItem(DockItem item)
		{
			if (TabViewControl != null)
			{
				item.ActualSelectionScope.Suspend();

				var tabViewItem = item.TabViewItem;

				item.TabViewItem.Content = null;
				TabViewControl?.ItemCollection.Remove(tabViewItem);

				item.ActualSelectionScope.Resume();

				tabViewItem.OnDetachedInternal();
			}
			else
				Host?.Children.Remove(item);
		}

		internal override bool IsItemVisible(DockItem item)
		{
			return TabViewControl != null ? item.TabViewItem.IsSelected : Host.Children.Contains(item);
		}

		protected override void OnItemAdded(DockItem item)
		{
			AttachItem(item);

			UpdateItemsPresenterVisibility();
		}

		protected override void OnItemRemoved(DockItem item)
		{
			DetachItem(item);

			UpdateItemsPresenterVisibility();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			foreach (var item in Items)
				AttachItem(item);

			TabViewControl?.SetBinding(TabView.TabViewControl.SelectedItemProperty, new Binding
			{
				Path = new PropertyPath(SelectedItemProperty),
				Mode = BindingMode.TwoWay,
				Source = this,
				Converter = TabViewItemConverter.Instance
			});

			UpdateItemsPresenterVisibility();
		}

		protected virtual int MinimumItemsCount => 1;

		private void UpdateItemsPresenterVisibility()
		{
			if (TabViewControl != null) 
				TabViewControl.ItemsPresenterVisibility = Items.Count >= MinimumItemsCount ? ElementVisibility.Auto : ElementVisibility.Collapsed;
		}

		protected override void OnTemplateContractDetaching()
		{
			TabViewControl?.ClearValue(TabView.TabViewControl.SelectedItemProperty);

			foreach (var item in Items)
				DetachItem(item);

			base.OnTemplateContractDetaching();
		}

		private sealed class TabViewItemConverter : IValueConverter
		{
			public static readonly TabViewItemConverter Instance = new();

			private TabViewItemConverter()
			{
			}

			private static object Convert(object value, Type targetType)
			{
				if (value == null)
					return null;

				if (typeof(TabViewItem).IsAssignableFrom(targetType) && value is DockItem)
					return ((DockItem)value).TabViewItem;

				if (typeof(DockItem).IsAssignableFrom(targetType) && value is DockTabViewItem)
					return ((DockTabViewItem)value).DockItem;

				throw new InvalidOperationException();
			}

			public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			{
				return Convert(value, targetType);
			}

			public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			{
				return Convert(value, targetType);
			}
		}
	}
}