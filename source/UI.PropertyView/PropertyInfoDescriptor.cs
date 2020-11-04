// <copyright file="PropertyInfoDescriptor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Zaaml.UI.Controls.PropertyView
{
	internal static class PropertyInfoDescriptor
	{
		public static PropertyDescriptor CreateDescriptor(Type propertyObjectType, PropertyInfo propertyInfo, PropertyDescriptorProvider provider)
		{
			var propertyType = propertyInfo.PropertyType;
			var genericListType = propertyType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>));

			if (genericListType != null)
			{
				var itemType = genericListType.GetGenericArguments().Single();
				var collectionPropertyDescriptorType = typeof(ListPropertyInfoDescriptor<,,>).MakeGenericType(propertyObjectType, propertyType, itemType);

				return (PropertyDescriptor) Activator.CreateInstance(collectionPropertyDescriptorType, propertyInfo, provider);
			}

			var genericCollectionType = propertyType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>));

			if (genericCollectionType != null)
			{
				var itemType = genericCollectionType.GetGenericArguments().Single();
				var collectionPropertyDescriptorType = typeof(CollectionPropertyInfoDescriptor<,,>).MakeGenericType(propertyObjectType, propertyType, itemType);

				return (PropertyDescriptor) Activator.CreateInstance(collectionPropertyDescriptorType, propertyInfo, provider);
			}

			var propertyDescriptorType = typeof(PropertyInfoDescriptor<,>).MakeGenericType(propertyObjectType, propertyType);

			return (PropertyDescriptor) Activator.CreateInstance(propertyDescriptorType, propertyInfo, provider);
		}

		public static Func<TTarget, T> CreatePropertyGetter<TTarget, T>(PropertyInfo propertyInfo)
		{
			var targetExp = Expression.Parameter(typeof(TTarget));
			var targetExpConverted = GetConvertedParameter(targetExp, propertyInfo.ReflectedType);

			var property = Expression.Property(targetExpConverted, propertyInfo);
			var propertyConverted = typeof(T) != propertyInfo.PropertyType
				? (Expression) Expression.Convert(property, typeof(T))
				: property;

			var propertyGetter = Expression.Lambda<Func<TTarget, T>>(propertyConverted, targetExp).Compile();

			return target => propertyGetter(target);
		}

		public static Func<TTarget, T, TTarget> CreatePropertySetter<TTarget, T>(PropertyInfo propertyInfo)
		{
			var targetExp = Expression.Parameter(typeof(TTarget));
			var targetExpConverted = GetConvertedParameter(targetExp, propertyInfo.ReflectedType);

			var valueExp = Expression.Parameter(typeof(T));
			var valueExpConverted = typeof(T) != propertyInfo.PropertyType
				? (Expression) Expression.Convert(valueExp, propertyInfo.PropertyType)
				: valueExp;

			var property = Expression.Property(targetExpConverted, propertyInfo);
			var assignExp = Expression.Assign(property, valueExpConverted);

			var blockExp = Expression.Block(
				assignExp,
				targetExpConverted
			);

			var propertySetter = Expression.Lambda<Func<TTarget, T, TTarget>>(blockExp, targetExp, valueExp).Compile();

			return (target, value) => propertySetter(target, value);
		}

		public static string GetCategory(PropertyInfo propertyInfo, PropertyDescriptorProvider provider)
		{
			return null;
		}

		private static Expression GetConvertedParameter(ParameterExpression parameter, Type type)
		{
			if (parameter.Type == type)
				return parameter;

			return type.IsValueType ? Expression.Unbox(parameter, type) : Expression.Convert(parameter, type);
		}

		public static string GetDescription(PropertyInfo propertyInfo, PropertyDescriptorProvider provider)
		{
			return propertyInfo.Name;
		}

		public static string GetDisplayName(PropertyInfo propertyInfo, PropertyDescriptorProvider provider)
		{
			return propertyInfo.Name;
		}

		public static string GetName(PropertyInfo propertyInfo, PropertyDescriptorProvider provider)
		{
			return propertyInfo.Name;
		}
	}

	public class PropertyInfoDescriptor<TTarget, T> : PropertyDescriptor<TTarget, T>
	{
		private static readonly T DefaultValue = default;
		private readonly Func<TTarget, T> _getter;
		private readonly Func<TTarget, T, TTarget> _setter;

		public PropertyInfoDescriptor(PropertyInfo propertyInfo, PropertyDescriptorProvider provider) : base(provider)
		{
			PropertyInfo = propertyInfo;

			IsReadOnly = propertyInfo.CanWrite == false;
			Name = PropertyInfoDescriptor.GetName(propertyInfo, provider);
			DisplayName = PropertyInfoDescriptor.GetDescription(propertyInfo, provider);
			Category = PropertyInfoDescriptor.GetCategory(propertyInfo, provider);
			Description = PropertyInfoDescriptor.GetDescription(propertyInfo, provider);

			_getter = PropertyInfoDescriptor.CreatePropertyGetter<TTarget, T>(propertyInfo);
			_setter = propertyInfo.CanWrite ? PropertyInfoDescriptor.CreatePropertySetter<TTarget, T>(propertyInfo) : null;
		}

		public override string Category { get; }

		public override string Description { get; }

		public override string DisplayName { get; }

		public override bool IsReadOnly { get; }

		public override string Name { get; }

		public PropertyInfo PropertyInfo { get; }

		public override T GetValue(TTarget component)
		{
			return _getter(component);
		}

		public override TTarget ResetValue(TTarget component)
		{
			if (IsReadOnly)
				throw new NotSupportedException();

			return _setter(component, DefaultValue);
		}

		public override TTarget SetValue(TTarget component, T value)
		{
			if (_setter == null)
				throw new NotSupportedException();

			return _setter(component, value);
		}
	}
}