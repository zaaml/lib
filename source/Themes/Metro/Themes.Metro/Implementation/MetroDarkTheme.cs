// <copyright file="MetroDarkTheme.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.MetroThemeImplementation
{
	public sealed class MetroDarkTheme : SkinnedTheme
	{
		private static readonly Lazy<MetroDarkTheme> LazyInstance = new Lazy<MetroDarkTheme>(() => new MetroDarkTheme());

		private MetroDarkTheme()
		{
		}

		protected override Theme BaseThemeCore => MetroTheme.Instance;

		public static MetroDarkTheme Instance => LazyInstance.Value;

		public override string Name => "Metro Dark";

		protected override string SkinName => "Dark";
	}
}