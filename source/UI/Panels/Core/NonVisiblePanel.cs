// <copyright file="NonVisiblePanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Panels.Core
{
  internal sealed class NonVisiblePanel : Panel
  {
    #region Fields

    private bool _allowMeasure;

    #endregion

    #region Ctors

    public NonVisiblePanel()
    {
      Opacity = 0;
      IsHitTestVisible = false;
    }

    #endregion

    #region Properties

    public bool AllowMeasure
    {
      get => _allowMeasure;
      set
      {
        if (_allowMeasure == value)
          return;

        _allowMeasure = value;
        InvalidateMeasure();
      }
    }

    #endregion

    #region  Methods

    protected override Size ArrangeOverrideCore(Size finalSize)
    {
      return finalSize;
    }

    protected override Size MeasureOverrideCore(Size availableSize)
    {
      if (AllowMeasure == false)
        return XamlConstants.ZeroSize;

      foreach (var child in UIChildren)
        child.Measure(XamlConstants.ZeroSize);

      return XamlConstants.ZeroSize;
    }

    #endregion
  }
}