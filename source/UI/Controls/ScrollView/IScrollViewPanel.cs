// <copyright file="IScrollViewPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;

namespace Zaaml.UI.Controls.ScrollView
{
	public interface IScrollViewPanel
	{
		event EventHandler<ScrollInfoChangedEventArgs> ScrollInfoChanged;

		bool CanHorizontallyScroll { get; set; }

		bool CanVerticallyScroll { get; set; }

		Size Extent { get; }

		Vector Offset { get; set; }

		Size Viewport { get; }

		void UpdateScrollInfo();

		void ExecuteScrollCommand(ScrollCommandKind command);
	}

	public interface IDelegateScrollViewPanel
	{
		IScrollViewPanel ScrollViewPanel { get; }
	}
}