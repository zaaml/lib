// <copyright file="Expander.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Primitives
{
  public enum ExpandDirection
  {
    Down,
    Left,
    Right,
    Up
  }

  public class Expander : HeaderedContentControl
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty IsExpandedProperty = DPM.Register<bool, Expander>
      ("IsExpanded");

    public static readonly DependencyProperty ExpandDirectionProperty = DPM.Register<ExpandDirection, Expander>
      ("ExpandDirection", ExpandDirection.Down);

    #endregion

    #region Ctors

    static Expander()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<Expander>();
    }

    public Expander()
    {
      this.OverrideStyleKey<Expander>();
    }

    #endregion

    #region Properties

    public ExpandDirection ExpandDirection
    {
      get => (ExpandDirection) GetValue(ExpandDirectionProperty);
      set => SetValue(ExpandDirectionProperty, value);
    }

    public bool IsExpanded
    {
      get => (bool) GetValue(IsExpandedProperty);
      set => SetValue(IsExpandedProperty, value);
    }

    #endregion
  }
}