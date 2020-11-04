// <copyright file="DropDownButtonBase.RoutedEvents..cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;

#if SILVERLIGHT
using RoutedEventArgs = System.Windows.RoutedEventArgsSL;
using RoutedEventHandler = System.Windows.RoutedEventHandlerSL;
using RoutedEvent = System.Windows.RoutedEventSL;

#endif

namespace Zaaml.UI.Controls.DropDown
{
	public partial class DropDownButtonBase
	{
		public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(nameof(Click), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DropDownButtonBase));

		public event RoutedEventHandler Click
		{
			add => this.AddRoutedHandler(ClickEvent, value);
			remove => this.RemoveRoutedHandler(ClickEvent, value);
		}

		private void RaiseOnClick()
		{
			this.RaiseRoutedEvent(new RoutedEventArgs(ClickEvent, this));
		}
	}
}