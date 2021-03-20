// <copyright file="ExpressionScope.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.Expressions
{
	internal sealed class ExpressionScope : IExpressionScope
	{
		public static readonly ExpressionScope Empty = new(true);

		public ExpressionScope()
		{
		}

		internal ExpressionScope(bool isSealed)
		{
			IsSealed = isSealed;
		}

		private bool IsSealed { get; }

		private Dictionary<string, ExpressionParameter> Parameters { get; } = new();

		public object GetParameterValue(string parameterName)
		{
			return Parameters.TryGetValue(parameterName, out var parameter) ? parameter.Value : null;
		}

		public void SetParameterValue(string parameterName, object value)
		{
			if (IsSealed)
				throw new InvalidOperationException("The scope is sealed and can not be changed.");

			if (Parameters.TryGetValue(parameterName, out var parameter) == false)
				Parameters[parameterName] = parameter = new ExpressionParameter();

			parameter.Value = value;
		}

		public IExpressionParameter GetParameter(string parameterName)
		{
			return Parameters.TryGetValue(parameterName, out var parameter) ? parameter : null;
		}

		object IExpressionScope.ConvertValue(object value, Type targetType)
		{
			return Convert.ChangeType(value is IExpressionParameter parameter ? parameter.Value : value, targetType);
		}
	}
}