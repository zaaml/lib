// <copyright file="GenericPropertyResolver.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore
{
	internal abstract class GenericPropertyResolver
	{
		public Type PropertyType => PropertyTypeCore;

		protected abstract Type PropertyTypeCore { get; }
		private static Dictionary<Type, GenericTypePropertyResolver> TypeResolversDictionary { get; } = new();

		public object GetValue(object targetObject)
		{
			return GetValueCore(targetObject);
		}

		protected abstract object GetValueCore(object targetObject);

		public static GenericPropertyResolver ResolveProperty(object targetObject, object targetProperty)
		{
			var type = targetObject.GetType();

			if (TypeResolversDictionary.TryGetValue(type, out var typePropertyResolver) == false)
				TypeResolversDictionary[type] = typePropertyResolver = new GenericTypePropertyResolver(type);

			return typePropertyResolver.ResolvePropertyForType(targetProperty);
		}

		public void SetValue(object targetObject, object value)
		{
			SetValueCore(targetObject, value);
		}

		protected abstract void SetValueCore(object targetObject, object value);

		private class GenericTypePropertyResolver
		{
			public GenericTypePropertyResolver(Type type)
			{
				Type = type;
			}

			public Type Type { get; }

			public GenericPropertyResolver ResolvePropertyForType(object targetProperty)
			{
				if (targetProperty is DependencyProperty dependencyProperty)
					return new DependencyPropertyResolver(dependencyProperty);

				if (targetProperty is string stringProperty)
				{
					if (typeof(DependencyObject).IsAssignableFrom(Type))
					{
						dependencyProperty = DependencyPropertyManager.GetDependencyProperty(stringProperty, Type);

						if (dependencyProperty != null)
							return new DependencyPropertyResolver(dependencyProperty);
					}

					var propertyInfo = Type.GetProperty(stringProperty);

					if (propertyInfo != null)
						return new PropertyInfoResolver(propertyInfo);
				}

				return null;
			}
		}

		private sealed class PropertyInfoResolver : GenericPropertyResolver
		{
			public PropertyInfoResolver(PropertyInfo propertyInfo)
			{
				PropertyInfo = propertyInfo;
			}

			public PropertyInfo PropertyInfo { get; }

			protected override Type PropertyTypeCore => PropertyInfo.PropertyType;

			protected override object GetValueCore(object targetObject)
			{
				return PropertyInfo.GetValue(targetObject);
			}

			protected override void SetValueCore(object targetObject, object value)
			{
				PropertyInfo.SetValue(targetObject, value);
			}
		}

		private sealed class DependencyPropertyResolver : GenericPropertyResolver
		{
			public DependencyPropertyResolver(DependencyProperty dependencyProperty)
			{
				DependencyProperty = dependencyProperty;
			}

			public DependencyProperty DependencyProperty { get; }

			protected override Type PropertyTypeCore => DependencyProperty.PropertyType;

			protected override object GetValueCore(object targetObject)
			{
				var dependencyObject = (DependencyObject) targetObject;

				return dependencyObject.GetValue(DependencyProperty);
			}

			protected override void SetValueCore(object targetObject, object value)
			{
				var dependencyObject = (DependencyObject) targetObject;

				dependencyObject.SetValue(DependencyProperty, value);
			}
		}
	}
}