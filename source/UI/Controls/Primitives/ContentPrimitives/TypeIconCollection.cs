// <copyright file="TypeIconCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Primitives.ContentPrimitives
{
	public sealed class TypeIconCollection : DependencyObjectCollectionBase<TypeIcon>
	{
		private readonly TypeIconSelector _iconSelector;

		internal TypeIconCollection(TypeIconSelector iconSelector)
		{
			_iconSelector = iconSelector;
		}

		protected override void OnItemAdded(TypeIcon obj)
		{
			base.OnItemAdded(obj);

			_iconSelector.OnChangedInternal();
		}

		protected override void OnItemRemoved(TypeIcon obj)
		{
			base.OnItemRemoved(obj);

			_iconSelector.OnChangedInternal();
		}
	}
}