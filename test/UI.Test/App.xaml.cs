// <copyright file="App.xaml.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.MetroThemeImplementation;
using Zaaml.PresentationCore.Theming;
using Zaaml.Themes;

namespace Zaaml.UI.Test
{
	public partial class App
	{
		public App()
		{
			ThemeManager.LoadThemePart<MetroThemePart>();
			ThemeManager.LoadThemePart<Themes.UI.MetroThemePart>();
			ThemeManager.ApplicationTheme = Metro.Office;
		}
	}
}