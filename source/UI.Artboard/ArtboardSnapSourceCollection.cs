// <copyright file="ArtboardSnapSourceCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Artboard
{
	public sealed class ArtboardSnapSourceCollection : InheritanceContextDependencyObjectCollection<ArtboardSnapSource>
	{
		internal ArtboardSnapSourceCollection(UIElement element)
		{
			Element = element;
		}

		public UIElement Element { get; }
	}
}