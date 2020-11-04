// <copyright file="ISelectorAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;

namespace Zaaml.UI.Controls.Core
{
	internal interface ISelectorAdvisor<T> : IDisposable where T : FrameworkElement, ISelectable
	{
		SelectorController<T> Controller { get; set; }

		int Count { get; }

		bool HasSource { get; }

		bool CompareValues(object value1, object value2);

		int GetIndexOfItem(T item);

		int GetIndexOfItemSource(object itemSource);

		int GetIndexOfValue(object value);

		bool TryGetItem(int index, out T item);

		bool TryGetItemBySource(object itemSource, out T item);

		object GetItemSource(int index);

		object GetItemSource(T item);

		object GetValue(int index);

		void Lock(T item);

		void Unlock(T item);
	}
}