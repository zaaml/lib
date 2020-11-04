// <copyright file="IRibbonGroupReducer.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.UI.Controls.Ribbon
{
  internal interface IRibbonGroupReducer
  {
    #region  Methods

    int GetReduceLevelsCount(IEnumerable<RibbonItem> items);
    RibbonControlGroupCollection Reduce(IEnumerable<RibbonItem> items, int reduceLevel);

    #endregion
  }
}