// <copyright file="ArtboardAdornerCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Artboard
{
	public sealed class ArtboardAdornerCollection : DependencyObjectCollectionBase<ArtboardAdorner>
	{
		internal ArtboardAdornerCollection(UIElement element)
		{
			Element = element;
		}

		internal UIElement Element { get; }

		protected override void OnItemAdded(ArtboardAdorner adorner)
		{
			base.OnItemAdded(adorner);

			adorner.AdornedElement = Element;
		}

		protected override void OnItemRemoved(ArtboardAdorner adorner)
		{
			adorner.AdornedElement = null;

			base.OnItemRemoved(adorner);
		}
	}
}