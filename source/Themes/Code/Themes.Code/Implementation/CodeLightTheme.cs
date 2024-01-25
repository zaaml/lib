// <copyright file="CodeLightTheme.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.CodeThemeImplementation
{
	public sealed class CodeLightTheme : SkinnedTheme
	{
		private static readonly Lazy<CodeLightTheme> LazyInstance = new(() => new CodeLightTheme());

		private CodeLightTheme()
		{
		}

		protected override Theme BaseThemeCore => CodeTheme.Instance;

		public static CodeLightTheme Instance => LazyInstance.Value;

		public override string Name => "Code Light";

		protected override string SkinName => "Light";
	}
}