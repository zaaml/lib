// <copyright file="DefaultRibbonGroupReducer.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using Zaaml.Core.Extensions;

namespace Zaaml.UI.Controls.Ribbon
{
  internal class DefaultRibbonGroupReducer : IRibbonGroupReducer
  {
    #region Static Fields and Constants

    public static readonly IRibbonGroupReducer Instance = new DefaultRibbonGroupReducer();

    #endregion

    #region Ctors

    private DefaultRibbonGroupReducer()
    {
    }

    #endregion

    #region Interface Implementations

    #region IRibbonGroupReducer

    public int GetReduceLevelsCount(IEnumerable<RibbonItem> items)
    {
      return RibbonUtils.GroupItems(items.AsIList(), item => RibbonItemStyle.Default).Select(g => g.ReduceLevelCount).Sum();
    }

    public RibbonControlGroupCollection Reduce(IEnumerable<RibbonItem> items, int reduceLevel)
    {
      var grouppedItems = RibbonUtils.GroupItems(items.AsIList(), item => RibbonItemStyle.Default);

      if (reduceLevel == 0)
        return grouppedItems;

      do
      {
        var current = reduceLevel;
        for (var iGroup = grouppedItems.Count - 1; iGroup >= 0 && reduceLevel > 0; iGroup--)
        {
          var controlGroup = grouppedItems[iGroup];
          if (controlGroup.CanReduce == false) continue;

          grouppedItems[iGroup] = controlGroup.Reduce();
          reduceLevel--;
        }

        if (reduceLevel == current)
          break;
      } while (reduceLevel > 0);

      return grouppedItems;
    }

    #endregion

    #endregion
  }

  internal class RibbonControlGroupCollection : List<RibbonItemGroup>
  {
  }
}