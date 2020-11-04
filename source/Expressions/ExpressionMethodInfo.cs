// <copyright file="ExpressionMethodInfo.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Reflection;

namespace Zaaml.Expressions
{
	internal readonly struct ExpressionMethodInfo
	{
		public ExpressionMethodInfo(object instance, MethodInfo methodInfo)
		{
			Instance = instance;
			MethodInfo = methodInfo;
		}

		public static readonly ExpressionMethodInfo Empty = new ExpressionMethodInfo(null, null);

		public readonly object Instance;

		public readonly MethodInfo MethodInfo;
	}
}