// <copyright file="MetroThemesExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.MarkupExtensions;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.MetroThemeImplementation
{
	public sealed class MetroThemesExtension : MarkupExtensionBase
	{
		public MetroThemeKind Theme { get; set; } = MetroThemeKind.Office;

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			switch (Theme)
			{
				case MetroThemeKind.Dark:
					return MetroDarkTheme.Instance;
				case MetroThemeKind.Light:
					return MetroLightTheme.Instance;
				case MetroThemeKind.Office:
					return MetroOfficeTheme.Instance;
				default:
					return DefaultTheme.Instance;
			}
		}
	}
}