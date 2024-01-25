// <copyright file="ControlTemplateBuilder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.TemplateCore
{
	internal static class ControlTemplateBuilder
	{
		private static readonly Dictionary<Type, ControlTemplate> TemplatesDictionary = new Dictionary<Type, ControlTemplate>();

		private static ControlTemplate BuildTemplate(Type type)
		{
			var typeNamespace = type.Namespace;
			var typeAssembly = new AssemblyName(type.Assembly.FullName).Name;
			var typeNamespacePrefix = $"xmlns:t='clr-namespace:{typeNamespace};assembly={typeAssembly}'";

			var templateString = $"<ControlTemplate {GenericControlTemplate.XamlNamespaces} {typeNamespacePrefix}><t:{type.Name} x:Name='TemplateRoot'/></ControlTemplate>";

			return XamlUtils.Load<ControlTemplate>(templateString);
		}

		public static ControlTemplate GetTemplate(Type type)
		{
			if (TemplatesDictionary.TryGetValue(type, out var template))
				return template;

			TemplatesDictionary[type] = template = BuildTemplate(type);

			return template;
		}
	}

	internal static class ControlTemplateBuilder<T> where T : FrameworkElement
	{
		public static ControlTemplate Template => ControlTemplateBuilder.GetTemplate(typeof(T));

		public static T GetImplementationRoot(Control control)
		{
			return control.GetImplementationRoot<T>();
		}
	}
}