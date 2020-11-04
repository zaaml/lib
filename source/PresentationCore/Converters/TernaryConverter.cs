// <copyright file="TernaryConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using Zaaml.Core;
using Zaaml.Core.Utils;

namespace Zaaml.PresentationCore.Converters
{
  public sealed class TernaryConverter : BaseValueConverter
  {
    #region Fields

    private XamlConvertCacheStruct _actualOperand;
    private XamlConvertCacheStruct _falseValue;

    private object _operand = Unset.Value;
    private XamlConvertCacheStruct _trueValue;

    #endregion

    #region Properties

    public object FalseValue
    {
      get => _falseValue.Value;
      set
      {
        CheckSeal();
        _falseValue.Value = value;
      }
    }

    internal bool IsSealed { get; set; }

    public object Operand
    {
      get => _operand.GetSetValueOrDefault();
      set
      {
        CheckSeal();
        _operand = value;
      }
    }

    public object TrueValue
    {
      get => _trueValue.Value;
      set
      {
        CheckSeal();
        _trueValue.Value = value;
      }
    }

    #endregion

    #region  Methods

    private void CheckSeal()
    {
      if (IsSealed)
        throw new InvalidOperationException("Converter is sealed and can not be changed");
    }

    private object Convert(bool result, Type targetType)
    {
      return result ? _trueValue.XamlConvert(targetType) : _falseValue.XamlConvert(targetType);
    }

    protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }


    protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var actualOperand = _operand.GetSetValueOrDefault(parameter);

      if (value == null && actualOperand == null)
        return Convert(true, targetType);

      if (value != null && actualOperand != null)
      {
        _actualOperand.Value = actualOperand;
        var convertedOperand = _actualOperand.XamlConvert(value.GetType());

        return Convert(Equals(value, convertedOperand), targetType);
      }

      if (parameter == null && _operand.IsUnset())
        return Convert(GetImplicitBool(value), targetType);

      return Convert(GetImplicitBool(value) == GetImplicitBool(actualOperand), targetType);
    }

    private static bool GetImplicitBool(object value)
    {
      return BoolUtils.ImplicitConvertFrom(value);
    }

    #endregion
  }
}