// <copyright file="DependencyObjectExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using Zaaml.Core;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Extensions
{
	[DebuggerStepThrough]
	public static class DependencyObjectExtensions
	{
		public static ClassList AddClass(this DependencyObject dependencyObject, string className)
		{
			return Extension.AddClass(dependencyObject, className);
		}

		public static void AddValueChanged(this DependencyObject depObj, DependencyProperty depProp, EventHandler<PropertyValueChangedEventArgs> handler)
		{
			depObj.GetServiceOrCreate<PropertyChangeService>().AddValueChanged(depProp, handler);
		}

		public static void AddValueChanged(this DependencyObject depObj, string propertyName, EventHandler<PropertyValueChangedEventArgs> handler)
		{
			depObj.GetServiceOrCreate<PropertyChangeService>().AddValueChanged(propertyName, handler);
		}

		public static void ClearExpandoValue(this DependencyObject depObj, string propertyName)
		{
			depObj.ClearValue(DependencyPropertyManager.GetExpandoProperty(propertyName));
		}

		public static void ClearReadOnlyValue(this DependencyObject dependencyObject, DependencyPropertyKey key)
		{
			dependencyObject.ClearValue(key);
		}

		public static object GetExpandoValue(this DependencyObject depObj, string propertyName)
		{
			return depObj.GetValue(DependencyPropertyManager.GetExpandoProperty(propertyName));
		}

		public static object GetReadOnlyValue(this DependencyObject depObj, DependencyPropertyKey propertyKey)
		{
			return depObj.GetValue(propertyKey.DependencyProperty);
		}

		public static T GetReadOnlyValue<T>(this DependencyObject depObj, DependencyPropertyKey propertyKey)
		{
			return (T)depObj.GetValue(propertyKey.DependencyProperty);
		}

		public static T GetValue<T>(this DependencyObject dependencyObject, DependencyProperty property)
		{
			return (T)dependencyObject.GetValue(property);
		}

		public static Binding ReadLocalBinding(this DependencyObject target, DependencyProperty dependencyProperty)
		{
			return (target.ReadLocalValue(dependencyProperty) as BindingExpression)?.ParentBinding;
		}

		public static BindingExpression ReadLocalBindingExpression(this DependencyObject target, DependencyProperty dependencyProperty)
		{
			return target.ReadLocalValue(dependencyProperty) as BindingExpression;
		}

		public static object ReadLocalExpandoValue(this DependencyObject depObj, string propertyName)
		{
			return depObj.ReadLocalValue(DependencyPropertyManager.GetExpandoProperty(propertyName));
		}

		public static ClassList RemoveClass(this DependencyObject dependencyObject, string className)
		{
			return Extension.RemoveClass(dependencyObject, className);
		}

		public static void RemoveValueChanged(this DependencyObject depObj, DependencyProperty depProp, EventHandler<PropertyValueChangedEventArgs> handler)
		{
			depObj.GetService<PropertyChangeService>()?.RemoveValueChanged(depProp, handler);
		}

		public static void RemoveValueChanged(this DependencyObject depObj, string propertyName, EventHandler<PropertyValueChangedEventArgs> handler)
		{
			depObj.GetService<PropertyChangeService>()?.RemoveValueChanged(propertyName, handler);
		}

		public static void RestoreLocalValue(this DependencyObject target, DependencyProperty dependencyProperty, object localValue)
		{
			if (localValue == DependencyProperty.UnsetValue)
				target.ClearValue(dependencyProperty);
			else if (localValue is BindingExpression localValueBindingExpression)
				target.SetBinding(dependencyProperty, localValueBindingExpression.ParentBinding);
			else
				target.SetValue(dependencyProperty, localValue);
		}

		public static void SetBinding(this DependencyObject depObj, DependencyProperty depProp, Binding binding)
		{
			BindingOperations.SetBinding(depObj, depProp, binding);
		}

		public static void SetExpandoBinding(this DependencyObject depObj, string propertyName, Binding binding)
		{
			BindingOperations.SetBinding(depObj, DependencyPropertyManager.GetExpandoProperty(propertyName), binding);
		}

		public static void SetExpandoValue(this DependencyObject depObj, string propertyName, object value)
		{
			depObj.SetValue(DependencyPropertyManager.GetExpandoProperty(propertyName), value);
		}

		public static void SetReadOnlyValue<T>(this DependencyObject dependencyObject, DependencyPropertyKey key, T value)
		{
			dependencyObject.SetValue(key, value);
		}

		public static void SetReadOnlyValue(this DependencyObject dependencyObject, DependencyPropertyKey key, object value)
		{
			dependencyObject.SetValue(key, value);
		}

		public static void SetValue<T>(this DependencyObject dependencyObject, DependencyProperty property, T value)
		{
			dependencyObject.SetValue(property, value);
		}

		public static ClassList ToggleClass(this DependencyObject dependencyObject, string className)
		{
			return Extension.ToggleClass(dependencyObject, className);
		}
	}
}