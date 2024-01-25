// <copyright file="UIElementSyncCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Zaaml.PresentationCore
{
	internal sealed class UIElementSyncCollection
	{
		private readonly List<UIElement> _elements = new();

		public void Add(UIElement element)
		{
			_elements.Add(element);
		}

		public void Clear()
		{
			_elements.Clear();
		}

		public bool Contains(UIElement element)
		{
			return _elements.Contains(element);
		}

		public void Insert(int index, UIElement element)
		{
			_elements.Insert(index, element);
		}

		public void Remove(UIElement element)
		{
			_elements.Remove(element);
		}

		public void RemoveAt(int index)
		{
			_elements.RemoveAt(index);
		}

		public void Sync(UIElementCollection collection)
		{
			if (collection == null)
				throw new ArgumentNullException(nameof(collection));

			if (collection.Count != _elements.Count)
			{
				SyncImpl(collection);

				return;
			}

			for (var i = 0; i < _elements.Count; i++)
			{
				if (ReferenceEquals(_elements[i], collection[i]))
					continue;

				SyncImpl(collection);

				return;
			}
		}

		private void SyncImpl(UIElementCollection collection)
		{
			collection.Clear();

			foreach (var element in _elements)
				collection.Add(element);
		}
	}
}