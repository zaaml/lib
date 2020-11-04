// <copyright file="ExpressionParameter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Expressions
{
	internal sealed class ExpressionParameter : IExpressionParameter
	{
		public object Value { get; set; }

		object IExpressionParameter.Value => Value;
	}
}