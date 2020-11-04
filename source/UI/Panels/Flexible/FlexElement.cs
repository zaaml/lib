// <copyright file="FlexElement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Extensions;
using Zaaml.Core.Packed;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore.Extensions;

namespace Zaaml.UI.Panels.Flexible
{
  public struct FlexElement
  {
    #region Static Fields and Constants

    private static readonly FlexLength DefaultLength = FlexLength.Auto;
    private const double DefaultMaxLength = double.MaxValue;
    private const double DefaultMinLength = 0.0;
    private const FlexStretchDirection DefaultStretchDirection = FlexStretchDirection.Both;
    private const FlexOverflowBehavior DefaultOverflowBehavior = FlexOverflowBehavior.None;

    public static readonly FlexElement Default = new FlexElement(DefaultMinLength, DefaultMaxLength)
    {
      Length = DefaultLength,
      StretchDirection = DefaultStretchDirection,
      OverflowBehavior = DefaultOverflowBehavior
    };

    #endregion

    // ReSharper disable once UnusedParameter.Local
    internal FlexElement(double minLength, double maxLength, bool clamp) : this()
    {
      EnsureInitialized();

      _minLength = minLength;
      _maxLength = maxLength.Clamp(_minLength, double.PositiveInfinity);
    }

    public FlexElement(double minLength, double maxLength) : this()
    {
      EnsureInitialized();

      if (minLength > maxLength)
        throw new ArgumentOutOfRangeException();

      _minLength = minLength;
      _maxLength = maxLength;
    }

    #region Fields

    private double _actualLength;
    private double _desiredLength;
    private short _expandPriority;
    private double _maxLength;
    private double _minLength;
    private double _lengthValue;
    private ushort _packedValue;
    private short _shrinkPriority;

    #endregion

    #region Properties

    internal bool CanExpand
    {
      get
      {
        if (LengthUnitType == FlexLengthUnitType.Star)
          return true;

        var stretchDirection = StretchDirection;
        return stretchDirection == FlexStretchDirection.Both || stretchDirection == FlexStretchDirection.Expand;
      }
    }

    internal bool CanShrink
    {
      get
      {
        if (LengthUnitType == FlexLengthUnitType.Star)
          return true;

        var stretchDirection = StretchDirection;
        return stretchDirection == FlexStretchDirection.Both || stretchDirection == FlexStretchDirection.Shrink;
      }
    }

    public bool IsStar => LengthUnitType == FlexLengthUnitType.Star;

    public bool IsFixed => IsStar == false;

    public bool CanDistribute(FlexDistributeDirection direction)
    {
      if (StretchDirection == FlexStretchDirection.None && IsStar == false)
        return false;

      if (direction == FlexDistributeDirection.Expand)
        return ActualLength < MaxLength && (CanExpand || IsStar);

      return ActualLength > MinLength && (CanShrink || IsStar);
    }

    public void Fill(double actualLength)
    {
      EnsureInitialized();

      var length = Length;
      actualLength = actualLength.Clamp(_minLength, _maxLength);

      if (length.IsStar)
        _actualLength = actualLength;
      else
      {
        var desiredLength = DesiredLength;

        if (actualLength.IsCloseTo(desiredLength))
          _actualLength = desiredLength;
        else if (actualLength.IsGreaterThan(desiredLength))
          _actualLength = CanExpand ? actualLength : desiredLength;
        else
          _actualLength = CanShrink ? actualLength : desiredLength;
      }
    }

    public double ActualLength
    {
      get => _actualLength;
      set => _actualLength = value.Clamp(MinLength, MaxLength);
    }

    public double DesiredLength
    {
      get
      {
        var length = Length;
        if (length.IsStar)
          return ActualLength;

        return length.IsAuto ? _desiredLength : length.Value;
      }

      private set => _desiredLength = value.Clamp(MinLength, MaxLength);
    }

    // ReSharper disable once ConvertToAutoProperty
    public short ExpandPriority
    {
      get => _expandPriority;
      set => _expandPriority = value;
    }

    public bool IsRound
    {
      get => PackedDefinition.IsRound.GetValue(_packedValue);
      private set => PackedDefinition.IsRound.SetValue(ref _packedValue, value);
    }

