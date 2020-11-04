// <copyright file="SwitchConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows.Markup;
using Zaaml.Core.Collections;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Converters
{
  [ContentProperty("Value")]
  public abstract class SwitchOption
  {
    #region Fields

    private XamlConvertCacheStruct _keyCache;
    private XamlConvertCacheStruct _valueCache;

    #endregion

    #region Properties

    public object Key
    {
      get => _keyCache.Value;
      set => _keyCache.Value = value;
    }

    public object Value
    {
      get => _valueCache.Value;
      set => _valueCache.Value = value;
    }

    #endregion

    #region  Methods

    internal object XamlConvertKey(Type targetType)
    {
      return _keyCache.XamlConvert(targetType);
    }

    internal object XamlConvertValue(Type targetType)
    {
      return _valueCache.XamlConvert(targetType);
    }

    #endregion
  }

  public class Case : SwitchOption
  {
  }

  public class Default : SwitchOption
  {
  }

  public class OptionCollection : CollectionBase<SwitchOption>
  {
  }

  [ContentProperty("Options")]
  public sealed class SwitchConverter : BaseValueConverter
  {
    #region Static Fields and Constants

    private static readonly SwitchOption FallBackCase = new Case();

    #endregion

    #region Ctors

    public SwitchConverter()
    {
      Options = new OptionCollection();
    }

    #endregion

    #region Properties

    public OptionCollection Options { get; }

    #endregion

    #region  Methods

    protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return ConvertImpl(value, targetType, SwitchConvertDirection.Reverse);
    }


    protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return ConvertImpl(value, targetType, SwitchConvertDirection.Direct);
    }

    private object ConvertImpl(object value, Type targetType, SwitchConvertDirection direction)
    {
      var valueType = value?.GetType() ?? typeof(object);

      Default defaultCase = null;

      foreach (var option in Options)
      {
        var caseOption = option as Case;
        if (caseOption != null)
        {
          var caseKey = GetKey(caseOption, direction, valueType);
          if (Equals(value, caseKey))
            return GetValue(caseOption, direction, targetType);

          continue;
        }

        var defaultOption = option as Default;
        if (defaultOption != null && defaultCase != null)
          throw new Exception("SwitchConverter can have only 1 default case");

        defaultCase = defaultOption;
      }

      return defaultCase != null ? GetValue(defaultCase, direction, targetType) : FallBackCase.Value.XamlConvert(targetType);
    }

    private static object GetKey(SwitchOption switchOption, SwitchConvertDirection direction, Type targetType)
    {
      return direction == SwitchConvertDirection.Direct ? switchOption.XamlConvertKey(targetType) : switchOption.XamlConvertValue(targetType);
    }

    private static object GetValue(SwitchOption switchOption, SwitchConvertDirection direction, Type targetType)
    {
      return direction == SwitchConvertDirection.Direct ? switchOption.XamlConvertValue(targetType) : switchOption.XamlConvertKey(targetType);
    }

    #endregion

    #region  Nested Types

    private enum SwitchConvertDirection
    {
      Direct,
      Reverse
    }

    #endregion
  }
}