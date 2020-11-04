// <copyright file="PropertyResolver.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Interactivity
{
	internal static class PropertyResolver
	{
		#region  Methods

		public static void CopyFrom(IPropertySubject target, IPropertySubject source)
		{
			var propertyKind = source.PropertyKind & PropertyKind.Unspecified;

			switch (propertyKind)
			{
				case PropertyKind.Expando:
					SetExpandoProperty(target, GetExpandoProperty(source));
					break;
				case PropertyKind.Explicit:
					SetProperty(target, GetProperty(source));
					break;
				case PropertyKind.Implicit:
					SetProperty(target, GetProperty(source));
					break;
			}
		}

		public static string GetExpandoProperty(IPropertySubject interactivityObject)
		{
			var propertyKind = interactivityObject.PropertyKind;
			var isResolved = IsResolved(interactivityObject);
			propertyKind &= PropertyKind.Unspecified;

			if (propertyKind == PropertyKind.Unspecified || propertyKind != PropertyKind.Expando)
				return null;

			return isResolved ? ((DependencyProperty) interactivityObject.PropertyStore).GetName() : (string) interactivityObject.PropertyStore;
		}

		public static object GetProperty(IPropertySubject interactivityObject)
		{
			var propertyKind = interactivityObject.PropertyKind;

			propertyKind &= PropertyKind.Unspecified;

			var property = interactivityObject.PropertyStore;

			switch (propertyKind)
			{
				case PropertyKind.Unspecified:
					return null;
				case PropertyKind.Explicit:
					return (DependencyProperty) property;
				case PropertyKind.Implicit:
					return property as string ?? ((DependencyProperty) property).GetName();
			}

			return null;
		}

		public static DependencyProperty GetServiceProperty(IPropertySubject interactivityObject)
		{
			var propertyKind = interactivityObject.PropertyKind;
			var isResolved = (propertyKind & PropertyKind.Resolved) != 0;

			propertyKind &= PropertyKind.Unspecified;

			if (propertyKind == PropertyKind.Unspecified)
				return null;

			if (propertyKind == PropertyKind.Explicit)
				return (DependencyProperty) interactivityObject.PropertyStore;

			if (propertyKind == PropertyKind.Expando)
			{
				if (isResolved)
					return (DependencyProperty) interactivityObject.PropertyStore;

				var resolvedProperty = DependencyPropertyManager.GetExpandoProperty((string) interactivityObject.PropertyStore);
				interactivityObject.PropertyStore = resolvedProperty;
				interactivityObject.PropertyKind |= PropertyKind.Resolved;
				return resolvedProperty;
			}

			var propertyName = interactivityObject.PropertyStore as string ?? ((DependencyProperty) interactivityObject.PropertyStore).GetName();
			return DependencyPropertyProxyManager.GetDependencyProperty(propertyName);
		}

		public static bool IsResolved(IPropertySubject target)
		{
			return (target.PropertyKind & PropertyKind.Resolved) != 0;
		}

		public static bool IsSpecified(IPropertySubject target)
		{
			return (target.PropertyKind & PropertyKind.Unspecified) != PropertyKind.Unspecified;
		}

		public static DependencyProperty ResolveProperty(IPropertySubject interactivityObject, Type targetType)
		{
			var propertyKind = interactivityObject.PropertyKind & PropertyKind.Unspecified;

			switch (propertyKind)
			{
				case PropertyKind.Explicit:
					return (DependencyProperty) GetProperty(interactivityObject);
				case PropertyKind.Implicit:
					return DependencyPropertyManager.GetDependencyProperty(GetProperty(interactivityObject) as string, targetType);
				case PropertyKind.Expando:
					return DependencyPropertyManager.GetExpandoProperty(GetExpandoProperty(interactivityObject));
			}

			return null;
		}

		public static DependencyProperty ResolveProperty(IPropertySubject interactivityObject)
		{
			if (((InteractivityObject)interactivityObject).IsLoaded == false)
				return null;

			var propertyKind = interactivityObject.PropertyKind;
			var isResolved = IsResolved(interactivityObject);

			if (isResolved)
				return interactivityObject.PropertyStore as DependencyProperty;

			switch (propertyKind)
			{
				case PropertyKind.Explicit:
					interactivityObject.PropertyKind |= PropertyKind.Resolved;
					return (DependencyProperty)interactivityObject.PropertyStore;
				case PropertyKind.Implicit:
					{
						var subject = interactivityObject.ActualSubject;
						if (subject == null)
							return null;

						var resolvedProperty = DependencyPropertyManager.GetDependencyProperty((string)interactivityObject.PropertyStore, subject.GetType());
						if (resolvedProperty != null)
							interactivityObject.PropertyStore = resolvedProperty;

						interactivityObject.PropertyKind |= PropertyKind.Resolved;
						return resolvedProperty;
					}
				case PropertyKind.Expando:
					{
						var resolvedProperty = DependencyPropertyManager.GetExpandoProperty((string)interactivityObject.PropertyStore);
						interactivityObject.PropertyStore = resolvedProperty;
						interactivityObject.PropertyKind |= PropertyKind.Resolved;
						return resolvedProperty;
					}
			}

			return null;
		}

		public static void SetExpandoProperty(IPropertySubject interactivityObject, string property)
		{
			if (ReferenceEquals(interactivityObject.PropertyStore, property))
				return;

			UnresolveProperty(interactivityObject);

			interactivityObject.PropertyStore = property;
			interactivityObject.PropertyKind = PropertyKind.Expando;
		}

		public static void SetProperty(IPropertySubject interactivityObject, object property)
		{
			if (ReferenceEquals(interactivityObject.PropertyStore, property))
				return;

			UnresolveProperty(interactivityObject);

			var dependencyProperty = property as DependencyProperty;
			if (dependencyProperty != null)
			{
				interactivityObject.PropertyStore = dependencyProperty;
				interactivityObject.PropertyKind = PropertyKind.Explicit;
			}

			var stringProperty = property as string;
			if (stringProperty != null)
			{
				interactivityObject.PropertyStore = stringProperty;
				interactivityObject.PropertyKind = PropertyKind.Implicit;
			}
		}

		public static void UnresolveProperty(IPropertySubject interactivityObject)
		{
			if (IsResolved(interactivityObject) == false || IsSpecified(interactivityObject) == false)
				return;

			var propertyKind = interactivityObject.PropertyKind & PropertyKind.Unspecified;

			switch (propertyKind)
			{
				case PropertyKind.Implicit:
					interactivityObject.PropertyStore = GetProperty(interactivityObject);
					break;
				case PropertyKind.Expando:
					interactivityObject.PropertyStore = GetExpandoProperty(interactivityObject);
					break;
			}

			interactivityObject.PropertyKind &= PropertyKind.Unspecified;
		}

		#endregion
	}

	[Flags]
	internal enum PropertyKind
	{
		Unspecified = Implicit | Expando,

		Explicit = 0,
		Implicit = 1,
		Expando = 2,

		Resolved = 4
	}

	internal interface IPropertySubject : IInteractivitySubject
	{
		#region Properties

		DependencyObject ActualSubject { get; }
		PropertyKind PropertyKind { get; set; }

		object PropertyStore { get; set; }

		#endregion

		#region  Methods

		void OnPropertyChanged(DependencyProperty oldProperty, DependencyProperty newProperty);

		#endregion
	}
}