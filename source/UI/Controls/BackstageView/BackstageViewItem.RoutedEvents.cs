// <copyright file="BackstageViewItem.RoutedEvents.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;

#if SILVERLIGHT
using RoutedEventArgs = System.Windows.RoutedEventArgsSL;
using RoutedEventHandler = System.Windows.RoutedEventHandlerSL;
using RoutedEvent = System.Windows.RoutedEventSL;
#endif

namespace Zaaml.UI.Controls.BackstageView
{
	public partial class BackstageViewItem
	{
		#region Static Fields and Constants

		public static readonly RoutedEvent SelectedEvent = EventManager.RegisterRoutedEvent(nameof(Selected), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BackstageViewItem));
		public static readonly RoutedEvent UnselectedEvent = EventManager.RegisterRoutedEvent(nameof(Unselected), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BackstageViewItem));

		#endregion

		#region Type: Fields

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

		#endregion

		#region  Methods

		private void RaiseSelectedEvent()
		{
			this.RaiseRoutedEvent(new RoutedEventArgs(SelectedEvent, this));
		}

		private void RaiseUnselectedEvent()
		{
			this.RaiseRoutedEvent(new RoutedEventArgs(UnselectedEvent, this));
		}

		#endregion
	}
}