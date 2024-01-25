// <copyright file="PathBindingBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows;
using NativeBinding = System.Windows.Data.Binding;

namespace Zaaml.PresentationCore.Data.MarkupExtensions
{
	public abstract class PathBindingBase : BindingBaseExtension
	{
		private static readonly PropertyPathConverter PathConverter = new();

		public object Path { get; set; }

		private PropertyPath EvaluatePropertyPath(IServiceProvider serviceProvider)
		{
			if (Path is PropertyPath propertyPath)
				return propertyPath;

			var dpPath = Path as DependencyProperty;

			if (Path is string stringPath)
			{
				using var typeDescriptorContext = TypeDescriptorContext.FromServiceProvider(serviceProvider);

				return (PropertyPath)PathConverter.ConvertFrom(typeDescriptorContext, CultureInfo.CurrentCulture, stringPath);
			}

			return dpPath != null ? new PropertyPath(dpPath) : null;
		}

		protected override void FinalizeXamlInitializationCore(IServiceProvider serviceProvider)
		{
			base.FinalizeXamlInitializationCore(serviceProvider);

			Path = EvaluatePropertyPath(serviceProvider);
		}

		protected override NativeBinding GetBindingCore(IServiceProvider serviceProvider)
		{
			var actualPath = EvaluatePropertyPath(serviceProvider);

			var binding = actualPath != null ? new NativeBinding { Path = actualPath } : new NativeBinding { BindsDirectlyToSource = true };

			InitSource(binding);
			InitBinding(binding);

			return binding;
		}

		protected abstract void InitSource(NativeBinding binding);
	}
}