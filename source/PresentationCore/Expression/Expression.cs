// <copyright file="Expression.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore
{
	public abstract class Expression
	{
		public static readonly DependencyProperty ScopeProperty = DPM.RegisterAttached<ExpressionScope>
			("Scope", typeof(Expression));

		public static Expression Empty { get; } = new Expression<object>(s => null);

		protected abstract object EvalCore(ExpressionScope expressionScope);

		internal object EvalInternal(ExpressionScope expressionScope)
		{
			return EvalCore(expressionScope);
		}

		[TypeConverter(typeof(ExpressionScopeTypeConverter))]
		public static ExpressionScope GetScope(DependencyObject element)
		{
			return (ExpressionScope) element.GetValue(ScopeProperty);
		}

		[TypeConverter(typeof(ExpressionScopeTypeConverter))]
		public static void SetScope(DependencyObject element, ExpressionScope value)
		{
			element.SetValue(ScopeProperty, value);
		}

		protected abstract Type ExpressionType { get; }

		internal Type ExpressionTypeInternal => ExpressionType;
	}

	public sealed class Expression<T> : Expression
	{
		private readonly Func<ExpressionScope, T> _expressionFunc;

		public Expression(Func<ExpressionScope, T> expressionFunc)
		{
			_expressionFunc = expressionFunc;
		}

		public T Eval(ExpressionScope expressionScope)
		{
			return _expressionFunc(expressionScope);
		}

		protected override object EvalCore(ExpressionScope expressionScope)
		{
			return Eval(expressionScope);
		}

		protected override Type ExpressionType => typeof(T);
	}

	internal sealed class DeferredExpression : Expression
	{
		private static readonly MethodInfo BuildExpressionMethodInfo = typeof(DeferredExpression).GetMethod(nameof(BuildExpression), BindingFlags.Instance | BindingFlags.NonPublic);
		private readonly string _expressionString;
		private Expression _expression;

		internal DeferredExpression(string expressionString)
		{
			_expressionString = expressionString;
		}

		private Expression<T> BuildExpression<T>()
		{
			return new(ExpressionEngine.Instance.CompileFunc<T>(_expressionString));
		}

		internal void EnsureExpression(Type expressionType)
		{
			if (ExpressionType == expressionType && _expression != null)
				return;

			_expression = (Expression)BuildExpressionMethodInfo.MakeGenericMethod(expressionType).Invoke(this, null);
		}

		protected override object EvalCore(ExpressionScope expressionScope)
		{
			_expression ??= new Expression<object>(ExpressionEngine.Instance.CompileFunc<object>(_expressionString));

			return _expression.EvalInternal(expressionScope);
		}

		protected override Type ExpressionType => _expression?.ExpressionTypeInternal ?? typeof(object);
	}

	internal class ExpressionTypeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return new DeferredExpression((string) value);
		}
	}
}