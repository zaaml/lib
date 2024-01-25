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

		public static readonly PropertyDescriptorProvider Instance = new();
		private static readonly MethodInfo GetPropertiesGenericMethodInfo = typeof(PropertyDescriptorProvider).GetMethod(nameof(GetPropertyDescriptors), BindingFlags.Instance | BindingFlags.Public);

		private static readonly Dictionary<Type, Func<object, PropertyDescriptorProvider, PropertyDescriptorCollection>>
			PropertiesGetterDictionary = new();

		private readonly Dictionary<Type, PropertyDescriptorCollection> _propertyDescriptorsDictionary = new();

		private static readonly HashSet<Type> NoPropertyTypes;

		static PropertyDescriptorProvider()
		{
			NoPropertyTypes = new HashSet<Type>
			{
				typeof(Type).Assembly.GetType("System.RuntimeType"),
				typeof(Type),
				typeof(string)
			};
		}

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

		protected virtual BindingFlags BindingFlags => DefaultBindingFlags; 
		
		private PropertyDescriptorCollection CreatePropertyDescriptorCollection(Type propertyObjectType)
		{
			var bindingFlags = BindingFlags;

			var propertyDescriptors = propertyObjectType.GetProperties(bindingFlags)
				.Select(pi => pi)
				.Where(ProvideDescriptor)
				.Where(pi => pi.GetIndexParameters().Length == 0)
				.OrderBy(p => p.Name)
				.Select(propertyInfo => PropertyInfoDescriptor.CreateDescriptor(propertyObjectType, propertyInfo, this)).ToList();

			return new PropertyDescriptorCollection(propertyDescriptors);
		}

		protected virtual bool ProvideDescriptor(PropertyInfo propertyInfo)
		{
			return true;
		}

		internal PropertyDescriptorCollection GetPropertiesInternal(object propertyObject)
		{
			if (propertyObject == null)
				return null;

			var propertiesGetter = PropertiesGetterDictionary.GetValueOrCreate(propertyObject.GetType(), CreatePropertiesGetter);

			return propertiesGetter(propertyObject, this);
		}

		protected virtual bool ProvidePropertiesForType(Type type)
		{
			return NoPropertyTypes.Contains(type) == false;
		}

		public virtual PropertyDescriptorCollection GetPropertyDescriptors<T>(T propertyObject)
		{
			if (propertyObject == null)
				return PropertyDescriptorCollection.Empty;
			
			var type = propertyObject.GetType();

			if (ProvidePropertiesForType(type) == false)
				return PropertyDescriptorCollection.Empty;

			return _propertyDescriptorsDictionary.GetValueOrCreate(type, CreatePropertyDescriptorCollection);
		}
	}
}