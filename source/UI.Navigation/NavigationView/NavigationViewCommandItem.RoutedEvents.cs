// <copyright file="NavigationViewCommandItem.RoutedEvents.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.NavigationView
{
	public partial class NavigationViewCommandItem
	{
		public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(nameof(Click), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NavigationViewCommandItem));

		public event RoutedEventHandler Click
		{
			add => this.AddRoutedHandler(ClickEvent, value);
			remove => this.RemoveRoutedHandler(ClickEvent, value);
		}

		private void RaiseClickEvent()
		{
			this.RaiseRoutedEvent(new RoutedEventArgs(ClickEvent, this));
		}
	}
}