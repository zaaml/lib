// <copyright file="ListViewItem.RoutedEvents.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;

#if SILVERLIGHT
using RoutedEventArgs = System.Windows.RoutedEventArgsSL;
using RoutedEventHandler = System.Windows.RoutedEventHandlerSL;
using RoutedEvent = System.Windows.RoutedEventSL;
#endif

namespace Zaaml.UI.Controls.ListView
{
  public partial class ListViewItem
  {
    #region Static Fields and Constants

    public static readonly RoutedEvent SelectedEvent = EventManager.RegisterRoutedEvent(nameof(Selected), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ListViewItem));
    public static readonly RoutedEvent UnselectedEvent = EventManager.RegisterRoutedEvent(nameof(Unselected), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ListViewItem));

    #endregion

    #region Fields

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
