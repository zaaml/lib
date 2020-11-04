// <copyright file="MetroDarkDesignTimeTheme.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;

namespace Zaaml.Theming
{
	public sealed class MetroDarkDesignTimeTheme : DesignTimeThemeResourceDictionary
	{
		#region Ctors

		public MetroDarkDesignTimeTheme() : base(MetroDarkTheme.Instance)
		{
		}

		#endregion
	}
}