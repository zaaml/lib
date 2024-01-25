// <copyright file="HitTestUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Zaaml.PresentationCore.Utils
{
	internal static class HitTestUtils
	{
		internal static List<UIElement> CursorHitTest(this FrameworkElement relative)
		{
			var uiElements = new List<UIElement>();
			var position = Mouse.GetPosition(relative);

			VisualTreeHelper.HitTest(relative, r =>
			{
				if (r is UIElement uie)
					uiElements.Add(uie);

				return HitTestFilterBehavior.Continue;
			}, result => HitTestResultBehavior.Continue, new PointHitTestParameters(position));

			return uiElements;
		}

		public static IEnumerable<UIElement> ScreenDeviceHitTest(this FrameworkElement reference, Point screenDevicePoint)
		{
			var uiElements = new List<UIElement>();

			VisualTreeHelper.HitTest(reference, r =>
			{
				if (r is UIElement uie)
					uiElements.Add(uie);

				return HitTestFilterBehavior.Continue;
			}, result => HitTestResultBehavior.Continue, new PointHitTestParameters(reference.PointFromScreen(screenDevicePoint)));

			return uiElements.AsEnumerable().Reverse();
		}

		public static IEnumerable<UIElement> ScreenDeviceHitTest(Point screenDevicePoint)
		{
			var uiElements = new List<UIElement>();

			foreach (var visualRoot in PresentationTreeUtils.EnumerateVisualRoots().OfType<FrameworkElement>().Where(fre => fre.Dispatcher.CheckAccess()))
			{
				VisualTreeHelper.HitTest(visualRoot, r =>
				{
					if (r is UIElement uie)
						uiElements.Add(uie);

					return HitTestFilterBehavior.Continue;
				}, result => HitTestResultBehavior.Continue, new PointHitTestParameters(visualRoot.PointFromScreen(screenDevicePoint)));
			}

			return uiElements.AsEnumerable().Reverse();
		}

		public static IEnumerable<UIElement> ScreenDeviceHitTest(this FrameworkElement reference, Rect screenDeviceRect)
		{
			var uiElements = new List<UIElement>();

			VisualTreeHelper.HitTest(reference, r =>
			{
				if (r is UIElement uie)
					uiElements.Add(uie);

				return HitTestFilterBehavior.Continue;
			}, result => HitTestResultBehavior.Continue, new GeometryHitTestParameters(new RectangleGeometry(reference.TransformScreenDeviceRectToClient(screenDeviceRect))));

			return uiElements.AsEnumerable().Reverse();
		}

		public static IEnumerable<UIElement> ScreenDeviceHitTest(Rect screenDeviceRect)
		{
			var uiElements = new List<UIElement>();

			foreach (var visualRoot in PresentationTreeUtils.EnumerateVisualRoots().OfType<FrameworkElement>().Where(fre => fre.Dispatcher.CheckAccess()))
			{
				if (visualRoot.Dispatcher.CheckAccess() == false)
					continue;

				VisualTreeHelper.HitTest(visualRoot, r =>
				{
					if (r is UIElement uie)
						uiElements.Add(uie);

					return HitTestFilterBehavior.Continue;
				}, result => HitTestResultBehavior.Continue, new GeometryHitTestParameters(new RectangleGeometry(visualRoot.TransformScreenDeviceRectToClient(screenDeviceRect))));
			}

			return uiElements.AsEnumerable().Reverse();
		}
	}
}