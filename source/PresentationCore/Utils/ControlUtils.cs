// <copyright file="ControlUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.Core.Runtime;

namespace Zaaml.PresentationCore.Utils
{
	internal static class ControlUtils
	{
		public static void OverrideIsTabStop<TControl>(bool isTabStop) where TControl : Control
		{
			KeyboardNavigation.IsTabStopProperty.OverrideMetadata(typeof(TControl), new FrameworkPropertyMetadata(isTabStop ? BooleanBoxes.True : BooleanBoxes.False));
		}
	}
}