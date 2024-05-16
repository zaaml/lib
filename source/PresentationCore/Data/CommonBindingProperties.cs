// <copyright file="CommonBindingProperties.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Zaaml.PresentationCore.Data
{
	internal class CommonBindingProperties
	{
		public IValueConverter Converter;
		public CultureInfo ConverterCulture;
		public object ConverterParameter;
		public object FallbackValue = DependencyProperty.UnsetValue;

		public BindingMode Mode = BindingMode.Default;

		public bool NotifyOnValidationError;
		public string StringFormat;
		public object TargetNullValue = DependencyProperty.UnsetValue;
		public UpdateSourceTrigger UpdateSourceTrigger;
		public bool ValidatesOnDataErrors;
		public bool ValidatesOnExceptions;
		public bool ValidatesOnNotifyDataErrors = true;

		public CommonBindingProperties Clone()
		{
			return new CommonBindingProperties
			{
				Converter = Converter,
				FallbackValue = FallbackValue,
				TargetNullValue = TargetNullValue,
				StringFormat = StringFormat,
				Mode = Mode,
				ConverterCulture = ConverterCulture,
				ConverterParameter = ConverterParameter,
				NotifyOnValidationError = NotifyOnValidationError,
				ValidatesOnDataErrors = ValidatesOnDataErrors,
				ValidatesOnExceptions = ValidatesOnExceptions,
				ValidatesOnNotifyDataErrors = ValidatesOnNotifyDataErrors,
				UpdateSourceTrigger = UpdateSourceTrigger,
			};
		}

		public void CopyFromBinding(Binding binding)
		{
			Converter = binding.Converter;
			FallbackValue = binding.FallbackValue;
			TargetNullValue = binding.TargetNullValue;
			StringFormat = binding.StringFormat;
			Mode = binding.Mode;
			ConverterCulture = binding.ConverterCulture;
			ConverterParameter = binding.ConverterParameter;
			NotifyOnValidationError = binding.NotifyOnValidationError;
			ValidatesOnDataErrors = binding.ValidatesOnDataErrors;
			ValidatesOnExceptions = binding.ValidatesOnExceptions;

#if NET_FRAMEWORK || NETCOREAPP
			ValidatesOnNotifyDataErrors = binding.ValidatesOnNotifyDataErrors;
#endif

			UpdateSourceTrigger = binding.UpdateSourceTrigger;
		}

		public void InitBinding(Binding binding)
		{
			//if (Converter != null)
			binding.Converter = Converter;
			//if (FallbackValue != DependencyProperty.UnsetValue)
			binding.FallbackValue = FallbackValue;
			//if (TargetNullValue != DependencyProperty.UnsetValue)
			binding.TargetNullValue = TargetNullValue;
			//if (string.IsNullOrEmpty(StringFormat) == false)
			binding.StringFormat = StringFormat;
			//if (Mode != BindingMode.Default)
			binding.Mode = Mode;
			//if (ConverterCulture != null)
			binding.ConverterCulture = ConverterCulture;
			//if (ConverterParameter != null)
			binding.ConverterParameter = ConverterParameter;

			binding.NotifyOnValidationError = NotifyOnValidationError;
			binding.ValidatesOnDataErrors = ValidatesOnDataErrors;
			binding.ValidatesOnExceptions = ValidatesOnExceptions;
#if NET_FRAMEWORK || NETCOREAPP
			binding.ValidatesOnNotifyDataErrors = ValidatesOnNotifyDataErrors;
#endif
			binding.UpdateSourceTrigger = UpdateSourceTrigger;
		}
	}
}