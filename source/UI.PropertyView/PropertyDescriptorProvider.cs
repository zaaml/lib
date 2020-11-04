// <copyright file="PropertyDescriptorProvider.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Zaaml.Core.Extensions;

namespace Zaaml.UI.Controls.PropertyView
{
	public class PropertyDescriptorProvider
	{
		private const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty;

		public static readonly PropertyDescriptorProvider Instance = new PropertyDescriptorProvider();
		private static readonly MethodInfo GetPropertiesGenericMethodInfo = typeof(PropertyDescriptorProvider).GetMethod(nameof(GetPropertyDescriptors), BindingFlags.Instance | BindingFlags.Public);

		private static readonly Dictionary<Type, Func<object, PropertyDescriptorProvider, PropertyDescriptorCollection>>
			PropertiesGetterDictionary = new Dictionary<Type, Func<object, PropertyDescriptorProvider, PropertyDescriptorCollection>>();

		private readonly Dictionary<Type, PropertyDescriptorCollection> _propertyDescriptorsDictionary = new Dictionary<Type, PropertyDescriptorCollection>();

		protected PropertyDescriptorProvider()
		{
		}

		private static Func<object, PropertyDescriptorProvider, PropertyDescriptorCollection> CreatePropertiesGetter(Type type)
		{
			var targetExp = Expression.Parameter(typeof(object));
			var convertedExp = Expression.Convert(targetExp, type);
			var providerExp = Expression.Parameter(typeof(PropertyDescriptorProvider));
			var methodInfo = GetPropertiesGenericMethodInfo.MakeGenericMethod(type);
			var propertiesExp = Expression.Call(providerExp, methodInfo, convertedExp);

			return Expression.Lambda<Func<object, PropertyDescriptorProvider, PropertyDescriptorCollection>>(propertiesExp, targetExp, providerExp).Compile();
		}

		private PropertyDescriptorCollection CreatePropertyDescriptorCollection(Type propertyObjectType)
		{
			var propertyDescriptors = propertyObjectType.GetProperties(DefaultBindingFlags).Select(pi => pi).Where(pi => pi.GetIndexParameters().Length == 0).OrderBy(p => p.Name)
				.Select(propertyInfo => PropertyInfoDescriptor.CreateDescriptor(propertyObjectType, propertyInfo, this)).ToList();

			return new PropertyDescriptorCollection(propertyDescriptors);
		}

		internal PropertyDescriptorCollection GetPropertiesInternal(object propertyObject)
		{
			if (propertyObject == null)
				return null;

			var propertiesGetter = PropertiesGetterDictionary.GetValueOrCreate(propertyObject.GetType(), CreatePropertiesGetter);

			return propertiesGetter(propertyObject, this);
		}

		public virtual PropertyDescriptorCollection GetPropertyDescriptors<T>(T propertyObject)
		{
			return propertyObject == null ? null : _propertyDescriptorsDictionary.GetValueOrCreate(propertyObject.GetType(), CreatePropertyDescriptorCollection);
		}
	}
}