// <copyright file="RibbonItemMeasurement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using Zaaml.Core.Extensions;

namespace Zaaml.UI.Controls.Ribbon
{
  internal class RibbonItemMeasurement
  {
    #region Fields

    private Dictionary<int, Size> _customSizes;

    public Size Large;
    public Size Medium;
    public Size Small;

    #endregion

    #region Ctors

    public RibbonItemMeasurement()
    {
      Reset();
    }

    #endregion

    #region  Methods

    private void EnsureCustomSizeDictionary()
    {
      if (_customSizes == null)
        _customSizes = new Dictionary<int, Size>();
    }

    public Size GetCustomSize(int reduceLevel)
    {
      EnsureCustomSizeDictionary();
      return IDictionaryExtensions.GetValueOrDefault(_customSizes, reduceLevel, Size.Empty);
    }

    public Size GetSize(RibbonItemStyle itemStyle)
    {
      switch (itemStyle)
      {
        case RibbonItemStyle.Large:
          return Large;
        case RibbonItemStyle.Medium:
          return Medium;
        case RibbonItemStyle.Small:
          return Small;
        default:
          throw new ArgumentOutOfRangeException(nameof(itemStyle));
      }
    }

    public void Reset()
    {
      Large = Size.Empty;
      Small = Size.Empty;
      Medium = Size.Empty;

      _customSizes?.Clear();
    }

    public void SetCustomSize(int reduceLevel, Size size)
    {
      _customSizes[reduceLevel] = size;
    }

    public void SetSize(RibbonItemStyle itemStyle, Size size)
    {
      switch (itemStyle)
      {
        case RibbonItemStyle.Large:
          Large = size;
          break;
        case RibbonItemStyle.Medium:
          Medium = size;
          break;
        case RibbonItemStyle.Small:
          Small = size;
          break;
      }
    }

    #endregion
  }
}