// <copyright file="FullLayout.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Zaaml.UI.Controls.Docking
{
	internal static class FullLayout
	{
		static FullLayout()
		{
			LayoutTypes = new[]
			{
				typeof(FloatLayout),
				typeof(DockLayout),
				typeof(DocumentLayout),
				typeof(AutoHideLayout),
				typeof(TabLayout),
				typeof(SplitLayout),
				typeof(HiddenLayout)
			};

			foreach (var layoutType in LayoutTypes)
				RuntimeHelpers.RunClassConstructor(layoutType.TypeHandle);

			LayoutProperties = new List<DependencyProperty>();
			LayoutProperties.AddRange(GetLayoutProperties(LayoutKind.Float));
			LayoutProperties.AddRange(GetLayoutProperties(LayoutKind.Dock));
			LayoutProperties.AddRange(GetLayoutProperties(LayoutKind.Document));
			LayoutProperties.AddRange(GetLayoutProperties(LayoutKind.AutoHide));
			LayoutProperties.AddRange(GetLayoutProperties(LayoutKind.Tab));
			LayoutProperties.AddRange(GetLayoutProperties(LayoutKind.Split));
			LayoutProperties.AddRange(GetLayoutProperties(LayoutKind.Hidden));
		}

		public static List<DependencyProperty> LayoutProperties { get; }

		internal static Type[] LayoutTypes { get; }

		internal static IEnumerable<LayoutKind> EnumerateLayoutKinds()
		{
			yield return LayoutKind.Float;
			yield return LayoutKind.Dock;
			yield return LayoutKind.Document;
			yield return LayoutKind.AutoHide;
			yield return LayoutKind.Tab;
			yield return LayoutKind.Split;
			yield return LayoutKind.Hidden;
		}

		public static IEnumerable<DependencyProperty> GetLayoutProperties(LayoutKind layoutKind)
		{
			return BaseLayout.GetLayoutProperties(GetLayoutType(layoutKind));
		}

		internal static Type GetLayoutType(LayoutKind layoutKind)
		{
			switch (layoutKind)
			{
				case LayoutKind.Float:

					return typeof(FloatLayout);

				case LayoutKind.Dock:

					return typeof(DockLayout);

				case LayoutKind.Tab:

					return typeof(TabLayout);

				case LayoutKind.Split:

					return typeof(SplitLayout);

				case LayoutKind.Document:

					return typeof(DocumentLayout);

				case LayoutKind.AutoHide:

					return typeof(AutoHideLayout);

				case LayoutKind.Hidden:

					return typeof(HiddenLayout);

				default:

					throw new ArgumentOutOfRangeException(nameof(layoutKind));
			}
		}
	}
}