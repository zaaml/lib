// <copyright file="TreeViewItem.RoutedEvents.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;

#if SILVERLIGHT
using RoutedEventArgs = System.Windows.RoutedEventArgsSL;
using RoutedEventHandler = System.Windows.RoutedEventHandlerSL;
using RoutedEvent = System.Windows.RoutedEventSL;
#endif

namespace Zaaml.UI.Controls.TreeView
{
	public partial class TreeViewItem
	{
		public static readonly RoutedEvent SelectedEvent = EventManager.RegisterRoutedEvent(nameof(Selected), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TreeViewItem));
		public static readonly RoutedEvent UnselectedEvent = EventManager.RegisterRoutedEvent(nameof(Unselected), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TreeViewItem));

		public static readonly RoutedEvent ExpandedEvent = EventManager.RegisterRoutedEvent(nameof(Expanded), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TreeViewItem));
		public static readonly RoutedEvent CollapsedEvent = EventManager.RegisterRoutedEvent(nameof(Collapsed), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TreeViewItem));

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

		public event RoutedEventHandler Expanded
		{
			add => this.AddRoutedHandler(ExpandedEvent, value);
			remove => this.RemoveRoutedHandler(ExpandedEvent, value);
		}

		public event RoutedEventHandler Collapsed
		{
			add => this.AddRoutedHandler(CollapsedEvent, value);
			remove => this.RemoveRoutedHandler(CollapsedEvent, value);
		}

		private void RaiseSelectedEvent()
		{
			this.RaiseRoutedEvent(new RoutedEventArgs(SelectedEvent, this));
		}

		private void RaiseUnselectedEvent()
		{
			this.RaiseRoutedEvent(new RoutedEventArgs(UnselectedEvent, this));
		}

		private void RaiseExpandedEvent()
		{
			this.RaiseRoutedEvent(new RoutedEventArgs(ExpandedEvent, this));
		}

		private void RaiseCollapsedEvent()
		{
			this.RaiseRoutedEvent(new RoutedEventArgs(CollapsedEvent, this));
		}
	}
}