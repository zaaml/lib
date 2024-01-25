// <copyright file="DependencyPropertyExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.PropertyCore.Extensions
{
  internal static class DependencyPropertyExtensions
  {
    #region  Methods

    public static DependencyPropertyValueInfo GetDependencyPropertyValueInfo(this DependencyObject dependencyObject, DependencyProperty dependencyProperty)
    {
      return new DependencyPropertyValueInfo(dependencyObject, dependencyProperty);
    }

    public static PropertyValueSource GetValueSource(this DependencyObject dependencyObject, DependencyProperty dependencyProperty)
    {
      return dependencyObject.GetDependencyPropertyValueInfo(dependencyProperty).ValueSource;
    }

		// TODO: Review logic. Current isn't correct. It returns true when current value differs from default value, but value source is default (SetCurrentValue)
    public static bool HasLocalValue(this DependencyObject dependencyObject, DependencyProperty dependencyProperty)
    {
      return dependencyObject.GetDependencyPropertyValueInfo(dependencyProperty).HasLocalValue;
    }

    public static bool IsDefaultValue(this DependencyObject dependencyObject, DependencyProperty dependencyProperty)
    {
      return dependencyObject.GetDependencyPropertyValueInfo(dependencyProperty).IsDefaultValue;
    }

    public static bool IsDependencyPropertyUnsetValue(this object value)
    {
      return ReferenceEquals(value, DependencyProperty.UnsetValue);
    }

    public static bool TryGetNonDefaultValue<TValue>(this DependencyObject dependencyObject, DependencyProperty dependencyProperty, out TValue value)
    {
      var valueInfo = dependencyObject.GetDependencyPropertyValueInfo(dependencyProperty);

      value = default(TValue);

      if (valueInfo.IsDefaultValue)
        return false;

      value = (TValue) valueInfo.Value;

      return true;
    }

    #endregion
  }
}