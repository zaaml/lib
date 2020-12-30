// <copyright file="TrackBarControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.UI.Controls.Primitives.TrackBar
{
  [ContentProperty(nameof(ItemCollection))]
  [TemplateContractType(typeof(TrackBarTemplateContract))]
  public partial class TrackBarControl : RangeControlBase
  {
    #region Static Fields and Constants

    private static readonly DependencyPropertyKey ItemCollectionPropertyKey = DPM.RegisterReadOnly<TrackBarItemCollection, TrackBarControl>
      ("ItemCollectionPrivate");

    public static readonly DependencyProperty ItemCollectionProperty = ItemCollectionPropertyKey.DependencyProperty;

    public static readonly DependencyProperty OrientationProperty = DPM.Register<Orientation, TrackBarControl>
      ("Orientation", Orientation.Horizontal);

    #endregion

    #region Fields

    private TrackBarItem _dragItem;
    private Range<double> _originRange;
    private byte _packedValue;
    private Point _trackBarOrigin;

    public event EventHandler<TrackBarControlDragEventArgs> DragStarted;
    public event EventHandler<TrackBarControlDragEventArgs> DragEnded;

    #endregion

    #region Ctors

    static TrackBarControl()
    {
	    ControlUtils.OverrideIsTabStop<TrackBarControl>(false);
      DefaultStyleKeyHelper.OverrideStyleKey<TrackBarControl>();
    }

    public TrackBarControl()
    {
      this.OverrideStyleKey<TrackBarControl>();
    }

    #endregion

    #region Properties

    private bool Initializing
    {
      get => PackedDefinition.Initializing.GetValue(_packedValue);
      set => PackedDefinition.Initializing.SetValue(ref _packedValue, value);
    }

    public TrackBarItemCollection ItemCollection
    {
      get { return this.GetValueOrCreate(ItemCollectionPropertyKey, () => new TrackBarItemCollection(this)); }
    }

    public Orientation Orientation
    {
      get => (Orientation) GetValue(OrientationProperty);
      set => SetValue(OrientationProperty, value);
    }

    private double PixelRatio => TrackBarPanel?.PixelRatio ?? 0.0;

    private TrackBarTemplateContract TemplateContract => (TrackBarTemplateContract) TemplateContractInternal;

    private TrackBarPanel TrackBarPanel => TemplateContract.TrackBarPanel;

    #endregion

    #region  Methods

    private void AssignIndices()
    {
      for (var index = 0; index < ItemCollection.Count; index++)
        ItemCollection[index].Index = index;
    }

    private void BeginInitImpl()
    {
      Initializing = true;
    }

    private void Clamp()
    {
      foreach (var item in ItemCollection.OfType<TrackBarValueItem>())
        item.Clamp();

      TrackBarValueItem prev = null;
      foreach (var item in ItemCollection.OfType<TrackBarValueItem>())
      {
        ClampRange(prev, item);
        prev = item;
      }

      ClampRange(prev, null);
    }

    private void ClampRange(TrackBarValueItem first, TrackBarValueItem second)
    {
      var minimum = first?.Value ?? Minimum;
      var maximum = second?.Value ?? Maximum;

      var startIndex = first?.Index ?? 0;
      var endIndex = second?.Index ?? ItemCollection.Count - 1;

      var count = endIndex - startIndex - 2;

      if (count <= 0)
        return;

      var range = maximum - minimum;

      for (var i = startIndex; i < endIndex; i++)
      {
	      if (ItemCollection[i] is TrackBarRangeItem contentItem)
          contentItem.Range = range;
      }
    }

    private void EndInitImpl()
    {
      Initializing = false;

      AssignIndices();
      Clamp();
    }

    private void FinishDrag()
    {
      if (_dragItem == null)
        return;

      var dragItem = _dragItem;

      _dragItem = null;

      ReleaseMouseCapture();

      OnDragEndedPrivate(dragItem);
    }

    private void InvalidatePanel()
    {
      TrackBarPanel?.InvalidateMeasure();
    }

    protected virtual void OnDragEnded(TrackBarItem item)
    {
    }

    private void OnDragEndedPrivate(TrackBarItem item)
    {
      OnDragEnded(item);
      DragEnded?.Invoke(this, new TrackBarControlDragEventArgs(item));
      item.OnDragEndedInternal();
    }

    protected virtual void OnDragStarted(TrackBarItem item)
    {
    }

    private void OnDragStartedPrivate(TrackBarItem item)
    {
      OnDragStarted(item);
      DragStarted?.Invoke(this, new TrackBarControlDragEventArgs(item));
      item.OnDragStartedInternal();
    }

    internal void OnItemAddedInternal(TrackBarItem item)
    {
      FinishDrag();

      if (Initializing == false)
      {
        AssignIndices();
        Clamp();
      }

      TrackBarPanel?.Children.Add(item);

      item.TrackBar = this;
    }

    internal void OnItemMouseLeftButtonDown(TrackBarItem item, MouseButtonEventArgs e)
    {
      if (e.Handled || item.CanDrag == false)
        return;

      _dragItem = CaptureMouse() ? item : null;

      if (_dragItem == null)
        return;

      _trackBarOrigin = e.GetPosition(this);

      if (item is TrackBarValueItem valueItem)
      {
        _originRange = new Range<double>(valueItem.Value, valueItem.Value);
        OnDragStartedPrivate(_dragItem);

        return;
      }

      var rangeItem = (TrackBarRangeItem) item;
      
      _originRange = new Range<double>(rangeItem.PrevValueItem?.Value ?? Minimum, rangeItem.NextValueItem?.Value ?? Maximum);

			OnDragStartedPrivate(_dragItem);
    }

    internal void OnItemRemovedInternal(TrackBarItem item)
    {
      FinishDrag();

      if (Initializing == false)
      {
        AssignIndices();
        Clamp();
      }

      item.TrackBar = null;

      TrackBarPanel?.Children.Remove(item);
    }

    protected override void OnLostMouseCapture(MouseEventArgs e)
    {
      base.OnLostMouseCapture(e);

      FinishDrag();
    }

    protected override void OnMaximumChanged(double oldValue, double newValue)
    {
      base.OnMaximumChanged(oldValue, newValue);

      if (Initializing == false)
        Clamp();

      InvalidatePanel();
    }

    protected override void OnMinimumChanged(double oldValue, double newValue)
    {
      base.OnMinimumChanged(oldValue, newValue);

      if (Initializing == false)
        Clamp();

      InvalidatePanel();
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonUp(e);

      if (_dragItem == null)
				return;

	    UpdateValueOnMouseEvent(e);

      FinishDrag();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);

			if (_dragItem == null) 
	      return;

      UpdateValueOnMouseEvent(e);
    }

    private void UpdateValueOnMouseEvent(MouseEventArgs e)
    {
	    if (_dragItem == null)
		    return;

	    var pixelRatio = PixelRatio;

			if (pixelRatio.IsZero())
				return;

	    var orientation = Orientation;
	    var trackBarPosition = e.GetPosition(this);
	    var pixelDelta = trackBarPosition.AsOriented(orientation).Direct - _trackBarOrigin.AsOriented(orientation).Direct;
	    var valueDelta = pixelDelta / pixelRatio;

	    if (_dragItem is TrackBarValueItem valueItem)
	    {
		    valueItem.SetValueInternal(_originRange.Minimum + valueDelta);

		    return;
	    }

	    var rangeItem = (TrackBarRangeItem) _dragItem;
	    var prevValueItem = rangeItem.PrevValueItem;
	    var nextValueItem = rangeItem.NextValueItem;

	    if (prevValueItem == null || nextValueItem == null)
		    return;

	    if (valueDelta < 0)
	    {
		    var minimumValue = prevValueItem.PrevValueItem?.Value ?? Minimum;
		    var newValue = _originRange.Minimum + valueDelta;

		    if (newValue < minimumValue)
			    valueDelta = minimumValue - _originRange.Minimum;

		    prevValueItem.Value = _originRange.Minimum + valueDelta;
		    nextValueItem.Value = _originRange.Maximum + valueDelta;
	    }
	    else if (valueDelta > 0)
	    {
		    var maximumValue = nextValueItem.NextValueItem?.Value ?? Maximum;
		    var newValue = _originRange.Maximum + valueDelta;

		    if (newValue > maximumValue)
			    valueDelta = maximumValue - _originRange.Maximum;

		    prevValueItem.Value = _originRange.Minimum + valueDelta;
		    nextValueItem.Value = _originRange.Maximum + valueDelta;
	    }
    }

    protected override void OnTemplateContractAttached()
    {
      base.OnTemplateContractAttached();

      TrackBarPanel.TrackBar = this;
      TrackBarPanel.Children.AddRange(ItemCollection);
    }

    protected override void OnTemplateContractDetaching()
    {
      TrackBarPanel.TrackBar = null;
      TrackBarPanel.Children.Clear();

      base.OnTemplateContractDetaching();
    }

    internal void OnTrackBarItemValueChanged(TrackBarValueItem valueItem)
    {
      if (Initializing)
        return;

      valueItem.Clamp();

      ClampRange(valueItem.PrevValueItem, valueItem);
      ClampRange(valueItem, valueItem.NextValueItem);

      TrackBarPanel?.InvalidateMeasure();
    }

    #endregion

    #region  Nested Types

    private static class PackedDefinition
    {
      #region Static Fields and Constants

      public static readonly PackedBoolItemDefinition Initializing;

      #endregion

      #region Ctors

      static PackedDefinition()
      {
        var allocator = new PackedValueAllocator();

        Initializing = allocator.AllocateBoolItem();
      }

      #endregion
    }

    #endregion
  }

  public sealed class TrackBarTemplateContract : TemplateContract
  {
    #region Properties

    [TemplateContractPart(Required = true)]
    public TrackBarPanel TrackBarPanel { get; [UsedImplicitly] private set; }

    #endregion
  }
}