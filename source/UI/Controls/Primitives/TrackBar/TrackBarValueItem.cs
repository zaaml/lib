// <copyright file="TrackBarValueItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.Core.Extensions;
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

		private byte _packedValue;
		private double _valueCache;

		static TrackBarValueItem()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<TrackBarValueItem>();
		}

		public TrackBarValueItem()
		{
			this.OverrideStyleKey<TrackBarValueItem>();

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
			get => (double) GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		internal void Clamp()
		{
			if (TrackBar == null)
				return;

			try
			{
				SuspendValueHandler = true;

				Value = ClampValue(PreserveValue ? _valueCache : Value);
			}
			finally
			{
				SuspendValueHandler = false;
			}
		}

		internal double ClampValue(double value)
		{
			if (TrackBar == null)
				return value;

			var trackBarMinimum = TrackBar.Minimum;
			var trackBarMaximum = TrackBar.Maximum;

			var minimum = PrevValueItem?.Value ?? trackBarMinimum;
			var maximum = NextValueItem?.Value ?? trackBarMaximum;

			if (value < trackBarMinimum || value > trackBarMaximum)
				return value.Clamp(trackBarMinimum, trackBarMaximum);

			return value.Clamp(minimum, maximum);
		}

		private double CoerceValuePrivate(double value)
		{
			if (value.IsPositiveInfinity() || value.IsNegativeInfinity() || value.IsNaN())
				throw new InvalidOperationException();

			return value;
		}

		protected virtual void OnValueChanged(double oldValue, double newValue)
		{
		}

		private void OnValueChangedPrivate(double oldValue, double newValue)
		{
			if (SuspendValueHandler)
				return;

			_valueCache = newValue;

			TrackBar?.OnTrackBarItemValueChanged(this);

			OnValueChanged(oldValue, newValue);
		}

		internal void SetValueInternal(double value)
		{
			value = ClampValue(value);

			_valueCache = value;

			this.SetCurrentValueInternal(ValueProperty, value);
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