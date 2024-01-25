// <copyright file="ArtboardAdorner.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Artboard
{
	[TemplateContractType(typeof(ArtboardAdornerTemplateContract))]
	public abstract class ArtboardAdorner : TemplateContractControl
	{
		private static readonly DependencyPropertyKey AdornedElementPropertyKey = DPM.RegisterReadOnly<FrameworkElement, ArtboardAdorner>
			("AdornedElement", null, d => d.OnAdornedElementPropertyChangedPrivate);

		private static readonly DependencyPropertyKey AdornerPanelPropertyKey = DPM.RegisterReadOnly<ArtboardAdornerPanel, ArtboardAdorner>
			("AdornerPanel", a => a.OnAdornerPanelPropertyChangedPrivate);

		private static readonly DependencyPropertyKey ArtboardCanvasPropertyKey = DPM.RegisterReadOnly<ArtboardCanvas, ArtboardAdorner>
			("ArtboardCanvas", d => d.OnArtboardCanvasPropertyChangedPrivate);

		public static readonly DependencyProperty ArtboardCanvasProperty = ArtboardCanvasPropertyKey.DependencyProperty;

		public ArtboardCanvas ArtboardCanvas
		{
			get => (ArtboardCanvas) GetValue(ArtboardCanvasProperty);
			internal set => this.SetReadOnlyValue(ArtboardCanvasPropertyKey, value);
		}

		private void OnArtboardCanvasPropertyChangedPrivate(ArtboardCanvas oldValue, ArtboardCanvas newValue)
		{
		}

		public static readonly DependencyProperty AdornerPanelProperty = AdornerPanelPropertyKey.DependencyProperty;

		public static readonly DependencyProperty AdornedElementProperty = AdornedElementPropertyKey.DependencyProperty;

		public FrameworkElement AdornedElement
		{
			get => (FrameworkElement) GetValue(AdornedElementProperty);
			internal set => this.SetReadOnlyValue(AdornedElementPropertyKey, value);
		}

		public ArtboardAdornerPanel AdornerPanel
		{
			get => (ArtboardAdornerPanel) GetValue(AdornerPanelProperty);
			internal set => this.SetReadOnlyValue(AdornerPanelPropertyKey, value);
		}

		internal Rect AdornerRect { get; private set; }


		internal void ArrangeAdorner(Rect rect)
		{
			AdornerRect = rect;

			AdornerPanel?.ArrangeAdorner(this);
		}

		private void AttachAdornerPanel(ArtboardAdornerPanel adornerPanel)
		{
			adornerPanel.Children.Add(this);
		}

		protected virtual void AttachElement(FrameworkElement adornedElement)
		{
		}

		private void DetachAdornerPanel(ArtboardAdornerPanel adornerPanel)
		{
			adornerPanel.Children.Remove(this);
		}

		protected virtual void DetachElement(FrameworkElement adornedElement)
		{
		}

		protected virtual void OnAdornedElementChanged(FrameworkElement oldValue, FrameworkElement newValue)
		{
		}

		private void OnAdornedElementPropertyChangedPrivate(FrameworkElement oldValue, FrameworkElement newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			if (oldValue != null)
			{
				DetachElement(oldValue);

				oldValue.InvalidateArrange();
			}

			if (newValue != null)
			{
				AttachElement(newValue);

				newValue.InvalidateArrange();
			}

			OnAdornedElementChanged(oldValue, newValue);
		}

		private void OnAdornerPanelPropertyChangedPrivate(ArtboardAdornerPanel oldPanel, ArtboardAdornerPanel newPanel)
		{
			if (ReferenceEquals(oldPanel, newPanel))
				return;

			if (oldPanel != null)
				DetachAdornerPanel(oldPanel);

			if (newPanel != null)
				AttachAdornerPanel(newPanel);
		}

		protected virtual void OnMatrixChanged()
		{
		}

		internal void OnMatrixChangedInternal()
		{
			OnMatrixChanged();
		}
	}

	public class ArtboardAdornerTemplateContract : TemplateContract
	{
	}
}