	  public Orientation Orientation
		{
		  get => PackedDefinition.IsHorizontal.GetValue(_packedValue) ? Orientation.Horizontal : Orientation.Vertical;
		  private set => PackedDefinition.IsHorizontal.SetValue(ref _packedValue, value == Orientation.Horizontal);
	  }

		private FlexLengthUnitType LengthUnitType
    {
      get => PackedDefinition.LengthUnitType.GetValue(_packedValue);
      set => PackedDefinition.LengthUnitType.SetValue(ref _packedValue, value);
		}

    private bool IsInitialized
    {
      get => PackedDefinition.IsInitialized.GetValue(_packedValue);
      set => PackedDefinition.IsInitialized.SetValue(ref _packedValue, value);
    }



    private void EnsureInitialized()
    {
      if (IsInitialized)
        return;

      Initilize();

      IsInitialized = true;
    }

    private void Initilize()
    {
      _maxLength = DefaultMaxLength;
      _lengthValue = DefaultLength.Value;
      LengthUnitType = DefaultLength.UnitType;
    }

    internal double ActualMaxLength => CanExpand ? MaxLength : DesiredLength;

    internal double ActualMinLength => CanShrink ? MinLength : DesiredLength;

    public double MaxLength
    {
      get
      {
        EnsureInitialized();
        return _maxLength;
      }
      set
      {
        EnsureInitialized();

        if (MinLength > value)
          throw new ArgumentOutOfRangeException();

        _maxLength = value;
      }
    }

    public double MinLength
    {
      get => _minLength;
      set
      {
        if (value > MaxLength)
          throw new ArgumentOutOfRangeException();

        _minLength = value;
      }
    }

    // ReSharper disable once ConvertToAutoProperty
    public FlexLength Length
    {
      get
      {
        EnsureInitialized();
        return new FlexLength(_lengthValue, LengthUnitType);
      }
      set
      {
        EnsureInitialized();
        _lengthValue = value.Value;
        LengthUnitType = value.UnitType;
      }
    }

    public FlexOverflowBehavior OverflowBehavior
    {
      get => PackedDefinition.OverflowBehavior.GetValue(_packedValue);
      set => PackedDefinition.OverflowBehavior.SetValue(ref _packedValue, value);
    }

    // ReSharper disable once ConvertToAutoProperty
    public short ShrinkPriority
    {
      get => _shrinkPriority;
      set => _shrinkPriority = value;
    }

    public FlexStretchDirection StretchDirection
    {
      get => PackedDefinition.StretchDirection.GetValue(_packedValue);
      set => PackedDefinition.StretchDirection.SetValue(ref _packedValue, value);
    }

    public double FixedLength => IsStar ? 0.0 : (Length.IsAuto ? DesiredLength : Length.Value);

    #endregion

    #region  Methods

    public FlexElement Clone()
    {
      return this;
    }

    public FlexElement WithRounding(bool useLayoutRaounding)
    {
      var clone = this;

      clone.IsRound = useLayoutRaounding;
      if (useLayoutRaounding)
        clone.RoundImpl();

      return clone;
    }

    public short GetPriority(FlexDistributeDirection priority)
    {
      return priority == FlexDistributeDirection.Expand ? ExpandPriority : ShrinkPriority;
    }

		private void RoundImpl()
    {
      EnsureInitialized();

      IsRound = true;

      _minLength = _minLength.LayoutRound(Orientation, RoundingMode.FromZero);
      _maxLength = _maxLength.LayoutRound(Orientation, RoundingMode.ToZero);

      if (_minLength > _maxLength)
        _maxLength = _minLength;

      _actualLength = _actualLength.Clamp(_minLength, _maxLength);
      _desiredLength = _desiredLength.Clamp(_minLength, _maxLength);
    }

    public void SetLengths(double minLength, double maxLength)
    {
      SetLengths(minLength, maxLength, DesiredLength, ActualLength);
    }

    public void SetLengths(double minLength, double maxLength, double desiredLength, double currentLength)
    {
      if (_minLength > _maxLength)
        throw new ArgumentOutOfRangeException();

      _minLength = minLength;
      _maxLength = maxLength;

      _desiredLength = desiredLength.Clamp(_minLength, _maxLength);
      _actualLength = currentLength.Clamp(_minLength, _maxLength);

      IsInitialized = true;

      if (IsRound)
        RoundImpl();
    }

