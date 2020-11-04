// <copyright file="RibbonHeaderPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.UI.Panels;
using Zaaml.UI.Panels.Flexible;
using Zaaml.UI.Panels.Interfaces;
using Panel = Zaaml.UI.Panels.Core.Panel;

namespace Zaaml.UI.Controls.Ribbon
{
  public sealed class RibbonHeaderPanel : Panel
  {
    #region Fields

    private readonly CategoriesHost _categoriesHost;
    private readonly PagesHost _pagesHost;

    private RibbonHeaderPresenter _presenter;

    #endregion

    #region Ctors

    public RibbonHeaderPanel()
    {
      _categoriesHost = new CategoriesHost(this);
      _pagesHost = new PagesHost(this);

      Children.Add(_categoriesHost);
      Children.Add(_pagesHost);
    }

    #endregion

    #region Properties

    internal RibbonHeaderPresenter Presenter
    {
      get => _presenter;
      set
      {
        if (ReferenceEquals(_presenter, value))
          return;

        _presenter = value;

        _categoriesHost.Children.Clear();
        _pagesHost.Children.Clear();

        InvalidateMeasure();
      }
    }

    #endregion

    #region  Methods

    protected override Size ArrangeOverrideCore(Size finalSize)
    {
      return this.ArrangeStackLine(Orientation.Vertical, new Range<int>(0, 2), 0, 0, null, null).Size;
    }

    protected override Size MeasureOverrideCore(Size availableSize)
    {
      var size = new Size(availableSize.Width, double.PositiveInfinity);

      _pagesHost.Measure(size);

      if (Presenter.PageCategoriesPresenter != null)
        foreach (var category in Presenter.PageCategoriesPresenter.Items.ActualItemsInternal)
          category.UpdatePagesSize();

      _categoriesHost.Measure(size);

      return new OrientedSize(Orientation.Vertical).StackSize(_categoriesHost.DesiredSize).StackSize(_pagesHost.DesiredSize).Size;
    }

    #endregion

    #region  Nested Types

    private sealed class PagesHost : Panel, IFlexPanel
    {
      #region Ctors

      public PagesHost(RibbonHeaderPanel headerPanel)
      {
        HeaderPanel = headerPanel;
        Layout = new FlexPanelLayout(this);
      }

      #endregion

      #region Properties

      private RibbonHeaderPanel HeaderPanel { get; }

      private FlexPanelLayout Layout { get; }

      private FrameworkElement Menu => Presenter?.Menu;

      private RibbonPagesPresenter PagesPresenter => Presenter?.PagesPresenter;

      private RibbonHeaderPresenter Presenter => HeaderPanel.Presenter;

      #endregion

      #region  Methods

      protected override Size ArrangeOverrideCore(Size finalSize)
      {
        return Layout.Arrange(finalSize);
      }

      protected override Size MeasureOverrideCore(Size availableSize)
      {
        Children.Clear();

        if (Menu != null)
          Children.Add(Menu);

        if (PagesPresenter != null)
          Children.Add(PagesPresenter);

        return Layout.Measure(availableSize);
      }

      #endregion

      #region Interface Implementations

      #region IFlexPanel

      IFlexDistributor IFlexPanel.Distributor => FlexDistributor.LastToFirst;

      bool IFlexPanel.HasHiddenChildren { get; set; }

      double IFlexPanel.Spacing => 0.0;

      FlexStretch IFlexPanel.Stretch => FlexStretch.Fill;

      FlexElement IFlexPanel.GetFlexElement(UIElement child)
      {
        // Menu
        if (ReferenceEquals(Menu, child))
          return new FlexElement {StretchDirection = FlexStretchDirection.None};

        // PagesPresenter
        if (ReferenceEquals(PagesPresenter, child))
          return new FlexElement {StretchDirection = FlexStretchDirection.Shrink};

        return child.GetFlexElement(this);
      }

      bool IFlexPanel.GetIsHidden(UIElement child)
      {
        return FlexPanel.GetIsHidden(child);
      }

      void IFlexPanel.SetIsHidden(UIElement child, bool value)
      {
        FlexPanel.SetIsHidden(child, value);
      }

      #endregion

      #region IOrientedPanel

      Orientation IOrientedPanel.Orientation => Orientation.Horizontal;

      #endregion

      #endregion
    }

    private sealed class CategoriesHost : Panel, IFlexPanel
    {
      #region Fields

      private FlexLength _leftTitleLength;

      private FlexLength _qatLength;

      #endregion

      #region Ctors

      public CategoriesHost(RibbonHeaderPanel headerPanel)
      {
        Layout = new FlexPanelLayout(this);
        HeaderPanel = headerPanel;
      }

      #endregion

      #region Properties

      private FrameworkElement FooterElement => Presenter?.FooterElement;

      private FrameworkElement HeaderElement => Presenter?.HeaderElement;

      private RibbonHeaderPanel HeaderPanel { get; }

