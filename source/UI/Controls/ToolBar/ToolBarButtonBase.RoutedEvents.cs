using System.Windows;
using Zaaml.PresentationCore.Extensions;
#if SILVERLIGHT
using RoutedEventArgs = System.Windows.RoutedEventArgsSL;
using RoutedEventHandler = System.Windows.RoutedEventHandlerSL;
using RoutedEvent = System.Windows.RoutedEventSL;

#endif

namespace Zaaml.UI.Controls.ToolBar
{
  public partial class ToolBarButtonBase
  {
    #region Static Fields and Constants

    public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(nameof(Click), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ToolBarButtonBase));

    #endregion

    #region Fields

    public event RoutedEventHandler Click
    {
      add => this.AddRoutedHandler(ClickEvent, value);
      remove => this.RemoveRoutedHandler(ClickEvent, value);
    }

    #endregion

    #region  Methods

    private void RaiseOnClick()
    {
      this.RaiseRoutedEvent(new RoutedEventArgs(ClickEvent, this));
    }

    #endregion
  }
}
