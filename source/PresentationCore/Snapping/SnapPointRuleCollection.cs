// <copyright file="SnapPointRuleCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Zaaml.PresentationCore.Snapping
{
  [TypeConverter(typeof(SnapPointRuleCollectionTypeConverter))]
  public sealed class SnapPointRuleCollection : List<SnapPointRule>
  {
    #region Static Fields and Constants

    private static readonly char[] Separators = {',', ' '};

    private static readonly Dictionary<string, SnapPoint> ConvertDictionary = new Dictionary<string, SnapPoint>(StringComparer.OrdinalIgnoreCase)
    {
      {"LeftBottom", SnapPoint.LeftBottom},
      {"LeftCenter", SnapPoint.LeftCenter},
      {"LeftTop", SnapPoint.LeftTop},
      {"TopLeft", SnapPoint.TopLeft},
      {"TopCenter", SnapPoint.TopCenter},
      {"TopRight", SnapPoint.TopRight},
      {"RightTop", SnapPoint.RightTop},
      {"RightCenter", SnapPoint.RightCenter},
      {"RightBottom", SnapPoint.RightBottom},
      {"BottomRight", SnapPoint.BottomRight},
      {"BottomCenter", SnapPoint.BottomCenter},
      {"BottomLeft", SnapPoint.BottomLeft},
      {"LB", SnapPoint.LeftBottom},
      {"LC", SnapPoint.LeftCenter},
      {"LT", SnapPoint.LeftTop},
      {"TL", SnapPoint.TopLeft},
      {"TC", SnapPoint.TopCenter},
      {"TR", SnapPoint.TopRight},
      {"RT", SnapPoint.RightTop},
      {"RC", SnapPoint.RightCenter},
      {"RB", SnapPoint.RightBottom},
      {"BR", SnapPoint.BottomRight},
      {"BC", SnapPoint.BottomCenter},
      {"BL", SnapPoint.BottomLeft}
    };

    #endregion

    #region  Methods

    public static SnapPointRuleCollection Parse(string strValue)
    {
      var snapPointRuleCollection = new SnapPointRuleCollection();

      var delimitedValues = strValue.Split(Separators, StringSplitOptions.RemoveEmptyEntries);

      foreach (var snapPointRuleStr in delimitedValues)
      {
        var hyphenIndex = snapPointRuleStr.IndexOf("-", StringComparison.OrdinalIgnoreCase);
        if (hyphenIndex == -1)
          throw new InvalidOperationException($"Can not convert string value '{snapPointRuleStr}' to value of type SnapPointRule");

        var targetStr = snapPointRuleStr.Substring(0, hyphenIndex);
        var sourceStr = snapPointRuleStr.Substring(hyphenIndex + 1, snapPointRuleStr.Length - hyphenIndex - 1);

        SnapPoint target;
        SnapPoint source;

        if (ConvertDictionary.TryGetValue(targetStr, out target) == false)
          throw new InvalidOperationException($"Can not convert string value '{targetStr}' to value of type SnapPoint");


        if (ConvertDictionary.TryGetValue(sourceStr, out source) == false)
          throw new InvalidOperationException($"Can not convert string value '{sourceStr}' to value of type SnapPoint");

        snapPointRuleCollection.Add(new SnapPointRule
        {
          Target = target,
          Source = source
        });
      }

      return snapPointRuleCollection;
    }

    #endregion
  }
}