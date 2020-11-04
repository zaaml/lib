// <copyright file="RibbonToolBarGroup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Ribbon
{
  [ContentProperty("RibbonToolBars")]
  public class RibbonToolBarGroup : RibbonItem
  {
    #region Static Fields and Constants

    private static readonly DependencyPropertyKey RibbonToolBarsPropertyKey = DPM.RegisterReadOnly<RibbonToolBarCollection, RibbonToolBarGroup>
      ("RibbonToolBarsInt");

    public static readonly DependencyProperty RibbonToolBarsProperty = RibbonToolBarsPropertyKey.DependencyProperty;

    #endregion

    #region Properties

    public RibbonToolBarCollection RibbonToolBars => this.GetValueOrCreate(RibbonToolBarsPropertyKey, CreateRibbonToolBarCollection);

    #endregion

    #region  Methods

    private RibbonToolBarCollection CreateRibbonToolBarCollection()
    {
      return new RibbonToolBarCollection(OnRibbonItemAdded, OnRibbonItemRemoved);
    }

    private void OnRibbonItemAdded(RibbonToolBar toolBar)
    {
    }

    private void OnRibbonItemRemoved(RibbonToolBar toolBar)
    {
    }

    #endregion
  }
}