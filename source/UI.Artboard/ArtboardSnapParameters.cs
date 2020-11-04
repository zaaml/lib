// <copyright file="ArtboardSnapParameters.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Artboard
{
	public readonly struct ArtboardSnapParameters
	{
		public static readonly ArtboardSnapParameters Empty = new ArtboardSnapParameters(Rect.Empty, null);

		public ArtboardSnapParameters(Rect rect, ArtboardSnapEngineContext context)
		{
			Rect = rect;
			Context = context;
		}

		public ArtboardSnapEngineContext Context { get; }

		public Rect Rect { get; }
	}
}