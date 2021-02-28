// <copyright file="ToolBarTrayPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Extensions;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.UI.Panels;
using Zaaml.UI.Panels.Flexible;
using Zaaml.UI.Panels.Interfaces;
using ZaamlPanel = Zaaml.UI.Panels.Core.Panel;

namespace Zaaml.UI.Controls.ToolBar
{
  public sealed class ToolBarTrayPanel : ZaamlPanel, IFlexPanel
  {
    #region Fields

    private readonly FlexElementCollection _flexElements = new FlexElementCollection();
    private List<Band> _bands;
    private byte _packedValue;

    #endregion

    #region Properties

    private Orientation ActualOrientation => ToolBarTray?.Orientation ?? default(Orientation);

    internal List<Band> Bands => _bands ?? (_bands = BuildBandsImpl());

    internal bool IsInMeasurePass
    {
      get => PackedDefinition.IsInMeasurePass.GetValue(_packedValue);
      private set => PackedDefinition.IsInMeasurePass.SetValue(ref _packedValue, value);
    }

    internal MeasurePassKind MeasurePass
    {
      get => PackedDefinition.MeasurePass.GetValue(_packedValue);
      private set => PackedDefinition.MeasurePass.SetValue(ref _packedValue, value);
    }

    internal ToolBarTray ToolBarTray { get; set; }

    #endregion

    #region  Methods

    protected override Size ArrangeOverrideCore(Size finalSize)
    {
      var orientation = ActualOrientation;
      var finalOriented = finalSize.AsOriented(orientation);
      var resultOrientedSize = new OrientedSize(orientation.Rotate());

      foreach (var band in _bands)
      {
        var bandFinalSize = PanelHelper.ArrangeStackLine(band.ToolBars, orientation, resultOrientedSize.Direct, 0, band.DesiredSize.GetIndirect(orientation), null);
        resultOrientedSize = resultOrientedSize.StackSize(bandFinalSize.Size);
      }

      return resultOrientedSize.ExpandTo(finalOriented).Size;
    }

    private void BuildBands()
    {
      if (_bands != null && ToolBarTray != null && ToolBarTray.IsDragging)
        return;

      _bands = BuildBandsImpl();
    }

    private List<Band> BuildBandsImpl()
    {
      return Children.Cast<ToolBarControl>().GroupBy(t => t.Band).OrderBy(g => g.Key).Select(g => new Band(g.OrderBy(t => t.BandIndex))).ToList();
    }

    protected override Size MeasureOverrideCore(Size availableSize)
    {
      try
      {
        IsInMeasurePass = true;
        var orientation = ActualOrientation;

        BuildBands();

        var orientedConstraint = new OrientedSize(orientation, availableSize)
        {
          Indirect = double.PositiveInfinity
        };

        var childConstraint = new OrientedSize(orientation)
        {
          Direct = double.PositiveInfinity,
          Indirect = double.PositiveInfinity
        };

        var useLayoutRounding = UseLayoutRounding;
        _flexElements.UseLayoutRounding = useLayoutRounding;

        foreach (var band in _bands)
        {
          _flexElements.EnsureCount(band.ToolBars.Count);

          var bandOrientedSize = new OrientedSize(orientation);
          var needSecondPass = false;

          MeasurePass = MeasurePassKind.First;

          for (var index = 0; index < band.ToolBars.Count; index++)
          {
            var toolBarControl = band.ToolBars[index];

            toolBarControl.ItemsPresenterInternal?.ItemsHostInternal?.SuspendOverflow();

            toolBarControl.MeasureMinMaxLength(childConstraint, out var minLength, out var maxLength);

            var flexItem = new FlexElement { StretchDirection = FlexStretchDirection.Both }.WithMinMaxLength(minLength, maxLength).WithUIElement(toolBarControl, orientation).WithRounding(useLayoutRounding);

            _flexElements[index] = flexItem;
            needSecondPass |= flexItem.ActualLength > toolBarControl.Length;

            bandOrientedSize = bandOrientedSize.StackSize(toolBarControl.DesiredSize);
          }

          MeasurePass = MeasurePassKind.Second;

          var bandDesiredSize = bandOrientedSize;

          if (needSecondPass || bandOrientedSize.Direct.IsGreaterThan(orientedConstraint.Direct))
          {
            bandDesiredSize = new OrientedSize(orientation);

            FlexDistributor.LastToFirst.Distribute(_flexElements, orientedConstraint.Direct);

            for (var index = 0; index < band.ToolBars.Count; index++)
            {
              var toolBarControl = band.ToolBars[index];
              var toolBarSizeConstraint = bandOrientedSize.Clone.ChangeDirect(_flexElements[index].ActualLength);

              toolBarControl.Measure(toolBarSizeConstraint.Size);

              if (toolBarControl.ItemsPresenterInternal?.ItemsHostInternal?.ResumeOverflow() == true)
                toolBarControl.Measure(toolBarSizeConstraint.Size);

              bandDesiredSize = bandDesiredSize.StackSize(toolBarControl.DesiredSize);
            }
          }
          else
          {
            foreach (var toolBarControl in band.ToolBars)
              toolBarControl.ItemsPresenterInternal?.ItemsHostInternal?.ResumeOverflow();
          }

					band.DesiredSize = bandDesiredSize.Size;
        }

        return _bands.Aggregate(new OrientedSize(orientation.Rotate()), (current, band) => current.StackSize(band.DesiredSize)).Size;
      }
      finally
      {
        IsInMeasurePass = false;
      }
    }

    internal void NormalizeBands()
    {
      for (var iBand = 0; iBand < Bands.Count; iBand++)
      {
        var band = Bands[iBand];

        for (var bandIndex = 0; bandIndex < band.ToolBars.Count; bandIndex++)
        {
          var toolBarControl = band.ToolBars[bandIndex];

          toolBarControl.Band = iBand;
          toolBarControl.BandIndex = bandIndex;
        }
      }
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
      return child.GetFlexElement(this).WithStretchDirection(FlexStretchDirection.Both).WithOverflowBehavior(FlexOverflowBehavior.Hide);
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

    Orientation IOrientedPanel.Orientation => ActualOrientation;

    #endregion

    #endregion

    #region  Nested Types

    private static class PackedDefinition
    {
      #region Static Fields and Constants

      public static readonly PackedBoolItemDefinition IsInMeasurePass;
      public static readonly PackedEnumItemDefinition<MeasurePassKind> MeasurePass;

      #endregion

      #region Ctors

      static PackedDefinition()
      {
        var allocator = new PackedValueAllocator();

        IsInMeasurePass = allocator.AllocateBoolItem();
        MeasurePass = allocator.AllocateEnumItem<MeasurePassKind>();
      }

      #endregion
    }

    internal enum MeasurePassKind
    {
      First,
      Second
    }

    #endregion
  }
}