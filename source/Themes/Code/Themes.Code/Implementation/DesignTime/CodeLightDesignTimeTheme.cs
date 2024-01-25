// <copyright file="CodeLightDesignTimeTheme.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;

namespace Zaaml.CodeThemeImplementation.DesignTime
{
	public sealed class CodeLightDesignTimeTheme : DesignTimeThemeResourceDictionary
	{
		public CodeLightDesignTimeTheme() : base(CodeLightTheme.Instance)
		{
		}
	}
}