    public override string ToString()
    {
      return $"Cur={ActualLength}, Min={MinLength}, Max={MaxLength}, Desired={DesiredLength}";
    }

    public FlexElement WithUIElement(UIElement element, Orientation orientation)
    {
      var clone = this;

      var minLength = MinLength;
      var maxLength = Math.Max(minLength, MaxLength);

      var freChild = element as FrameworkElement;
      if (freChild != null)
      {
        double freMinSize;
        double freMaxSize;

        if (freChild.TryGetNonDefaultValue(orientation == Orientation.Horizontal ? FrameworkElement.MinWidthProperty : FrameworkElement.MinHeightProperty, out freMinSize))
          minLength = Math.Min(freMinSize, minLength);

        if (freChild.TryGetNonDefaultValue(orientation == Orientation.Horizontal ? FrameworkElement.MaxWidthProperty : FrameworkElement.MaxHeightProperty, out freMaxSize))
          maxLength = Math.Max(minLength, Math.Max(freMaxSize, maxLength));
      }

      var desired = element.DesiredSize.AsOriented(orientation).Direct;

      clone.SetLengths(minLength, maxLength, desired, desired);

      return clone;
    }

    public FlexElement WithDesiredLength(double desiredLength)
    {
      var clone = this;
      clone.DesiredLength = desiredLength;
      return clone;
    }

    public FlexElement WithActualLength(double actualLength)
    {
      var clone = this;
      clone.ActualLength = actualLength;
      return clone;
    }

    public FlexElement WithLength(FlexLength length)
    {
      var clone = this;
      clone.Length = length;
      return clone;
    }

	  public FlexElement WithOrientation(Orientation orientation)
	  {
		  var clone = this;
		  clone.Orientation = orientation;
		  return clone;
	  }

		public FlexElement WithExpandPriority(short sxpandPriority)
    {
      var clone = this;
      clone.ExpandPriority = sxpandPriority;
      return clone;
    }

    public FlexElement WithMaxLength(double maxLength)
    {
      var clone = this;
      clone.MaxLength = maxLength;
      return clone;
    }

    public FlexElement WithMinMaxLength(double minLength, double maxLength)
    {
      var clone = this;

      clone.SetLengths(minLength, maxLength);

      return clone;
    }

    public FlexElement WithMinLength(double minLength)
    {
      var clone = this;
      clone.MinLength = minLength;
      return clone;
    }

    public FlexElement WithOverflowBehavior(FlexOverflowBehavior overflowBehavior)
    {
      var clone = this;
      clone.OverflowBehavior = overflowBehavior;
      return clone;
    }

    public FlexElement WithShrinkPriority(short shrinkPriority)
    {
      var clone = this;
      clone.ShrinkPriority = shrinkPriority;
      return clone;
    }

    public FlexElement WithStretchDirection(FlexStretchDirection stretchDirection)
    {
      var clone = this;
      clone.StretchDirection = stretchDirection;
      return clone;
    }

    #endregion

    #region  Nested Types

    private static class PackedDefinition
    {
      #region Static Fields and Constants

      public static readonly PackedBoolItemDefinition IsRound;
      public static readonly PackedBoolItemDefinition IsHorizontal;
      public static readonly PackedBoolItemDefinition IsInitialized;
      public static readonly PackedEnumItemDefinition<FlexLengthUnitType> LengthUnitType;
      public static readonly PackedEnumItemDefinition<FlexStretchDirection> StretchDirection;
      public static readonly PackedEnumItemDefinition<FlexOverflowBehavior> OverflowBehavior;

      #endregion

      #region Ctors

      static PackedDefinition()
      {
        var allocator = new PackedValueAllocator();

        IsRound = allocator.AllocateBoolItem();
        IsHorizontal= allocator.AllocateBoolItem();
        IsInitialized = allocator.AllocateBoolItem();
        LengthUnitType = allocator.AllocateEnumItem<FlexLengthUnitType>();
        StretchDirection = allocator.AllocateEnumItem<FlexStretchDirection>();
        OverflowBehavior = allocator.AllocateEnumItem<FlexOverflowBehavior>();
      }

      #endregion
    }

    #endregion
  }
}