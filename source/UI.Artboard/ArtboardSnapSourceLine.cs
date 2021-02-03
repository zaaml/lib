// <copyright file="ArtboardSnapSourceLine.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Artboard
{
	public abstract class ArtboardSnapSourceLine : ArtboardSnapSourcePrimitive
	{
		protected ArtboardSnapSourceLine(ArtboardSnapSource source) : base(source)
		{
		}

		public abstract ArtboardAxis Axis { get; }

		public sealed override bool CanSnap(ArtboardSnapTargetPrimitive targetPrimitive)
		{
			return (targetPrimitive is ArtboardSnapTargetLine targetLine && Axis == targetLine.Axis || targetPrimitive is ArtboardSnapTargetPoint) && CanSnapCore(targetPrimitive);
		}

		protected virtual bool CanSnapCore(ArtboardSnapTargetPrimitive targetPrimitive)
		{
			return true;
		}

		public abstract double GetAxisValue(ArtboardSnapParameters parameters);
	}
}