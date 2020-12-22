// <copyright file="VirtualItemCollection.RealMode.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;

namespace Zaaml.UI.Controls.Core
{
	internal partial class VirtualItemCollection<T>
	{
		private List<GeneratedItem> RealGeneratedItems { get; } = new List<GeneratedItem>();

		private void RealEnterGeneration()
		{
			IsGenerating = true;
		}

		private T RealGetCurrent(int index)
		{
			return RealGeneratedItems[index].Item;
		}

		private int RealGetIndexFromItem(T item)
		{
			return RealGeneratedItems.FindIndex(g => ReferenceEquals(g.Item, item));
		}

		private int RealGetIndexFromSource(object source)
		{
			return RealGeneratedItems.FindIndex(g => ReferenceEquals(g.Item, source));
		}

		private T RealGetItemFromIndex(int index)
		{
			return RealGeneratedItems[index].Item;
		}

		private object RealGetSourceFromIndex(int index)
		{
			return RealGeneratedItems[index].Item;
		}

		private object RealGetSourceFromItem(T item)
		{
			return item;
		}

		private void RealLeaveGeneration()
		{
			IsGenerating = false;
		}

		private void RealLockItem(T item)
		{
		}

		private UIElement RealRealize(in int index)
		{
			return RealGeneratedItems[index].Item;
		}

		private void RealReset()
		{
			for (var index = 0; index < RealGeneratedItems.Count; index++)
				DetachItem(index, RealGeneratedItems[index]);

			RealGeneratedItems.Clear();

			foreach (T item in IndexedSource)
			{
				var generatedItem = new GeneratedItem(this)
				{
					Item = item,
					Source = item
				};

				RealGeneratedItems.Add(generatedItem);

				AttachItem(RealGeneratedItems.Count - 1, generatedItem);
			}
		}

		private void RealUnlockItem(T item)
		{
		}
	}
}