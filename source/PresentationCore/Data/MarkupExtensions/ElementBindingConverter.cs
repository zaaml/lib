// <copyright file="ElementBindingConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.Converters;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Data.MarkupExtensions
{
  internal class ElementBindingConverter : WrapConverter
  {
    #region Fields

    private readonly string _elementName;

    #endregion

    #region Ctors

    public ElementBindingConverter(string elementName, IValueConverter converter)
      : base(converter)
    {
      _elementName = elementName;
    }

    #endregion

    #region  Methods

    protected override object ConvertBackOverride(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }

    protected override object ConvertOverride(object value, Type targetType, object parameter, CultureInfo culture)
    {
	    if (value is FrameworkElement fre)
        NameScopeUtils.FindName(fre, _elementName);

      return value;
    }

    #endregion
  }
}