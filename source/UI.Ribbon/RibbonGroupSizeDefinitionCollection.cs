// <copyright file="RibbonGroupSizeDefinitionCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Zaaml.Core.Extensions;

namespace Zaaml.UI.Controls.Ribbon
{
  public sealed class RibbonGroupSizeDefinitionCollection : List<RibbonGroupSizeDefinition>, IRibbonGroupReducer
  {
    #region Interface Implementations

    #region IRibbonGroupReducer

    int IRibbonGroupReducer.GetReduceLevelsCount(IEnumerable<RibbonItem> items)
    {
      return Count;
    }

    RibbonControlGroupCollection IRibbonGroupReducer.Reduce(IEnumerable<RibbonItem> items, int reduceLevel)
    {
      var itemList = items.AsIList();

      var actualLevel = Math.Min(reduceLevel, Count - 1);
      if (actualLevel < 0)
      {
        foreach (var item in itemList)
          item.ActualItemStyle = RibbonItemStyle.Default;
      }
      else
      {
        var reduceLevelDefinition = this[actualLevel];
        var definitions = reduceLevelDefinition.Definitions;
        var itemDefinitionsCount = definitions.Count;
        for (var i = 0; i < itemList.Count; i++)
          itemList[i].ActualItemStyle = i < itemDefinitionsCount ? definitions[i].ItemStyle : RibbonItemStyle.Default;
      }

      return RibbonUtils.GroupItems(itemList, null);
    }

    #endregion

    #endregion
  }
}