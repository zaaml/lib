// <copyright file="ArtboardSnapRectSource.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.UI.Controls.Artboard
{
	public sealed class ArtboardSnapRectSource : ArtboardSnapSource
	{
		private readonly ArtboardRectSnapLine _bottom;
		private readonly ArtboardRectSnapPoint _bottomLeft;
		private readonly ArtboardRectSnapPoint _bottomRight;
		private readonly ArtboardRectSnapLine _left;
		private readonly ArtboardRectSnapLine _right;
		private readonly ArtboardRectSnapLine _top;
		private readonly ArtboardRectSnapPoint _topLeft;
		private readonly ArtboardRectSnapPoint _topRight;

		public ArtboardSnapRectSource()
		{
			_topLeft = CreateSnapPoint(RectPoint.TopLeft);
			_topRight = CreateSnapPoint(RectPoint.TopRight);
			_bottomRight = CreateSnapPoint(RectPoint.BottomRight);
			_bottomLeft = CreateSnapPoint(RectPoint.BottomLeft);

			_left = CreateSnapLine(RectSide.Left);
			_top = CreateSnapLine(RectSide.Top);
			_right = CreateSnapLine(RectSide.Right);
			_bottom = CreateSnapLine(RectSide.Bottom);
		}

		private ArtboardRectSnapLine CreateSnapLine(RectSide rectSide)
		{
			return new ArtboardRectSnapLine(rectSide, this);
		}

		private ArtboardRectSnapPoint CreateSnapPoint(RectPoint rectPoint)
		{
			return new ArtboardRectSnapPoint(rectPoint, this);
		}

		public override IEnumerable<ArtboardSnapSourcePrimitive> GetSnapPrimitives(ArtboardSnapEngineContextParameters parameters)
		{
			var side = parameters.Side;

			var hasLeft = ArtboardSnapEngineUtils.HasLeft(side);
			var hasTop = ArtboardSnapEngineUtils.HasTop(side);
			var hasRight = ArtboardSnapEngineUtils.HasRight(side);
			var hasBottom = ArtboardSnapEngineUtils.HasBottom(side);

			if (hasTop && hasLeft)
				yield return _topLeft;

			if (hasTop && hasRight)
				yield return _topRight;

			if (hasBottom && hasRight)
				yield return _bottomRight;

			if (hasBottom && hasLeft)
				yield return _bottomLeft;

			if (hasLeft)
				yield return _left;

			if (hasTop)
				yield return _top;

			if (hasRight)
				yield return _right;

			if (hasBottom)
				yield return _bottom;
		}

		private sealed class ArtboardRectSnapPoint : ArtboardSnapSourcePoint
		{
			public ArtboardRectSnapPoint(RectPoint rectPoint, ArtboardSnapRectSource source) : base(source)
			{
				RectPoint = rectPoint;
			}

			private RectPoint RectPoint { get; }

			public override Point GetLocation(ArtboardSnapParameters parameters)
			{
				return RectUtils.GetPoint(parameters.Rect, RectPoint);
			}
		}

		private sealed class ArtboardRectSnapLine : ArtboardSnapSourceLine
		{
			public ArtboardRectSnapLine(RectSide side, ArtboardSnapRectSource source) : base(source)
			{
				Side = side;
			}

			public override ArtboardAxis Axis
			{
				get
				{
					return Side switch
					{
						RectSide.Left => ArtboardAxis.Y,
						RectSide.Right => ArtboardAxis.Y,
						RectSide.Top => ArtboardAxis.X,
						RectSide.Bottom => ArtboardAxis.X,
						_ => throw new ArgumentOutOfRangeException()
					};
				}
			}

			private RectSide Side { get; }

			public override double GetAxisValue(ArtboardSnapParameters parameters)
			{
				return RectUtils.GetSideAxisValue(parameters.Rect, Side);
			}
		}
	}
}