// <copyright file="MetroOfficeSkin.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.MetroThemeImplementation;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.Themes
{
	public sealed class MetroOfficeSkin : ThemeSkin<MetroOfficeTheme>
	{
		public MetroOfficeSkin() : base(MetroOfficeTheme.Instance)
		{
		}
	}
}