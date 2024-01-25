// <copyright file="SolidColorBrushGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Zaaml.PresentationCore.Converters;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Theming
{
	public sealed class SolidColorBrushGenerator : SkinResourceExpressionGenerator<SolidColorBrush>
	{
		public static readonly DependencyProperty ExpressionScopeProperty = DPM.RegisterAttached<ExpressionScope, SolidColorBrushGenerator>
			("ExpressionScope");

		[TypeConverter(typeof(ExpressionTypeConverter))]
		public Expression ColorExpression { get; set; }

		[TypeConverter(typeof(ExpressionScopeTypeConverter))]
		public ExpressionScope ExpressionScope
		{
			get => GetExpressionScope(this);
			set => SetExpressionScope(this, value);
		}

		protected override void CopyFrom(SkinResourceGenerator generatorSource)
		{
			base.CopyFrom(generatorSource);

			var sourceBrushGenerator = (SolidColorBrushGenerator) generatorSource;

			ColorExpression = sourceBrushGenerator.ColorExpression;
			ExpressionScope = sourceBrushGenerator.ExpressionScope;
		}

		protected override SkinResourceGenerator CreateInstance()
		{
			return new SolidColorBrushGenerator();
		}

		protected override IEnumerable<KeyValuePair<string, SolidColorBrush>> GenerateCore()
		{
			var generatorScope = ProcessScope(ExpressionScope);

			foreach (var kv in this)
			{
				var template = kv.Value;
				var valueScope = ProcessScope(GetExpressionScope(template));

				valueScope.ParentScope = generatorScope;

				var color = ColorExpression != null ? XamlConverter.Convert<Color>(ColorExpression.EvalInternal(valueScope)) : new Color();
				var solidColorBrush = new SolidColorBrush {Color = color};

				yield return new KeyValuePair<string, SolidColorBrush>(kv.Key, solidColorBrush);
			}
		}

		[TypeConverter(typeof(ExpressionScopeTypeConverter))]
		public static ExpressionScope GetExpressionScope(DependencyObject dependencyObject)
		{
			return (ExpressionScope) dependencyObject.GetValue(ExpressionScopeProperty);
		}

		[TypeConverter(typeof(ExpressionScopeTypeConverter))]
		public static void SetExpressionScope(DependencyObject dependencyObject, ExpressionScope value)
		{
			dependencyObject.SetValue(ExpressionScopeProperty, value);
		}
	}
}