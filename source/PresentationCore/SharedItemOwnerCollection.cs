// <copyright file="SharedItemOwnerCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core.Weak.Collections;

namespace Zaaml.PresentationCore
{
	internal class SharedItemOwnerCollection : WeakLinkedList<FrameworkElement>
	{
		public SharedItemOwnerCollection(ISharedItem sharedItem)
		{
			SharedItem = sharedItem;
		}

		public ISharedItem SharedItem { get; }

		protected override void OnCollectionChanged()
		{
			base.OnCollectionChanged();

			SharedItem.IsShared = IsEmpty == false;
		}
	}
}