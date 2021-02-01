// <copyright file="MarkupExtensionBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using Zaaml.Core;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.MarkupExtensions
{
	public abstract class MarkupExtensionBase : MarkupExtension
	{
		#region Static Fields and Constants

		private static readonly BindingFlags BindingFlagsExt = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

		#endregion

		#region Ctors

		internal MarkupExtensionBase()
		{
		}

		#endregion

		#region  Methods

	  protected object DefaultPropertyValue(IServiceProvider serviceProvider)
	  {
		  GetTarget(serviceProvider, out _, out var targetProperty, out _);

	    var propertyInfo = targetProperty as PropertyInfo;

	    if (propertyInfo != null)
	      return RuntimeUtils.CreateDefaultValue(propertyInfo.PropertyType);

	    if (targetProperty is DependencyProperty dependencyProperty)
	      return RuntimeUtils.CreateDefaultValue(dependencyProperty.GetPropertyType());

	    return null;
    }

		protected bool GetCurrentValue(object targetObject, object targetProperty, out object result)
		{
			result = null;

			if (targetObject == null || targetProperty == null)
				return false;

			try
			{
				var propertyInfo = targetProperty as PropertyInfo;

				if (propertyInfo != null && propertyInfo.CanRead)
				{
					result = propertyInfo.GetValue(targetObject, null);

					return true;
				}

				var methodInfo = targetProperty as MethodInfo;

				if (methodInfo != null)
				{
					result = methodInfo.Invoke(null, new[] { targetObject });

					return true;
				}

				var dependencyProperty = GetDependencyProperty(targetProperty);

				if (dependencyProperty != null)
				{
					if (targetObject is DependencyObject dependencyTarget)
					{
						result = dependencyTarget.GetValue(dependencyProperty);

						return true;
					}
				}
			}
			catch
			{
				return false;
			}

			return false;
		}

	  protected static Type GetPropertyType(object targetProperty)
	  {
      try
      {
        var propertyInfo = targetProperty as PropertyInfo;

        if (propertyInfo != null)
          return propertyInfo.PropertyType;

        var methodInfo = targetProperty as MethodInfo;

        if (methodInfo != null)
        {
          var parameters = methodInfo.GetParameters();

          if (methodInfo.IsStatic)
          {
            if (methodInfo.ReturnType == typeof(void))
            {
              if (parameters.Length == 2)
                return parameters[1].ParameterType;
            }
            else if (parameters.Length == 1)
              return methodInfo.ReturnType;
          }

          return null;
        }

        var dependencyProperty = GetDependencyProperty(targetProperty);

        if (dependencyProperty != null)
          return dependencyProperty.GetPropertyType();
      }
      catch
      {
        return null;
      }

	    return null;
	  }

		protected static DependencyProperty GetDependencyProperty(object property)
		{
			if (property is DependencyProperty dependencyProperty)
				return dependencyProperty;

			var propertyInfo = property as PropertyInfo;

			return propertyInfo == null ? null : DependencyPropertyManager.GetDependencyProperty(propertyInfo);
		}

		private static object GetFieldData(object target, string fieldName)
		{
			return target.GetType().GetField(fieldName, BindingFlags.GetField | BindingFlagsExt)?.GetValue(target);
		}

		private static object GetPropertyData(object target, string fieldName)
		{
			return target.GetType().GetProperty(fieldName, BindingFlagsExt)?.GetValue(target, new object[0]);
		}

		private bool GetSilverlightInstanceBuilderServiceProviderTarget(IServiceProvider serviceProvider, out object targetProperty)
		{
			targetProperty = null;

			try
			{
				var targetms = GetFieldData(serviceProvider, "targetNode");
				var properties = (IEnumerable) GetFieldData(targetms, "properties");

				if (properties == null)
					return false;

				foreach (var kv in properties)
				{
					var key = GetFieldData(kv, "key");
					var value = GetFieldData(kv, "value");
					var instance = GetFieldData(value, "instance");

					if (!ReferenceEquals(this, instance)) 
						continue;

					targetProperty = GetPropertyData(key, "DependencyProperty") as DependencyProperty;

					return targetProperty != null;
				}
			}
			catch (Exception e)
			{
				LogService.LogError(e);
			}

			return false;
		}

		protected bool GetTarget(IServiceProvider serviceProvider, out object target, out object targetProperty, out bool reflected)
		{
			target = null;
			targetProperty = null;
			reflected = false;

			var targetProvider = (IProvideValueTarget) serviceProvider?.GetService(typeof(IProvideValueTarget));

			if (targetProvider == null)
				return false;

			target = targetProvider.TargetObject;
			targetProperty = targetProvider.TargetProperty;

			if (target == null)
				return false;

			if (targetProperty != null)
				return true;

			if (PresentationCoreUtils.IsInDesignMode == false)
				return false;

			reflected = true;

			var serviceProviderTypeName = serviceProvider.GetType().Name;

			if (serviceProviderTypeName == "SilverlightInstanceBuilderServiceProvider")
				return GetSilverlightInstanceBuilderServiceProviderTarget(serviceProvider, out targetProperty);

			return false;
		}

		#endregion
	}
}