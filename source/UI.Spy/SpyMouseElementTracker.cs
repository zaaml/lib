// <copyright file="SpyMouseElementTracker.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;

namespace Zaaml.UI.Controls.Spy
{
	public class SpyMouseElementTracker : SpyElementTracker
	{
		protected override void BeginTrackCore()
		{
			MouseInternal.MouseMove += MouseInternalOnMouseMove;

			UpdateElement();
		}

		protected override void EndTrackCore()
		{
			MouseInternal.MouseMove -= MouseInternalOnMouseMove;
		}

		private void MouseInternalOnMouseMove(object sender, MouseEventArgsInt e)
		{
			UpdateElement();
		}

		private void UpdateElement()
		{
			var window = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsMouseOver);

			if (window == null)
				return;

			var position = Mouse.GetPosition(window);

			DependencyObject visualHit = null;

			VisualTreeHelper.HitTest(window, result => result.GetVisualAncestorsAndSelf().OfType<SpyElementAdorner>().Any() ? HitTestFilterBehavior.ContinueSkipSelfAndChildren : HitTestFilterBehavior.Continue, result =>
			{
				visualHit = result.VisualHit;

				return HitTestResultBehavior.Stop;
			}, new PointHitTestParameters(position));

			ElementCore = visualHit as UIElement;
		}
	}
}