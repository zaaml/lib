// <copyright file="ListViewItem.RoutedEvents.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.ListView
{
	public partial class ListViewItem
	{
		public static readonly RoutedEvent SelectedEvent = EventManager.RegisterRoutedEvent(nameof(Selected), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ListViewItem));

		public static readonly RoutedEvent UnselectedEvent = EventManager.RegisterRoutedEvent(nameof(Unselected), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ListViewItem));

		public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(nameof(Click), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ListViewItem));

		public event RoutedEventHandler Selected
		{
			add => this.AddRoutedHandler(SelectedEvent, value);
			remove => this.RemoveRoutedHandler(SelectedEvent, value);
		}

		public event RoutedEventHandler Unselected
		{
			add => this.AddRoutedHandler(UnselectedEvent, value);
			remove => this.RemoveRoutedHandler(UnselectedEvent, value);
		}

		public event RoutedEventHandler Click
		{
			add => this.AddRoutedHandler(ClickEvent, value);
			remove => this.RemoveRoutedHandler(ClickEvent, value);
		}

		private void RaiseClickEvent()
		{
			this.RaiseRoutedEvent(new RoutedEventArgs(ClickEvent, this));
		}

		private void RaiseSelectedEvent()
		{
			this.RaiseRoutedEvent(new RoutedEventArgs(SelectedEvent, this));
		}

		private void RaiseUnselectedEvent()
		{
			this.RaiseRoutedEvent(new RoutedEventArgs(UnselectedEvent, this));
		}
	}
}