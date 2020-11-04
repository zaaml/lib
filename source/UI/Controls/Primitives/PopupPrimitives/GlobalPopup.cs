// <copyright file="GlobalPopup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{

  internal class GlobalPopup
  {
    #region Static Fields and Constants

    public static readonly GlobalPopup Instance = new GlobalPopup();

    #endregion

    #region Fields

    private readonly Canvas _host = new Canvas { Opacity = 0, IsHitTestVisible = false, Width = 0, Height = 0 };
    private readonly System.Windows.Controls.Primitives.Popup _nativePopup = new System.Windows.Controls.Primitives.Popup { IsOpen = true };

    #endregion

    #region Ctors

    private GlobalPopup()
    {
      _nativePopup.Child = _host;
      _host.Resources = ThemeManager.CreateThemeResourceDictionary();
    }

    #endregion

    #region  Methods

    public static bool IsAncestorOf(FrameworkElement element)
    {
      return ReferenceEquals(Instance._nativePopup, element.GetRoot(MixedTreeEnumerationStrategy.DisconnectedThenVisualThenLogicalInstance));
    }

    public void Attach(FrameworkElement frameworkElement)
    {
      _host.Children.Add(frameworkElement);
      UpdatePopup();
    }

    public void Detach(FrameworkElement frameworkElement)
    {
      _host.Children.Remove(frameworkElement);
      UpdatePopup();
    }

    private void UpdatePopup()
    {
      //_nativePopup.IsOpen = _host.Children.Cast<UIElement>().Any();
    }

    #endregion
  }
}