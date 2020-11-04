// <copyright file="DataContextMemberEvaluator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Reflection;
using System.Windows.Data;
using Zaaml.Core;

namespace Zaaml.PresentationCore.Data
{
	internal readonly struct DataContextMemberEvaluator<T>
	{
		public static readonly DataContextMemberEvaluator<T> Empty = new DataContextMemberEvaluator<T>(null, null);

		public DataContextMemberEvaluator(Binding binding, Type type)
		{
			ValueGetter = null;

			if (binding == null || type == null)
				return;

			if ((binding.Source != null || binding.ElementName != null || binding.RelativeSource == null) == false)
				return;

			var path = binding.Path?.Path;

			if (string.IsNullOrEmpty(path))
				return;

			var propertyInfo = type.GetProperty(path, BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public);
			var fieldInfo = type.GetField(path, BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Public);

			if (propertyInfo == null && fieldInfo == null)
				return;

			try
			{
				var sourceExpression = System.Linq.Expressions.Expression.Parameter(typeof(object));
				var castExpression = System.Linq.Expressions.Expression.Convert(sourceExpression, type);
				var expression = System.Linq.Expressions.Expression.PropertyOrField(castExpression, path);

				ValueGetter = System.Linq.Expressions.Expression.Lambda<Func<object, T>>(expression, sourceExpression).Compile();
			}
			catch (Exception e)
			{
				LogService.LogError(e);
			}
		}

		public bool IsEmpty => ValueGetter == null;

		private Func<object, T> ValueGetter { get; }

		public T GetValue(object treeNodeData)
		{
			if (IsEmpty)
				throw new InvalidOperationException();

			return ValueGetter(treeNodeData);
		}
	}
}