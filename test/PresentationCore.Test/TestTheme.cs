// <copyright file="TestTheme.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;

namespace Zaaml.PresentationCore.Test
{
	public sealed class TestTheme : SkinnedTheme
	{
		public static readonly TestTheme Instance = new TestTheme();

		private TestTheme()
		{
			AddThemeResourceInternal(new ThemeResource { Key = "ThemeResource1", Value = 1 });
			AddThemeResourceInternal(new ThemeResource { Key = "ThemeResource2", Value = 2 });
			AddThemeResourceInternal(new ThemeResource { Key = "ThemeResource3", Value = 3 });
			AddThemeResourceInternal(new ThemeResource { Key = "ThemeResource4", Value = 4 });
		}

		protected override Theme BaseThemeCore { get; } = new TestBaseTheme();

		public override string Name => "Test";

		protected override string SkinName => "Default";

		private class TestBaseTheme : Theme
		{
			public override string Name => "Base";
		}
	}
}