// <copyright file="CodeLightSkin.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.CodeThemeImplementation;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.Themes
{
	public sealed class CodeLightSkin : ThemeSkin<CodeLightTheme>
	{
		public CodeLightSkin() : base(CodeLightTheme.Instance)
		{
		}
	}
}