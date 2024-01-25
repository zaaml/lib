// <copyright file="MetroLightSkin.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.MetroThemeImplementation;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.Themes
{
	public sealed class MetroLightSkin : ThemeSkin<MetroLightTheme>
	{
		public MetroLightSkin() : base(MetroLightTheme.Instance)
		{
		}
	}
}