// <copyright file="IconTypeConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Zaaml.Core;
using Zaaml.PresentationCore.MarkupExtensions;
using Zaaml.PresentationCore.TypeConverters;
#if !SILVERLIGHT
using System.Xaml;
#endif

namespace Zaaml.UI.Controls.Primitives.ContentPrimitives
{
	internal class IconTypeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return IconConverterImpl.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return IconConverterImpl.ConvertFrom(context, culture, value);
		}
	}

	internal static class IconConverterImpl
	{
		public static bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || sourceType == typeof(Uri) || sourceType == typeof(Geometry) || typeof(ImageSource).IsAssignableFrom(sourceType);
		}

		public static object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			try
			{
				if (value == null)
					return null;

				if (value is IconBase)
					return value;

				if (context != null)
				{
					var imageSource = (ImageSource)SafeImageSourceConverter.Converter.ConvertFrom(context, culture, value);

					if (imageSource != null)
						return new BitmapIcon { Source = imageSource };
				}

				if (value is string stringValue)
					return UpdateBaseUri(context, new BitmapIcon { Source = new BitmapImage(new Uri(stringValue, UriKind.RelativeOrAbsolute)) });

				var uriValue = value as Uri;

				if (uriValue != null)
					return UpdateBaseUri(context, new BitmapIcon { Source = new BitmapImage(uriValue) });

				if (value is ImageSource imageSourceValue)
					return new BitmapIcon { Source = imageSourceValue };

				if (value is Geometry geometry)
					return new PathIcon { Data = geometry };

				throw new InvalidOperationException();
			}
			catch (Exception e)
			{
				LogService.LogError(e);
			}

			return new BitmapIcon();
		}

		private static BitmapIcon UpdateBaseUri(IServiceProvider context, BitmapIcon icon)
		{
#if !SILVERLIGHT
			var rootProvider = context?.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;

			if (rootProvider?.RootObject is DependencyObject ro)
				icon.BaseUri = BaseUriHelper.GetBaseUri(ro);
#endif

			return icon;
		}
	}

	public class IconConverterExtension : MarkupExtensionBase, IValueConverter
	{
		public static readonly IconConverterExtension Instance = new();

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return Instance;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return IconConverterImpl.ConvertFrom(null, culture, value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return new NotSupportedException();
		}
	}
}