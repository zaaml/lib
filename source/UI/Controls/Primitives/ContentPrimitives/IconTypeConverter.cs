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
    #region  Methods

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
	    return base.CanConvertTo(context, destinationType);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
	    return base.ConvertTo(context, culture, value, destinationType);
    }

    public override bool IsValid(ITypeDescriptorContext context, object value)
    {
	    return base.IsValid(context, value);
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return IconConverterImpl.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      return IconConverterImpl.ConvertFrom(context, culture, value);
    }

    #endregion
  }

  internal static class IconConverterImpl
  {
    #region  Methods

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

        if (context != null)
        {
          var imageSource = (ImageSource)SafeImageSourceConverter.Converter.ConvertFrom(context, culture, value);
          if (imageSource != null)
            return new BitmapIcon { Source = imageSource };
        }

        var stringValue = value as string;

        if (stringValue != null)
          return UpdateBaseUri(context, new BitmapIcon {Source = new BitmapImage(new Uri(stringValue, UriKind.RelativeOrAbsolute))});

        var uriValue = value as Uri;
        if (uriValue != null)
          return UpdateBaseUri(context, new BitmapIcon {Source = new BitmapImage(uriValue)});

        var imageSourceValue = value as ImageSource;
        if (imageSourceValue != null)
          return new BitmapIcon { Source = imageSourceValue };

        var geometry = value as Geometry;
        if (geometry != null)
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

      var ro = rootProvider?.RootObject as DependencyObject;
      if (ro != null)
        icon.BaseUri = BaseUriHelper.GetBaseUri(ro);
#endif

      return icon;
    }

    #endregion
  }

  public class IconConverter : MarkupExtensionBase, IValueConverter
  {
    #region Static Fields and Constants

    public static readonly IconConverter Instance = new IconConverter();

    #endregion

    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return Instance;
    }

    #endregion

    #region Interface Implementations

    #region IValueConverter

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return IconConverterImpl.ConvertFrom(null, culture, value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return new NotSupportedException();
    }

    #endregion

    #endregion
  }
}