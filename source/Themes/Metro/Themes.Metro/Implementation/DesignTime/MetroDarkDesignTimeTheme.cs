// <copyright file="MetroDarkDesignTimeTheme.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;

namespace Zaaml.MetroThemeImplementation.DesignTime
{
	public sealed class MetroDarkDesignTimeTheme : DesignTimeThemeResourceDictionary
	{
		public MetroDarkDesignTimeTheme() : base(MetroDarkTheme.Instance)
		{
		}
	}
}