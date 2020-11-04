// <copyright file="SnapPointRule.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Snapping
{
  public struct SnapPointRule
  {
    public SnapPointRule(SnapPoint source, SnapPoint target)
    {
      Source = source;
      Target = target;
    }

    #region Properties

    public SnapPoint Source { get; set; }
    public SnapPoint Target { get; set; }

    #endregion
  }
}