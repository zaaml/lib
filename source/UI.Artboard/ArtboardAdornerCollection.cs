// <copyright file="ArtboardAdornerCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.Artboard
{
	public sealed class ArtboardAdornerCollection : DependencyObjectCollectionBase<ArtboardAdorner>
	{
		private ArtboardAdornerPanel _adornerPanel;
		private ArtboardCanvas _artboardCanvas;
		private Rect _elementRect;

		internal ArtboardAdornerCollection(FrameworkElement adornedElement)
		{
			AdornedElement = adornedElement;

			AdornedElement.Loaded += AdornedElementOnLoaded;
			AdornedElement.Unloaded += AdornedElementOnUnloaded;
			AdornedElement.LayoutUpdated += AdornedElementOnLayoutUpdated;

			UpdateAdornerPanelAndCanvas();
		}

		internal FrameworkElement AdornedElement { get; }

		internal ArtboardAdornerPanel AdornerPanel
		{
			get => _adornerPanel;
			private set
			{
				if (ReferenceEquals(_adornerPanel, value))
					return;

				if (_adornerPanel != null)
					DetachAdornerPanel(_adornerPanel);

				_adornerPanel = value;

				if (_adornerPanel != null)
					AttachAdornerPanel(_adornerPanel);
			}
		}

		internal ArtboardCanvas ArtboardCanvas
		{
			get => _artboardCanvas;
			private set
			{
				if (ReferenceEquals(_artboardCanvas, value))
					return;

				if (_artboardCanvas != null)
					DetachCanvas(_artboardCanvas);

				_artboardCanvas = value;

				if (_artboardCanvas != null)
					AttachCanvas(_artboardCanvas);

				UpdateRect();
			}
		}

		internal Rect ElementRect
		{
			get => _elementRect;
			set
			{
				if (Equals(_elementRect, value))
					return;

				_elementRect = value;

				foreach (var adorner in this)
					adorner.ArrangeAdorner(_elementRect);
			}
		}

		private void AdornedElementOnLayoutUpdated(object sender, EventArgs e)
		{
			UpdateRect();
		}

		private void AdornedElementOnLoaded(object sender, RoutedEventArgs e)
		{
			UpdateAdornerPanelAndCanvas();
		}

		private void AdornedElementOnUnloaded(object sender, RoutedEventArgs e)
		{
			UpdateAdornerPanelAndCanvas();
		}

		private void AdornerPanelOnMatrixChanged(object sender, EventArgs e)
		{
			foreach (var adorner in this)
				adorner.OnMatrixChangedInternal();
		}

		private void AttachAdornerPanel(ArtboardAdornerPanel adornerPanel)
		{
			foreach (var adorner in this) 
				adorner.AdornerPanel = adornerPanel;

			adornerPanel.MatrixChanged += AdornerPanelOnMatrixChanged;
		}

		private void AttachCanvas(ArtboardCanvas artboardCanvas)
		{
			foreach (var adorner in this)
				adorner.ArtboardCanvas = artboardCanvas;
		}

		private void DetachAdornerPanel(ArtboardAdornerPanel adornerPanel)
		{
			adornerPanel.MatrixChanged -= AdornerPanelOnMatrixChanged;

			foreach (var adorner in this)
				adorner.AdornerPanel = null;
		}

		private void DetachCanvas(ArtboardCanvas artboardCanvas)
		{
			foreach (var adorner in this)
				adorner.ArtboardCanvas = null;
		}

		private void EnsureAdornerPanelAndCanvas(out ArtboardAdornerPanel adornerPanel, out ArtboardCanvas artboardCanvas)
		{
			adornerPanel = null;
			
			var adornedElement = AdornedElement;

			artboardCanvas = adornedElement?.GetVisualAncestors().OfType<ArtboardCanvas>().FirstOrDefault();

			var artboardControl = artboardCanvas?.ArtboardControl;
			var artboardAdornerPresenter = artboardControl?.AdornerPresenterInternal;

			if (artboardAdornerPresenter == null)
				return;

			if (artboardAdornerPresenter.AdornerPanel == null)
				artboardAdornerPresenter.ApplyTemplate();

			adornerPanel = artboardAdornerPresenter.AdornerPanel;
		}

		private Rect GetElementRect()
		{
			if (ArtboardCanvas == null || AdornedElement == null || AdornedElement.IsDescendantOf(ArtboardCanvas) == false)
				return Rect.Empty;

			var transform = AdornedElement.TransformToAncestor(ArtboardCanvas);
			var rect = new Rect(AdornedElement.RenderSize);

			return transform.TransformBounds(rect);
		}

		protected override void OnItemAdded(ArtboardAdorner adorner)
		{
			base.OnItemAdded(adorner);

			adorner.AdornedElement = AdornedElement;
			adorner.ArtboardCanvas = ArtboardCanvas;
			adorner.AdornerPanel = AdornerPanel;
		}

		protected override void OnItemRemoved(ArtboardAdorner adorner)
		{
			adorner.AdornedElement = null;
			adorner.ArtboardCanvas = null;
			adorner.AdornerPanel = null;

			base.OnItemRemoved(adorner);
		}

		internal void UpdateAdornerPanelAndCanvas()
		{
			EnsureAdornerPanelAndCanvas(out var adornerPanel, out var artboardCanvas);

			AdornerPanel = adornerPanel;
			ArtboardCanvas = artboardCanvas;
		}

		private void UpdateRect()
		{
			ElementRect = GetElementRect();
		}
	}
}