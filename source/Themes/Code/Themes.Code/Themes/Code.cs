// <copyright file="Code.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Zaaml.CodeThemeImplementation;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Panels.Core;

// ReSharper disable once CheckNamespace
namespace Zaaml.Themes
{
	public static class Code
	{
		static Code()
		{
			RuntimeHelpers.RunClassConstructor(typeof(ControlTemplateRoot).TypeHandle);
		}

		public static IEnumerable<Theme> Collection
		{
			get
			{
				yield return Dark;
				yield return Light;
			}
		}

		public static Theme Dark => CodeDarkTheme.Instance;

		public static Theme Light => CodeLightTheme.Instance;
	}
}