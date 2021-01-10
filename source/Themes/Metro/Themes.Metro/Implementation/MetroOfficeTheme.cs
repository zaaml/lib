// <copyright file="MetroOfficeTheme.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.MetroThemeImplementation
{
	public sealed class MetroOfficeTheme : SkinnedTheme
	{
		private static readonly Lazy<MetroOfficeTheme> LazyInstance = new Lazy<MetroOfficeTheme>(() => new MetroOfficeTheme());

		private MetroOfficeTheme()
		{
		}

		protected override Theme BaseThemeCore => MetroTheme.Instance;

		public static MetroOfficeTheme Instance => LazyInstance.Value;

		public override string Name => "Metro Office";

		protected override string SkinName => "Office";
	}
}