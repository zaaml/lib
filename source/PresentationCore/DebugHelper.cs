// <copyright file="DebugHelper.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Zaaml.Core.Monads;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Interfaces;

namespace Zaaml.PresentationCore
{
	internal static class DebugHelper
	{
		#region  Methods

		public static string DumpVisualTree(object obj)
		{
			var dependencyObject = obj as DependencyObject;

			if (dependencyObject == null)
				return "";

			return string.Join(Environment.NewLine, dependencyObject.GetVisualAncestorsAndSelf().Reverse().Select(a =>
			{
				var typeName = a.GetType().Name;
				var fre = a as FrameworkElement;

				return fre?.Name != null ? $"{typeName} [{fre.Name}]" : $"{typeName}";
			}));
		}

		public static object EvaluateObjectValue(object input)
		{
			var be = input as BindingExpression;
			var binding = input as Binding ?? be.Return(b => b.ParentBinding);

			return binding != null ? BindingEvaluator.EvaluateBinding(binding) : input;
		}

		public static string GetName(this IPanel element)
		{
			var fre = element as FrameworkElement;

			return fre?.Name ?? string.Empty;
		}

		public static string GetName(this UIElement element)
		{
			var fre = element as FrameworkElement;

			return fre?.Name ?? string.Empty;
		}

		public static bool NameContains(this IPanel panel, string name)
		{
			return GetName(panel).IndexOf(name, StringComparison.OrdinalIgnoreCase) != -1;
		}

		public static bool TypeNameContains(this object obj, string str)
		{
			return obj.GetType().Name.IndexOf(str, StringComparison.OrdinalIgnoreCase) != -1;
		}

		#endregion
	}

	internal static class DebugHelperExtensions
	{
		#region  Methods

		public static string GetName(this UIElement element)
		{
			return DebugHelper.GetName(element);
		}

		#endregion
	}
}