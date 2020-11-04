// <copyright file="ExpressionBindingExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore.Converters;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Data.MarkupExtensions
{
	public sealed class ExpressionBindingExtension : BindingBaseExtension
	{
		#region Static Fields and Constants

		private static readonly PropertyPath PropertyPath = new PropertyPath(ExpressionValue.ValueProperty);
		private static readonly ExpressionScope FallbackScope = new ExpressionScope();

		#endregion

		#region Type: Fields

		private readonly ExpressionValue _expressionValue = new ExpressionValue();
		private string _expression;
		private Func<ExpressionScope, object> _expressionFunc = ExpressionEngine.FallbackExpressionFunc;
		private ExpressionScope _scope;

		#endregion

		#region Properties

		public string Expression
		{
			get => _expression;
			set
			{
				if (string.Equals(_expression, value))
					return;

				_expression = value;

				OnExpressionChanged();
			}
		}

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

		#endregion

		#region  Methods

		protected override System.Windows.Data.Binding GetBindingCore(IServiceProvider serviceProvider)
		{
			return new System.Windows.Data.Binding { Path = PropertyPath, Source = _expressionValue, Converter = XamlConverter.Instance };
		}

		private void OnExpressionChanged()
		{
			try
			{
				var expression = (Expression ?? string.Empty).Trim();

				if (expression.StartsWith("'") && expression.EndsWith("'"))
					expression = expression.Substring(1, expression.Length - 2);

				_expressionFunc = string.IsNullOrEmpty(expression) ? ExpressionEngine.FallbackExpressionFunc : ExpressionEngine.Instance.Compile(expression);
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e);

				_expressionFunc = ExpressionEngine.FallbackExpressionFunc;
			}
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
			_expressionValue.Value = _expressionFunc(Scope ?? FallbackScope);
		}

		#endregion

		#region  Nested Types

		private sealed class ExpressionValue : DependencyObject
		{
			#region Static Fields and Constants

			public static readonly DependencyProperty ValueProperty = DPM.Register<object, ExpressionValue>
				("Value");

			#endregion

			#region Properties

			public object Value
			{
				get => GetValue(ValueProperty);
				set => SetValue(ValueProperty, value);
			}

			#endregion
		}

		#endregion
	}
}