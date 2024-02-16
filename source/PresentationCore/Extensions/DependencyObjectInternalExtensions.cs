// <copyright file="DependencyObjectInternalExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Extensions
{
	internal static partial class DependencyObjectInternalExtensions
	{
		[DebuggerStepThrough]
		internal static void ClearExpandoValue(this DependencyObject depObj, string propertyName, bool skipPropertyChangedHandler)
		{
			var prop = DependencyPropertyManager.GetExpandoProperty(propertyName);

			if (skipPropertyChangedHandler == false)
			{
				depObj.ClearValue(prop);

				return;
			}

			try
			{
				depObj.SuspendPropertyChangedCallback(prop);

				depObj.ClearValue(prop);
			}
			finally
			{
				depObj.ResumePropertyChangedCallback(prop);
			}
		}

		[DebuggerStepThrough]
		internal static void ClearValue<T>(this DependencyObject dependencyObject, DependencyProperty property, Action<T> cleanUpAction)
		{
			cleanUpAction(dependencyObject.GetValue<T>(property));
			dependencyObject.ClearValue(property);
		}

		[DebuggerStepThrough]
		internal static void ClearValue(this DependencyObject dependencyObject, DependencyPropertyKey key, bool skipPropertyChangedHandler)
		{
			if (skipPropertyChangedHandler == false)
			{
				dependencyObject.ClearValue(key);

				return;
			}

			try
			{
				dependencyObject.SuspendPropertyChangedCallback(key.DependencyProperty);

				dependencyObject.ClearValue(key);
			}
			finally
			{
				dependencyObject.ResumePropertyChangedCallback(key.DependencyProperty);
			}
		}

		internal static void EnsureBindingSet(this DependencyObject depObj, DependencyProperty depProp, Binding binding)
		{
			BindingUtil.EnsureBindingSet(depObj, depProp, binding);
		}

		internal static void EnsureValueSet<T>(this DependencyObject depObj, DependencyProperty depProp, T value)
		{
			var currentValueRaw = depObj.GetValue(depProp);

			if (currentValueRaw is not T currentValue || Equals(currentValue, value) == false)
				depObj.SetValue(depProp, value);
		}


		[DebuggerStepThrough]
		internal static T GetValueOrCreate<T>(this DependencyObject dependencyObject, DependencyProperty dependencyProperty,
			Func<T> factoryMethod) where T : class
		{
			var value = (T)dependencyObject.GetValue(dependencyProperty);

			if (value == null)
				dependencyObject.SetValue(dependencyProperty, value = factoryMethod());

			return value;
		}

		[DebuggerStepThrough]
		internal static T GetValueOrCreate<T>(this DependencyObject dependencyObject, DependencyPropertyKey dependencyPropertyKey,
			Func<T> factoryMethod) where T : class
		{
			var value = (T)dependencyObject.GetValue(dependencyPropertyKey.DependencyProperty);

			if (value == null)
				dependencyObject.SetReadOnlyValue(dependencyPropertyKey, value = factoryMethod());

			return value;
		}

		[DebuggerStepThrough]
		internal static T GetValueOrCreateOrDefault<T>(this DependencyObject dependencyObject, DependencyProperty dependencyProperty, bool create, Func<T> factoryMethod)
		{
			var value = (T)dependencyObject.GetValue(dependencyProperty);

			if (value == null && create)
				dependencyObject.SetValue(dependencyProperty, value = factoryMethod());

			return value;
		}

		[DebuggerStepThrough]
		internal static T GetValueOrCreateOrDefault<T>(this DependencyObject dependencyObject, DependencyPropertyKey dependencyPropertyKey, bool create, Func<T> factoryMethod)
		{
			var value = (T)dependencyObject.GetValue(dependencyPropertyKey.DependencyProperty);

			if (value == null && create)
				dependencyObject.SetReadOnlyValue(dependencyPropertyKey, value = factoryMethod());

			return value;
		}

		internal static void SetCurrentValueInternal(this DependencyObject depObj, DependencyProperty property, object value)
		{
			depObj.SetCurrentValue(property, value);
		}

		internal static IDisposable SetDisposableValue(this DependencyObject depObj, DependencyProperty dependencyProperty, object value)
		{
			var restoreValueHelper = new LocalValueStore(depObj, dependencyProperty);
			var binding = value is BindingExpression bindingExpression ? bindingExpression.ParentBinding : value as Binding;

			if (binding != null)
				depObj.SetBinding(dependencyProperty, binding);
			else
				depObj.SetValue(dependencyProperty, value);

			return restoreValueHelper;
		}

		[DebuggerStepThrough]
		internal static void SetExpandoValue(this DependencyObject depObj, string propertyName, object value, bool skipPropertyChangedHandler)
		{
			if (skipPropertyChangedHandler == false)
			{
				depObj.SetExpandoValue(propertyName, value);

				return;
			}

			var prop = DependencyPropertyManager.GetExpandoProperty(propertyName);

			try
			{
				depObj.SuspendPropertyChangedCallback(prop);

				depObj.SetValue(prop, value);
			}
			finally
			{
				depObj.ResumePropertyChangedCallback(prop);
			}
		}

		[DebuggerStepThrough]
		internal static void SetValue<T>(this DependencyObject dependencyObject, DependencyProperty property, T value, Action<T> cleanUpAction)
		{
			cleanUpAction(dependencyObject.GetValue<T>(property));
			dependencyObject.SetValue(property, value);
		}

		[DebuggerStepThrough]
		internal static void SetValue(this DependencyObject dependencyObject, DependencyPropertyKey key, object value, bool skipPropertyChangedHandler)
		{
			if (skipPropertyChangedHandler == false)
			{
				dependencyObject.SetValue(key, value);

				return;
			}

			try
			{
				dependencyObject.SuspendPropertyChangedCallback(key.DependencyProperty);

				dependencyObject.SetValue(key, value);
			}
			finally
			{
				dependencyObject.ResumePropertyChangedCallback(key.DependencyProperty);
			}
		}

		[DebuggerStepThrough]
		internal static void SetValue<T>(this DependencyObject dependencyObject, DependencyPropertyKey key, T value, bool skipPropertyChangedHandler)
		{
			if (skipPropertyChangedHandler == false)
			{
				dependencyObject.SetValue(key, value);

				return;
			}

			try
			{
				dependencyObject.SuspendPropertyChangedCallback(key.DependencyProperty);

				dependencyObject.SetValue(key, value);
			}
			finally
			{
				dependencyObject.ResumePropertyChangedCallback(key.DependencyProperty);
			}
		}

		[DebuggerStepThrough]
		internal static void SetValue<T>(this DependencyObject dependencyObject, DependencyProperty property, T value, bool skipPropertyChangedHandler)
		{
			if (skipPropertyChangedHandler == false)
			{
				dependencyObject.SetValue(property, value);

				return;
			}

			try
			{
				dependencyObject.SuspendPropertyChangedCallback(property);

				dependencyObject.SetValue(property, value);
			}
			finally
			{
				dependencyObject.ResumePropertyChangedCallback(property);
			}
		}

		[DebuggerStepThrough]
		internal static void SetValue(this DependencyObject dependencyObject, DependencyProperty property, object value, bool skipPropertyChangedHandler)
		{
			if (skipPropertyChangedHandler == false)
			{
				dependencyObject.SetValue(property, value);

				return;
			}

			try
			{
				dependencyObject.SuspendPropertyChangedCallback(property);

				dependencyObject.SetValue(property, value);
			}
			finally
			{
				dependencyObject.ResumePropertyChangedCallback(property);
			}
		}
	}
}