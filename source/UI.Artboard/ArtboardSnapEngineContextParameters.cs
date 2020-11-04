// <copyright file="ArtboardSnapEngineContextParameters.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Artboard
{
	public readonly struct ArtboardSnapEngineContextParameters
	{
		public ArtboardSnapEngineContextParameters(UIElement element, ArtboardSnapRectSide side)
		{
			Element = element;
			Side = side;
		}

		public ArtboardSnapRectSide Side { get; }

		public UIElement Element { get; }
	}
}