// <copyright file="ValueRangeControlBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.Core;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Primitives
{
  public abstract class ValueRangeControlBase : RangeControlBase
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty ValueProperty = DPM.Register<double, ValueRangeControlBase>
      ("Value", v => v.OnValueChangedPrivate);

    #endregion

    #region Fields

    public event EventHandler<ValueChangedEventArgs<double>> ValueChanged;

    #endregion

    #region Properties

    public double Value
    {
      get => (double) GetValue(ValueProperty);
      set => SetValue(ValueProperty, value);
    }

    #endregion

    #region  Methods

    protected virtual void OnValueChanged(double oldValue, double newValue)
    {
      ValueChanged?.Invoke(this, new ValueChangedEventArgs<double>(oldValue, newValue));
    }

    private void OnValueChangedPrivate(double oldValue, double newValue)
    {
      OnValueChanged(oldValue, newValue);
    }

    #endregion
  }
}