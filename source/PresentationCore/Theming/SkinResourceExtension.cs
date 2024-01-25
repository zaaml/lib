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
		public string Key { get; set; }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			GetTarget(serviceProvider, out var target, out var targetProperty, out _);

			if (target is ISkinResourceKey targetProxy)
			{
				var propertyName = (targetProperty as PropertyInfo)?.Name;

				if (propertyName == null)
					return null;

				targetProxy.SetResourceKey(propertyName, Key);
			}

			var propertyInfo = targetProperty as PropertyInfo;

			if (propertyInfo != null)
				return RuntimeUtils.CreateDefaultValue(propertyInfo.PropertyType);

			if (targetProperty is DependencyProperty dependencyProperty)
				return RuntimeUtils.CreateDefaultValue(dependencyProperty.GetPropertyType());

			return null;
		}
	}

	public interface ISkinResourceKey
	{
		void SetResourceKey(string propertyName, string key);
	}
}