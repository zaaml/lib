// <copyright file="SolidColorBrushInterpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;

namespace Zaaml.PresentationCore.Animation
{
	public sealed class SolidColorBrushInterpolator : Interpolator<SolidColorBrush>
	{
		public static readonly SolidColorBrushInterpolator Instance = new();

		private SolidColorBrushInterpolator()
		{
		}

		protected internal override void EvaluateCore(ref SolidColorBrush value, SolidColorBrush start, SolidColorBrush end, double progress)
		{
			value.Color = ColorInterpolator.Instance.Evaluate(start.Color, end.Color, progress);
		}
	}
}
