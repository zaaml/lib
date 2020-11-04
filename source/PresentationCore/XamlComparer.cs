// <copyright file="XamlComparer.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core.Converters;
using Zaaml.PresentationCore.Interactivity;

namespace Zaaml.PresentationCore
{
  public enum ComparerOperator
  {
    Equal,
    NotEqual,
    LessThan,
    LessThanOrEqual,
    GreaterThan,
    GreaterThanOrEqual
  }

  internal static class XamlValueComparer
  {
    #region Static Fields and Constants

    public static readonly ITriggerValueComparer TriggerComparer = new XamlValueComparerImpl();

    #endregion

    #region  Methods

    public static bool AreEqual(object leftValue, object rightValue)
    {
	    var compare = TryCompare(ref leftValue, ref rightValue, out var baseEquals);

      if (compare.HasValue)
        return compare.Value == 0;

      return baseEquals ?? EqualsAsString(leftValue, rightValue);
    }

    public static int? Compare(object leftValue, object rightValue)
    {
	    return TryCompare(ref leftValue, ref rightValue, out _);
    }

    private static void Convert(ref object leftValue, ref object rightValue)
    {
      if (leftValue.GetType() != rightValue.GetType())
      {
        var left2RightConverter = rightValue.GetConverter(leftValue.GetType());
        var right2LeftConverter = leftValue.GetConverter(rightValue.GetType());

        if (left2RightConverter != null)
          rightValue = left2RightConverter.Convert(rightValue);
        else if (right2LeftConverter != null)
          leftValue = right2LeftConverter.Convert(leftValue);
      }
    }

    private static bool EqualsAsString(object leftValue, object rightValue)
    {
      if (leftValue.GetType() != rightValue.GetType()) return false;

      var converter = leftValue.GetConverter<string>();

      if (converter == null) return false;

      try
      {
        var leftStrValue = converter.Convert<string>(leftValue);
        var rightStrValue = converter.Convert<string>(rightValue);

        return leftStrValue.Equals(rightStrValue, StringComparison.OrdinalIgnoreCase);
      }
      catch (Exception)
      {
        return false;
      }
    }

    public static bool EvaluateCompare(object leftValue, object rightValue, ComparerOperator comparerOperator)
    {
	    var compare = TryCompare(ref leftValue, ref rightValue, out _);

      if (compare.HasValue == false)
        return comparerOperator == ComparerOperator.NotEqual;

      var iCompareResult = compare.Value;

      switch (comparerOperator)
      {
        case ComparerOperator.Equal:
          return iCompareResult == 0;
        case ComparerOperator.NotEqual:
          return iCompareResult != 0;
        case ComparerOperator.LessThan:
          return iCompareResult < 0;
        case ComparerOperator.LessThanOrEqual:
          return iCompareResult <= 0;
        case ComparerOperator.GreaterThan:
          return iCompareResult > 0;
        case ComparerOperator.GreaterThanOrEqual:
          return iCompareResult >= 0;
        default:
          return false;
      }
    }

    private static bool? IsEqualBase(object leftValue, object rightValue)
    {
      if (leftValue == null && rightValue == null)
        return true;

      if (ReferenceEquals(leftValue, rightValue))
        return true;

      if (leftValue == null ^ rightValue == null)
        return false;

      return null;
    }

    private static int? TryAsComparable(object leftValue, object rightValue)
    {
      if (leftValue.GetType() != rightValue.GetType()) return null;

      if (leftValue is IComparable leftComparable)
        return leftComparable.CompareTo(rightValue);

      if (leftValue.GetType().IsValueType && leftValue.Equals(rightValue))
        return 0;

      return null;
    }

    private static int? TryCompare(ref object leftValue, ref object rightValue, out bool? baseEquals)
    {
      baseEquals = IsEqualBase(leftValue, rightValue);

      if (baseEquals.HasValue)
        return baseEquals.Value ? (int?) 0 : null;

      try
      {
        Convert(ref leftValue, ref rightValue);
      }
      catch (Exception)
      {
        return null;
      }

      return TryAsComparable(leftValue, rightValue);
    }

    #endregion

    #region  Nested Types

    private class XamlValueComparerImpl : ITriggerValueComparer
    {
      #region Interface Implementations

      #region ITriggerValueComparer

      bool ITriggerValueComparer.Compare(object triggerSourceValue, object operand)
      {
        return AreEqual(triggerSourceValue, operand);
      }

      #endregion

      #endregion
    }

    #endregion
  }
}