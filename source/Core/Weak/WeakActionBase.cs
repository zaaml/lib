// <copyright file="WeakActionBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Reflection;

namespace Zaaml.Core.Weak
{
	internal class WeakActionBase
	{
		#region Fields

		private readonly object _invoker;
		private readonly WeakReference _targetReference;

		#endregion

		#region Ctors

		protected WeakActionBase(object target, MethodInfo method)
		{
			if (target != null)
				_targetReference = new WeakReference(target);

#if SILVERLIGHT
			_invoker = method.IsPublic == false ? (object) ExpressionAction.CreateInvoker(method, target?.GetType()) : method;
#else
			_invoker = method;
#endif
		}

		#endregion

		#region  Methods

		protected void InvokeImpl(object[] parameters)
		{
			var target = _targetReference?.Target;

			if (_targetReference != null && target == null)
				return;

			var method = _invoker as MethodInfo;

			if (method != null)
				method.Invoke(target, parameters);
			else
			{
				var invoker = (Action<object, object[]>) _invoker;
				invoker(target, parameters);
			}
		}

		#endregion

		#region  Nested Types

#if SILVERLIGHT
		#region  Nested Types

		private static class ExpressionAction
		{
		#region  Methods

			public static Action<object, object[]> CreateInvoker(MethodInfo methodInfo, Type targetType)
			{
				var paramsInfo = methodInfo.GetParameters();

				var parameterExpression = Expression.Parameter(typeof(object[]), "args");

				Expression[] argsExp = null;

				if (paramsInfo.Length > 0)
				{
					argsExp = new Expression[paramsInfo.Length];
					for (var i = 0; i < paramsInfo.Length; i++)
					{
						Expression paramAccessorExp = Expression.ArrayIndex(parameterExpression, Expression.Constant(i));
						Expression paramCastExp = Expression.Convert(paramAccessorExp, paramsInfo[i].ParameterType);
						argsExp[i] = paramCastExp;
					}
				}

				var targetParameter = Expression.Parameter(typeof(object));
				var callExpr = Expression.Call(methodInfo.IsStatic == false ? Expression.Convert(targetParameter, targetType) : null, methodInfo, argsExp);

				return Expression.Lambda<Action<object, object[]>>(callExpr, targetParameter, parameterExpression).Compile();
			}

		#endregion
		}

		#endregion

#endif

		#endregion
	}
}