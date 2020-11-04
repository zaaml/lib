// <copyright file="ArtboardAdornerFactoryCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Artboard
{
	public sealed class ArtboardAdornerFactoryCollection : InheritanceContextDependencyObjectCollection<ArtboardAdornerFactory>
	{
		internal ArtboardAdornerFactoryCollection(ArtboardControl artboardControl)
		{
			ArtboardControl = artboardControl;
		}

		public ArtboardControl ArtboardControl { get; }

		protected override void OnItemAdded(ArtboardAdornerFactory adornerFactory)
		{
			base.OnItemAdded(adornerFactory);

			ArtboardControl.OnAdornerFactoryAdded(adornerFactory);
		}

		protected override void OnItemRemoved(ArtboardAdornerFactory adornerFactory)
		{
			ArtboardControl.OnAdornerFactoryRemoved(adornerFactory);

			base.OnItemRemoved(adornerFactory);
		}
	}
}