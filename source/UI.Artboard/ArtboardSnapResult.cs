// <copyright file="ArtboardSnapResult.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Artboard
{
	public readonly struct ArtboardSnapResult
	{
		public ArtboardSnapResult(ArtboardSnapParameters parameters, ArtboardSnapping horizontalSnapping, ArtboardSnapping verticalSnapping)
		{
			Parameters = parameters;
			HorizontalSnapping = horizontalSnapping;
			VerticalSnapping = verticalSnapping;
		}

		public ArtboardSnapParameters Parameters { get; }

		public Rect OriginRect => Parameters.Rect;

		public Rect SnapRect
		{
			get
			{
				var rect = OriginRect;

				var horizontalVector = ArtboardSnapEngine.CalcVector(HorizontalSnapping, Parameters);
				var verticalVector = ArtboardSnapEngine.CalcVector(VerticalSnapping, Parameters);

				rect.Offset(horizontalVector.X, verticalVector.Y);

				return rect;
			}
		}

		public ArtboardSnapping HorizontalSnapping { get; }

		public ArtboardSnapping VerticalSnapping { get; }
	}
}