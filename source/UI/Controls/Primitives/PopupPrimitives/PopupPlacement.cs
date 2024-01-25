// <copyright file="PopupPlacement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Snapping;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	public abstract class PopupPlacement : InheritanceContextObject
	{
		private static readonly DependencyPropertyKey PopupPropertyKey = DPM.RegisterReadOnly<Popup, PopupPlacement>
			("Popup", d => d.OnPopupPropertyChangedPrivate);

		public static readonly DependencyProperty PopupProperty = PopupPropertyKey.DependencyProperty;
		public static readonly DependencyProperty AttachedProperty = DPM.RegisterAttached<PopupPlacement, PopupPlacement>("Attached");

		private bool _isPopupOpen;

		internal PopupPlacement()
		{
		}

		protected bool IsPopupOpen
		{
			get => _isPopupOpen;
			private set
			{
				if (_isPopupOpen == value)
					return;

				_isPopupOpen = value;

				if (_isPopupOpen)
					OnPopupOpenedInternal();
				else
					OnPopupClosedInternal();
			}
		}

		public Popup Popup
		{
			get => (Popup)GetValue(PopupProperty);
			private set => this.SetReadOnlyValue(PopupPropertyKey, value);
		}

		internal virtual Rect ScreenBoundsCore => ScreenBoundsOverride;

		protected virtual Rect ScreenBoundsOverride => Screen.FromElement(Popup.Child).Bounds;

		internal virtual bool ShouldConstraint => true;

		internal Rect Arrange(Size desiredSize)
		{
			return Constraint(ArrangeOverride(desiredSize));
		}

		protected abstract Rect ArrangeOverride(Size desiredSize);

		internal void AttachPopup(Popup popup)
		{
			if (popup == null)
				throw new ArgumentNullException(nameof(popup));

			if (ReferenceEquals(Popup, null) == false)
				throw new InvalidOperationException("Popup is already attached.");

			Popup = popup;
			IsPopupOpen = popup.IsOpen;

			OnPopupAttached();
		}

		private Rect Constraint(Rect arrangeOverride)
		{
			return ShouldConstraint ? Snapper.Constraint(ScreenBoundsCore, arrangeOverride, ConvertOptions(Popup?.PlacementOptions ?? PopupPlacementOptions.None)) : arrangeOverride;
		}

		// TODO Implement preview arrange logic
		//internal Rect PreviewArrange(Size desiredSize)
		//{
		//}

		internal static SnapOptions ConvertOptions(PopupPlacementOptions options)
		{
			var snapOptions = SnapOptions.None;

			if ((options & PopupPlacementOptions.Fit) != 0)
				snapOptions |= SnapOptions.Fit;

			if ((options & PopupPlacementOptions.Move) != 0)
				snapOptions |= SnapOptions.Move;

			if ((options & PopupPlacementOptions.Fit) != 0)
				snapOptions |= SnapOptions.Fit;

			return snapOptions;
		}

		internal void DetachPopup(Popup popup)
		{
			if (popup == null)
				throw new ArgumentNullException(nameof(popup));

			if (ReferenceEquals(Popup, popup) == false)
				throw new InvalidOperationException("Popup is not attached.");

			OnPopupDetaching();

			Popup = null;

			IsPopupOpen = false;
		}

		public static PopupPlacement GetAttached(DependencyObject element)
		{
			return (PopupPlacement)element.GetValue(AttachedProperty);
		}

		//protected abstract Rect PreviewArrangeOverride(Size desiredSize);

		protected void Invalidate()
		{
			Popup?.InvalidatePlacement();
		}

		protected virtual void OnPopupAttached()
		{
		}

		protected virtual void OnPopupClosed()
		{
		}

		internal virtual void OnPopupClosedInternal()
		{
			IsPopupOpen = false;

			OnPopupClosed();
		}

		protected virtual void OnPopupDetaching()
		{
		}

		protected virtual void OnPopupOpened()
		{
		}

		internal virtual void OnPopupOpenedInternal()
		{
			IsPopupOpen = true;

			OnPopupOpened();
		}

		private void OnPopupPropertyChangedPrivate(Popup oldValue, Popup newValue)
		{
		}

		public static void SetAttached(DependencyObject element, PopupPlacement value)
		{
			element.SetValue(AttachedProperty, value);
		}
	}
}