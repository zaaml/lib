// <copyright file="DiscreteDoubleValueCoercer.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.PropertyCore
{
	public sealed class DiscreteDoubleValueCoercer : DoubleValueCoercer
	{
		public static readonly DependencyProperty MultiplicityProperty = DPM.Register<double, DiscreteDoubleValueCoercer>
			("Multiplicity", d => d.OnMultiplicityPropertyChangedPrivate);

		public double Multiplicity
		{
			get => (double)GetValue(MultiplicityProperty);
			set => SetValue(MultiplicityProperty, value);
		}

		public override double CoerceValue(double value)
		{
			if (Multiplicity == 0)
				return value;

			return (value / Multiplicity).RoundMidPointToEven() * Multiplicity;
		}

		private void OnMultiplicityPropertyChangedPrivate(double oldValue, double newValue)
		{
			OnChanged();
		}
	}
}