// <copyright file="TemplateContractBinder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.Core.Reflection;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.TemplateCore
{
	internal class TemplateContractBinder
	{
		private static readonly Dictionary<Type, TemplateContractInfo> TemplateContractInfos = new Dictionary<Type, TemplateContractInfo>();

		private readonly GetTemplateChild _templateChildProvider;

		public TemplateContractBinder(FrameworkElement frameworkElement)
		{
			_templateChildProvider = new TemplateDiscovery(frameworkElement).GetChild;
		}

		public TemplateContractBinder(GetTemplateChild templateChildProvider)
		{
			_templateChildProvider = templateChildProvider;
		}

		public void Attach(TemplateContract templateContract)
		{
			if (templateContract == null)
				throw new ArgumentNullException(nameof(templateContract));

			var type = templateContract.GetType();
			TemplateContractInfos.GetValueOrCreate(type, () => new TemplateContractInfo(type)).Bind(templateContract, _templateChildProvider);
		}

		public void Detach(TemplateContract templateContract)
		{
			TemplateContractInfos.GetValueOrDefault(templateContract.GetType())?.Unbind(templateContract);
		}

		private class TemplateContractInfo
		{
			private readonly List<TemplatePartDescriptionInfo> _partDescriptions = new List<TemplatePartDescriptionInfo>();

			public TemplateContractInfo(Type type)
			{
				foreach (var propertyInfo in type.GetProperties().Where(p => p.HasAttribute<TemplateContractPartAttribute>(false)))
					_partDescriptions.Add(new TemplatePartDescriptionInfo(propertyInfo));

				foreach (var fieldInfo in type.GetFields().Where(p => p.HasAttribute<TemplateContractPartAttribute>(false)))
					_partDescriptions.Add(new TemplatePartDescriptionInfo(fieldInfo));
			}

			public void Bind(TemplateContract templateContract, GetTemplateChild templateChildProvider)
			{
				foreach (var partDescription in _partDescriptions)
				{
					var templatePart = templateChildProvider(partDescription.Name);

					if (templatePart == null)
					{
						if (partDescription.Required)
							throw new TemplateValidationException(partDescription.Name);

						continue;
					}

					if (partDescription.PartType.IsInstanceOfType(templatePart))
						partDescription.Setter(templateContract, templatePart);
				}
			}

			public void Unbind(TemplateContract templateContract)
			{
				foreach (var partDescription in _partDescriptions)
					partDescription.Setter(templateContract, null);
			}
		}

		private class TemplateDiscovery
		{
			private readonly Dictionary<string, FrameworkElement> _childrenMap;

			public TemplateDiscovery(FrameworkElement frameworkElement)
			{
				_childrenMap = new Dictionary<string, FrameworkElement>();

				foreach (var fre in frameworkElement.GetVisualDescendants().OfType<FrameworkElement>().Where(d => !string.IsNullOrEmpty(d.Name)))
				{
					if (_childrenMap.ContainsKey(fre.Name) == false)
						_childrenMap.Add(fre.Name, fre);
				}

				//_childrenMap = frameworkElement.VisualDescendants()
				//  .OfType<FrameworkElement>()
				//  .Where(d => !string.IsNullOrEmpty(d.Name))
				//  .ToDictionary(d => d.Name, d => d);
			}

			public DependencyObject GetChild(string name)
			{
				return _childrenMap.GetValueOrDefault(name);
			}
		}

		private class TemplatePartDescriptionInfo
		{
			public TemplatePartDescriptionInfo(PropertyInfo propertyInfo)
			{
				var declaringPI = propertyInfo.TransformToDeclaringType();
				var partAttribute = declaringPI.GetAttribute<TemplateContractPartAttribute>();

				Name = partAttribute.Name ?? propertyInfo.Name;
				Required = partAttribute.Required;
				PartType = propertyInfo.PropertyType;
				Setter = AccessorFactory.CreatePropertySetter<object, object>(declaringPI);
			}

			public TemplatePartDescriptionInfo(FieldInfo fieldInfo)
			{
				var declaringFI = fieldInfo.TransformToDeclaringType();
				var partAttribute = declaringFI.GetAttribute<TemplateContractPartAttribute>();

				Name = partAttribute.Name ?? fieldInfo.Name;
				Required = partAttribute.Required;
				PartType = fieldInfo.FieldType;
				Setter = AccessorFactory.CreateFieldSetter<object, object>(declaringFI);
			}

			public string Name { get; }
			public Type PartType { get; }
			public bool Required { get; }
			public ValueSetter<object, object> Setter { get; }
		}
	}
}