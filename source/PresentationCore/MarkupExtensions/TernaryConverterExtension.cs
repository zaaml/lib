// <copyright file="TernaryConverterExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core;
using Zaaml.PresentationCore.Converters;

namespace Zaaml.PresentationCore.MarkupExtensions
{
  public class TernaryConverterExtension : MarkupExtensionBase
  {
    #region Fields

    private object _falseValue = Unset.Value;
    private object _operand = Unset.Value;
    private TernaryConverter _ternaryConverter;
    private object _trueValue = Unset.Value;

    #endregion

    #region Properties

    public object FalseValue
    {
      get => _falseValue.GetSetValueOrDefault();
      set
      {
        _falseValue = value;
        _ternaryConverter = null;
      }
    }

    public object Operand
    {
      get => _operand.GetSetValueOrDefault();
      set
      {
        _operand = value;
        _ternaryConverter = null;
      }
    }

    public object TrueValue
    {
      get => _trueValue.GetSetValueOrDefault();
      set
      {
        _trueValue = value;
        _ternaryConverter = null;
      }
    }

    #endregion

    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      if (_ternaryConverter != null)
        return _ternaryConverter;

      _ternaryConverter = new TernaryConverter();

      if (_operand.IsSet())
        _ternaryConverter.Operand = Operand;

      if (_trueValue.IsSet())
        _ternaryConverter.TrueValue = TrueValue;

      if (_falseValue.IsSet())
        _ternaryConverter.FalseValue = FalseValue;

      _ternaryConverter.IsSealed = true;

      return _ternaryConverter;
    }

    #endregion
  }
}