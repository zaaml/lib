// <copyright file="PopupShadowChrome.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
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

			ShadowWindow.Child = ShadowChrome;
		}

		private bool IsOpen
		{
			set
			{
				if (_isOpen == value)
					return;

				_isOpen = value;

				if (_isOpen)
				{
					UpdatePosition();

					ShadowWindow.IsOpen = true;
				}
				else
					ShadowWindow.IsOpen = false;
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
			get => (double) GetValue(ShadowOpacityProperty);
			set => SetValue(ShadowOpacityProperty, value);
		}

		public double ShadowSize
		{
			get => (double) GetValue(ShadowSizeProperty);
			set => SetValue(ShadowSizeProperty, value);
		}

		private DecoratorWindow ShadowWindow { get; } = new DecoratorWindow();

		private void AttachParentPopup(Popup parentPopup)
		{
			parentPopup.IsOpenChanged += ParentPopupOnIsOpenChanged;
			parentPopup.Panel.ArrangedInternal += OnArranged;
		}

		private void DetachParentPopup(Popup parentPopup)
		{
			parentPopup.IsOpenChanged -= ParentPopupOnIsOpenChanged;
			parentPopup.Panel.ArrangedInternal -= OnArranged;
		}

		private void OnArranged(object sender, EventArgs e)
		{
			UpdatePosition();
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

			ShadowChrome.Width = panelActualSize.Width;
			ShadowChrome.Height = panelActualSize.Height;

			ShadowWindow.Left = actualOffset.X - ShadowSize;
			ShadowWindow.Top = actualOffset.Y - ShadowSize;
			ShadowWindow.Width = panelActualSize.Width + ShadowSize * 2;
			ShadowWindow.Height = panelActualSize.Height + ShadowSize * 2;
		}
	}
}