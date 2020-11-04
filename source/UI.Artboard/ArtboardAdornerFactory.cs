// <copyright file="ArtboardAdornerFactory.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Artboard
{
	public abstract class ArtboardAdornerFactory : InheritanceContextObject
	{
		internal ArtboardAdorner CreateAdorner()
		{
			var adorner = CreateAdornerBase();

			adorner.Factory = this;

			return adorner;
		}

		protected abstract ArtboardAdorner CreateAdornerBase();
	}

	public abstract class ArtboardAdornerFactory<TAdorner> : ArtboardAdornerFactory where TAdorner : ArtboardAdorner
	{
		protected sealed override ArtboardAdorner CreateAdornerBase()
		{
			return CreateAdornerCore();
		}

		protected abstract TAdorner CreateAdornerCore();
	}
}