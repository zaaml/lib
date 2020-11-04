// <copyright file="PropertyMetadataXm.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Zaaml.PresentationCore.PropertyCore
{
  [Flags]
  public enum FrameworkPropertyMetadataOptionsXm
  {
    None = 0,
    AffectsMeasure = 1,
    AffectsArrange = 2,
    AffectsParentMeasure = 4,
    AffectsParentArrange = 8
  }

  public delegate object CoerceValueCallback(DependencyObject d, object baseValue);

  public class PropertyMetadataXm : PropertyMetadata
  {
    #region Ctors

    public PropertyMetadataXm(object defaultValue)
      : this(defaultValue, FrameworkPropertyMetadataOptionsXm.None, null, null)
    {
    }

    public PropertyMetadataXm(PropertyChangedCallback propertyChangedCallback)
      : this(FrameworkPropertyMetadataOptionsXm.None, propertyChangedCallback, null)
    {
    }

    public PropertyMetadataXm(object defaultValue, PropertyChangedCallback propertyChangedCallback)
      : this(defaultValue, FrameworkPropertyMetadataOptionsXm.None, propertyChangedCallback, null)
    {
    }

    public PropertyMetadataXm(object defaultValue, FrameworkPropertyMetadataOptionsXm flags)
      : this(defaultValue, flags, null, null)
    {
    }


    public PropertyMetadataXm(object defaultValue, FrameworkPropertyMetadataOptionsXm flags, PropertyChangedCallback propertyChangedCallback)
      : this(defaultValue, flags, propertyChangedCallback, null)
    {
    }

    public PropertyMetadataXm(object defaultValue, PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback)
      : this(defaultValue, FrameworkPropertyMetadataOptionsXm.None, propertyChangedCallback, coerceValueCallback)
    {
    }

    public PropertyMetadataXm(FrameworkPropertyMetadataOptionsXm flags, PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback)
      : base(WrapCoerceCallback(WrapFrameworkFlagsCallback(propertyChangedCallback, flags), coerceValueCallback))
    {
    }

    public PropertyMetadataXm(object defaultValue, FrameworkPropertyMetadataOptionsXm flags, PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback)
      : base(defaultValue, WrapCoerceCallback(WrapFrameworkFlagsCallback(propertyChangedCallback, flags), coerceValueCallback))
    {
    }

    #endregion

    #region  Methods

    private static PropertyChangedCallback WrapCoerceCallback(PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback)
    {
      if (propertyChangedCallback == null && coerceValueCallback == null)
        return null;

      if (coerceValueCallback == null)
        return propertyChangedCallback;

      var exception = false;

      return delegate(DependencyObject o, DependencyPropertyChangedEventArgs args)
      {
        if (CoercionService.EnterCoercion(o, args) == false)
          return;

        try
        {
          var coercedValue = args.NewValue;

          coercedValue = coerceValueCallback(o, coercedValue);

          if (Equals(args.NewValue, coercedValue) == false)
          {
            o.SetValue(args.Property, args.OldValue);
            o.SetValue(args.Property, coercedValue);
          }
        }
        catch (Exception ex)
        {
          o.SetValue(args.Property, args.OldValue);
          exception = true;

          throw new InvalidOperationException("An error occurred while handling property change. See the inner exception for details", ex);
        }
        finally
        {
          var coerceState = CoercionService.LeaveCoercion(o, args);

          if (exception == false)
            propertyChangedCallback?.Invoke(o, coerceState.CoercedValueEventArgs);
        }
      };
    }

    private static PropertyChangedCallback WrapFrameworkFlagsCallback(PropertyChangedCallback propertyChangedCallback, FrameworkPropertyMetadataOptionsXm flags)
    {
      return delegate(DependencyObject o, DependencyPropertyChangedEventArgs args)
      {
	      if (o is UIElement uie)
        {
          if ((flags & FrameworkPropertyMetadataOptionsXm.AffectsMeasure) != 0)
            uie.InvalidateMeasure();

          if ((flags & FrameworkPropertyMetadataOptionsXm.AffectsArrange) != 0)
            uie.InvalidateArrange();

          if (VisualTreeHelper.GetParent(uie) is UIElement parent)
          {
            if ((flags & FrameworkPropertyMetadataOptionsXm.AffectsParentMeasure) != 0)
              parent.InvalidateMeasure();

            if ((flags & FrameworkPropertyMetadataOptionsXm.AffectsParentArrange) != 0)
              parent.InvalidateArrange();
          }
        }

        propertyChangedCallback?.Invoke(o, args);
      };
    }

    #endregion

    #region  Nested Types

    private struct CoercionState
    {
      public CoercionState(DependencyPropertyChangedEventArgs coercedValueEventArgs)
      {
        CoercedValueEventArgs = coercedValueEventArgs;
      }

      public DependencyPropertyChangedEventArgs CoercedValueEventArgs { get; }
    }

    private static class CoercionService
    {
      #region Static Fields and Constants

      private static readonly Dictionary<(DependencyObject, DependencyProperty), CoercionState> CoercionObjects = new Dictionary<(DependencyObject, DependencyProperty), CoercionState>();

      #endregion

      #region  Methods

      public static bool EnterCoercion(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
      {
	      var key = (dependencyObject, args.Property);

				try
        {
	        return CoercionObjects.TryGetValue(key, out var coerceState) == false;
        }
        finally
        {
          CoercionObjects[key] = new CoercionState(args);
        }
      }

      public static CoercionState LeaveCoercion(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
      {
	      var key = (dependencyObject, args.Property);
				var coerceState = CoercionObjects[key];

        CoercionObjects.Remove(key);

        return coerceState;
      }

      #endregion
    }

    #endregion
  }
}