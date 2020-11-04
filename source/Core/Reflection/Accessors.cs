// <copyright file="Accessors.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Zaaml.Core.Reflection
{
  public delegate TResult ValueGetter<in TSource, out TResult>(TSource target);

  public delegate void ValueSetter<in TTarget, in TValue>(TTarget target, TValue value);

  public delegate TResult IndexedValueGetter<in TSource, out TResult>(TSource target, int index);

  public delegate object ValueGetter(object target);

  public delegate void ValueSetter(object target, object value);

  public delegate object IndexedValueGetter(object target, int index);

	internal static class AccessorFactory
  {
    #region Static Fields

    public const BindingFlags DefaultFieldGetterBindingFlags =
      BindingFlags.GetField | BindingFlags.Public | BindingFlags.Instance;

    public const BindingFlags DefaultFieldSetterBindingFlags =
      BindingFlags.SetField | BindingFlags.Public | BindingFlags.Instance;

    public const BindingFlags DefaultPropertyGetterBindingFlags =
      BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance;

    public const BindingFlags DefaultPropertySetterBindingFlags =
      BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance;

    #endregion

    #region Methods

    public static Func<object[], object> CreateConstructor(ConstructorInfo constructorInfo, IEnumerable<Type> argTypes)
    {
      var argTypesArray = argTypes.ToArray();
      var expectedArgs = constructorInfo.GetParameters().Select(pi => pi.ParameterType).ToArray();

      if (argTypesArray.Length != expectedArgs.Length || argTypesArray.Where((t, i) => t != expectedArgs[i]).Any())
        throw new Exception("Can not create constructor.");

      var paramExp = Expression.Parameter(typeof (object[]));
      var argExp = argTypesArray.Select((t, i) => Expression.Convert(Expression.ArrayAccess(paramExp, Expression.Constant(i)), t));
      var newExp = Expression.New(constructorInfo, argExp);

      return Expression.Lambda<Func<object[], object>>(newExp, paramExp).Compile();
    }

    public static Func<object> CreateConstructor(ConstructorInfo constructorInfo)
    {
      if (constructorInfo.GetParameters().Any())
        throw new Exception("Can not create parameterless constructor");

      var newExp = Expression.New(constructorInfo);
      return Expression.Lambda<Func<object>>(newExp).Compile();
    }

    public static ValueGetter CreateFieldGetter(FieldInfo fieldInfo)
    {
      return CreateFieldGetter<object, object>(fieldInfo).AsUntypedGetter();
    }

    public static ValueGetter CreateFieldGetter<TSource>(string name, BindingFlags bindingFlags = DefaultFieldGetterBindingFlags)
    {
      return CreateFieldGetter<object, object>(name, bindingFlags).AsUntypedGetter();
    }

    public static ValueGetter<TSource, TResult> CreateFieldGetter<TSource, TResult>(FieldInfo fieldInfo)
    {
      var targetExp = Expression.Parameter(typeof (TSource));
      var targetExpConverted = GetConvertedParameter(targetExp, fieldInfo.ReflectedType);

      var field = Expression.Field(targetExpConverted, fieldInfo);
      var fieldConverted = typeof (TResult) != fieldInfo.FieldType ? (Expression) Expression.Convert(field, typeof (TResult)) : field;

      var fieldGetter = Expression.Lambda<Func<TSource, TResult>>(fieldConverted, targetExp).Compile();
      return target => fieldGetter(target);
    }

    public static ValueGetter<TSource, TResult> CreateFieldGetter<TSource, TResult>(string name, BindingFlags bindingFlags = DefaultFieldGetterBindingFlags)
    {
      return CreateFieldGetter<TSource, TResult>(GetFieldInfoChecked(typeof (TSource), name, bindingFlags));
    }

    public static ValueSetter<TSource, TValue> CreateFieldSetter<TSource, TValue>(string name, BindingFlags bindingFlags = DefaultFieldSetterBindingFlags)
    {
      return CreateFieldSetter<TSource, TValue>(GetFieldInfoChecked(typeof (TSource), name, bindingFlags));
    }

    public static ValueSetter<TTarget, TValue> CreateFieldSetter<TTarget, TValue>(FieldInfo fieldInfo)
    {
      var targetExp = Expression.Parameter(typeof (TTarget));
      var targetExpConverted = GetConvertedParameter(targetExp, fieldInfo.ReflectedType);

      var valueExp = Expression.Parameter(typeof (TValue));
      var valueExpConverted = typeof (TValue) != fieldInfo.FieldType
        ? (Expression) Expression.Convert(valueExp, fieldInfo.FieldType)
        : valueExp;

      var property = Expression.Field(targetExpConverted, fieldInfo);
      var assignExp = Expression.Assign(property, valueExpConverted);

      var fieldSetter = Expression.Lambda<Action<TTarget, TValue>>(assignExp, targetExp, valueExp).Compile();

      return (target, value) => fieldSetter(target, value);
    }

    public static ValueSetter CreateFieldSetter(string name, BindingFlags bindingFlags = DefaultFieldSetterBindingFlags)
    {
      return CreateFieldSetter<object, object>(name, bindingFlags).AsUntypedSetter();
    }

    public static ValueSetter CreateFieldSetter(FieldInfo fieldInfo)
    {
      return CreateFieldSetter<object, object>(fieldInfo).AsUntypedSetter();
    }

    public static IndexedValueGetter<TSource, TResult> CreateIndexedValueGetter<TSource, TResult>(PropertyInfo propertyInfo)
    {
      var targetArgExp = Expression.Parameter(typeof (TSource));
      var targetArgConvertedExp = GetConvertedParameter(targetArgExp, propertyInfo.ReflectedType);
      var indexArgExp = Expression.Parameter(typeof (int));
      var indexerExp = Expression.MakeIndex(targetArgConvertedExp, propertyInfo, new Expression[] {indexArgExp});
      var indexer = Expression.Lambda<Func<TSource, int, TResult>>(Expression.Convert(indexerExp, typeof (TResult)), targetArgExp, indexArgExp).Compile();

      return (target, index) => indexer(target, index);
    }

    public static IndexedValueGetter CreateIndexedValueGetter(PropertyInfo propertyInfo)
    {
      return CreateIndexedValueGetter<object, object>(propertyInfo).AsUntypedIndexedValueGetter();
    }

    public static ValueGetter<TSource, TResult> CreatePropertyGetter<TSource, TResult>(PropertyInfo propertyInfo)
    {
      var targetExp = Expression.Parameter(typeof (TSource));
      var targetExpConverted = GetConvertedParameter(targetExp, propertyInfo.ReflectedType);

      var property = Expression.Property(targetExpConverted, propertyInfo);
      var propertyConverted = typeof (TResult) != propertyInfo.PropertyType
        ? (Expression) Expression.Convert(property, typeof (TResult))
        : property;

      var propertyGetter = Expression.Lambda<Func<TSource, TResult>>(propertyConverted, targetExp).Compile();
      return target => propertyGetter(target);
    }

		public static ValueGetter<TTarget, TValue> CreateGetter<TTarget, TValue>(string memberName)
		{
			var type = typeof (TTarget);
      var fieldInfo = type.GetField(memberName, BindingFlags.Instance | BindingFlags.GetField | BindingFlags.Public);

      if (fieldInfo != null)
        return CreateFieldGetter<TTarget, TValue>(fieldInfo);

      var propertyInfo = type.GetProperty(memberName, BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public);

			if (propertyInfo != null)
				return CreatePropertyGetter<TTarget, TValue>(propertyInfo);

			return null;
		}

		public static ValueGetter CreateGetter(Type type, string memberName)
		{
			var fieldInfo = type.GetField(memberName, BindingFlags.Instance | BindingFlags.GetField | BindingFlags.Public);

			if (fieldInfo != null)
				return CreateFieldGetter(fieldInfo);

			var propertyInfo = type.GetProperty(memberName, BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public);

			if (propertyInfo != null)
				return CreatePropertyGetter(propertyInfo);

			return null;
		}


    public static ValueGetter<TSource, TResult> CreatePropertyGetter<TSource, TResult>(string name, BindingFlags bindingFlags = DefaultPropertyGetterBindingFlags)
    {
      return CreatePropertyGetter<TSource, TResult>(GetPropertyInfoChecked(typeof (TSource), name, bindingFlags));
    }

    public static ValueGetter CreatePropertyGetter(PropertyInfo propertyInfo)
    {
      return CreatePropertyGetter<object, object>(propertyInfo).AsUntypedGetter();
    }

    public static ValueGetter CreatePropertyGetter(string name, BindingFlags bindingFlags = DefaultPropertyGetterBindingFlags)
    {
      return CreatePropertyGetter<object, object>(name, bindingFlags).AsUntypedGetter();
    }

    public static ValueSetter<TTarget, TValue> CreatePropertySetter<TTarget, TValue>(PropertyInfo propertyInfo)
    {
      var targetExp = Expression.Parameter(typeof (TTarget));
      var targetExpConverted = GetConvertedParameter(targetExp, propertyInfo.ReflectedType);

      var valueExp = Expression.Parameter(typeof (TValue));
      var valueExpConverted = typeof (TValue) != propertyInfo.PropertyType
        ? (Expression) Expression.Convert(valueExp, propertyInfo.PropertyType)
        : valueExp;

      var property = Expression.Property(targetExpConverted, propertyInfo);
      var assignExp = Expression.Assign(property, valueExpConverted);
      var propertySetter = Expression.Lambda<Action<TTarget, TValue>>(assignExp, targetExp, valueExp).Compile();

      return (target, value) => propertySetter(target, value);
    }

    public static ValueSetter<TTarget, TValue> CreatePropertySetter<TTarget, TValue>(string name, BindingFlags bindingFlags = DefaultPropertySetterBindingFlags)
    {
      return CreatePropertySetter<TTarget, TValue>(GetPropertyInfoChecked(typeof (TTarget), name, bindingFlags));
    }

    public static ValueSetter CreatePropertySetter(PropertyInfo propertyInfo)
    {
      return CreatePropertySetter<object, object>(propertyInfo).AsUntypedSetter();
    }

    public static ValueSetter CreatePropertySetter(string name, BindingFlags bindingFlags = DefaultPropertySetterBindingFlags)
    {
      return CreatePropertySetter<object, object>(name, bindingFlags).AsUntypedSetter();
    }

    public static FieldInfo GetFieldInfoChecked(Type type, string fieldName, BindingFlags bindingFlags)
    {
      var fieldInfo = type.GetField(fieldName, bindingFlags);

      if (fieldInfo == null)
        throw new ArgumentException("Can not resolve field with specified name and binding flags");

      return fieldInfo;
    }

    public static PropertyInfo GetPropertyInfoChecked(Type type, string propertyName, BindingFlags bindingFlags)
    {
      var propertyInfo = type.GetProperty(propertyName, bindingFlags);

      if (propertyInfo == null)
        throw new ArgumentException("Can not resolve property with specified name and binding flags");

      return propertyInfo;
    }

    private static ValueGetter AsUntypedGetter(this ValueGetter<object, object> typedGetter)
    {
      return t => typedGetter(t);
    }

    private static IndexedValueGetter AsUntypedIndexedValueGetter(this IndexedValueGetter<object, object> typedIndexedValueGetter)
    {
      return (t, i) => typedIndexedValueGetter(t, i);
    }

    private static ValueSetter AsUntypedSetter(this ValueSetter<object, object> typedSetter)
    {
      return (t, v) => typedSetter(t, v);
    }

    private static Expression GetConvertedParameter(ParameterExpression parameter, Type type)
    {
      if (parameter.Type == type)
        return parameter;

      return type.IsValueType ? Expression.Unbox(parameter, type) : Expression.Convert(parameter, type);
    }

    #endregion
  }
}