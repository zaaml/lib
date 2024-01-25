// <copyright file="LightenColorExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;
using Zaaml.Core.ColorModel;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.MarkupExtensions
{
	public sealed class LightenColorExtension : ValueColorFunctionExtension
	{
		protected override Color Apply(Color color)
		{
			return ColorFunctions.Lighten(color.ToRgbColor(), Amount, CoreUnits).ToXamlColor();
		}
	}
}