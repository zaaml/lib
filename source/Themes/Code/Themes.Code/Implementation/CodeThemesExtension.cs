// <copyright file="CodeThemesExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.MarkupExtensions;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.CodeThemeImplementation
{
	public sealed class CodeThemesExtension : MarkupExtensionBase
	{
		public CodeThemeKind Theme { get; set; } = CodeThemeKind.Light;

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			switch (Theme)
			{
				case CodeThemeKind.Dark:
					return CodeDarkTheme.Instance;
				case CodeThemeKind.Light:
					return CodeLightTheme.Instance;
				default:
					return DefaultTheme.Instance;
			}
		}
	}
}