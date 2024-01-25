// <copyright file="PopupShadowChrome.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Zaaml.Platform;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Decorators;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	public class PopupShadowChrome : FixedTemplateControl<Panel>
	{
		public static readonly DependencyProperty ShadowSizeProperty = DPM.Register<double, PopupShadowChrome>
			("ShadowSize", s => s.OnShadowSizeChanged);

		public static readonly DependencyProperty ShadowOpacityProperty = DPM.Register<double, PopupShadowChrome>
			("ShadowOpacity", 1.0, s => s.OnShadowOpacityChanged);

		private bool _isOpen;
		private Popup _parentPopup;

		public PopupShadowChrome()
		{
			ShadowChrome = new ShadowChrome
			{
				ShadowSize = 0,
				ShadowOpacity = 1.0
			};

			ShadowWindow = new DecoratorWindow
			{
				Title = "PopupShadowDecorator",
				Child = ShadowChrome
			};

			ShadowChrome.BindProperties(CornerRadiusProperty, this, CornerRadiusProperty);
		}

		private bool IsOpen
		{
			get => _isOpen;
			set
			{
				if (_isOpen == value)
					return;

				_isOpen = value;

				if (_isOpen)
				{
					ParentPopup?.Panel?.InvalidateArrange();

					CompositionTarget.Rendering += CompositionTargetOnRendering;
				}
				else
				{
					Close();

					CompositionTarget.Rendering -= CompositionTargetOnRendering;
				}
			}
		}

		private Popup ParentPopup
		{
			get => _parentPopup;
			set
			{
				if (ReferenceEquals(_parentPopup, value))
					return;

				if (_parentPopup != null)
					DetachParentPopup(_parentPopup);

				_parentPopup = value;

				if (_parentPopup != null)
					AttachParentPopup(value);

				UpdateIsOpen();
				UpdatePosition();
				InvalidateArrange();
			}
		}

		private ShadowChrome ShadowChrome { get; }

		public double ShadowOpacity
		{
			get => (double)GetValue(ShadowOpacityProperty);
			set => SetValue(ShadowOpacityProperty, value);
		}

		public double ShadowSize
		{
			get => (double)GetValue(ShadowSizeProperty);
			set => SetValue(ShadowSizeProperty, value);
		}

		private DecoratorWindow ShadowWindow { get; }

		private void AttachParentPopup(Popup parentPopup)
		{
			parentPopup.IsOpenChanged += ParentPopupOnIsOpenChanged;
			parentPopup.Panel.ArrangedInternal += OnArranged;
		}

		private void Close()
		{
			ShadowWindow.IsOpen = false;
		}

		private void CompositionTargetOnRendering(object sender, EventArgs e)
		{
			EnsureZOrder();
		}

		private void DetachParentPopup(Popup parentPopup)
		{
			parentPopup.IsOpenChanged -= ParentPopupOnIsOpenChanged;
			parentPopup.Panel.ArrangedInternal -= OnArranged;
		}

		private void EnsureZOrder()
		{
			var popupHwndSource = ParentPopup?.HwndSource;
			var decoratorHwndSource = ShadowWindow.HwndSource;

			if (popupHwndSource != null && decoratorHwndSource != null)
			{
				if (NativeMethods.GetWindow(popupHwndSource.Handle, GetWindowCommand.HwndNext) != decoratorHwndSource.Handle)
					ShadowWindow.SetZOrder(popupHwndSource.Handle);
			}
		}

		private void OnArranged(object sender, EventArgs e)
		{
			UpdatePosition();

			if (IsOpen && ShadowWindow.IsOpen == false)
				ShadowWindow.IsOpen = true;
		}

		protected override void OnLoaded()
		{
			base.OnLoaded();

			ParentPopup = this.GetVisualAncestors().OfType<PopupPanel>().FirstOrDefault()?.Popup;
		}

		private void OnShadowOpacityChanged()
		{
			ShadowChrome.ShadowOpacity = ShadowOpacity;
		}

		private void OnShadowSizeChanged()
		{
			ShadowChrome.ShadowSize = ShadowSize;
			UpdatePosition();
			InvalidateArrange();
		}

		protected override void OnUnloaded()
		{
			ParentPopup = null;

			base.OnUnloaded();
		}

		private void Open()
		{
			if (IsOpen)
			{
				UpdatePosition();

				ShadowWindow.IsOpen = true;
			}
		}

		private void ParentPopupOnIsOpenChanged(object sender, EventArgs e)
		{
			UpdateIsOpen();
		}

		private void UpdateIsOpen()
		{
			IsOpen = ParentPopup?.IsOpen ?? false;
		}

		private void UpdatePosition()
		{
			if (ParentPopup == null)
				return;

			var popupPanel = ParentPopup.Panel;
			var actualOffset = popupPanel.ActualOffset;
			var panelActualSize = popupPanel.ActualSize;

			var panelRect = new Rect(new Point(actualOffset.X, actualOffset.Y), panelActualSize);
			var chromeRect = panelRect.GetInflated(popupPanel.CalcInflate().Negate());
			var windowRect = chromeRect.GetInflated(ShadowSize, ShadowSize);

			ShadowChrome.Width = chromeRect.Width;
			ShadowChrome.Height = chromeRect.Height;

			ShadowWindow.Left = windowRect.Left;
			ShadowWindow.Top = windowRect.Top;
			ShadowWindow.Width = windowRect.Width;
			ShadowWindow.Height = windowRect.Height;
		}
	}
}