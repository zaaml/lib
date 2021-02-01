// <copyright file="ExpressionCompiler.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Zaaml.Expressions
{
	internal abstract class ExpressionCompiler
	{
		protected static readonly MethodInfo GetParameterMethod = typeof(IExpressionScope).GetMethod("GetParameter", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic);
		protected static readonly MethodInfo ConvertValueMethod = typeof(IExpressionScope).GetMethod("ConvertValue", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic);

		public abstract void AddParameter(FactorParameterNode parameterExpression);

		public abstract Expression CompileBinaryExpression(ExpressionType operation, Expression left, Expression right);

		public abstract Expression CompileConstant(string value);

		public abstract Expression CompileInvokeMethod(string methodName, IEnumerable<Expression> args);

		public abstract Expression CompileParameter(string parameterName);
	}

	internal sealed class ExpressionCompiler<T> : ExpressionCompiler
	{
		public ExpressionCompiler(ExpressionEngine engine)
		{
			Engine = engine;
		}

		private ExpressionEngine Engine { get; }

		private Dictionary<string, Expression> ParameterDictionary { get; } = new();

		private ParameterExpression ScopeParameter { get; } = Expression.Parameter(typeof(IExpressionScope));

		public override void AddParameter(FactorParameterNode parameterExpression)
		{
			if (ParameterDictionary.ContainsKey(parameterExpression.Identifier))
				return;

			var getParameterExpression = Expression.Call(ScopeParameter, GetParameterMethod, Expression.Constant(parameterExpression.Identifier));

			ParameterDictionary.Add(parameterExpression.Identifier, getParameterExpression);
		}

		public Func<IExpressionScope, T> Compile(ExpressionNode expressionNode)
		{
			return Expression.Lambda<Func<IExpressionScope, T>>(ConvertExpression(expressionNode.Visit(new ExpressionVisitor(this)), typeof(T)), ScopeParameter).Compile();
		}

		public override Expression CompileBinaryExpression(ExpressionType operation, Expression left, Expression right)
		{
			return Expression.MakeBinary(operation, left, right);
		}

		public override Expression CompileConstant(string value)
		{
			var doubleValue = double.Parse(value, CultureInfo.InvariantCulture);

			if (doubleValue % 1 == 0)
			{
				try
				{
					return Expression.Constant((int) doubleValue);
				}
				catch
				{
					return Expression.Constant(doubleValue);
				}
			}

			return Expression.Constant(doubleValue);
		}

		public override Expression CompileInvokeMethod(string methodName, IEnumerable<Expression> args)
		{
			var method = Engine.GetMethod(methodName);

			if (method.MethodInfo == null)
				throw new InvalidOperationException($"Method {methodName} not found");

			var argsArray = args.ToList();
			var methodParameters = method.MethodInfo.GetParameters();

			if (argsArray.Count != methodParameters.Length)
				throw new InvalidOperationException($"Method {methodName} signature fail");

			if (method.Instance != null)
				return Expression.Call(Expression.Constant(method.Instance), method.MethodInfo, argsArray.Select((a, i) => ConvertExpression(a, methodParameters[i].ParameterType)));

			return Expression.Call(method.MethodInfo, argsArray.Select((a, i) => ConvertExpression(a, methodParameters[i].ParameterType)));
		}

		public override Expression CompileParameter(string parameterName)
		{
			return ParameterDictionary[parameterName];
		}

		private Expression ConvertExpression(Expression valueExpression, Type targetType)
		{
			return Expression.Convert(Expression.Call(ScopeParameter, ConvertValueMethod, Expression.Convert(valueExpression, typeof(object)), Expression.Constant(targetType)), targetType);
		}
	}
}