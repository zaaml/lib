// <copyright file="Metro.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Zaaml.MetroThemeImplementation;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Panels.Core;

// ReSharper disable once CheckNamespace
namespace Zaaml.Themes
{
	public static class Metro
	{
		static Metro()
		{
			RuntimeHelpers.RunClassConstructor(typeof(ControlTemplateRoot).TypeHandle);
		}

		public static IEnumerable<Theme> Collection
		{
			get
			{
				yield return Dark;
				yield return Light;
				yield return Office;
			}
		}

		public static Theme Dark => MetroDarkTheme.Instance;

		public static Theme Light => MetroLightTheme.Instance;

		public static Theme Office => MetroOfficeTheme.Instance;
	}
}