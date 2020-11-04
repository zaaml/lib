// <copyright file="BindingBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows.Data;
using NativeBinding = System.Windows.Data.Binding;

namespace Zaaml.PresentationCore.Data.MarkupExtensions
{
  public abstract class BindingBaseExtension : BindingMarkupExtension
  {
    #region Fields

    internal readonly CommonBindingProperties BindingProperties = new CommonBindingProperties();

    #endregion

    #region Properties

    public AllowedFramework AllowedFramework
    {
      get => BindingProperties.AllowedFramework;
      set => BindingProperties.AllowedFramework = value;
    }

    public IValueConverter Converter
    {
      get => BindingProperties.Converter;
      set => BindingProperties.Converter = value;
    }

    public CultureInfo ConverterCulture
    {
      get => BindingProperties.ConverterCulture;
      set => BindingProperties.ConverterCulture = value;
    }

    public object ConverterParameter
    {
      get => BindingProperties.ConverterParameter;
      set => BindingProperties.ConverterParameter = value;
    }

    public object FallbackValue
    {
      get => BindingProperties.FallbackValue;
      set => BindingProperties.FallbackValue = value;
    }

    public BindingMode Mode
    {
      get => BindingProperties.Mode;
      set => BindingProperties.Mode = value;
    }

    public bool NotifyOnValidationError
    {
      get => BindingProperties.NotifyOnValidationError;
      set => BindingProperties.NotifyOnValidationError = value;
    }

    public string StringFormat
    {
      get => BindingProperties.StringFormat;
      set => BindingProperties.StringFormat = value;
    }

    public object TargetNullValue
    {
      get => BindingProperties.TargetNullValue;
      set => BindingProperties.TargetNullValue = value;
    }

    public UpdateSourceTrigger UpdateSourceTrigger
    {
      get => BindingProperties.UpdateSourceTrigger;
      set => BindingProperties.UpdateSourceTrigger = value;
    }

    public bool ValidatesOnDataErrors
    {
      get => BindingProperties.ValidatesOnDataErrors;
      set => BindingProperties.ValidatesOnDataErrors = value;
    }

    public bool ValidatesOnExceptions
    {
      get => BindingProperties.ValidatesOnExceptions;
      set => BindingProperties.ValidatesOnExceptions = value;
    }

    public bool ValidatesOnNotifyDataErrors
    {
      get => BindingProperties.ValidatesOnNotifyDataErrors;
      set => BindingProperties.ValidatesOnNotifyDataErrors = value;
    }

		#endregion

		#region  Methods

		protected abstract NativeBinding GetBindingCore(IServiceProvider serviceProvider);

    protected internal sealed override NativeBinding GetBinding(IServiceProvider serviceProvider)
    {
      if (XamlConstants.Framework == FrameworkType.WPF && (AllowedFramework & AllowedFramework.WPF) == 0)
        return UnallowedBinding;

      if (XamlConstants.Framework == FrameworkType.Silverlight && (AllowedFramework & AllowedFramework.Silverlight) == 0)
        return UnallowedBinding;

      return GetBindingCore(serviceProvider);
    }

    protected virtual void InitBinding(NativeBinding binding)
    {
      BindingProperties.InitBinding(binding);
    }

    #endregion
  }
}