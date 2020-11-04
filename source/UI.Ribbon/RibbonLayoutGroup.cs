// <copyright file="RibbonLayoutGroup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Markup;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Ribbon
{
  [ContentProperty("Content")]
  public class RibbonLayoutGroup : RibbonItem, IRibbonCustomLayoutItem
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty ContentProperty = DPM.Register<object, RibbonLayoutGroup>
      ("Content");

    public static readonly DependencyProperty ReduceLevelsCountProperty = DPM.Register<int, RibbonLayoutGroup>
      ("ReduceLevelsCount");

    private static readonly DependencyPropertyKey ActualReduceLevelPropertyKey = DPM.RegisterReadOnly<int, RibbonLayoutGroup>
      ("ActualReduceLevelInt", -1);

    public static readonly DependencyProperty ActualReduceLevelProperty = ActualReduceLevelPropertyKey.DependencyProperty;

    #endregion

    #region Ctors

    static RibbonLayoutGroup()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<RibbonLayoutGroup>();
    }

    public RibbonLayoutGroup()
    {
      this.OverrideStyleKey<RibbonLayoutGroup>();
    }

    #endregion

    #region Properties

    public int ActualReduceLevel
    {
      get => (int) GetValue(ActualReduceLevelProperty);
      private set => this.SetReadOnlyValue(ActualReduceLevelPropertyKey, value);
    }

    public object Content
    {
      get => GetValue(ContentProperty);
      set => SetValue(ContentProperty, value);
    }

    public int ReduceLevelsCount
    {
      get => (int) GetValue(ReduceLevelsCountProperty);
      set => SetValue(ReduceLevelsCountProperty, value);
    }

    #endregion

    #region  Methods

    protected override Size MeasureOverride(Size availableSize)
    {
      var measureOverride = base.MeasureOverride(availableSize);

      if (MaxWidth.IsPositiveInfinity() == false && MaxWidth.IsGreaterThan(measureOverride.Width))
        measureOverride.Width = Math.Min(availableSize.Width, MaxWidth);

      return measureOverride;
    }

    #endregion

    #region Interface Implementations

    #region IRibbonCustomLayoutItem

    int IRibbonCustomLayoutItem.ReduceLevel
    {
      get => ActualReduceLevel;
      set => ActualReduceLevel = value;
    }

    int IRibbonCustomLayoutItem.ReduceLevelsCount => ReduceLevelsCount;

    #endregion

    #endregion
  }
}