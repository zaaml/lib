// <copyright file="MetroLightTheme.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.MetroThemeImplementation
{
	public sealed class MetroLightTheme : SkinnedTheme
	{
		private static readonly Lazy<MetroLightTheme> LazyInstance = new Lazy<MetroLightTheme>(() => new MetroLightTheme());

		private MetroLightTheme()
		{
		}

		protected override Theme BaseThemeCore => MetroTheme.Instance;

		public static MetroLightTheme Instance => LazyInstance.Value;

		public override string Name => "Metro Light";

		protected override string SkinName => "Light";
	}
}