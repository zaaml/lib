// <copyright file="ArtboardSnapTargetElementCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Artboard
{
	public sealed class ArtboardSnapTargetCollection : InheritanceContextDependencyObjectCollection<ArtboardSnapTarget>
	{
		internal ArtboardSnapTargetCollection(ArtboardSnapEngine engine)
		{
			Engine = engine;
		}

		public ArtboardSnapEngine Engine { get; }
	}
}