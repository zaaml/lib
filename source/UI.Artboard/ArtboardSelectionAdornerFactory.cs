// <copyright file="ArtboardSelectionAdornerFactory.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Artboard
{
	public class ArtboardSelectionAdornerFactory : ArtboardAdornerFactory<ArtboardSelectionAdorner>
	{
		protected override ArtboardSelectionAdorner CreateAdornerCore()
		{
			return new ArtboardSelectionAdorner();
		}
	}
}