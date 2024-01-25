// <copyright file="MetroDarkSkin.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.MetroThemeImplementation;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.Themes
{
	public sealed class MetroDarkSkin : ThemeSkin<MetroDarkTheme>
	{
		public MetroDarkSkin() : base(MetroDarkTheme.Instance)
		{
		}
	}
}