// <copyright file="BindingEvaluator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Data
{
	internal static class BindingEvaluator
	{
		private static readonly Dictionary<Type, BindingEvaluatorImpl> Evaluators = [];

		private static readonly DependencyProperty ValueProperty = DPM.RegisterAttached<object>
			("Value", typeof(BindingEvaluator));

		public static object EvaluateBinding(Binding binding)
		{
			return EvaluateBinding(binding, typeof(object));
		}

		public static object EvaluateBinding(Binding binding, Type propertyType)
		{
			return Evaluators.GetValueOrCreate(propertyType, () => new BindingEvaluatorImpl(propertyType)).EvaluateBindingImpl(binding);
		}

		public static object EvaluateBinding(DependencyObject target, Binding binding)
		{
			target.SetBinding(ValueProperty, binding);

			var value = target.GetValue(ValueProperty);

			target.ClearValue(ValueProperty);

			return value;
		}

		public static object GetValue(DependencyObject element)
		{
			return element.GetValue(ValueProperty);
		}

		public static void SetValue(DependencyObject element, object value)
		{
			element.SetValue(ValueProperty, value);
		}

		private class BindingEvaluatorImpl : DependencyObject
		{
			private readonly DependencyProperty _valueProperty;

			public BindingEvaluatorImpl(Type targetType)
			{
				_valueProperty = DependencyProperty.RegisterAttached($"Value{targetType.TypeHandle}", targetType, typeof(BindingEvaluator), new PropertyMetadata(targetType.CreateDefaultValue()));
			}

			public object EvaluateBindingImpl(Binding binding)
			{
				this.SetBinding(_valueProperty, binding);

				var value = GetValue(_valueProperty);

				ClearValue(_valueProperty);

				return value;
			}
		}
	}
}