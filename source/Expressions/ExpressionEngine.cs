// <copyright file="ExpressionEngine.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Zaaml.Expressions
{
	internal partial class ExpressionEngine
	{
		private Dictionary<string, ExpressionMethodInfo> Methods { get; } = new Dictionary<string, ExpressionMethodInfo>();

		public Func<IExpressionScope, T> Compile<T>(string expressionString)
		{
			var expressionCompiler = new ExpressionCompiler<T>(this);

			return expressionCompiler.Compile(ExpressionGrammar.Parser.Expr(expressionString));
		}

		internal ExpressionMethodInfo GetMethod(string name)
		{
			return Methods.TryGetValue(name, out var expressionMethodInfo) ? expressionMethodInfo : ExpressionMethodInfo.Empty;
		}

		public void RegisterMethod(MethodInfo methodInfo)
		{
			Methods.Add(methodInfo.Name, new ExpressionMethodInfo(null, methodInfo));
		}

		private void RegisterMethodImpl(string name, Delegate func)
		{
			Methods.Add(name, new ExpressionMethodInfo(func.Target, func.Method));
		}
	}
}