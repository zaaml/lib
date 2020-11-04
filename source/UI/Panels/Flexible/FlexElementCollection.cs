// <copyright file="FlexElementCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using Zaaml.Core.Collections;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Panels.Flexible
{
  public sealed class FlexElementCollection : CollectionBase<FlexElement>
  {
    #region Static Fields and Constants

    private static readonly List<FlexElementCollection> Pool = new List<FlexElementCollection>();

    #endregion

    #region Fields

    private bool _useLayoutRounding;

    #endregion

    #region Ctors

    public FlexElementCollection()
    {
    }

    public FlexElementCollection(int capacity) : base(capacity)
    {
    }

    #endregion

    #region Properties

    internal double Actual => this.Sum(i => i.ActualLength);

    public bool CanExpand
    {
      get
      {
        var count = Count;

        for (var i = 0; i < count; i++)
        {
          var flexElement = this[i];

          if (flexElement.ActualLength < flexElement.MaxLength && flexElement.CanExpand)
            return true;
        }

        return false;
      }
    }

    public bool CanShrink
    {
      get
      {
        var count = Count;

        for (var i = 0; i < count; i++)
        {
          var flexElement = this[i];

          if (flexElement.ActualLength > flexElement.MinLength && flexElement.CanShrink)
            return true;
        }

        return false;
      }
    }

    internal double Desired => this.Sum(i => i.DesiredLength);

    internal double ActualMaximum => this.Sum(i => i.ActualMaxLength);

    internal double ActualMinimum => this.Sum(i => i.ActualMinLength);

    public bool UseLayoutRounding
    {
      get => _useLayoutRounding;
      set
      {
        if (_useLayoutRounding == value)
          return;

        _useLayoutRounding = value;

        for (var i = 0; i < Count; i++)
          this[i] = this[i].WithRounding(value);
      }
    }

	  internal Orientation Orientation => Count > 0 ? this[0].Orientation : Orientation.Horizontal;

	  #endregion

    #region  Methods

    internal void CopyTo(FlexElementCollection collection)
    {
      var count = Count;

      collection.EnsureCount(count);
      collection.UseLayoutRounding = UseLayoutRounding;

      for (var i = 0; i < count; i++)
        collection[i] = this[i];
    }

    public void EnsureCount(int count)
    {
      if (Count < count)
      {
        Capacity = count;
        while (Count < count)
          Add(new FlexElement());
      }
      else
        RemoveRange(count, Count - count);
    }

    protected override void InsertItem(int index, FlexElement item)
    {
      base.InsertItem(index, item.WithRounding(UseLayoutRounding));
    }

    internal FlexElementCollection MountCopy()
    {
      return Mount(this);
    }

    internal static FlexElementCollection Mount(FlexElementCollection copyFrom)
    {
      var collection = Mount(copyFrom.Capacity);
      copyFrom.CopyTo(collection);
      return collection;
    }

    internal static FlexElementCollection Mount(int capacity)
    {
      if (Pool.Count == 0)
        return new FlexElementCollection(capacity);

      var bestCollectionIndex = Pool.Count - 1;
      var bestCollectionCapacity = -1;

      for (var i = Pool.Count - 1; i > 0; i--)
      {
        var collection = Pool[i];

        if (collection.Capacity >= capacity)
        {
          Pool.RemoveAt(i);

          if (capacity > collection.Capacity)
            collection.Capacity = capacity;

          return collection;
        }

        if (collection.Capacity <= bestCollectionCapacity)
          continue;

        bestCollectionIndex = i;
        bestCollectionCapacity = collection.Capacity;
      }

      {
        var collection = Pool[bestCollectionIndex];

        Pool.RemoveAt(bestCollectionIndex);

        if (capacity > collection.Capacity)
          collection.Capacity = capacity;

        return collection;
      }
    }

    internal static void Release(FlexElementCollection collection)
    {
      collection.Clear();
      Pool.Add(collection);
    }

    public double ResizeToDesired()
    {
      var result = 0.0;
      for (var index = 0; index < Count; index++)
      {
        var item = this[index];
        result += item.ActualLength = item.DesiredLength;
        this[index] = item;
      }

      return result;
    }

    public double ResizeToMaximum()
    {
      var result = 0.0;
      for (var index = 0; index < Count; index++)
      {
        var item = this[index];
        result += item.ActualLength = item.ActualMaxLength;
        this[index] = item;
      }

      return result;
    }

    public double ResizeToMinimum()
    {
      var result = 0.0;
      for (var index = 0; index < Count; index++)
      {
        var item = this[index];
        result += item.ActualLength = item.ActualMinLength;
        this[index] = item;
      }

      return result;
    }

    protected override void SetItem(int index, FlexElement item)
    {
      base.SetItem(index, item.WithRounding(UseLayoutRounding));
    }

    public double Stretch(FlexStretch stretch, double target, IFlexDistributor distributor)
    {
      if (target.IsInfinity())
      {
        if (stretch == FlexStretch.Uniform)
          StretchToUniform();
        else
          StretchToNone();

        return Desired;
      }

      switch (stretch)
      {
        case FlexStretch.Fill:
          StretchToFill(target);
          break;
        case FlexStretch.FillUniform:
          StretchToFillUniform(target);
          break;
        case FlexStretch.Uniform:
          StretchToUniform();
          break;
        case FlexStretch.None:
          StretchToNone();
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      if (stretch == FlexStretch.None || stretch == FlexStretch.Uniform)
        return Actual;

      var current = Actual;
      var shouldShrink = current.IsGreaterThan(target, XamlConstants.LayoutComparisonPrecision) && CanShrink;
      var shouldExpand = current.IsLessThan(target, XamlConstants.LayoutComparisonPrecision) && CanExpand;

      if (shouldShrink || shouldExpand)
        distributor.Distribute(this, target);

      return Actual;
    }

    private void StretchToFill(double target)
    {
      var starValue = 0.0;
      var fixedLength = 0.0;

      for (var i = 0; i < Count; i++)
      {
        var flexElement = this[i];

        if (flexElement.Length.IsStar)
          starValue += flexElement.Length.Value;
        else
        {
          fixedLength += flexElement.FixedLength;

          flexElement.ActualLength = flexElement.DesiredLength;

          this[i] = flexElement;
        }
      }

      if (target - fixedLength <= 0)
        return;

      var starSize = FlexUtils.CalcStarValue(target, fixedLength, starValue);

      Debug.Assert(starSize.IsInfinity() == false && starSize.IsNaN() == false);

      for (var i = 0; i < Count; i++)
      {
        var flexElement = this[i];
        var flexLength = flexElement.Length;

        if (flexLength.IsAuto)
          flexElement.ActualLength = flexElement.DesiredLength;
        if (flexLength.IsAbsolute)
          flexElement.Fill(flexLength.Value);
        else if (flexLength.IsStar)
        {
          var actualLength = flexLength.Value * starSize;

          if (UseLayoutRounding)
            actualLength = actualLength.RoundMidPointFromZero();

          flexElement.Fill(actualLength);
          this[i] = flexElement;
        }

        this[i] = flexElement;
      }

      if (UseLayoutRounding == false)
        return;

      FlexDistributor.DistributeRoundingError(this, Actual - target, true);
    }

    private void StretchToFillUniform(double target)
    {
      var fixedLength = 0.0;
      var fixedCount = 0;

      for (var i = 0; i < Count; i++)
      {
        var flexElement = this[i];

        if (flexElement.IsStar)
          continue;

        fixedCount++;
        fixedLength += flexElement.FixedLength;

        flexElement.ActualLength = flexElement.DesiredLength;

        this[i] = flexElement;
      }

      if (fixedCount == Count)
        return;

      var uniformTarget = (target - fixedLength).Clamp(0, target);
      var uniformCount = Count - fixedCount;
      var uniformLength = uniformTarget / uniformCount;
      var roundingError = 0.0;

      if (UseLayoutRounding)
      {
        uniformLength = uniformLength.Truncate();
        roundingError = uniformTarget - uniformLength * uniformCount;
      }

      for (var i = 0; i < Count; i++)
      {
        var flexElement = this[i];

        if (flexElement.IsStar == false)
          continue;

        flexElement.Fill(uniformLength);

        if (roundingError > 0 && flexElement.ActualLength + 1 < flexElement.MaxLength)
        {
          flexElement.ActualLength += 1;
          roundingError--;
        }

        this[i] = flexElement;
      }
    }

    private void StretchToNone()
    {
      for (var index = 0; index < Count; index++)
      {
        var flexItem = this[index];

        flexItem.ActualLength = flexItem.DesiredLength;

        this[index] = flexItem;
      }
    }

    private void StretchToUniform()
    {
      var maxVal = this.Max(f => f.DesiredLength);

      for (var index = 0; index < Count; index++)
      {
        var flexItem = this[index];

        flexItem.ActualLength = maxVal;

        this[index] = flexItem;
      }
    }

    public bool TryExpandToMaximum(double target)
    {
      if (ActualMaximum.IsLessThan(target) == false) return false;

      ResizeToMaximum();

      return true;
    }

    public bool TryShrinkToMinimum(double target)
    {
      if (ActualMinimum.IsGreaterThan(target) == false) return false;

      ResizeToMinimum();

      return true;
    }

    #endregion
  }
}