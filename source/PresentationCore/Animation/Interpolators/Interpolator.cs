// <copyright file="Interpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.Animation
{
	public static class Interpolator
	{
		private static readonly Dictionary<Type, IInterpolator> Interpolators = new()
		{
			{ typeof(double), DoubleInterpolator.Instance },
			{ typeof(byte), ByteInterpolator.Instance },
			{ typeof(short), ShortInterpolator.Instance },
			{ typeof(int), IntInterpolator.Instance },
			{ typeof(long), LongInterpolator.Instance },
			{ typeof(bool), BoolInterpolator.Instance },
			{ typeof(decimal), DecimalInterpolator.Instance },
			{ typeof(float), SingleInterpolator.Instance },
			{ typeof(Point), PointInterpolator.Instance },
			{ typeof(Size), SizeInterpolator.Instance },
			{ typeof(Rect), RectInterpolator.Instance },
			{ typeof(Color), ColorInterpolator.Instance },
			{ typeof(Matrix), MatrixInterpolator.Instance },
			{ typeof(TranslateTransform), TranslateTransformInterpolator.Instance },
			{ typeof(RotateTransform), RotateTransformInterpolator.Instance },
			{ typeof(ScaleTransform), ScaleTransformInterpolator.Instance },
			{ typeof(SkewTransform), SkewTransformInterpolator.Instance },
			{ typeof(MatrixTransform), MatrixTransformInterpolator.Instance },
			{ typeof(SolidColorBrush), SolidColorBrushInterpolator.Instance },
		};

		public static IInterpolator GetInterpolator(Type targetType)
		{
			return Interpolators.GetValueOrDefault(targetType);
		}

		public static Interpolator<T> GetInterpolator<T>()
		{
			return (Interpolator<T>)GetInterpolator(typeof(T));
		}
	}

	public abstract class Interpolator<T> : IInterpolator<T>
	{
		protected internal abstract void EvaluateCore(ref T value, T start, T end, double progress);

		public void Evaluate(ref T value, T start, T end, double progress)
		{
			EvaluateCore(ref value, start, end, progress.Clamp(0.0, 1.0));
		}

		IAnimationValue IInterpolator.CreateAnimationValue()
		{
			return this.CreateAnimationValue();
		}
	}
}