      private FlexPanelLayout Layout { get; }

      private ContentPresenter LeftTitlePresenter { get; } = new ContentPresenter();

      private RibbonPageCategoriesPresenter PageCategoriesPresenter => Presenter?.PageCategoriesPresenter;

      private RibbonHeaderPresenter Presenter => HeaderPanel.Presenter;

      private RibbonToolBar QuickAccessToolBar => Presenter?.QuickAccessToolBar;

      private ContentPresenter RightTitlePresenter { get; } = new ContentPresenter();

      private FrameworkElement TitleElement => Presenter?.TitleElement;

      #endregion

      #region  Methods

      protected override Size ArrangeOverrideCore(Size finalSize)
      {
        return Layout.Arrange(finalSize);
      }

      protected override Size MeasureOverrideCore(Size availableSize)
      {
        Children.Clear();

        if (HeaderElement != null)
          Children.Add(HeaderElement);

        var qatDesiredSize = 0.0;
        if (QuickAccessToolBar != null)
        {
          Children.Add(QuickAccessToolBar);
          QuickAccessToolBar.Measure(new Size(double.PositiveInfinity, availableSize.Height));
          qatDesiredSize = QuickAccessToolBar.DesiredSize.Width;
        }

        Children.Add(LeftTitlePresenter);

        if (PageCategoriesPresenter != null)
          Children.Add(PageCategoriesPresenter);

        Children.Add(RightTitlePresenter);

        if (FooterElement != null)
          Children.Add(FooterElement);

        LeftTitlePresenter.Content = null;
        RightTitlePresenter.Content = TitleElement;

        var pagesPanel = Presenter?.Ribbon?.PagesPresenter?.ItemsHostInternal;
        var categoriesPanel = PageCategoriesPresenter?.ItemsHostInternal;

        if (pagesPanel != null && categoriesPanel != null)
        {
          var pagesOffset = pagesPanel.GetScreenLocation().X;
          var selfOffset = this.GetScreenLocation().X;
          var offset = pagesOffset - selfOffset + categoriesPanel.Offset;

          _qatLength = new FlexLength(Math.Min(qatDesiredSize, offset), FlexLengthUnitType.Pixel);
          _leftTitleLength = new FlexLength(Math.Max(0, offset - _qatLength.Value), FlexLengthUnitType.Pixel);
        }
        else
        {
          _qatLength = FlexLength.Auto;
          _leftTitleLength = FlexLength.Auto;
        }

        var measure = Layout.Measure(availableSize);

        var leftTitleElement = Layout.GetActualElement(this, LeftTitlePresenter);
        var rightTitleElement = Layout.GetActualElement(this, RightTitlePresenter);

        if (leftTitleElement.ActualLength > rightTitleElement.ActualLength == false)
          return measure;

        RightTitlePresenter.Content = null;
        LeftTitlePresenter.Content = TitleElement;
        LeftTitlePresenter.Measure(new Size(leftTitleElement.ActualLength, LeftTitlePresenter.DesiredSize.Height));

        return measure;
      }

      #endregion

      #region Interface Implementations

      #region IFlexPanel

      FlexStretch IFlexPanel.Stretch => FlexStretch.Fill;

      public IFlexDistributor Distributor => FlexDistributor.LastToFirst;

      bool IFlexPanel.HasHiddenChildren { get; set; }

      double IFlexPanel.Spacing => 0;

      FlexElement IFlexPanel.GetFlexElement(UIElement child)
      {
        // Header
        if (ReferenceEquals(HeaderElement, child))
          return new FlexElement {StretchDirection = FlexStretchDirection.None};

        // Qat
        if (ReferenceEquals(QuickAccessToolBar, child))
          return new FlexElement {Length = _qatLength, StretchDirection = FlexStretchDirection.Shrink};

        // Left title
        if (ReferenceEquals(LeftTitlePresenter, child))
          return new FlexElement {Length = _leftTitleLength, StretchDirection = FlexStretchDirection.Both};

        // Categories
        if (ReferenceEquals(PageCategoriesPresenter, child))
          return new FlexElement {StretchDirection = FlexStretchDirection.Shrink};

        // Right title
        if (ReferenceEquals(RightTitlePresenter, child))
          return new FlexElement {Length = FlexLength.Star, StretchDirection = FlexStretchDirection.Shrink};

        // Footer
        if (ReferenceEquals(FooterElement, child))
          return new FlexElement {StretchDirection = FlexStretchDirection.None};

        return child.GetFlexElement(this);
      }

      bool IFlexPanel.GetIsHidden(UIElement child)
      {
        return FlexPanel.GetIsHidden(child);
      }

      void IFlexPanel.SetIsHidden(UIElement child, bool value)
      {
        FlexPanel.SetIsHidden(child, value);
      }

      #endregion

      #region IOrientedPanel

      Orientation IOrientedPanel.Orientation => Orientation.Horizontal;

      #endregion

      #endregion
    }

    #endregion
  }
}