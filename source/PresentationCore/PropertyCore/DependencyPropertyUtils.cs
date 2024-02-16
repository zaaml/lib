// <copyright file="DependencyPropertyUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Markup;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.PropertyCore.Extensions;

namespace Zaaml.PresentationCore.PropertyCore
{
	internal static class DependencyPropertyUtils
	{
		private static int AdvancePosition(string str, int index, int change)
		{
			var strLength = str.Length;

			while (index >= 0 && index < strLength && SkipWhiteSpaceDependencyPropertyName(str[index]))
				index += change;

			return index >= 0 && index < strLength ? index : -1;
		}

		internal static bool CopyDependencyPropertyValue(DependencyProperty dependencyProperty, DependencyObject source, DependencyObject target)
		{
			var valueSource = GetValueSource(source, dependencyProperty);

			switch (valueSource)
			{
				case PropertyValueSource.Default:
					target.ClearValue(dependencyProperty);
					return true;
				case PropertyValueSource.Local:
					target.SetValue(dependencyProperty, source.GetValue(dependencyProperty));
					return true;
				case PropertyValueSource.LocalBinding:
					target.CopyBinding(source, dependencyProperty);
					return true;
				case PropertyValueSource.Inherited:
					return false;
				case PropertyValueSource.TemplatedParent:
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static object GetDefaultValue(DependencyObject dependencyObject, DependencyProperty dependencyProperty)
		{
			return new DependencyPropertyValueInfo(dependencyObject, dependencyProperty).DefaultValue;
		}

		public static object GetDefaultValue(Type forType, DependencyProperty dependencyProperty)
		{
			var defaultMetadataValue = GetMetadataDefaultValue(forType, dependencyProperty);

			return defaultMetadataValue.IsDependencyPropertyUnsetValue() ? dependencyProperty.GetPropertyType().CreateDefaultValue() : defaultMetadataValue;
		}

		public static object GetMetadataDefaultValue(DependencyObject dependencyObject, DependencyProperty dependencyProperty)
		{
			return new DependencyPropertyValueInfo(dependencyObject, dependencyProperty).PropertyMetadata.DefaultValue;
		}

		public static object GetMetadataDefaultValue(Type forType, DependencyProperty dependencyProperty)
		{
			return dependencyProperty.GetMetadata(forType).DefaultValue;
		}

		public static PropertyValueSource GetValueSource(DependencyObject dependencyObject, DependencyProperty dependencyProperty)
		{
			return new DependencyPropertyValueInfo(dependencyObject, dependencyProperty).ValueSource;
		}

		public static DependencyProperty ResolveAttachedDependencyProperty(string property, IServiceProvider serviceProvider)
		{
			var typeResolver = (IXamlTypeResolver)serviceProvider?.GetService(typeof(IXamlTypeResolver));

			if (typeResolver == null)
				return null;

			var dotIndex = property.IndexOf(".", StringComparison.Ordinal);

			if (dotIndex == -1)
				return null;

			var startIndex = AdvancePosition(property, 0, 1);

			if (startIndex == -1)
				return null;

			var typeName = property.Substring(startIndex, dotIndex - startIndex);
			var type = typeResolver.Resolve(typeName);

			if (type == null)
				return null;

			dotIndex = AdvancePosition(property, dotIndex + 1, 1);

			if (dotIndex == -1)
				return null;

			var endIndex = AdvancePosition(property, property.Length - 1, -1);

			if (endIndex == -1 || endIndex <= dotIndex)
				return null;

			return DependencyPropertyManager.GetDependencyProperty(property.Substring(dotIndex, endIndex - dotIndex + 1), type);
		}

		internal static DependencyProperty ResolveProperty(DependencyObject actualTarget, DependencyProperty property)
		{
			return actualTarget != null ? ResolveProperty(actualTarget.GetType(), property) : null;
		}

		internal static DependencyProperty ResolveProperty(Type type, DependencyProperty property)
		{
			var resolvedProperty = property;

			var propertyName = DependencyPropertyProxyManager.GetPropertyName(property, type);

			if (propertyName != null)
				resolvedProperty = DependencyPropertyManager.GetDependencyProperty(propertyName, type);

			return resolvedProperty;
		}

		private static bool SkipWhiteSpaceDependencyPropertyName(char ch)
		{
			return ch == '(' || ch == ')' || char.IsWhiteSpace(ch);
		}
	}
}