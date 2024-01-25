// <copyright file="ColorInterpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Animation
{
	public sealed class ColorInterpolator : PrimitiveInterpolator<Color>
	{
		public static readonly ColorInterpolator Instance = new();

		private ColorInterpolator()
		{
		}

		protected internal override Color EvaluateCore(Color start, Color end, double progress)
		{
			var rgbStartColor = start.ToRgbColor();
			var rgbEndColor = end.ToRgbColor();

			return (rgbStartColor + (rgbEndColor - rgbStartColor) * progress).ToXamlColor();
		}
	}
}