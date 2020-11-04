// <copyright file="SkinBindingBaseExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.Theming;
using NativeBinding = System.Windows.Data.Binding;

namespace Zaaml.PresentationCore.Data.MarkupExtensions
{
  public abstract class SkinBindingBaseExtension : BindingBaseExtension
  {
    #region Static Fields and Constants

    private static readonly PropertyPath PropertyPath = new PropertyPath(Extension.ActualSkinProperty);

    #endregion

    #region Ctors

    internal SkinBindingBaseExtension()
    {
    }

    #endregion

    #region Properties

    public string SkinPath { get; set; }

    protected abstract RelativeSource Source { get; }

    #endregion

    #region  Methods

    protected override NativeBinding GetBindingCore(IServiceProvider serviceProvider)
    {
      var binding = new NativeBinding
      {
        Path = PropertyPath,
        RelativeSource = Source,
      };

      InitBinding(binding);

      if (binding.Converter == null)
      {
        binding.Converter = SkinResourceConverter.Instance;
        binding.ConverterParameter = SkinPath;
      }
      else
      {
        binding.Converter = new WrapConverter(binding.Converter, binding.ConverterParameter);
        binding.ConverterParameter = SkinPath;
      }

      return binding;
    }

    #endregion

    #region  Nested Types

    private sealed class WrapConverter : IValueConverter
    {
      #region Ctors

      public WrapConverter(IValueConverter innerConverter, object converterParameter)
      {
        InnerConverter = innerConverter;
        ConverterParameter = converterParameter;
      }

      #endregion

      #region Properties

      private object ConverterParameter { get; }

      private IValueConverter InnerConverter { get; }

      #endregion

      #region Interface Implementations

      #region IValueConverter

      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
        var skinResource = SkinResourceConverter.Instance.Convert(value, targetType, parameter, culture);

        return InnerConverter.Convert(skinResource, targetType, ConverterParameter, culture);
      }

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      {
        throw new NotSupportedException();
      }

      #endregion

      #endregion
    }

    #endregion
  }
}