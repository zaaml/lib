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
		protected static readonly PropertyPath ActualSkinPropertyPath = new(Extension.ActualSkinProperty);

		internal SkinBindingBaseExtension()
		{
		}

		public string SkinPath { get; set; }

		protected override NativeBinding GetBindingCore(IServiceProvider serviceProvider)
		{
			var binding = new NativeBinding();

			SetBindingSource(binding);
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

		protected abstract void SetBindingSource(NativeBinding binding);

		private sealed class WrapConverter : IValueConverter
		{
			public WrapConverter(IValueConverter innerConverter, object converterParameter)
			{
				InnerConverter = innerConverter;
				ConverterParameter = converterParameter;
			}

			private object ConverterParameter { get; }

			private IValueConverter InnerConverter { get; }

			public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			{
				var skinResource = SkinResourceConverter.Instance.Convert(value, targetType, parameter, culture);

				return InnerConverter.Convert(skinResource, targetType, ConverterParameter, culture);
			}

			public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			{
				throw new NotSupportedException();
			}
		}
	}
}