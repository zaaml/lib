// <copyright file="HiddenLayout.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;
using System.Windows;

namespace Zaaml.UI.Controls.Docking
{
	public sealed class HiddenLayout : BaseLayout
	{
		static HiddenLayout()
		{
			RegisterLayoutProperties<HiddenLayout>(Enumerable.Empty<DependencyProperty>());
		}

		public override LayoutKind LayoutKind => LayoutKind.Hidden;
	}
}