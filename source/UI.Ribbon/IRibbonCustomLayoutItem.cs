// <copyright file="IRibbonCustomLayoutItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Ribbon
{
  public interface IRibbonCustomLayoutItem
  {
    #region Properties

    int ReduceLevel { get; set; }

    int ReduceLevelsCount { get; }

    #endregion
  }
}