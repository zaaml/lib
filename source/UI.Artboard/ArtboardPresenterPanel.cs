// <copyright file="ArtboardPresenterPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core;

namespace Zaaml.UI.Controls.Artboard
{
	public sealed class ArtboardPresenterPanel : ArtboardPanel
	{
		private ArtboardControl _artboard;

		internal ArtboardControl Artboard
		{
			get => _artboard;
			set
			{
				if (ReferenceEquals(_artboard, value))
					return;

				if (_artboard != null)
				{
					_artboard.CanvasCollection.CanvasAdded -= OnCanvasAdded;
					_artboard.CanvasCollection.CanvasRemoved -= OnCanvasRemoved;

					foreach (var canvas in _artboard.CanvasCollection)
						DetachCanvas(canvas);
				}

				_artboard = value;

				if (_artboard != null)
				{
					_artboard.CanvasCollection.CanvasAdded += OnCanvasAdded;
					_artboard.CanvasCollection.CanvasRemoved += OnCanvasRemoved;

					foreach (var canvas in _artboard.CanvasCollection)
						AttachCanvas(canvas);
				}

				InvalidateMeasure();
			}
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

			if (_artboard == null)
				return;

			foreach (var canvas in _artboard.CanvasCollection)
				canvas.DesignHeight = DesignHeight;
		}

		protected override void OnDesignWidthChanged()
		{
			base.OnDesignWidthChanged();

			if (_artboard == null)
				return;

			foreach (var canvas in _artboard.CanvasCollection)
				canvas.DesignWidth = DesignWidth;
		}

		protected override void OnOffsetXChanged()
		{
			base.OnOffsetXChanged();

			if (_artboard == null)
				return;

			foreach (var canvas in _artboard.CanvasCollection)
				canvas.OffsetX = OffsetX;
		}

		protected override void OnOffsetYChanged()
		{
			base.OnOffsetYChanged();

			if (_artboard == null)
				return;

			foreach (var canvas in _artboard.CanvasCollection)
				canvas.OffsetY = OffsetY;
		}

		protected override void OnZoomChanged()
		{
			base.OnZoomChanged();

			if (_artboard == null)
				return;

			foreach (var canvas in _artboard.CanvasCollection)
				canvas.Zoom = Zoom;
		}
	}
}