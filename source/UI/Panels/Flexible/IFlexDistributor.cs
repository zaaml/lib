// <copyright file="IFlexPanelDistributor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Panels.Flexible
{
  public interface IFlexDistributor
  {
    #region  Methods

    void Distribute(FlexElementCollection elements, double target);

    #endregion
  }
}