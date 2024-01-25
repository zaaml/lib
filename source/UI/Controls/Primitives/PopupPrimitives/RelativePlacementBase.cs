// <copyright file="RelativePlacementBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	public abstract class RelativePlacementBase : PopupPlacement
	{
		public static readonly DependencyProperty TargetProperty = DPM.Register<FrameworkElement, RelativePlacementBase>
			("Target", p => p.OnTargetChanged);

		public static readonly DependencyProperty ScreenBoundsProperty = DPM.Register<Rect, RelativePlacementBase>
			("ScreenBounds", Rect.Empty, p => p.Invalidate);

		private ScreenBoxObserver _screenBoxObserver;

		internal RelativePlacementBase()
		{
		}

		protected FrameworkElement ActualTarget => Target ?? Popup.GetVisualParent<FrameworkElement>();

		public Rect ScreenBounds
		{
			get => (Rect)GetValue(ScreenBoundsProperty);
			set => SetValue(ScreenBoundsProperty, value);
		}

		protected override Rect ScreenBoundsOverride => ScreenBounds.IsEmpty ? Screen.FromElement(ActualTarget ?? Popup).Bounds : ScreenBounds;

		public FrameworkElement Target
		{
			get => (FrameworkElement)GetValue(TargetProperty);
			set => SetValue(TargetProperty, value);
		}

		protected Rect TargetScreenBox => _screenBoxObserver?.ScreenBox ?? Rect.Empty;

		internal override void OnPopupClosedInternal()
		{
			base.OnPopupClosedInternal();

			_screenBoxObserver = _screenBoxObserver.DisposeExchange();
		}

		internal override void OnPopupOpenedInternal()
		{
			base.OnPopupOpenedInternal();

			var target = ActualTarget;

			if (target == null)
				return;

			_screenBoxObserver = _screenBoxObserver.DisposeExchange(new ScreenBoxObserver(target, OnTargetScreeBoxChanged));
		}

		private void OnTargetChanged(FrameworkElement oldTarget, FrameworkElement newTarget)
		{
			_screenBoxObserver = _screenBoxObserver.DisposeExchange();

			if (IsPopupOpen == false || newTarget == null)
				return;

			_screenBoxObserver = new ScreenBoxObserver(newTarget, OnTargetScreeBoxChanged);

			Invalidate();
		}

		protected virtual void OnTargetScreeBoxChanged()
		{
			if (IsPopupOpen == false)
				return;

			Invalidate();
		}
	}
}