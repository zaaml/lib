// <copyright file="ValidationHelper.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.ValidationSummary
{
	internal class ValidationHelper
	{
		#region Static Fields and Constants

		internal static readonly DependencyProperty ValidationMetadataProperty = DPM.RegisterAttached<ValidationMetadata, ValidationHelper>
			("ValidationMetadata");

		#endregion

		#region  Methods

		private static Type GetCustomOrCLRType(object instance)
		{
			var customTypeProvider = instance as ICustomTypeProvider;
			if (customTypeProvider != null)
				return customTypeProvider.GetCustomType() ?? instance.GetType();

			return instance.GetType();
		}

		private static PropertyInfo GetProperty(Type entityType, string propertyPath)
		{
			var type = entityType;
			var strArray = propertyPath.Split('.');

			for (var index = 0; index < strArray.Length; ++index)
			{
				var property = type.GetProperty(strArray[index]);
				if (property == null || !property.CanRead)
					return null;

				if (index == strArray.Length - 1)
					return property;

				type = property.PropertyType;
			}

			return null;
		}

		public static ValidationMetadata GetValidationMetadata(DependencyObject inputControl)
		{
			if (inputControl == null)
				throw new ArgumentNullException(nameof(inputControl));

			return inputControl.GetValue(ValidationMetadataProperty) as ValidationMetadata;
		}

		public static ValidationMetadata ParseMetadata(FrameworkElement element, bool forceUpdate, out object entity, out BindingExpression bindingExpression)
		{
			entity = null;
			bindingExpression = null;
			if (element == null)
				return null;
			if (!forceUpdate)
			{
				var validationMetadata = element.GetValue(ValidationMetadataProperty) as ValidationMetadata;
				if (validationMetadata != null)
					return validationMetadata;
			}

			foreach (var field in element.GetType().GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy))
			{
				if (field.FieldType != typeof(DependencyProperty)) continue;

				var bindingExpression1 = element.GetBindingExpression((DependencyProperty) field.GetValue(null));

				if (bindingExpression1?.ParentBinding?.Path == null) continue;

				entity = bindingExpression1.DataItem ?? element.DataContext;

				if (entity == null) continue;

				if (bindingExpression1.ParentBinding.Mode == BindingMode.TwoWay)
				{
					bindingExpression = bindingExpression1;
					break;
				}

				if (bindingExpression == null || string.Compare(bindingExpression1.ParentBinding.Path.Path, bindingExpression.ParentBinding.Path.Path, StringComparison.Ordinal) < 0)
					bindingExpression = bindingExpression1;
			}

			if (bindingExpression == null)
				return null;

			var metadata = ParseMetadata(bindingExpression.ParentBinding.Path.Path, entity);
			element.SetValue(ValidationMetadataProperty, metadata);
			return metadata;
		}

		public static ValidationMetadata ParseMetadata(string bindingPath, object entity)
		{
			if (entity == null || string.IsNullOrEmpty(bindingPath)) return null;

			var property = GetProperty(GetCustomOrCLRType(entity), bindingPath);
			if (property == null) return null;

			var validationMetadata = new ValidationMetadata();
			foreach (var customAttribute in property.GetCustomAttributes(false))
			{
				if (customAttribute is RequiredAttribute)
					validationMetadata.IsRequired = true;
				else
				{
					var displayAttribute = customAttribute as DisplayAttribute;
					if (displayAttribute == null) continue;

					validationMetadata.Description = displayAttribute.GetDescription();
					validationMetadata.Caption = displayAttribute.GetName();
				}
			}

			if (validationMetadata.Caption == null)
				validationMetadata.Caption = property.Name;

			return validationMetadata;
		}

		public static void SetValidationMetadata(DependencyObject inputControl, ValidationMetadata value)
		{
			if (inputControl == null)
				throw new ArgumentNullException(nameof(inputControl));

			inputControl.SetValue(ValidationMetadataProperty, value);
		}

		#endregion
	}
}
