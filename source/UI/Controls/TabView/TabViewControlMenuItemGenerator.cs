// <copyright file="TabViewControlMenuItemGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Input;
using Zaaml.PresentationCore.CommandCore;
using Zaaml.UI.Controls.Menu;

namespace Zaaml.UI.Controls.TabView
{
	internal class TabViewControlMenuItemGenerator : MenuItemGeneratorBase
	{
		public TabViewControlMenuItemGenerator(TabViewControl tabViewControl)
		{
			TabViewControl = tabViewControl;
			SelectCommand = new RelayCommand(OnSelectCommand);
		}

		private ICommand SelectCommand { get; }

		public TabViewControl TabViewControl { get; }

		protected override void AttachItem(MenuItemBase item, object source)
		{
			var menuItem = (MenuItem)item;
			var tabViewItem = source as TabViewItem;

			if (tabViewItem == null)
			{
				menuItem.Header = source;
				menuItem.HeaderTemplate = TabViewControl.ItemHeaderTemplate;
				menuItem.HeaderTemplateSelector = TabViewControl.ItemHeaderTemplateSelector;
				menuItem.DataContext = source;
			}
			else
				tabViewItem.AttachMenuItem(menuItem);


			menuItem.Command = SelectCommand;
			menuItem.CommandParameter = source;
		}

		protected override MenuItemBase CreateItem(object source)
		{
			return new MenuItem { HorizontalContentAlignment = HorizontalAlignment.Left };
		}

		protected override void DetachItem(MenuItemBase item, object source)
		{
			var menuItem = (MenuItem)item;
			var tabViewItem = source as TabViewItem;

			if (tabViewItem == null)
			{
				menuItem.Header = null;
				menuItem.DataContext = null;
				menuItem.HeaderTemplate = null;
				menuItem.HeaderTemplateSelector = null;
			}
			else
				tabViewItem.DetachMenuItem(menuItem);

			menuItem.Command = null;
			menuItem.CommandParameter = null;
		}

		protected override void DisposeItem(MenuItemBase item, object source)
		{
		}

		private void OnSelectCommand(object parameter)
		{
			TabViewControl.OnMenuItemSelect(parameter);
		}
	}
}