// <copyright file="ArtboardPresenterPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core;

namespace Zaaml.UI.Controls.Artboard
{
	public sealed class ArtboardPresenterPanel : ArtboardPanel
	{
		private protected override void AttachArtboard(ArtboardControl artboard)
		{
			base.AttachArtboard(artboard);

			artboard.CanvasCollection.CanvasAdded += OnCanvasAdded;
			artboard.CanvasCollection.CanvasRemoved += OnCanvasRemoved;

			foreach (var canvas in artboard.CanvasCollection)
				AttachCanvas(canvas);

			InvalidateMeasure();
		}

		private void AttachCanvas(ArtboardCanvas canvas)
		{
			canvas.DesignWidth = DesignWidth;
			canvas.DesignHeight = DesignHeight;
			canvas.OffsetX = OffsetX;
			canvas.OffsetY = OffsetY;
			canvas.Zoom = Zoom;

			Children.Add(canvas);
		}

		private protected override void DetachArtboard(ArtboardControl artboard)
		{
			artboard.CanvasCollection.CanvasAdded -= OnCanvasAdded;
			artboard.CanvasCollection.CanvasRemoved -= OnCanvasRemoved;

			foreach (var canvas in artboard.CanvasCollection)
				DetachCanvas(canvas);

			InvalidateMeasure();

			base.DetachArtboard(artboard);
		}

		private void DetachCanvas(ArtboardCanvas canvas)
		{
			Children.Remove(canvas);

			canvas.DesignWidth = 0;
			canvas.DesignHeight = 0;
			canvas.OffsetX = 0;
			canvas.OffsetY = 0;
			canvas.Zoom = 1;
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			if (Artboard == null)
				return new Size();

			var designSize = new Size(Artboard.DesignWidth, Artboard.DesignHeight);

			for (var index = 0; index < Artboard.CanvasCollection.Count; index++)
				SetZIndex(Artboard.CanvasCollection[index], index);

			base.MeasureOverrideCore(designSize);

			return designSize;
		}

		private void OnCanvasAdded(object sender, EventArgs<ArtboardCanvas> e)
		{
			AttachCanvas(e.Value);
		}

		private void OnCanvasRemoved(object sender, EventArgs<ArtboardCanvas> e)
		{
			DetachCanvas(e.Value);
		}

		protected override void OnDesignHeightChanged()
		{
			base.OnDesignHeightChanged();

			if (Artboard == null)
				return;

			foreach (var canvas in Artboard.CanvasCollection)
				canvas.DesignHeight = DesignHeight;
		}

		protected override void OnDesignWidthChanged()
		{
			base.OnDesignWidthChanged();

			if (Artboard == null)
				return;

			foreach (var canvas in Artboard.CanvasCollection)
				canvas.DesignWidth = DesignWidth;
		}

		protected override void OnOffsetXChanged()
		{
			base.OnOffsetXChanged();

			if (Artboard == null)
				return;

			foreach (var canvas in Artboard.CanvasCollection)
				canvas.OffsetX = OffsetX;
		}

		protected override void OnOffsetYChanged()
		{
			base.OnOffsetYChanged();

			if (Artboard == null)
				return;

			foreach (var canvas in Artboard.CanvasCollection)
				canvas.OffsetY = OffsetY;
		}

		protected override void OnZoomChanged()
		{
			base.OnZoomChanged();

			if (Artboard == null)
				return;

			foreach (var canvas in Artboard.CanvasCollection)
				canvas.Zoom = Zoom;
		}
	}
}