// <copyright file="ExpandPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Panel = Zaaml.UI.Panels.Core.Panel;

namespace Zaaml.UI.Panels
{
  public class ExpandPanel : Panel
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty OrientationProperty = DPM.Register<Orientation, ExpandPanel>
      ("Orientation", Orientation.Vertical, p => p.InvalidateMeasure);

    public static readonly DependencyProperty ExpandRatioProperty = DPM.Register<double, ExpandPanel>
      ("ExpandRatio", p => p.InvalidateMeasure);

    #endregion

    #region Properties

    public double ExpandRatio
    {
      get => (double) GetValue(ExpandRatioProperty);
      set => SetValue(ExpandRatioProperty, value);
    }

    public Orientation Orientation
    {
      get => (Orientation) GetValue(OrientationProperty);
      set => SetValue(OrientationProperty, value);
    }

    #endregion

    #region  Methods

    protected override Size MeasureOverrideCore(Size constraint)
    {
      var ret = base.MeasureOverrideCore(constraint);

      if (Orientation == Orientation.Vertical)
        constraint.Height = (ret.Height * ExpandRatio).Clamp(0.0, double.MaxValue);
      else
        constraint.Width = (ret.Width * ExpandRatio).Clamp(0.0, double.MaxValue);

      return base.MeasureOverrideCore(constraint);
    }

    #endregion
  }
}