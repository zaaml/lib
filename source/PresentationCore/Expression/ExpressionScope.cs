// <copyright file="ExpressionScope.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using Zaaml.Expressions;
using Zaaml.PresentationCore.Converters;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore
{
	[TypeConverter(typeof(ExpressionScopeTypeConverter))]
	[ContentProperty(nameof(Parameters))]
	public sealed class ExpressionScope : InheritanceContextObject, IExpressionScope
	{
		#region Static Fields and Constants

		private static readonly DependencyPropertyKey ParametersPropertyKey = DPM.RegisterReadOnly<ExpressionParameterCollection, ExpressionScope>
			("ParametersInt");

		public static readonly DependencyProperty ParametersProperty = ParametersPropertyKey.DependencyProperty;
		private static readonly char[] ScopeListSeparator = new[] { ';' };

		#endregion

		#region Fields

		internal event EventHandler<EventArgs> ScopeChanged;

		#endregion

		#region Properties

		public ExpressionParameterCollection Parameters => this.GetValueOrCreate(ParametersPropertyKey, () => new ExpressionParameterCollection(this));

		internal ExpressionScope ParentScope { get; set; }

		#endregion

		#region  Methods

		internal ExpressionScope CloneInternal()
		{
			return this;
		}

		public ExpressionParameter GetParameter(string parameterName)
		{
			return Parameters.GetParameter(parameterName) ?? ParentScope?.GetParameter(parameterName);
		}

		internal void OnParameterAdded(ExpressionParameter parameter)
		{
			ScopeChanged?.Invoke(this, EventArgs.Empty);
		}

		internal void OnParameterRemoved(ExpressionParameter parameter)
		{
			ScopeChanged?.Invoke(this, EventArgs.Empty);
		}

		internal void OnParameterValueChanged(ExpressionParameter expressionParameter, object oldValue, object newValue)
		{
			ScopeChanged?.Invoke(this, EventArgs.Empty);
		}

		internal static ExpressionScope Parse(string str)
		{
			var expressionScope = new ExpressionScope();

			var split = str.Split(ScopeListSeparator, StringSplitOptions.RemoveEmptyEntries);

			foreach (var parameter in split)
			{
				var colonIndex = parameter.IndexOf(':');

				if (colonIndex == -1)
					return null;

				var parameterName = parameter.Substring(0, colonIndex).Trim();
				var parameterValue = parameter.Substring(colonIndex + 1).Trim();

				expressionScope.Parameters.Add(new ExpressionParameter(parameterName, parameterValue));
			}

			return expressionScope;
		}

		#endregion

		#region Interface Implementations

		#region IExpressionScope

		object IExpressionScope.ConvertValue(object value, Type targetType)
		{
			var parameter = value as ExpressionParameter;

			return parameter != null ? parameter.ConvertValue(targetType) : XamlConverter.Convert(value, targetType);
		}

		IExpressionParameter IExpressionScope.GetParameter(string parameterName)
		{
			return GetParameter(parameterName);
		}

		#endregion

		#endregion
	}
}