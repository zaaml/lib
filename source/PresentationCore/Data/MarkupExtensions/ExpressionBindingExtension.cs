// <copyright file="ExpressionBindingExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using Zaaml.PresentationCore.Converters;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Data.MarkupExtensions
{
	public sealed class ExpressionBindingExtension : BindingBaseExtension
	{
		private static readonly PropertyPath PropertyPath = new PropertyPath(ExpressionValue.ValueProperty);
		private static readonly ExpressionScope FallbackScope = new ExpressionScope();
		private Expression _expression = Expression.Empty;

		private ExpressionValue _expressionValue;
		private ExpressionScope _scope;

		[TypeConverter(typeof(ExpressionTypeConverter))]
		public Expression Expression
		{
			get => _expression;
			set
			{
				if (ReferenceEquals(_expression, value))
					return;

				_expression = value;

				OnExpressionChanged();
			}
		}

		[TypeConverter(typeof(ExpressionScopeTypeConverter))]
		public ExpressionScope Scope
		{
			get => _scope;
			set
			{
				if (ReferenceEquals(_scope, value))
					return;

				if (_scope != null)
					_scope.ScopeChanged -= OnScopeChanged;

				_scope = value;

				if (_scope != null)
					_scope.ScopeChanged += OnScopeChanged;

				OnScopeChanged();
			}
		}

		protected override System.Windows.Data.Binding GetBindingCore(IServiceProvider serviceProvider)
		{
			_expressionValue = new ExpressionValue();

			if (_expression is DeferredExpression deferredExpression)
			{
				GetTarget(serviceProvider, out _, out var targetProperty, out _);

				var propertyType = GetPropertyType(targetProperty) ?? typeof(object);

				deferredExpression.EnsureExpression(propertyType);
			}

			UpdateValue();

			return new System.Windows.Data.Binding {Path = PropertyPath, Source = _expressionValue, Converter = XamlConverter.Instance};
		}

		private void OnExpressionChanged()
		{
			UpdateValue();
		}

		private void OnScopeChanged()
		{
			UpdateValue();
		}

		private void OnScopeChanged(object sender, EventArgs e)
		{
			UpdateValue();
		}

		private void UpdateValue()
		{
			if (_expressionValue == null)
				return;

			_expressionValue.Value = _expression.EvalInternal(Scope ?? FallbackScope);
		}

		private sealed class ExpressionValue : DependencyObject
		{
			public static readonly DependencyProperty ValueProperty = DPM.Register<object, ExpressionValue>
				("Value");

			public object Value
			{
				get => GetValue(ValueProperty);
				set => SetValue(ValueProperty, value);
			}
		}
	}
}