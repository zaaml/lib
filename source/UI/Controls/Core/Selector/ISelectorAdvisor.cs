// <copyright file="ISelectorAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;

namespace Zaaml.UI.Controls.Core
{
	internal interface ISelectorAdvisor<T> : IDisposable where T : FrameworkElement
	{
		SelectorController<T> Controller { get; set; }

		int Count { get; }

		bool HasSource { get; }

		bool IsVirtualizing { get; }

		bool CanSelectIndex(int index);

		bool CanSelectItem(T item);

		bool CanSelectSource(object source);

		bool CanSelectValue(object value);

		bool CompareValues(object value1, object value2);

		int GetIndexOfItem(T item);

		int GetIndexOfSource(object source);

		int GetIndexOfValue(object value);

		bool GetItemSelected(T item);

		object GetSource(int index);

		object GetValue(T item, object source);

		object GetSource(T item);

		bool GetSourceSelected(object source);

		void Lock(T item);

		void SetItemSelected(T item, bool value);

		void SetSourceSelected(object source, bool value);

		bool TryCreateSelection(int index, bool ensure, out Selection<T> selection);

		bool TryCreateSelection(object source, bool ensure, out Selection<T> selection);

		bool TryGetItem(int index, bool ensure, out T item);

		bool TryGetItem(object source, bool ensure, out T item);

		void Unlock(T item);
	}
}