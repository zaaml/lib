// <copyright file="DependencyPropertyProxyManager.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Interactivity
{
	internal static class DependencyPropertyProxyManager
	{
		private static readonly Dictionary<string, DependencyProperty> Name2Property = new();
		private static readonly Dictionary<DependencyProperty, string> Property2Name = new();

		private static DependencyProperty CreateProxyProperty(string propertyName, PropertyChangedCallback changedCallback)
		{
			var property = DependencyPropertyManager.RegisterAttached(propertyName, typeof(object), typeof(DependencyPropertyProxyManager),
				new ProxyPropertyMetadata(propertyName, null, changedCallback));

			Property2Name[property] = propertyName;

			return property;
		}

		public static DependencyProperty GetDependencyProperty(string propertyName)
		{
			return Name2Property.GetValueOrCreate(propertyName, () => CreateProxyProperty(propertyName, null));
		}

		public static DependencyProperty GetDependencyProperty(string propertyName, PropertyChangedCallback changedCallback)
		{
			return Name2Property.GetValueOrCreate(propertyName, () => CreateProxyProperty(propertyName, changedCallback));
		}

		public static string GetPropertyName(DependencyProperty property, Type targetType = null)
		{
			if (targetType == null)
				return Property2Name.GetValueOrDefault(property);

			return property.GetMetadata(targetType) is ProxyPropertyMetadata proxyPropertyMetadata
				? proxyPropertyMetadata.PropertyName
				: Property2Name.GetValueOrDefault(property);
		}

		private class ProxyPropertyMetadata : PropertyMetadata
		{
			public readonly string PropertyName;

			public ProxyPropertyMetadata(string propertyName, object defaultValue, PropertyChangedCallback propertyChangedCallback)
				: base(defaultValue, propertyChangedCallback)
			{
				PropertyName = propertyName;
			}
		}
	}
}