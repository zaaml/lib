// <copyright file="TrackBarRangeItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Primitives.TrackBar
{
  public class TrackBarRangeItem : TrackBarItem
  {
    #region Static Fields and Constants

    private static readonly DependencyPropertyKey RangePropertyKey = DPM.RegisterReadOnly<double, TrackBarRangeItem>
      ("Range");

    public static readonly DependencyProperty RangeProperty = RangePropertyKey.DependencyProperty;

    #endregion

    #region Ctors

    static TrackBarRangeItem()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<TrackBarRangeItem>();
    }

    public TrackBarRangeItem()
    {
      this.OverrideStyleKey<TrackBarRangeItem>();
    }

    #endregion

    #region Properties

    public double Range
    {
      get => (double) GetValue(RangeProperty);
      internal set => this.SetReadOnlyValue(RangePropertyKey, value);
    }

    #endregion
  }
}