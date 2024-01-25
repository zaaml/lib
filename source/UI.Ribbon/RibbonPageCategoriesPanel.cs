// <copyright file="RibbonPageCategoriesPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Collections;
using Zaaml.PresentationCore.Interfaces;
using Zaaml.UI.Panels;
using Zaaml.UI.Panels.Core;
using Zaaml.UI.Panels.Interfaces;

namespace Zaaml.UI.Controls.Ribbon
{
  public class RibbonPageCategoriesPanel : ItemsPanel<RibbonPageCategory>, IStackPanel
  {
    #region Fields

    private readonly List<UIElement> _elements = new List<UIElement>();
    private readonly ReadOnlyListWrapper<UIElement> _elementsWrapper;

    #endregion

    #region Ctors

    public RibbonPageCategoriesPanel()
    {
      _elementsWrapper = new ReadOnlyListWrapper<UIElement>(_elements);
    }

    #endregion

    #region Properties

    internal double Offset { get; private set; }

    internal RibbonPageCategoriesPresenter Presenter { get; set; }

    #endregion

    #region  Methods

    protected override Size ArrangeOverrideCore(Size finalSize)
    {
      return StackPanelLayout.Arrange(this, finalSize);
    }

    protected override Size MeasureOverrideCore(Size availableSize)
    {
      _elements.Clear();

      var found = false;

      Offset = 0;

      foreach (RibbonPageCategory pageCategory in Children)
      {
        if (found == false && string.IsNullOrEmpty(pageCategory.Header))
        {
          Offset += pageCategory.PagesSize.Width;

          continue;
        }

        found = true;

        _elements.Add(pageCategory);
      }

      return StackPanelLayout.Measure(this, availableSize);
    }

    #endregion

    #region Interface Implementations

    #region IOrientedPanel

    Orientation IOrientedPanel.Orientation => Orientation.Horizontal;

    #endregion

    #region IPanel

    IReadOnlyList<UIElement> IPanel.Elements => _elementsWrapper;

    #endregion

    #endregion
  }
}