// <copyright file="RangeControlBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Primitives
{
  public abstract class RangeControlBase : TemplateContractControl
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty MinimumProperty = DPM.Register<double, RangeControlBase>
      ("Minimum", 0.0, r => r.OnMinimumChangedPrivate);

    public static readonly DependencyProperty MaximumProperty = DPM.Register<double, RangeControlBase>
      ("Maximum", 1.0, r => r.OnMaximumChangedPrivate);

    #endregion

    #region Properties

    public double Maximum
    {
      get => (double) GetValue(MaximumProperty);
      set => SetValue(MaximumProperty, value);
    }

    public double Minimum
    {
      get => (double) GetValue(MinimumProperty);
      set => SetValue(MinimumProperty, value);
    }

    #endregion

    #region  Methods

    protected virtual void OnMaximumChanged(double oldValue, double newValue)
    {
    }

    private void OnMaximumChangedPrivate(double oldValue, double newValue)
    {
      OnMaximumChanged(oldValue, newValue);
    }

    protected virtual void OnMinimumChanged(double oldValue, double newValue)
    {
    }

    private void OnMinimumChangedPrivate(double oldValue, double newValue)
    {
      OnMinimumChanged(oldValue, newValue);
    }

    #endregion
  }
}