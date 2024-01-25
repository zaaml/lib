// <copyright file="CodeDarkTheme.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.CodeThemeImplementation
{
	public sealed class CodeDarkTheme : SkinnedTheme
	{
		private static readonly Lazy<CodeDarkTheme> LazyInstance = new(() => new CodeDarkTheme());

		private CodeDarkTheme()
		{
		}

		protected override Theme BaseThemeCore => CodeTheme.Instance;

		public static CodeDarkTheme Instance => LazyInstance.Value;

		public override string Name => "Code Dark";

		protected override string SkinName => "Dark";
	}
}