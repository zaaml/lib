// <copyright file="ArtboardSnapTargetPoint.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Artboard
{
	public abstract class ArtboardSnapTargetPoint : ArtboardSnapTargetPrimitive
	{
		protected ArtboardSnapTargetPoint(ArtboardSnapTarget target) : base(target)
		{
		}

		public sealed override bool CanSnap(ArtboardSnapSourcePrimitive sourcePrimitive)
		{
			return CanSnapCore(sourcePrimitive);
		}

		protected virtual bool CanSnapCore(ArtboardSnapSourcePrimitive sourcePrimitive)
		{
			return true;
		}

		public abstract Point GetLocation(Point sourceLocation, ArtboardSnapEngineContext context);
	}
}