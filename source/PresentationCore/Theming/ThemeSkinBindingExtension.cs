// <copyright file="ThemeSkinBindingExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Data;
using Zaaml.PresentationCore.MarkupExtensions;

namespace Zaaml.PresentationCore.Theming
{
	public sealed class ThemeSkinBindingExtension : MarkupExtensionBase
	{
		public ThemeSkinConverter SkinConverter { get; set; }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			var binding = new Binding(nameof(ThemeManagerInstance.ApplicationTheme))
			{
				Source = ThemeManager.Instance,
				Converter = SkinConverter
			};

			return binding.ProvideValue(serviceProvider);
		}
	}
}