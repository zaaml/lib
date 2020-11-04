// <copyright file="SkinResourceExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Reflection;
using System.Windows;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore.MarkupExtensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Theming
{
	public sealed class SkinResourceExtension : MarkupExtensionBase
	{
		#region Properties

		public string Key { get; set; }

		#endregion

		#region  Methods

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			object target;
			object targetProperty;
			bool reflected;

			GetTarget(serviceProvider, out target, out targetProperty, out reflected);

			var targetProxy = target as ISkinResourceKey;

			if (targetProxy != null)
			{
				var propertyName = (targetProperty as PropertyInfo)?.Name;

				if (propertyName == null)
					return null;

				targetProxy.SetResourceKey(propertyName, Key);
			}

			var propertyInfo = targetProperty as PropertyInfo;

			if (propertyInfo != null)
				return RuntimeUtils.CreateDefaultValue(propertyInfo.PropertyType);

			var dependencyProperty = targetProperty as DependencyProperty;

			if (dependencyProperty != null)
				return RuntimeUtils.CreateDefaultValue(dependencyProperty.GetPropertyType());

			return null;
		}

		#endregion
	}

	public interface ISkinResourceKey
	{
		#region  Methods

		void SetResourceKey(string propertyName, string key);

		#endregion
	}
}