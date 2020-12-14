// <copyright file="ClampAsset.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Assets
{
	public abstract class ClampAsset<TValue> : AssetBase
	{
		private static readonly DependencyPropertyKey ActualValuePropertyKey = DPM.RegisterReadOnly<TValue, ClampAsset<TValue>>
			("ActualValue", default);

		public static readonly DependencyProperty ActualValueProperty = ActualValuePropertyKey.DependencyProperty;

		public static readonly DependencyProperty ValueProperty = DPM.Register<TValue, ClampAsset<TValue>>
			("Value", default, d => d.OnValuePropertyChangedPrivate);

		public static readonly DependencyProperty MinimumProperty = DPM.Register<TValue, ClampAsset<TValue>>
			("Minimum", IntervalMinMax.Get<TValue>().Minimum, d => d.OnMinimumPropertyChangedPrivate);

		public static readonly DependencyProperty MaximumProperty = DPM.Register<TValue, ClampAsset<TValue>>
			("Maximum", IntervalMinMax.Get<TValue>().Maximum, d => d.OnMaximumPropertyChangedPrivate);

		public TValue ActualValue
		{
			get => (TValue) GetValue(ActualValueProperty);
			private set => this.SetReadOnlyValue(ActualValuePropertyKey, value);
		}

		public TValue Maximum
		{
			get => (TValue) GetValue(MaximumProperty);
			set => SetValue(MaximumProperty, value);
		}

		public TValue Minimum
		{
			get => (TValue) GetValue(MinimumProperty);
			set => SetValue(MinimumProperty, value);
		}

		public TValue Value
		{
			get => (TValue) GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		protected abstract TValue Clamp();

		protected override void EndInitCore()
		{
			UpdateActualValue();
		}

		private void OnMaximumPropertyChangedPrivate(TValue oldValue, TValue newValue)
		{
			UpdateActualValue();
		}

		private void OnMinimumPropertyChangedPrivate(TValue oldValue, TValue newValue)
		{
			UpdateActualValue();
		}

		private void OnValuePropertyChangedPrivate(TValue oldValue, TValue newValue)
		{
			UpdateActualValue();
		}

		private void UpdateActualValue()
		{
			if (Initializing)
				return;

			ActualValue = Clamp();
		}
	}
}