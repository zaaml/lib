// <copyright file="UIElementCollectionSpan.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;

namespace Zaaml.PresentationCore
{
	internal ref struct UIElementCollectionSpan
	{
		public UIElementCollectionSpan(UIElementCollection collection, int start, int length)
		{
			Collection = collection;
			Start = start;
			Length = length;
		}

		private UIElementCollection Collection { get; }

		private int Start { get; }

		private int Length { get; set; }

		public void Clear()
		{
			Collection.RemoveRange(Start, Length);

			Length = 0;
		}

		private UIElement GetElement(int index)
		{
			if (index >= Length)
				throw new ArgumentOutOfRangeException(nameof(index));

			return Collection[index + Start];
		}

		private void SetElement(int index, UIElement element)
		{
			if (index >= Length)
				throw new ArgumentOutOfRangeException(nameof(index));

			Collection[index + Start] = element;
		}

		public UIElement this[int index]
		{
			get => GetElement(index);
			set => SetElement(index, value);
		}

		public int Count => Length;

		public void Insert(int index, UIElement element)
		{
			if (index < 0 || index > Length)
				throw new ArgumentOutOfRangeException(nameof(index));

			Collection.Insert(index + Start, element);

			Length++;
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= Length || Length == 0)
				throw new ArgumentOutOfRangeException(nameof(index));

			Collection.RemoveAt(index + Start);

			Length--;
		}

		public void RemoveRange(int index, int count)
		{
			if (index < 0 || index + count > Length)
				throw new ArgumentOutOfRangeException(nameof(index));

			Collection.RemoveRange(index + Start, count);

			Length -= count;
		}

		public void Add(UIElement element)
		{
			Collection.Insert(Start + Length, element);

			Length++;
		}
	}
}