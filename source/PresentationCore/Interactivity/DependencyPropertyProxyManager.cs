// <copyright file="DependencyPropertyProxyManager.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.Core.Monads;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Interactivity
{
	internal static class DependencyPropertyProxyManager
	{
		#region Static Fields and Constants

		private static readonly Dictionary<string, DependencyProperty> Name2Property = new Dictionary<string, DependencyProperty>();
		private static readonly Dictionary<DependencyProperty, string> Property2Name = new Dictionary<DependencyProperty, string>();

		#endregion

		#region  Methods

		private static DependencyProperty CreateProxyProperty(string propertyName, PropertyChangedCallback changedCallback)
		{
			var property = DependencyPropertyManager.RegisterAttached
				(propertyName, typeof(object), typeof(DependencyPropertyProxyManager), new ProxyPropertyMetadata(propertyName, null, changedCallback));
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
			return targetType != null
				? property.GetMetadata(targetType).As<ProxyPropertyMetadata>().Return(p => p.PropertyName, Property2Name.GetValueOrDefault(property))
				: Property2Name.GetValueOrDefault(property);
		}

		#endregion

		#region  Nested Types

		private class ProxyPropertyMetadata : PropertyMetadata
		{
			#region Fields

			public readonly string PropertyName;

			#endregion

			#region Ctors

			public ProxyPropertyMetadata(string propertyName, object defaultValue, PropertyChangedCallback propertyChangedCallback)
				: base(defaultValue, propertyChangedCallback)
			{
				PropertyName = propertyName;
			}

			#endregion
		}

		#endregion
	}
}