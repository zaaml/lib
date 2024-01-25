// <copyright file="SpyUtil.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;
using System.Windows;
using System.Windows.Media;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.Spy
{
	internal static class SpyUtil
	{
		public static UIElement GetWindowElement(Window window, Point position)
		{
			var visualHit = WindowHitTest(window, position);

			return visualHit?.GetVisualAncestorsAndSelf().OfType<UIElement>().FirstOrDefault();
		}

		public static DependencyObject WindowHitTest(Window window, Point position)
		{
			DependencyObject visualHit = null;

			VisualTreeHelper.HitTest(window, result => result.GetVisualAncestorsAndSelf().OfType<SpyElementAdorner>().Any() ? HitTestFilterBehavior.ContinueSkipSelfAndChildren : HitTestFilterBehavior.Continue, result =>
			{
				visualHit = result.VisualHit;

				return HitTestResultBehavior.Stop;
			}, new PointHitTestParameters(position));

			return visualHit;
		}
	}
}