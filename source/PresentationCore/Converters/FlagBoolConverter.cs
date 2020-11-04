// <copyright file="FlagBoolConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Data;

namespace Zaaml.PresentationCore.Converters
{
  public class FlagBoolConverter : FlagConverterBase
  {
    #region Static Fields and Constants

    public static readonly IValueConverter Instance = new FlagBoolConverter();

    #endregion

    #region Ctors

    private FlagBoolConverter()
    {
    }

    #endregion

    #region  Methods

    protected override object GetValue(bool value)
    {
      return value;
    }

    #endregion
  }
}