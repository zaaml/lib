// <copyright file="SnapDefinition.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;
using Zaaml.Core.Extensions;
using Zaaml.Core.Interfaces;

namespace Zaaml.PresentationCore.Snapping
{
  [ContentProperty(nameof(SideRules))]
  public class SnapDefinition : ISealable
  {
    #region Fields

    private double _sideOffset;
    private double _sourceCornerOffset;
    private double _targetCornerOffset;

    #endregion

    #region Ctors

    static SnapDefinition()
    {
      Default.IsSealed = true;
      Center.IsSealed = true;
      CornerCenterCorner.IsSealed = true;
      CornerCorner.IsSealed = true;
      TargetCenterSourceCorner.IsSealed = true;
    }

    #endregion

    #region Properties

    public static SnapDefinition Default { get; } = new SnapDefinition
    {
      SideRules =
      {
        new SnapSideRule {Side = SnapSide.Left, Alternative = {SnapSide.Right}, PointRules = SnapPointRuleCollection.Parse("LT-RT, LB-RB")},
        new SnapSideRule {Side = SnapSide.Right, Alternative = {SnapSide.Left}, PointRules = SnapPointRuleCollection.Parse("RT-LT, RB-LB")},
        new SnapSideRule {Side = SnapSide.Top, Alternative = {SnapSide.Bottom}, PointRules = SnapPointRuleCollection.Parse("TL-BL, TR-BR")},
        new SnapSideRule {Side = SnapSide.Bottom, Alternative = {SnapSide.Top}, PointRules = SnapPointRuleCollection.Parse("BL-TL, BR-TR")}
      }
    };

    public static SnapDefinition CornerCorner { get; } = new SnapDefinition
    {
      SideRules =
      {
        new SnapSideRule {Side = SnapSide.Left, Alternative = {SnapSide.Right}, PointRules = SnapPointRuleCollection.Parse("LT-RT, LB-RB")},
        new SnapSideRule {Side = SnapSide.Right, Alternative = {SnapSide.Left}, PointRules = SnapPointRuleCollection.Parse("RT-LT, RB-LB")},
        new SnapSideRule {Side = SnapSide.Top, Alternative = {SnapSide.Bottom}, PointRules = SnapPointRuleCollection.Parse("TL-BL, TR-BR")},
        new SnapSideRule {Side = SnapSide.Bottom, Alternative = {SnapSide.Top}, PointRules = SnapPointRuleCollection.Parse("BL-TL, BR-TR")}
      }
    };

    // ReSharper disable once InconsistentNaming
    public static SnapDefinition CornerCornerRTL { get; } = new SnapDefinition
    {
      SideRules =
      {
        new SnapSideRule {Side = SnapSide.Left, Alternative = {SnapSide.Right}, PointRules = SnapPointRuleCollection.Parse("LT-RT, LB-RB")},
        new SnapSideRule {Side = SnapSide.Right, Alternative = {SnapSide.Left}, PointRules = SnapPointRuleCollection.Parse("RT-LT, RB-LB")},
        new SnapSideRule {Side = SnapSide.Top, Alternative = {SnapSide.Bottom}, PointRules = SnapPointRuleCollection.Parse("TR-BR, TL-BL")},
        new SnapSideRule {Side = SnapSide.Bottom, Alternative = {SnapSide.Top}, PointRules = SnapPointRuleCollection.Parse("BR-TR, BL-TL")}
      }
    };

    public static SnapDefinition Center { get; } = new SnapDefinition
    {
      SideRules =
      {
        new SnapSideRule {Side = SnapSide.Left, Alternative = {SnapSide.Right}, PointRules = SnapPointRuleCollection.Parse("LC-RC")},
        new SnapSideRule {Side = SnapSide.Right, Alternative = {SnapSide.Left}, PointRules = SnapPointRuleCollection.Parse("RC-LC")},
        new SnapSideRule {Side = SnapSide.Top, Alternative = {SnapSide.Bottom}, PointRules = SnapPointRuleCollection.Parse("TC-BC")},
        new SnapSideRule {Side = SnapSide.Bottom, Alternative = {SnapSide.Top}, PointRules = SnapPointRuleCollection.Parse("BC-TC")}
      }
    };

    public static SnapDefinition TargetCenterSourceCorner { get; } = new SnapDefinition
    {
      SideRules =
      {
        new SnapSideRule {Side = SnapSide.Left, Alternative = {SnapSide.Right}, PointRules = SnapPointRuleCollection.Parse("LC-RT, LC-RB")},
        new SnapSideRule {Side = SnapSide.Right, Alternative = {SnapSide.Left}, PointRules = SnapPointRuleCollection.Parse("RC-LT, RC-LB")},
        new SnapSideRule {Side = SnapSide.Top, Alternative = {SnapSide.Bottom}, PointRules = SnapPointRuleCollection.Parse("TC-BL, TC-BR")},
        new SnapSideRule {Side = SnapSide.Bottom, Alternative = {SnapSide.Top}, PointRules = SnapPointRuleCollection.Parse("BC-TL, BC-TR")}
      }
    };

    public static SnapDefinition CornerCenterCorner { get; } = new SnapDefinition
    {
      SideRules =
      {
        new SnapSideRule {Side = SnapSide.Left, Alternative = {SnapSide.Right}, PointRules = SnapPointRuleCollection.Parse("LT-RT, LC-RC, LB-RB")},
        new SnapSideRule {Side = SnapSide.Right, Alternative = {SnapSide.Left}, PointRules = SnapPointRuleCollection.Parse("RT-LT, RC-LC, RB-LB")},
        new SnapSideRule {Side = SnapSide.Top, Alternative = {SnapSide.Bottom}, PointRules = SnapPointRuleCollection.Parse("TL-BL, TC-BC, TR-BR")},
        new SnapSideRule {Side = SnapSide.Bottom, Alternative = {SnapSide.Top}, PointRules = SnapPointRuleCollection.Parse("BL-TL, BC-TC, BR-TR")}
      }
    };

    public double SideOffset
    {
      get => _sideOffset;
      set
      {
        this.ThrowWhenSeal();
        _sideOffset = value;
      }
    }

    public SnapSideRuleCollection SideRules { get; } = new SnapSideRuleCollection();

    public double SourceCornerOffset
    {
      get => _sourceCornerOffset;
      set
      {
        this.ThrowWhenSeal();
        _sourceCornerOffset = value;
      }
    }

    public double TargetCornerOffset
    {
      get => _targetCornerOffset;
      set
      {
        this.ThrowWhenSeal();
        _targetCornerOffset = value;
      }
    }

    private bool IsSealed { get; set; }

    bool ISealable.IsSealed => IsSealed;

    #endregion

    #region  Methods

    internal IEnumerable<SnapSideRule> EnumerateActualSideRules(SnapSide side)
    {
      var snapSideRule = GetActualSnapSideRule(side);

      yield return snapSideRule;

      foreach (var alternativeSide in snapSideRule.Alternative)
        yield return GetActualSnapSideRule(alternativeSide);
    }

    private SnapSideRule GetActualSnapSideRule(SnapSide side)
    {
      return GetSnapSideRule(side) ?? Default.GetSnapSideRule(side);
    }

    private SnapSideRule GetSnapSideRule(SnapSide side)
    {
      return SideRules.FirstOrDefault(r => r.Side == side);
    }

    void ISealable.Seal()
    {
      IsSealed = true;
    }

    #endregion
  }
}