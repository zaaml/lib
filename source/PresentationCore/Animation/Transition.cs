// <copyright file="Transition.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.MarkupExtensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Animation
{
	[TypeConverter(typeof(TransitionTypeConverter))]
	[ContentProperty(nameof(EasingFunction))]
	public class Transition : AssetBase
	{
		public static readonly DependencyProperty BeginTimeProperty = DPM.Register<TimeSpan?, Transition>
			("BeginTime", TimeSpan.Zero);

		public static readonly DependencyProperty DurationProperty = DPM.Register<Duration, Transition>
			("Duration", Duration.Automatic);

		public static readonly DependencyProperty EasingFunctionProperty = DPM.Register<IEasingFunction, Transition>
			("EasingFunction");

		public static readonly DependencyProperty AccelerationRatioProperty = DPM.Register<double, Transition>
			("AccelerationRatio", 0.0);

		public static readonly DependencyProperty DecelerationRatioProperty = DPM.Register<double, Transition>
			("DecelerationRatio", 0.0);

		public static readonly DependencyProperty SpeedRatioProperty = DPM.Register<double, Transition>
			("SpeedRatio", 1.0);

		public double AccelerationRatio
		{
			get => (double) GetValue(AccelerationRatioProperty);
			set => SetValue(AccelerationRatioProperty, value);
		}

		public TimeSpan? BeginTime
		{
			get => (TimeSpan?) GetValue(BeginTimeProperty);
			set => SetValue(BeginTimeProperty, value);
		}

		public double DecelerationRatio
		{
			get => (double) GetValue(DecelerationRatioProperty);
			set => SetValue(DecelerationRatioProperty, value);
		}

		public Duration Duration
		{
			get => (Duration) GetValue(DurationProperty);
			set => SetValue(DurationProperty, value);
		}

		public IEasingFunction EasingFunction
		{
			get => (IEasingFunction) GetValue(EasingFunctionProperty);
			set => SetValue(EasingFunctionProperty, value);
		}

		public double SpeedRatio
		{
			get => (double) GetValue(SpeedRatioProperty);
			set => SetValue(SpeedRatioProperty, value);
		}

		internal static bool TryParse(string strValue, out Transition transition)
		{
			transition = null;

			if (TimeSpan.TryParse(strValue, CultureInfo.InvariantCulture, out var timeSpan))
			{
				transition = new Transition
				{
					Duration = timeSpan
				};

				return true;
			}

			return false;
		}
	}

	public sealed class TransitionExtension : MarkupExtensionBase
	{
		private Transition _transition;

		private Transition ActualTransition => _transition ??= new Transition();

		public TimeSpan BeginTime
		{
			get => _transition?.BeginTime ?? default(TimeSpan);
			set => ActualTransition.BeginTime = value;
		}

		public Duration Duration
		{
			get => _transition?.Duration ?? default(Duration);

			set => ActualTransition.Duration = value;
		}

		public IEasingFunction EasingFunction
		{
			get => _transition?.EasingFunction;
			set => ActualTransition.EasingFunction = value;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return _transition;
		}
	}

	internal sealed class TransitionTypeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext td, Type t)
		{
			return t == typeof(string);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			var strValue = value as string;

			if (strValue == null)
				return base.ConvertFrom(context, culture, value);

			if (Transition.TryParse(strValue, out var transition))
				return transition;

			return base.ConvertFrom(context, culture, value);
		}
	}
}