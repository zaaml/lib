// <copyright file="CompositeCollectionContainerCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Data
{
	public sealed class CompositeCollectionContainerCollection : InheritanceContextDependencyObjectCollection<CompositeCollectionContainer>
	{
		private readonly CompositeCollection _collection;

		internal CompositeCollectionContainerCollection(CompositeCollection collection)
		{
			_collection = collection;
		}

		protected override void OnItemAdded(CompositeCollectionContainer container)
		{
			base.OnItemAdded(container);

			container.SetCollectionInternal(_collection);
		}

		protected override void OnItemRemoved(CompositeCollectionContainer container)
		{
			container.SetCollectionInternal(null);

			base.OnItemRemoved(container);
		}
	}
}