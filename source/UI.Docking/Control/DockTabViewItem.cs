// <copyright file="DockTabViewItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Zaaml.Core;
using Zaaml.PresentationCore.CommandCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.TabView;

namespace Zaaml.UI.Controls.Docking
{
	public class DockTabViewItem : TabViewItem
	{
		private static readonly RelayCommand<DockTabViewItem> StaticCloseCommand = new(OnStaticCloseCommandExecuted);

		static DockTabViewItem()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<DockTabViewItem>();
		}

		public DockTabViewItem(DockItem dockItem)
		{
			this.OverrideStyleKey<DockTabViewItem>();

			DockItem = dockItem;

			SetBinding(HeaderProperty, new Binding { Path = new PropertyPath(DockItem.TitleProperty), Source = dockItem });
			SetBinding(IconProperty, new Binding { Path = new PropertyPath(DockItem.IconProperty), Source = dockItem });
			SetBinding(DisplayIndexProperty, new Binding { Path = new PropertyPath(TabLayout.OrderProperty), Source = dockItem, Converter = DisplayIndexConverter.Instance, Mode = BindingMode.TwoWay });

			DataContext = dockItem;

			DragOutBehavior = new DragOutBehavior
			{
				DragOutCommand = new RelayCommand(OnDragOutCommandExecuted),
				ProcessHandledEvents = true,
				Target = this
			};

			CloseCommand = StaticCloseCommand;
			CloseCommandParameter = this;
		}

		public DockItem DockItem { get; }

		[UsedImplicitly]
		private DragOutBehavior DragOutBehavior { get; }

		internal override void OnActivated()
		{
			base.OnActivated();

			DockItem.SelectAndFocus();
		}

		internal void OnAttachedInternal()
		{
			if (DockItem.IsSelected)
				IsSelected = true;
		}

		internal void OnDetachedInternal()
		{
		}

		private void OnDragOutCommandExecuted()
		{
			DockItem.DragOutInternal(DragOutBehavior.OriginMousePosition);
		}

		protected override void OnIsSelectedChanged()
		{
			base.OnIsSelectedChanged();

			if (IsSelected == false)
				return;

			DockItem.Select();
		}

		private static void OnStaticCloseCommandExecuted(DockTabViewItem dockTabViewItem)
		{
			dockTabViewItem.DockItem.DockState = DockItemState.Hidden;
		}

		private class DisplayIndexConverter : IValueConverter
		{
			public static readonly DisplayIndexConverter Instance = new DisplayIndexConverter();

			private DisplayIndexConverter()
			{
			}

			public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			{
				return (int?)value ?? int.MaxValue;
			}

			public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			{
				return value;
			}
		}
	}
}