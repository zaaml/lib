// <copyright file="ArtboardItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Artboard
{
	[TemplateContractType(typeof(ArtboardItemTemplateContract))]
	[ContentProperty(nameof(Canvas))]
	public class ArtboardItem : TemplateContractControl, IArtboardComponentControl
	{
		private static readonly DependencyPropertyKey ArtboardControlPropertyKey = DPM.RegisterReadOnly<ArtboardControl, ArtboardItem>
			("ArtboardControl", d => d.OnArtboardControlPropertyChangedPrivate);

		public static readonly DependencyProperty ArtboardControlProperty = ArtboardControlPropertyKey.DependencyProperty;

		public static readonly DependencyProperty CanvasProperty = DPM.Register<ArtboardCanvas, ArtboardItem>
			("Canvas", d => d.OnCanvasPropertyChangedPrivate);

		private double _scrollOffsetX;
		private double _scrollOffsetY;
		private double _zoom;

		static ArtboardItem()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ArtboardItem>();
		}

		public ArtboardItem()
		{
			this.OverrideStyleKey<ArtboardItem>();
		}

		public ArtboardControl ArtboardControl
		{
			get => (ArtboardControl) GetValue(ArtboardControlProperty);
			internal set => this.SetReadOnlyValue(ArtboardControlPropertyKey, value);
		}

		public ArtboardCanvas Canvas
		{
			get => (ArtboardCanvas) GetValue(CanvasProperty);
			set => SetValue(CanvasProperty, value);
		}

		private ArtboardCanvasPresenter CanvasPresenter => TemplateContract.CanvasPresenter;

		private IEnumerable<IArtboardComponentControl> Components
		{
			get
			{
				if (IsTemplateAttached == false)
					yield break;
			}
		}

		private double ScrollOffsetX
		{
			get => _scrollOffsetX;
			set
			{
				if (_scrollOffsetX.IsCloseTo(value))
					return;

				_scrollOffsetX = value;

				OnOffsetChanged();
			}
		}

		private double ScrollOffsetY
		{
			get => _scrollOffsetY;
			set
			{
				if (_scrollOffsetY.IsCloseTo(value))
					return;

				_scrollOffsetY = value;

				OnOffsetChanged();
			}
		}

		private ArtboardItemTemplateContract TemplateContract => (ArtboardItemTemplateContract) TemplateContractInternal;

		private double Zoom
		{
			get => _zoom;
			set
			{
				if (_zoom.IsCloseTo(value))
					return;

				_zoom = value;

				OnZoomChanged();
			}
		}

		private void OnArtboardControlPropertyChangedPrivate(ArtboardControl oldValue, ArtboardControl newValue)
		{
			UpdateArtboardControl();
		}

		private void OnCanvasPropertyChangedPrivate(ArtboardCanvas oldValue, ArtboardCanvas newValue)
		{
			LogicalChildMentor.OnLogicalChildPropertyChanged(oldValue, newValue);

			if (CanvasPresenter != null)
				CanvasPresenter.Canvas = Canvas;
		}

		private void OnOffsetChanged()
		{
			UpdateOffset();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			CanvasPresenter.Canvas = Canvas;

			UpdateZoom();
			UpdateOffset();
		}

		protected override void OnTemplateContractDetaching()
		{
			CanvasPresenter.Canvas = null;

			base.OnTemplateContractDetaching();
		}

		private void OnZoomChanged()
		{
			UpdateZoom();
		}

		private void UpdateArtboardControl()
		{
			var artboardControl = ArtboardControl;

			foreach (var artboardComponentControl in Components)
				artboardComponentControl.Artboard = artboardControl;

			if (Canvas != null)
				Canvas.ArtboardControl = artboardControl;
		}

		private void UpdateOffset()
		{
			foreach (var component in Components)
			{
				component.ScrollOffsetX = ScrollOffsetX;
				component.ScrollOffsetY = ScrollOffsetY;
			}

			if (Canvas != null)
			{
				Canvas.ScrollOffsetX = ScrollOffsetX;
				Canvas.ScrollOffsetY = ScrollOffsetY;
			}
		}

		private void UpdateZoom()
		{
			foreach (var component in Components)
			{
				component.Zoom = Zoom;
				component.Zoom = Zoom;
			}

			if (Canvas != null)
				Canvas.Zoom = Zoom;
		}

		ArtboardControl IArtboardComponentControl.Artboard
		{
			get => ArtboardControl;
			set => ArtboardControl = value;
		}

		double IArtboardComponentControl.ScrollOffsetX
		{
			get => ScrollOffsetX;
			set => ScrollOffsetX = value;
		}

		double IArtboardComponentControl.ScrollOffsetY
		{
			get => ScrollOffsetY;
			set => ScrollOffsetY = value;
		}

		double IArtboardComponentControl.Zoom
		{
			get => Zoom;
			set => Zoom = value;
		}
	}
}