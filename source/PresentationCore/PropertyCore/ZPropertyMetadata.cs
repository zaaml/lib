// <copyright file="ZPropertyMetadata.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Zaaml.PresentationCore.PropertyCore
{
	internal sealed class ZPropertyMetadata : PropertyMetadata
	{
		public ZPropertyMetadata(object defaultValue)
			: this(defaultValue, ZFrameworkPropertyMetadataOptions.None, null, null)
		{
		}

		public ZPropertyMetadata(PropertyChangedCallback propertyChangedCallback)
			: this(ZFrameworkPropertyMetadataOptions.None, propertyChangedCallback, null)
		{
		}

		public ZPropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback)
			: this(defaultValue, ZFrameworkPropertyMetadataOptions.None, propertyChangedCallback, null)
		{
		}

		public ZPropertyMetadata(object defaultValue, ZFrameworkPropertyMetadataOptions flags)
			: this(defaultValue, flags, null, null)
		{
		}


		public ZPropertyMetadata(object defaultValue, ZFrameworkPropertyMetadataOptions flags, PropertyChangedCallback propertyChangedCallback)
			: this(defaultValue, flags, propertyChangedCallback, null)
		{
		}

		public ZPropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback)
			: this(defaultValue, ZFrameworkPropertyMetadataOptions.None, propertyChangedCallback, coerceValueCallback)
		{
		}

		public ZPropertyMetadata(ZFrameworkPropertyMetadataOptions flags, PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback)
			: base(WrapCoerceCallback(WrapFrameworkFlagsCallback(propertyChangedCallback, flags), coerceValueCallback))
		{
		}

		public ZPropertyMetadata(object defaultValue, ZFrameworkPropertyMetadataOptions flags, PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback)
			: base(defaultValue, WrapCoerceCallback(WrapFrameworkFlagsCallback(propertyChangedCallback, flags), coerceValueCallback))
		{
		}

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
						if (args.Property.ReadOnly)
						{
							var propertyKey = args.Property.GetDependencyPropertyInfo().DependencyPropertyKey;

							o.SetValue(propertyKey, args.OldValue);
							o.SetValue(propertyKey, coercedValue);
						}
						else
						{
							o.SetValue(args.Property, args.OldValue);
							o.SetValue(args.Property, coercedValue);
						}
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
					{
						if (Equals(args.OldValue, coerceState.CoercedValueEventArgs.NewValue) == false)
							propertyChangedCallback?.Invoke(o, coerceState.CoercedValueEventArgs);
					}
				}
			};
		}

		private static PropertyChangedCallback WrapFrameworkFlagsCallback(PropertyChangedCallback propertyChangedCallback, ZFrameworkPropertyMetadataOptions flags)
		{
			propertyChangedCallback ??= DPM.DefaultCallback;

			return delegate(DependencyObject o, DependencyPropertyChangedEventArgs args)
			{
				if (o is UIElement uie)
				{
					if ((flags & ZFrameworkPropertyMetadataOptions.AffectsMeasure) != 0)
						uie.InvalidateMeasure();

					if ((flags & ZFrameworkPropertyMetadataOptions.AffectsArrange) != 0)
						uie.InvalidateArrange();

					if (VisualTreeHelper.GetParent(uie) is UIElement parent)
					{
						if ((flags & ZFrameworkPropertyMetadataOptions.AffectsParentMeasure) != 0)
							parent.InvalidateMeasure();

						if ((flags & ZFrameworkPropertyMetadataOptions.AffectsParentArrange) != 0)
							parent.InvalidateArrange();
					}
				}

				propertyChangedCallback?.Invoke(o, args);
			};
		}

		private readonly struct CoercionState
		{
			public CoercionState(DependencyPropertyChangedEventArgs coercedValueEventArgs)
			{
				CoercedValueEventArgs = coercedValueEventArgs;
			}

			public DependencyPropertyChangedEventArgs CoercedValueEventArgs { get; }
		}

		private static class CoercionService
		{
			private static readonly Dictionary<(DependencyObject, DependencyProperty), CoercionState> CoercionObjects = new Dictionary<(DependencyObject, DependencyProperty), CoercionState>();

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
		}
	}
}