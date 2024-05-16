// <copyright file="BindingBaseExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows.Data;
using NativeBinding = System.Windows.Data.Binding;

namespace Zaaml.PresentationCore.Data.MarkupExtensions
{
	public abstract class BindingBaseExtension : BindingMarkupExtension
	{
		internal readonly CommonBindingProperties BindingProperties = new();

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

		protected internal sealed override NativeBinding GetBinding(IServiceProvider serviceProvider)
		{
			return GetBindingCore(serviceProvider);
		}

		protected abstract NativeBinding GetBindingCore(IServiceProvider serviceProvider);

		protected virtual void InitBinding(NativeBinding binding)
		{
			BindingProperties.InitBinding(binding);
		}
	}
}