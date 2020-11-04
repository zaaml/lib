﻿// <copyright file="MetroLightDesignTimeTheme.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;

namespace Zaaml.Theming
{
	public sealed class MetroLightDesignTimeTheme : DesignTimeThemeResourceDictionary
	{
		#region Ctors

		public MetroLightDesignTimeTheme() : base(MetroLightTheme.Instance)
		{
		}

		#endregion
	}
}