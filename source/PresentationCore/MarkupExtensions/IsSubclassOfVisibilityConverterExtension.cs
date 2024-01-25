// <copyright file="IsSubclassOfVisibilityConverterExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore.Converters;

namespace Zaaml.PresentationCore.MarkupExtensions
{
	public sealed class IsSubclassOfVisibilityConverterExtension : MarkupExtensionBase
	{
		public Visibility FalseVisibility { get; set; } = Visibility.Collapsed;

		public bool Self { get; set; } = true;

		public Visibility TrueVisibility { get; set; } = Visibility.Visible;

		public Type Type { get; set; }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return new IsSubclassOfVisibilityConverter
			{
				Self = Self,
				Type = Type,
				TrueVisibility = TrueVisibility,
				FalseVisibility = FalseVisibility
			};
		}
	}
}