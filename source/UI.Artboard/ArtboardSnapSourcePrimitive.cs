// <copyright file="ArtboardSnapPoint.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Artboard
{
	public abstract class ArtboardSnapSourcePrimitive
	{
		protected ArtboardSnapSourcePrimitive(ArtboardSnapSource source)
		{
			Source = source;
		}

		public ArtboardSnapSource Source { get; }

		public abstract bool CanSnap(ArtboardSnapTargetPrimitive targetPrimitive);
	}
}