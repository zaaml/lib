// <copyright file="RangeValueDataObject.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Zaaml.Core.Disposable;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Assets
{
	public class RangeValueAsset : AssetBase, INotifyPropertyChanged
	{
		public static readonly DependencyProperty MinimumProperty = DPM.Register<double, RangeValueAsset>
			("Minimum", r => r.OnMinimumChangedInt);

		public static readonly DependencyProperty MaximumProperty = DPM.Register<double, RangeValueAsset>
			("Maximum", 1.0, r => r.OnMaximumChangedInt);

		public static readonly DependencyProperty ValueProperty = DPM.Register<double, RangeValueAsset>
			("Value", r => r.OnValueChangedInt);

		private double _relativeValue;

		public double CoercedPercentageValue => PercentageValue.Clamp(0.0, 100);

		public double CoercedRelativeValue => RelativeValue.Clamp(0.0, 1.0);

		public double Maximum
		{
			get => (double) GetValue(MaximumProperty);
			set => SetValue(MaximumProperty, value);
		}

		public double Minimum
		{
			get => (double) GetValue(MinimumProperty);
			set => SetValue(MinimumProperty, value);
		}

		public double PercentageValue => _relativeValue * 100;

		public double RelativeValue
		{
			get => _relativeValue;
			private set
			{
				if (_relativeValue.IsCloseTo(value, 4))
					return;

				_relativeValue = value;
				OnRelativeValueChanged();

				OnPropertyChanged("RelativeValue");
				OnPropertyChanged("CoercedRelativeValue");
				OnPropertyChanged("PercentageValue");
				OnPropertyChanged("CoercedPercentageValue");
			}
		}

		public double Value
		{
			get => (double) GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}

		public IDisposable Bind(DependencyObject depObj, DependencyProperty valueProperty, DependencyProperty minimumProperty, DependencyProperty maximumProperty)
		{
			return new DisposableList
			(
				this.BindPropertiesDisposable(ValueProperty, depObj, valueProperty, BindingMode.TwoWay),
				this.BindPropertiesDisposable(MinimumProperty, depObj, minimumProperty, BindingMode.TwoWay),
				this.BindPropertiesDisposable(MaximumProperty, depObj, maximumProperty, BindingMode.TwoWay)
			);
		}

		protected virtual void OnMaximumChanged()
		{
		}

		private void OnMaximumChangedInt()
		{
			UpdateRelativeValue();
			OnMaximumChanged();
		}

		protected virtual void OnMinimumChanged()
		{
		}

		private void OnMinimumChangedInt()
		{
			UpdateRelativeValue();
			OnMinimumChanged();
		}

		protected virtual void OnPropertyChanged(string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected virtual void OnRelativeValueChanged()
		{
		}

		protected virtual void OnValueChanged()
		{
		}

		private void OnValueChangedInt()
		{
			UpdateRelativeValue();
			OnValueChanged();
		}

		private void UpdateRelativeValue()
		{
			var d = (Maximum - Minimum);
			RelativeValue = d.IsCloseTo(0.0, 4) ? 0.0 : (Value - Minimum) / d;
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}