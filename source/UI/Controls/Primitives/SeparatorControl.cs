// <copyright file="SeparatorControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Control = Zaaml.UI.Controls.Core.Control;

namespace Zaaml.UI.Controls.Primitives
{
  public sealed class SeparatorControl : Control
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty OrientationProperty = DPM.Register<Orientation, SeparatorControl>
      ("Orientation");

    #endregion

    #region Ctors

    static SeparatorControl()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<SeparatorControl>();
    }

    public SeparatorControl()
    {
      this.OverrideStyleKey<SeparatorControl>();

#if !SILVERLIGHT
      Focusable = false;
#endif
      IsTabStop = false;
    }

    #endregion

    #region Properties

    public Orientation Orientation
    {
      get => (Orientation) GetValue(OrientationProperty);
      set => SetValue(OrientationProperty, value);
    }

    #endregion
  }
}