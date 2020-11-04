// <copyright file="NumericUpDown.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Primitives
{
  public class NumericUpDown : UpDownBase<double>
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty MinimumProperty = DPM.Register<double, NumericUpDown>
      ("Minimum", 0d, n => n.OnMinimumChanged, n => n.CoerceMinimum);

    public static readonly DependencyProperty MaximumProperty = DPM.Register<double, NumericUpDown>
      ("Maximum", 100d, n => n.OnMaximumChanged, n => n.CoerceMaximum);

    public static readonly DependencyProperty IncrementProperty = DPM.Register<double, NumericUpDown>
      ("Increment", 1d, n => n.OnIncrementChanged, n => n.CoerceIncrement);

    public static readonly DependencyProperty DecimalPlacesProperty = DPM.Register<int, NumericUpDown>
      ("DecimalPlaces", 0, n => n.OnDecimalPlacesChanged, n => n.CoerceDecimalPlaces);

    #endregion

    #region Fields

    private int _coercionLevel;

    private string _formatString = "F0";

    #endregion

    #region Ctors

    static NumericUpDown()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<NumericUpDown>();
    }

    public NumericUpDown()
    {
      this.OverrideStyleKey<NumericUpDown>();
    }

    #endregion

    #region Properties

    public int DecimalPlaces
    {
      get => (int) GetValue(DecimalPlacesProperty);
      set => SetValue(DecimalPlacesProperty, value);
    }

    public double Increment
    {
      get => (double) GetValue(IncrementProperty);
      set => SetValue(IncrementProperty, value);
    }

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

    private void Coerce()
    {
      _coercionLevel++;

      CoerceMinMaxValue();

      var increment = Increment;
      EnsureValidDoubleValue(increment);

      _coercionLevel--;
    }

    private int CoerceDecimalPlaces(int newValue)
    {
      if (_coercionLevel > 0 || IsInitializing)
        return newValue;

      EnsureValidDoubleValue(newValue);

      return newValue;
    }

    private double CoerceIncrement(double newValue)
    {
      if (_coercionLevel > 0 || IsInitializing)
        return newValue;

      EnsureValidDoubleValue(newValue);

      return newValue;
    }

    private double CoerceMaximum(double newValue)
    {
      if (_coercionLevel > 0 || IsInitializing)
        return newValue;

      EnsureValidDoubleValue(newValue);

      return newValue;
    }

    private double CoerceMinimum(double newValue)
    {
      if (_coercionLevel > 0 || IsInitializing)
        return newValue;

      EnsureValidDoubleValue(newValue);

      return newValue;
    }

    private void CoerceMinMaxValue(ChangeMinMaxAction changeAction = null)
    {
      _coercionLevel++;

      var minimum = Minimum;
      var maximum = Maximum;
      var value = Value;

      EnsureValidDoubleValue(minimum);
      EnsureValidDoubleValue(maximum);
      EnsureValidDoubleValue(value);

      var minMaxHelper = new MinMaxValueHelper(minimum, maximum, value);
      if (minMaxHelper.IsValid == false || changeAction != null)
      {
        minMaxHelper.Fix();

        changeAction?.Invoke(ref minMaxHelper);

        Update(minimum, maximum, value, minMaxHelper);
      }

      _coercionLevel--;
    }

    protected override double CoerceValue(double newValue)
    {
      if (_coercionLevel > 0 || IsInitializing)
        return newValue;

      EnsureValidDoubleValue(newValue);

      return newValue;
    }

    private static void EnsureValidDoubleValue(double value)
    {
      if (value.IsNaN() || value.IsInfinity() || value > (double) decimal.MaxValue || value < (double) decimal.MinValue)
        throw new ArgumentException();
    }

    protected internal override string FormatValue()
    {
      return Value.ToString(_formatString, CultureInfo.CurrentCulture);
    }

    public void Init(double minimum, double maximum, double value)
    {
      var minMaxHelper = new MinMaxValueHelper(minimum, maximum, value);
      minMaxHelper.Fix();

      Update(minimum, maximum, value, minMaxHelper);
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      SetTextBoxFormattedValue();
      SetValidSpinDirection();
    }

    private void OnDecimalPlacesChanged(int oldValue, int newValue)
    {
      _formatString = string.Format(NumberFormatInfo.InvariantInfo, "F{0:D}", DecimalPlaces);
      SetTextBoxFormattedValue();
    }

    protected override void OnDecrement()
    {
      Value = (double) ((decimal) Value - (decimal) Increment);
    }

    protected override void OnEndInit()
    {
      Coerce();
      base.OnEndInit();
    }

    protected override void OnIncrement()
    {
      Value = (double) ((decimal) Value + (decimal) Increment);
    }

    protected virtual void OnIncrementChanged(double oldValue, double newValue)
    {
    }

    protected virtual void OnMaximumChanged(double oldValue, double newValue)
    {
      CoerceMinMaxValue((ref MinMaxValueHelper t) => t.ChangeMaximum(newValue));
    }

    protected virtual void OnMinimumChanged(double oldValue, double newValue)
    {
      CoerceMinMaxValue((ref MinMaxValueHelper t) => t.ChangeMinimum(newValue));
    }

    protected override void OnValueChanged(PropertyValueChangedEventArgs<double> e)
    {
      if (_coercionLevel == 0 && IsInitializing == false)
        CoerceMinMaxValue((ref MinMaxValueHelper t) => t.ChangeValue(e.NewValue));

      SetValidSpinDirection();
      SetTextBoxFormattedValue();

      base.OnValueChanged(e);
    }

    protected override double ParseValue(string text)
    {
      return double.Parse(text, CultureInfo.CurrentCulture);
    }

    private void SetValidSpinDirection()
    {
      if (Spinner == null)
        return;

      Spinner.ActualCanIncrease = Value < Maximum;
      Spinner.ActualCanDecrease = Value > Minimum;
    }

    private void Update(double initialMinimum, double initialMaximum, double initialValue, MinMaxValueHelper minMaxHelper)
    {
      if (initialMinimum.Equals(minMaxHelper.Minimum) == false)
        Minimum = minMaxHelper.Minimum;

      if (initialMaximum.Equals(minMaxHelper.Maximum) == false)
        Maximum = minMaxHelper.Maximum;

      if (initialValue.Equals(minMaxHelper.Value) == false)
        Value = minMaxHelper.Value;
    }

    #endregion

    #region  Nested Types

    private delegate void ChangeMinMaxAction(ref MinMaxValueHelper minMaxValueHelper);

    #endregion
  }

  internal struct MinMaxValueHelper
  {
    public double Minimum;
    public double Maximum;
    public double Value;

    public MinMaxValueHelper(double minimum, double maximum, double value)
    {
      Minimum = minimum;
      Maximum = maximum;
      Value = value;
    }

    public void Fix()
    {
      if (IsValid)
        return;

      if (Maximum.IsLessThan(Minimum))
        Maximum = Minimum;

      Value = Value.Clamp(Minimum, Maximum);
    }

    public void ChangeValue(double newValue)
    {
      if (Value.Equals(newValue))
        return;

      Value = newValue.Clamp(Minimum, Maximum);

      Fix();
    }

    public void ChangeMinimum(double newMinimum)
    {
      if (newMinimum.Equals(Minimum))
        return;

      Minimum = newMinimum;
      if (Maximum.IsLessThan(Minimum))
        Maximum = Minimum;

      Fix();
    }

    public void EnsureValid()
    {
      if (IsValid)
        throw new InvalidOperationException();
    }

    public void ChangeMaximum(double newMaximum)
    {
      if (newMaximum.Equals(Maximum))
        return;

      Maximum = newMaximum;
      if (Minimum.IsGreaterThan(Maximum))
        Minimum = Maximum;

      Fix();
    }

    public bool IsValid => Value.IsGreaterThanOrClose(Minimum) && Value.IsLessThanOrClose(Maximum);
  }
}