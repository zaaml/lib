// <copyright file="IExpressionScope.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Expressions
{
	internal interface IExpressionScope
	{
		object ConvertValue(object value, Type targetType);

		IExpressionParameter GetParameter(string parameterName);
	}
}