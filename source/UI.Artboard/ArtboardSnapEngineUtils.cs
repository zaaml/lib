// <copyright file="ArtboardSnapEngineUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Behaviors.Resizable;

namespace Zaaml.UI.Controls.Artboard
{
	internal static class ArtboardSnapEngineUtils
	{
		public static Rect CalcResizeRect(Rect originRect, Rect snapRect, ArtboardSnapRectSide snapSide)
		{
			if (HasLeft(snapSide))
			{
				originRect.Width = originRect.Right - snapRect.Left;
				originRect.X = snapRect.X;
			}

			if (HasTop(snapSide))
			{
				originRect.Height = originRect.Bottom - snapRect.Top;
				originRect.Y = snapRect.Y;
			}

			if (HasRight(snapSide))
				originRect.Width = snapRect.Right - originRect.Left;

			if (HasBottom(snapSide))
				originRect.Height = snapRect.Bottom - originRect.Top;

			return originRect;
		}

		public static ArtboardSnapRectSide GetResizeSide(ResizableHandleKind handleKind)
		{
			var snapSide = ArtboardSnapRectSide.None;

			if (ResizableBehaviorUtils.HasLeft(handleKind))
				snapSide |= ArtboardSnapRectSide.Left;

			if (ResizableBehaviorUtils.HasTop(handleKind))
				snapSide |= ArtboardSnapRectSide.Top;

			if (ResizableBehaviorUtils.HasRight(handleKind))
				snapSide |= ArtboardSnapRectSide.Right;

			if (ResizableBehaviorUtils.HasBottom(handleKind))
				snapSide |= ArtboardSnapRectSide.Bottom;

			return snapSide;
		}

		public static bool HasBottom(ArtboardSnapRectSide handleKind)
		{
			return (handleKind & ArtboardSnapRectSide.Bottom) != 0;
		}

		public static bool HasLeft(ArtboardSnapRectSide handleKind)
		{
			return (handleKind & ArtboardSnapRectSide.Left) != 0;
		}

		public static bool HasRight(ArtboardSnapRectSide handleKind)
		{
			return (handleKind & ArtboardSnapRectSide.Right) != 0;
		}

		public static bool HasTop(ArtboardSnapRectSide handleKind)
		{
			return (handleKind & ArtboardSnapRectSide.Top) != 0;
		}
	}
}