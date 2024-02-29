// <copyright file="IconConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Xaml;
using Zaaml.Core;
using Zaaml.PresentationCore.TypeConverters;

namespace Zaaml.UI.Controls.Primitives.ContentPrimitives
{
	internal static class IconConverter
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
						return new ImageIcon { Source = imageSource };
				}

				if (value is string stringValue)
					return UpdateBaseUri(context, new ImageIcon { Source = new BitmapImage(new Uri(stringValue, UriKind.RelativeOrAbsolute)) });

				var uriValue = value as Uri;

				if (uriValue != null)
					return UpdateBaseUri(context, new ImageIcon { Source = new BitmapImage(uriValue) });

				if (value is ImageSource imageSourceValue)
					return new ImageIcon { Source = imageSourceValue };

				if (value is Geometry geometry)
					return new PathIcon { Data = geometry };

				throw new InvalidOperationException();
			}
			catch (Exception e)
			{
				LogService.LogError(e);
			}

			return new ImageIcon();
		}

		private static ImageIcon UpdateBaseUri(IServiceProvider context, ImageIcon icon)
		{
			var rootProvider = context?.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;

			if (rootProvider?.RootObject is DependencyObject dependencyObject)
				icon.BaseUri = BaseUriHelper.GetBaseUri(dependencyObject);

			return icon;
		}
	}
}