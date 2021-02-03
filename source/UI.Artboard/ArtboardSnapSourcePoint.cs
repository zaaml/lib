// <copyright file="ArtboardSnapSourcePoint.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Artboard
{
	public abstract class ArtboardSnapSourcePoint : ArtboardSnapSourcePrimitive
	{
		protected ArtboardSnapSourcePoint(ArtboardSnapSource source) : base(source)
		{
		}

		public sealed override bool CanSnap(ArtboardSnapTargetPrimitive targetPrimitive)
		{
			return CanSnapCore(targetPrimitive);
		}

		protected virtual bool CanSnapCore(ArtboardSnapTargetPrimitive targetPrimitive)
		{
			return true;
		}

		public abstract Point GetLocation(ArtboardSnapParameters parameters);
	}
}