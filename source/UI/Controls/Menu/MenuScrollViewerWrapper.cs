// <copyright file="MenuScrollViewerWrapper.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;

#if SILVERLIGHT
using Zaaml.UI.Extensions;
#endif

namespace Zaaml.UI.Controls.Menu
{
	internal struct MenuScrollViewerWrapper
	{
		public MenuScrollViewerWrapper(FrameworkElement element)
		{
			_element = element;
		}

		private readonly FrameworkElement _element;

		public void ScrollToTop()
		{
			var nativeScroll = _element as ScrollViewer;
			var menuScroll = _element as MenuScrollViewer;

			nativeScroll?.ScrollToTop();
			menuScroll?.ResetScroll();
		}
	}
}