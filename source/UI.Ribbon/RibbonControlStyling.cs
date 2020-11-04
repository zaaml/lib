// <copyright file="RibbonControlStyling.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Ribbon
{
  public sealed class RibbonControlStyling
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty QuickAccessToolBarSkinProperty = DPM.RegisterAttached<SkinDictionary, RibbonControlStyling>
      ("QuickAccessToolBarSkin");

    #endregion

    #region  Methods

    public static SkinDictionary GetQuickAccessToolBarSkin(DependencyObject element)
    {
      return (SkinDictionary) element.GetValue(QuickAccessToolBarSkinProperty);
    }

    public static void SetQuickAccessToolBarSkin(DependencyObject element, SkinDictionary value)
    {
      element.SetValue(QuickAccessToolBarSkinProperty, value);
    }

    #endregion
  }
}