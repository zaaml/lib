// <copyright file="IPanelAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interfaces
{
  internal interface IPanelAdvisor
  {
    #region Properties

    bool ShouldArrange { get; }

    bool ShouldMeasure { get; }

    #endregion
  }
}