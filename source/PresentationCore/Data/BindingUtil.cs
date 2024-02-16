// <copyright file="BindingUtil.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Data
{
	internal static class BindingUtil
	{
		private static readonly DependencyProperty TargetPropertyProperty = DPM.RegisterAttached<object>
			("TargetProperty", typeof(BindingUtil));

		private static DependencyObject TargetObject { get; } = new DepObj();

		internal static BindingExpression BuildExpression(this Binding binding)
		{
			return BuildExpression(binding, TargetObject, TargetPropertyProperty);
		}

		internal static BindingExpression BuildExpression(this Binding binding, DependencyObject target, DependencyProperty targetProperty)
		{
			target.SetBinding(targetProperty, binding);

			var be = (BindingExpression)target.ReadLocalValue(targetProperty);

			target.ClearValue(targetProperty);

			return be;
		}

		public static Binding CloneBinding(this Binding binding)
		{
			return CloneBindingImpl(binding);
		}

		public static Binding CloneBindingImpl(Binding bindingBase)
		{
			var binding = bindingBase;

			if (binding == null)
				throw new NotSupportedException("Failed to clone binding");

			var result = new Binding
			{
				AsyncState = binding.AsyncState,
				BindingGroupName = binding.BindingGroupName,
				BindsDirectlyToSource = binding.BindsDirectlyToSource,
				Converter = binding.Converter,
				ConverterCulture = binding.ConverterCulture,
				ConverterParameter = binding.ConverterParameter,
				FallbackValue = binding.FallbackValue,
				IsAsync = binding.IsAsync,
				Mode = binding.Mode,
				NotifyOnSourceUpdated = binding.NotifyOnSourceUpdated,
				NotifyOnTargetUpdated = binding.NotifyOnTargetUpdated,
				NotifyOnValidationError = binding.NotifyOnValidationError,
				Path = binding.Path,
				StringFormat = binding.StringFormat,
				TargetNullValue = binding.TargetNullValue,
				UpdateSourceExceptionFilter = binding.UpdateSourceExceptionFilter,
				UpdateSourceTrigger = binding.UpdateSourceTrigger,
				ValidatesOnDataErrors = binding.ValidatesOnDataErrors,
				ValidatesOnExceptions = binding.ValidatesOnExceptions,
				XPath = binding.XPath,
			};

			if (bindingBase.RelativeSource != null)
				result.RelativeSource = bindingBase.RelativeSource;
			else if (bindingBase.ElementName != null)
				result.ElementName = bindingBase.ElementName;
			else if (bindingBase.Source != null)
				result.Source = bindingBase.Source;

			foreach (var validationRule in binding.ValidationRules)
				result.ValidationRules.Add(validationRule);

			return result;
		}

		private static object ConvertTemplateBindingExpression(TemplateBindingExpression templateBindingExpression)
		{
			return templateBindingExpression;
		}

		public static void EnsureBindingAttached(DependencyObject dependencyObject, DependencyProperty dependencyProperty)
		{
			if (!(dependencyObject.ReadLocalValue(dependencyProperty) is BindingExpression bindingExpression))
				return;

			// Rebind to actual DataContext
			if (bindingExpression.Status == BindingStatus.Unattached)
			{
				dependencyObject.ClearValue(dependencyProperty);
				dependencyObject.SetBinding(dependencyProperty, bindingExpression.ParentBinding);
			}
		}

		public static void EnsureBindingSet(DependencyObject dependencyObject, DependencyProperty dependencyProperty, Binding binding)
		{
			if (ReferenceEquals(dependencyObject.ReadLocalBinding(dependencyProperty), binding) == false)
			{
				dependencyObject.SetBinding(dependencyProperty, binding);
			}
		}

		internal static IEnumerable<BindingExpression> EnumerateInstalledBindings(DependencyObject dependencyObject)
		{
			var lve = dependencyObject.GetLocalValueEnumerator();

			while (lve.MoveNext())
			{
				var entry = lve.Current;

				if (!BindingOperations.IsDataBound(dependencyObject, entry.Property))
					continue;

				if (entry.Value is BindingExpression bindingExpression)
					yield return bindingExpression;
			}
		}

		internal static void RestoreBindingExpressionValue(DependencyObject target, DependencyProperty dependencyProperty, object bindingExpressionValue)
		{
			if (ReferenceEquals(target.ReadLocalValue(dependencyProperty), bindingExpressionValue))
				return;

			target.RestoreLocalValue(dependencyProperty, bindingExpressionValue as BindingExpression ?? ConvertTemplateBindingExpression(bindingExpressionValue as TemplateBindingExpression) ?? bindingExpressionValue);
		}

		private class DepObj : DependencyObject
		{
		}
	}
}