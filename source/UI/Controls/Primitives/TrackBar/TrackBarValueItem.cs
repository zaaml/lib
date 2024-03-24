// <copyright file="TrackBarValueItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.Core;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Primitives.TrackBar
{
	public class TrackBarValueItem : TrackBarItem
	{
		public static readonly DependencyProperty ValueProperty = DPM.Register<double, TrackBarValueItem>
			("Value", t => t.OnValueChangedPrivate, t => t.CoerceValuePrivate);

		public static readonly DependencyProperty ValueCoercerProperty = DPM.Register<DoubleValueCoercer, TrackBarValueItem>
			("ValueCoercer", d => d.OnValueCoercerPropertyChangedPrivate);

		private byte _packedValue;
		private double _valueCache;

		public event EventHandler<ValueChangedEventArgs<double>> ValueChanged;

		static TrackBarValueItem()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<TrackBarValueItem>();
		}

		public TrackBarValueItem()
		{
			this.OverrideStyleKey<TrackBarValueItem>();

			Panel.SetZIndex(this, 10000);
			PreserveValue = true;
		}

		internal bool PreserveValue
		{
			get => PackedDefinition.PreserveValue.GetValue(_packedValue);
			set => PackedDefinition.PreserveValue.SetValue(ref _packedValue, value);
		}

		private bool SuspendValueHandler
		{
			get => PackedDefinition.SuspendValueHandler.GetValue(_packedValue);
			set => PackedDefinition.SuspendValueHandler.SetValue(ref _packedValue, value);
		}

		public double Value
		{
			get => (double)GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		public DoubleValueCoercer ValueCoercer
		{
			get => (DoubleValueCoercer)GetValue(ValueCoercerProperty);
			set => SetValue(ValueCoercerProperty, value);
		}

		private protected override void ClampCore()
		{
			if (TrackBarControl == null)
				return;

			try
			{
				SuspendValueHandler = true;

				this.SetCurrentValueInternal(ValueProperty, ClampValue(PreserveValue ? _valueCache : Value));
			}
			finally
			{
				SuspendValueHandler = false;
			}
		}

		private double ClampValue(double value)
		{
			if (TrackBarControl == null)
				return value;

			var trackBarMinimum = TrackBarControl.Minimum;
			var trackBarMaximum = TrackBarControl.Maximum;

			var prevValue = PrevValueItem?.Value ?? trackBarMinimum;
			var nextValue = NextValueItem?.Value ?? trackBarMaximum;

			var minimum = prevValue.Clamp(trackBarMinimum, trackBarMaximum);
			var maximum = nextValue.Clamp(minimum, trackBarMaximum);

			if (value < trackBarMinimum || value > trackBarMaximum)
				return value.Clamp(trackBarMinimum, trackBarMaximum);

			return value.Clamp(minimum, maximum);
		}

		protected virtual double CoerceValue(double value)
		{
			return value;
		}

		private double CoerceValuePrivate(double value)
		{
			VerifyValue(value);

			value = CoerceValue(ValueCoercer?.CoerceValue(value) ?? value);

			VerifyValue(value);

			return value;
		}

		protected virtual void OnValueChanged(double oldValue, double newValue)
		{
		}

		private void OnValueChangedPrivate(double oldValue, double newValue)
		{
			if (SuspendValueHandler == false)
			{
				_valueCache = newValue;

				TrackBarControl?.OnTrackBarItemValueChanged(this);
			}

			OnValueChanged(oldValue, newValue);

			ValueChanged?.Invoke(this, new ValueChangedEventArgs<double>(oldValue, newValue));
		}

		private void OnValueCoercerChanged(object sender, EventArgs e)
		{
			ReCoerceValue();
		}

		private void OnValueCoercerPropertyChangedPrivate(DoubleValueCoercer oldValue, DoubleValueCoercer newValue)
		{
			if (oldValue != null)
				oldValue.Changed -= OnValueCoercerChanged;

			if (newValue != null)
				newValue.Changed += OnValueCoercerChanged;

			ReCoerceValue();
		}

		private void ReCoerceValue()
		{
			var originalValue = Value;
			var coercedValue = CoerceValuePrivate(originalValue);

			SetCurrentValue(ValueProperty, coercedValue);
		}

		internal void SetValueInternal(double value)
		{
			value = ClampValue(value);

			_valueCache = value;

			this.SetCurrentValueInternal(ValueProperty, value);
		}

		private static void VerifyValue(double value)
		{
			if (value.IsPositiveInfinity() || value.IsNegativeInfinity() || value.IsNaN())
				throw new InvalidOperationException();
		}

		private static class PackedDefinition
		{
			public static readonly PackedBoolItemDefinition SuspendValueHandler;
			public static readonly PackedBoolItemDefinition PreserveValue;

			static PackedDefinition()
			{
				var allocator = new PackedValueAllocator();

				SuspendValueHandler = allocator.AllocateBoolItem();
				PreserveValue = allocator.AllocateBoolItem();
			}
		}
	}
}