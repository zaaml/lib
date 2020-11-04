// <copyright file="FlexDistributor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Zaaml.Core.Extensions;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Panels.Flexible
{
  public sealed class FlexDistributor : IFlexDistributor
  {
    #region Static Fields and Constants

    public static readonly IFlexDistributor Equalizer = new FlexDistributor(Mode.Equal);
    public static readonly IFlexDistributor FirstToLast = new FlexDistributor(Mode.First);
    public static readonly IFlexDistributor LastToFirst = new FlexDistributor(Mode.Last);

    #endregion

    #region Fields

    private readonly FlexElementCollection _elementsCopy = new FlexElementCollection();
    private readonly Mode _mode;
    private readonly Stack<int> _priorityStack = new Stack<int>();
    private int _actualCount;
    private int _currentCount;
    private int[] _index;
    private FlexElement[] _sortedElements;
    private int[] _sortedIndex;

    #endregion

    #region Ctors

    private FlexDistributor(Mode mode)
    {
      _mode = mode;
    }

    #endregion

    #region  Methods

    private void Distribute(FlexElementCollection elements, FlexDistributeDirection direction, double target)
    {
      ArrayUtils.EnsureArrayLength(ref _index, elements.Count, false);

      _priorityStack.Clear();
      _priorityStack.Push(0);

      foreach (var flexItem in elements)
      {
        var priority = flexItem.GetPriority(direction);
        if (priority > _priorityStack.Peek())
          _priorityStack.Push(priority);
      }

      while (_priorityStack.Count > 0)
      {
        var priority = _priorityStack.Pop();

        _elementsCopy.EnsureCount(elements.Count);
        _elementsCopy.UseLayoutRounding = elements.UseLayoutRounding;

        var currentTarget = target;

        var j = 0;
        for (var i = 0; i < elements.Count; i++)
        {
          var flexItem = elements[i];

          if (flexItem.GetPriority(direction) < priority)
          {
            currentTarget -= flexItem.ActualLength;
            continue;
          }

          _index[j] = i;
          _elementsCopy[j] = flexItem;
          j++;
        }

        _elementsCopy.EnsureCount(j);

        currentTarget = Math.Max(0, currentTarget);

        if (direction == FlexDistributeDirection.Expand)
          ExpandAllImpl(_elementsCopy, currentTarget);
        else
          ShrinkAllImpl(_elementsCopy, currentTarget);

        for (var i = 0; i < _elementsCopy.Count; i++)
        {
          var flexItem = _elementsCopy[i];
          var index = _index[i];
          elements[index] = flexItem;
        }

        if (elements.Actual.IsCloseTo(target))
          return;
      }
    }

    private void ExpandAll(FlexElementCollection elements, double target)
    {
      Distribute(elements, FlexDistributeDirection.Expand, target);
    }

    private void ExpandAllImpl(FlexElementCollection elements, double target)
    {
      var useLayoutRounding = elements.UseLayoutRounding;
      var originalTarget = target;
	    var orientation = elements.Orientation;

      if (useLayoutRounding)
        target = target.LayoutRound(orientation, RoundingMode.MidPointFromZero);

      if (elements.TryExpandToMaximum(target))
        return;

      target -= Sort(elements, FlexDistributeDirection.Expand);

      try
      {
        var deadlock = true;
        do
        {
          var tmpTarget = target;

          for (var iItem = _currentCount - 1; iItem >= 0; iItem--)
          {
            var item = _sortedElements[iItem];
            if (item.ActualLength * (iItem + 1) <= tmpTarget)
            {
              deadlock = false;

              var success = true;
              var avg = tmpTarget / (iItem + 1);
            
              if (useLayoutRounding)
                avg = avg.LayoutRound(orientation, RoundingMode.MidPointFromZero);

							for (var jItem = iItem; jItem >= 0; jItem--)
              {
                var titem = _sortedElements[jItem];

                if (avg < titem.MaxLength)
                  titem.ActualLength = avg;
                else
                {
                  titem.ActualLength = titem.MaxLength;
                  success = false;
                }

                _sortedElements[jItem] = titem;
                
                if (success)
                  continue;

                target -= titem.ActualLength;

                RemoveAt(elements, jItem);
              }

              if (success)
                return;

              break;
            }

            tmpTarget -= item.ActualLength;
          }
        } while (_currentCount > 0 && deadlock == false);
      }
      finally
      {
        RestoreOrder(elements);
        DistributeRoundingError(elements, elements.Actual - originalTarget);
      }
    }

    private static void ExpandFromEnd(FlexElementCollection elements, double target)
    {
      var useLayoutRounding = elements.UseLayoutRounding;

      if (useLayoutRounding)
        target = target.LayoutRound(elements.Orientation, RoundingMode.MidPointFromZero);

      if (elements.TryExpandToMaximum(target))
        return;

      var current = elements.Actual;

      if (current.IsGreaterThanOrClose(elements.ActualMaximum))
        return;

      for (var index = elements.Count - 1; index >= 0; index--)
      {
        var item = elements[index];
        var compensation = Math.Max(-item.ActualLength, Math.Min(target - current, item.ActualMaxLength - item.ActualLength));
        item.ActualLength += compensation;

        elements[index] = item;

        current += compensation;

        if (current.IsCloseTo(target, XamlConstants.LayoutComparisonPrecision))
          break;
      }
    }

    private static void ExpandFromStart(FlexElementCollection elements, double target)
    {
      var useLayoutRounding = elements.UseLayoutRounding;

      if (useLayoutRounding)
        target = target.LayoutRound(elements.Orientation, RoundingMode.MidPointFromZero);

      if (elements.TryExpandToMaximum(target))
        return;

      var current = elements.Actual;

      if (current.IsGreaterThanOrClose(elements.ActualMaximum))
        return;

      for (var index = 0; index < elements.Count; index++)
      {
        var item = elements[index];
        var compensation = Math.Max(-item.ActualLength, Math.Min(target - current, item.ActualMaxLength - item.ActualLength));
        item.ActualLength += compensation;

        elements[index] = item;

        current += compensation;

        if (current.IsCloseTo(target, XamlConstants.LayoutComparisonPrecision))
          break;
      }
    }

    private void RemoveAt(FlexElementCollection elements, int index)
    {
      var item = _sortedElements[index];
      var actualIndex = _sortedIndex[index];

      elements[actualIndex] = item;

      for (var iItem = index; iItem < _currentCount - 1; iItem++)
      {
        _sortedElements[iItem] = _sortedElements[iItem + 1];
        _sortedIndex[iItem] = _sortedIndex[iItem + 1];
      }

      _currentCount--;
    }

    private void RestoreOrder(FlexElementCollection elements)
    {
      for (var i = 0; i < _currentCount; i++)
      {
        var item = _sortedElements[i];
        var index = _sortedIndex[i];
        elements[index] = item;
      }
    }

    private void ShrinkAll(FlexElementCollection elements, double target)
    {
      Distribute(elements, FlexDistributeDirection.Shrink, target);
    }

    private void ShrinkAllImpl(FlexElementCollection elements, double target)
    {
      var useLayoutRounding = elements.UseLayoutRounding;
      var originalTarget = target;
	    var orientation = elements.Orientation;

      if (useLayoutRounding)
        target = target.LayoutRound(orientation, RoundingMode.MidPointFromZero);

      if (elements.TryShrinkToMinimum(target))
        return;

      target -= Sort(elements, FlexDistributeDirection.Shrink);

      try
      {
        var deadlock = true;
        do
        {
          var tmpTarget = target;
          for (var iItem = 0; iItem < _currentCount; iItem++)
          {
            var item = _sortedElements[iItem];
            if (item.ActualLength * (_currentCount - iItem) >= tmpTarget)
            {
              deadlock = false;

              var success = true;
              var avg = tmpTarget / (_currentCount - iItem);

              if (useLayoutRounding)
                avg = avg.LayoutRound(orientation, RoundingMode.MidPointFromZero);

              for (var jItem = iItem; jItem < _currentCount; jItem++)
              {
                var titem = _sortedElements[jItem];

                if (avg > titem.MinLength)
                  titem.ActualLength = avg;
                else
                {
                  titem.ActualLength = titem.MinLength;
                  success = false;
                }

                _sortedElements[jItem] = titem;

                if (success)
                  continue;

                target -= titem.ActualLength;

                RemoveAt(elements, jItem);

                jItem--;
              }

              if (success)
                return;

              break;
            }

            tmpTarget -= item.ActualLength;
          }
        } while (_currentCount > 0 && deadlock == false);
      }
      finally
      {
        RestoreOrder(elements);
        DistributeRoundingError(elements, elements.Actual - originalTarget);
      }
    }

    internal static void DistributeRoundingError(FlexElementCollection elements, double roundingError, bool starsOnly = false)
    {
      if (roundingError.IsZero(XamlConstants.LayoutComparisonPrecision))
        return;

	    var orientation = elements.Orientation;

      roundingError = roundingError > 0 ? roundingError.LayoutRound(orientation, RoundingMode.FromZero) : roundingError.LayoutRound(orientation, RoundingMode.ToZero);

      var roundingErrorSign = Math.Sign(roundingError);
      var count = elements.Count;

      for (var i = count - 1; i > 0; i--)
      {
        var flexElement = elements[i];

        if (starsOnly && flexElement.IsStar == false)
          continue;

        if (DistributeRoundingError(ref roundingError, ref flexElement))
          elements[i] = flexElement;

        if (roundingErrorSign != Math.Sign(roundingError))
          break;
      }
    }

    private static bool DistributeRoundingError(ref double roundingError, ref FlexElement flexElement)
    {
      if (roundingError > 0)
      {
        if (flexElement.ActualLength - 1 >= flexElement.ActualMinLength == false)
          return false;

        flexElement.ActualLength -= 1;
        roundingError--;

        return true;
      }

      if (roundingError < 0)
      {
        if (flexElement.ActualLength + 1 <= flexElement.ActualMaxLength == false)
          return false;

        flexElement.ActualLength += 1;
        roundingError++;

        return true;
      }

      return false;
    }

    private static void ShrinkFromEnd(FlexElementCollection elements, double target)
    {
      var useLayoutRounding = elements.UseLayoutRounding;

      if (useLayoutRounding)
        target = target.LayoutRound(elements.Orientation, RoundingMode.MidPointFromZero);

      if (elements.TryShrinkToMinimum(target))
        return;

      var current = elements.Actual;

      if (current.IsLessThanOrClose(elements.ActualMinimum))
        return;

      for (var index = elements.Count - 1; index >= 0; index--)
      {
        var item = elements[index];
        var compensation = Math.Min(current - target, item.ActualLength - item.ActualMinLength);
        item.ActualLength -= compensation;

        elements[index] = item;

        current -= compensation;

        if (current.IsCloseTo(target, XamlConstants.LayoutComparisonPrecision))
          break;
      }
    }

    private static void ShrinkFromStart(FlexElementCollection elements, double target)
    {
      var useLayoutRounding = elements.UseLayoutRounding;

      if (useLayoutRounding)
        target = target.LayoutRound(elements.Orientation, RoundingMode.MidPointFromZero);

			if (elements.TryShrinkToMinimum(target))
        return;

      var current = elements.Actual;

      if (current.IsLessThanOrClose(elements.ActualMinimum))
        return;

      for (var index = 0; index < elements.Count; index++)
      {
        var item = elements[index];
        var compensation = Math.Min(current - target, item.ActualLength - item.ActualMinLength);
        item.ActualLength -= compensation;

        elements[index] = item;

        current -= compensation;

        if (current.IsCloseTo(target, XamlConstants.LayoutComparisonPrecision))
          break;
      }
    }

    private double Sort(FlexElementCollection elements, FlexDistributeDirection direction)
    {
      ArrayUtils.EnsureArrayLength(ref _sortedElements, elements.Count, false);
      ArrayUtils.EnsureArrayLength(ref _sortedIndex, elements.Count, false);

      _actualCount = 0;

      var fixedLength = 0.0;

      for (var index = elements.Count - 1; index >= 0; index--)
      {
        var item = elements[index];
        if (item.CanDistribute(direction) == false)
        {
          fixedLength += item.ActualLength;
          continue;
        }

        _sortedElements[_actualCount] = item;
        _sortedIndex[_actualCount] = index;
        _actualCount++;
      }

      Array.Sort(_sortedElements, _sortedIndex, 0, _actualCount, FlexItemComparer.Default);

      _currentCount = _actualCount;

      return fixedLength;
    }

    #endregion

    #region Interface Implementations

    #region IFlexDistributor

    public void Distribute(FlexElementCollection elements, double target)
    {
      var elementsActual = elements.Actual;

      if (elementsActual.IsCloseTo(target, XamlConstants.LayoutComparisonPrecision))
        return;

      var distribution = elementsActual.IsGreaterThan(target) ? FlexDistributeDirection.Shrink : FlexDistributeDirection.Expand;

      if (distribution == FlexDistributeDirection.Shrink)
      {
        switch (_mode)
        {
          case Mode.Equal:
            ShrinkAll(elements, target);
            break;
          case Mode.First:
            ShrinkFromStart(elements, target);
            break;
          case Mode.Last:
            ShrinkFromEnd(elements, target);
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
      else
      {
        switch (_mode)
        {
          case Mode.Equal:
            ExpandAll(elements, target);
            break;
          case Mode.First:
            ExpandFromStart(elements, target);
            break;
          case Mode.Last:
            ExpandFromEnd(elements, target);
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
    }

    #endregion

    #endregion

    #region  Nested Types

    private class FlexItemComparer : IComparer<FlexElement>
    {
      #region Static Fields and Constants

      private static readonly Comparer<double> DoubleComparer = Comparer<double>.Default;

      public static readonly IComparer<FlexElement> Default = new FlexItemComparer();

      #endregion

      #region Ctors

      private FlexItemComparer()
      {
      }

      #endregion

      #region Interface Implementations

      #region IComparer<FlexElement>

      public int Compare(FlexElement x, FlexElement y)
      {
        return DoubleComparer.Compare(x.ActualLength, y.ActualLength);
      }

      #endregion

      #endregion
    }

    private enum Mode
    {
      Equal,
      First,
      Last
    }

    #endregion
  }
}