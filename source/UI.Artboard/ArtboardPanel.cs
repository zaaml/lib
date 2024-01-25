// <copyright file="ArtboardPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Media;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Artboard
{
	public abstract class ArtboardPanel : Panel
	{
		private protected readonly CompositeTransform ScrollViewTransform = new CompositeTransform
		{
			ScaleX = 1.0,
			ScaleY = 1.0
		};

		private ArtboardControl _artboard;

		internal event EventHandler MatrixChanged;

		public ArtboardControl ArtboardControl
		{
			get => _artboard;
			internal set
			{
				if (ReferenceEquals(_artboard, value))
					return;

				if (_artboard != null)
					DetachArtboard(_artboard);

				_artboard = value;

				if (_artboard != null)
					AttachArtboard(_artboard);
			}
		}

		protected Matrix FromMatrix => ScrollViewTransform.Transform.Value;

		internal double ScrollOffsetX
		{
			get => -ScrollViewTransform.TranslateX;
			set
			{
				if (ScrollViewTransform.TranslateX.IsCloseTo(-value))
					return;

				ScrollViewTransform.TranslateX = -value;

				OnOffsetXChanged();
				OnMatrixChanged();
			}
		}

		internal double ScrollOffsetY
		{
			get => -ScrollViewTransform.TranslateY;
			set
			{
				if (ScrollViewTransform.TranslateY.IsCloseTo(-value))
					return;

				ScrollViewTransform.TranslateY = -value;

				OnOffsetYChanged();
				OnMatrixChanged();
			}
		}

		protected Matrix ToMatrix
		{
			get
			{
				var matrix = ScrollViewTransform.Transform.Value;

				matrix.Invert();

				return matrix;
			}
		}

		internal double Zoom
		{
			get => ScrollViewTransform.ScaleX;
			set
			{
				if (Zoom.IsCloseTo(value))
					return;

				ScrollViewTransform.ScaleX = ScrollViewTransform.ScaleY = value;

				OnZoomChanged();
				OnMatrixChanged();
			}
		}

		private protected virtual void AttachArtboard(ArtboardControl artboard)
		{
		}

		private protected virtual void DetachArtboard(ArtboardControl artboard)
		{
		}

		private void OnMatrixChanged()
		{
			MatrixChanged?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void OnOffsetXChanged()
		{
			InvalidateArrange();
		}

		protected virtual void OnOffsetYChanged()
		{
			InvalidateArrange();
		}

		protected virtual void OnZoomChanged()
		{
			InvalidateMeasure();
		}

		internal Point TransformFromDesignCoordinates(Point point)
		{
			return FromMatrix.Transform(point);
		}

		internal Rect TransformFromDesignCoordinates(Rect rect)
		{
			var matrix = FromMatrix;

			return new Rect(matrix.Transform(rect.TopLeft), matrix.Transform(rect.BottomRight));
		}

		internal Point TransformToDesignCoordinates(Point point)
		{
			return ToMatrix.Transform(point);
		}

		internal Rect TransformToDesignCoordinates(Rect rect)
		{
			var matrix = ToMatrix;

			return new Rect(matrix.Transform(rect.TopLeft), matrix.Transform(rect.BottomRight));
		}
	}
}