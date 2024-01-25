// <copyright file="DarkenColorExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;
using Zaaml.Core.ColorModel;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.MarkupExtensions
{
	public sealed class DarkenColorExtension : ValueColorFunctionExtension
	{
		protected override Color Apply(Color color)
		{
			return ColorFunctions.Darken(color.ToRgbColor(), Amount, CoreUnits).ToXamlColor();
		}
	}
}