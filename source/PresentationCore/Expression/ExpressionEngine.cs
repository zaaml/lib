// <copyright file="ExpressionEngine.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Media;
using Zaaml.Core.ColorModel;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore
{
	public sealed class ExpressionEngine
	{
		public static readonly ExpressionEngine Instance = new ExpressionEngine();

		private readonly Expressions.ExpressionEngine _engine = new Expressions.ExpressionEngine();

		private ExpressionEngine()
		{
			_engine.RegisterMethod<Color, double, Color>("Tint", (color, amount) => ColorFunctions.Tint(color.ToRgbColor(), amount, ColorFunctionUnits.Relative).ToXamlColor());
			_engine.RegisterMethod<Color, double, Color>("Shade", (color, amount) => ColorFunctions.Shade(color.ToRgbColor(), amount, ColorFunctionUnits.Relative).ToXamlColor());
			_engine.RegisterMethod<Color, double, Color>("Lighten", (color, amount) => ColorFunctions.Lighten(color.ToRgbColor(), amount, ColorFunctionUnits.Relative).ToXamlColor());
			_engine.RegisterMethod<Color, double, Color>("Darken", (color, amount) => ColorFunctions.Darken(color.ToRgbColor(), amount, ColorFunctionUnits.Relative).ToXamlColor());
		}

		internal Func<ExpressionScope, T> CompileFunc<T>(string expressionString)
		{
			var expressionFunc = _engine.Compile<T>(expressionString);

			return scope => expressionFunc(scope);
		}
	}
}