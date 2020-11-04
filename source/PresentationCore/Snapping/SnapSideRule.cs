// <copyright file="SnapSideRule.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Markup;

namespace Zaaml.PresentationCore.Snapping
{
  [ContentProperty(nameof(PointRules))]
  public class SnapSideRule
  {
    #region Fields

    private SnapSideCollection _alternative;

    private SnapPointRuleCollection _pointRules;

    #endregion

    #region Properties

    public SnapSideCollection Alternative
    {
      get => _alternative ?? (_alternative = new SnapSideCollection());
      set => _alternative = value;
    }

    public SnapPointRuleCollection PointRules
    {
      get => _pointRules ?? (_pointRules = new SnapPointRuleCollection());
      set => _pointRules = value;
    }

    public SnapSide Side { get; set; }

    #endregion
  }
}