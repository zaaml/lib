// <copyright file="IsSubclassOfConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows;

namespace Zaaml.PresentationCore.Converters
{
  public sealed class IsSubclassOfConverter : BaseValueConverter
  {
    #region Properties

    public bool Self { get; set; } = true;

		public Type Type { get; set; }

    #endregion

    #region  Methods

    protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }

    protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        return false;

      var type = value.GetType();

      return type.IsSubclassOf(Type) || (Self && Type == type);
    }

    #endregion
  }

  public sealed class IsSubclassOfVisibilityConverter : BaseValueConverter
  {
	  #region Properties

	  public bool Self { get; set; } = true;

		public Type Type { get; set; }

	  public Visibility TrueVisibility { get; set; } = Visibility.Visible;

	  public Visibility FalseVisibility { get; set; } = Visibility.Collapsed;

	  #endregion

	  #region  Methods

	  protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
	  {
		  throw new NotSupportedException();
	  }

	  protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
	  {
		  if (value == null)
			  return false;

		  var type = value.GetType();

		  return type.IsSubclassOf(Type) || (Self && Type == type) ? TrueVisibility : FalseVisibility;
	  }

	  #endregion
  }
}