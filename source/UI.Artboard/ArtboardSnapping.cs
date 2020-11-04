// <copyright file="ArtboardSnapping.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Artboard
{
	public readonly struct ArtboardSnapping
	{
		public static readonly ArtboardSnapping Empty = new ArtboardSnapping(null, null);

		public ArtboardSnapping(ArtboardSnapSourcePrimitive sourcePrimitive, ArtboardSnapTargetPrimitive targetPrimitive)
		{
			SourcePrimitive = sourcePrimitive;
			TargetPrimitive = targetPrimitive;
		}

		public ArtboardSnapSourcePrimitive SourcePrimitive { get; }

		public ArtboardSnapTargetPrimitive TargetPrimitive { get; }

		public bool IsValid => IsEmpty == false && SourcePrimitive.CanSnap(TargetPrimitive) && TargetPrimitive.CanSnap(SourcePrimitive);

		public bool IsEmpty => SourcePrimitive == null || TargetPrimitive == null;
	}
}