// <copyright file="CodeTheme.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.CodeThemeImplementation
{
	public sealed class CodeTheme : Theme
	{
		private static readonly Lazy<CodeTheme> LazyInstance = new(() => new CodeTheme());

		private CodeTheme()
		{
		}

		public static CodeTheme Instance => LazyInstance.Value;

		public override string Name => "Code";

		protected internal override void BindThemeResource(ThemeResourceReference themeResourceReference)
		{
		}

		protected internal override IEnumerable<ThemeResource> EnumerateResources()
		{
			return Enumerable.Empty<ThemeResource>();
		}

		protected internal override ThemeResource GetResource(string key)
		{
			return null;
		}
	}
}