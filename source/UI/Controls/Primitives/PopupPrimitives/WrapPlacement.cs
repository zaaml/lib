// <copyright file="WrapPlacement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
  public sealed class WrapPlacement : RelativePlacementBase
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty TargetSiteProperty = DPM.Register<WrapPlacementTargetSite, WrapPlacement>
      ("TargetSite", p => p.OnSiteChanged);

    #endregion

    #region Properties

    internal Rect TargetScreenBoxInt => TargetScreenBox;

    public WrapPlacementTargetSite TargetSite
    {
      get => (WrapPlacementTargetSite) GetValue(TargetSiteProperty);
      set => SetValue(TargetSiteProperty, value);
    }

    #endregion

    #region  Methods

    protected override Rect ArrangeOverride(Size desiredSize)
    {
      var offset = CalcOffset();
      return desiredSize.Rect().Offset(TargetScreenBox.GetTopLeft());
    }

    private Point CalcOffset()
    {
      var anchor = TargetSite;

      if (anchor == null)
        return new Point();

      var anchorPanel = Popup.Panel;

      if (anchor.IsVisualDescendantOf(anchorPanel) == false)
        return new Point();

      var anchorBox = anchor.TransformToVisual(anchorPanel).TransformBounds(anchor.GetClientBox());
      var inflate = anchorPanel.CalcInflate();
      var pointInflate = new Point(inflate.Left, inflate.Top);

      return anchorBox.GetTopLeft().Negate().Offset(pointInflate);
    }

    internal void OnSiteArrange()
    {
      if (IsPopupOpen)
        Popup.Panel.Offset = CalcOffset();
    }

    private void OnSiteChanged(WrapPlacementTargetSite oldTargetSite, WrapPlacementTargetSite newTargetSite)
    {
      if (oldTargetSite != null)
        oldTargetSite.Placement = null;

      if (newTargetSite != null)
        newTargetSite.Placement = this;
    }

    protected override void OnTargetScreeBoxChanged()
    {
      base.OnTargetScreeBoxChanged();

      if (IsPopupOpen)
        TargetSite?.InvalidateMeasure();
    }

    #endregion
  }

  public sealed class WrapPlacementTargetSite : Control
  {
    #region Static Fields and Constants

    private static readonly DependencyProperty ClipOwnerProperty = DPM.RegisterAttached<ClipOwner, WrapPlacementTargetSite>
      ("ClipOwner");

    #endregion

    #region Fields

    private WrapPlacement _placement;

    #endregion

    #region Ctors

    static WrapPlacementTargetSite()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<WrapPlacementTargetSite>();
    }

    public WrapPlacementTargetSite()
    {
      this.OverrideStyleKey<WrapPlacementTargetSite>();

      LayoutUpdated += OnLayoutUpdated;
    }

    #endregion

    #region Properties

    internal WrapPlacement Placement
    {
      get => _placement;
      set
      {
        if (ReferenceEquals(_placement, value))
          return;

        _placement = value;

        InvalidateMeasure();
      }
    }

    #endregion

    #region  Methods

    protected override Size ArrangeOverride(Size arrangeBounds)
    {
      var placement = Placement;

      if (placement == null)
        return base.ArrangeOverride(XamlConstants.ZeroSize);

      var arrangeOverride = base.ArrangeOverride(placement.TargetScreenBoxInt.Size());

      UpdateClip();

      return arrangeOverride;
    }

    private static ClipOwner GetClipOwner(DependencyObject element)
    {
      return (ClipOwner) element.GetValue(ClipOwnerProperty);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      var placement = Placement;

      return placement?.TargetScreenBoxInt.Size() ?? base.MeasureOverride(XamlConstants.ZeroSize);
    }

    private void OnLayoutUpdated(object sender, EventArgs eventArgs)
    {
      Placement?.OnSiteArrange();

      UpdateClip();
    }

    private static void SetClipOwner(DependencyObject element, ClipOwner value)
    {
      element.SetValue(ClipOwnerProperty, value);
    }

    private void UpdateClip()
    {
      try
      {
        var placement = Placement;

        if (placement?.Popup == null)
          return;

        var targetSize = placement.TargetScreenBoxInt.Size();

        if (targetSize.IsEmpty)
          return;

        var targetRect = targetSize.Rect();

        var panel = _placement.Popup.Panel;
        var inflate = panel.CalcInflate();

        foreach (var ancestor in this.GetVisualAncestors())
        {
          var fre = ancestor as FrameworkElement;
          if (fre == null)
            continue;

          var childRect = TransformToVisual(fre).TransformBounds(targetRect);

          var clipGeometry = fre.Clip;

          GeometryGroup combinedGeometry;
          if (clipGeometry == null)
          {
            combinedGeometry = new GeometryGroup
            {
              Children =
              {
                new RectangleGeometry(),
                new RectangleGeometry()
              },
              FillRule = FillRule.EvenOdd
            };

            SetClipOwner(combinedGeometry, new ClipOwner(this));
            fre.Clip = combinedGeometry;
          }
          else if (Equals(GetClipOwner(clipGeometry)?.TargetSite, this))
            combinedGeometry = (GeometryGroup) clipGeometry;
          else
            break;

          var panelRect = fre.RenderSize.Rect().GetInflated(inflate);

          var panelClipGeometry = (RectangleGeometry) combinedGeometry.Children[0];

          if (panelClipGeometry.Rect.IsCloseTo(panelRect) == false)
            panelClipGeometry.Rect = panelRect;

          var childClipGeometry = (RectangleGeometry) combinedGeometry.Children[1];

          if (childClipGeometry.Rect.IsCloseTo(childRect) == false)
            childClipGeometry.Rect = childRect;

          if (ReferenceEquals(panel, ancestor))
            break;
        }
      }
      catch (Exception e)
      {
        LogService.LogError(e);
      }
    }

    #endregion

    #region  Nested Types

    private class ClipOwner
    {
      #region Fields

      private readonly WeakReference _siteWeak;

      #endregion

      #region Ctors

      public ClipOwner(WrapPlacementTargetSite targetSite)
      {
        _siteWeak = new WeakReference(targetSite);
      }

      #endregion

      #region Properties

      public WrapPlacementTargetSite TargetSite => _siteWeak.GetTarget<WrapPlacementTargetSite>();

      #endregion
    }

    #endregion
  }
}