// <copyright file="ThemeAssemblyAttribute.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore.Theming
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class ThemeAssemblyAttribute : Attribute
	{
		#region Ctors

		public ThemeAssemblyAttribute(Type themeType, string source)
		{
			ThemeType = themeType;
			Source = source;
		}

		public ThemeAssemblyAttribute(Type themeType)
		{
			ThemeType = themeType;
			Source = string.Empty;
		}

		#endregion

		#region Properties

		public string Source { get; }

		public Type ThemeType { get; }

		#endregion
	}
}