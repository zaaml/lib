// <copyright file="DebugFrameworkElement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Diagnostics
{
#if DEBUG && DEV
	public static class DebugFrameworkElement
	{
		#region  Methods

		public static string Attach(FrameworkElement element)
		{
			return element?.ToString() ?? "null";
		}

		#endregion
	}
#endif
}