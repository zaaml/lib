// <copyright file="ArtboardAdorner.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
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
		private static readonly DependencyPropertyKey AdornedElementPropertyKey = DPM.RegisterReadOnly<UIElement, ArtboardAdorner>
			("AdornedElement", null, d => d.OnAdornedElementPropertyChangedPrivate);

		private static readonly DependencyPropertyKey AdornerPanelPropertyKey = DPM.RegisterReadOnly<ArtboardAdornerPanel, ArtboardAdorner>
			("AdornerPanel", null, d => d.OnAdornerPanelPropertyChangedPrivate);

		public static readonly DependencyProperty AdornerPanelProperty = AdornerPanelPropertyKey.DependencyProperty;

		public static readonly DependencyProperty AdornedElementProperty = AdornedElementPropertyKey.DependencyProperty;

		public UIElement AdornedElement
		{
			get => (UIElement) GetValue(AdornedElementProperty);
			internal set => this.SetReadOnlyValue(AdornedElementPropertyKey, value);
		}

		public ArtboardAdornerPanel AdornerPanel
		{
			get => (ArtboardAdornerPanel) GetValue(AdornerPanelProperty);
			private set => this.SetReadOnlyValue(AdornerPanelPropertyKey, value);
		}

		internal Rect AdornerRect { get; private set; }

		internal ArtboardAdornerFactory Factory { get; set; }

		internal void ArrangeAdorner(Rect rect)
		{
			UpdatePanel();

			AdornerRect = rect;

			AdornerPanel?.ArrangeAdorner(this);
		}

		protected virtual void AttachElement(UIElement adornedElement)
		{
		}

		protected virtual void AttachPanel(ArtboardAdornerPanel adornerPanel)
		{
			adornerPanel.Children.Add(this);

			adornerPanel.MatrixChanged += OnMatrixChanged;
		}

		protected virtual void DetachElement(UIElement adornedElement)
		{
		}

		protected virtual void DetachPanel(ArtboardAdornerPanel adornerPanel)
		{
			adornerPanel.MatrixChanged -= OnMatrixChanged;

			adornerPanel.Children.Remove(this);
		}

		private ArtboardAdornerPanel EnsurePanel()
		{
			var artboardCanvas = AdornedElement?.GetVisualParent() as ArtboardCanvas;
			var artboardAdornerPresenter = artboardCanvas?.ArtboardControl?.AdornerPresenterInternal;

			if (artboardAdornerPresenter == null)
				return null;

			if (artboardAdornerPresenter.AdornerPanel == null)
				artboardAdornerPresenter.ApplyTemplate();

			return artboardAdornerPresenter.AdornerPanel;
		}

		protected virtual void OnAdornedElementChanged(UIElement oldValue, UIElement newValue)
		{
		}

		private void OnAdornedElementPropertyChangedPrivate(UIElement oldValue, UIElement newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			UpdatePanel();

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

		protected virtual void OnAdornerPanelChanged(ArtboardAdornerPanel oldValue, ArtboardAdornerPanel newValue)
		{
		}

		private void OnAdornerPanelPropertyChangedPrivate(ArtboardAdornerPanel oldValue, ArtboardAdornerPanel newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			if (oldValue != null)
				DetachPanel(oldValue);

			if (newValue != null)
				AttachPanel(newValue);

			OnAdornerPanelChanged(oldValue, newValue);
		}

		protected virtual void OnMatrixChanged(object sender, EventArgs e)
		{
		}

		internal void UpdatePanel()
		{
			AdornerPanel = EnsurePanel();
		}
	}

	public class ArtboardAdornerTemplateContract : TemplateContract
	{
	}
}