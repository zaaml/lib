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

		bool CompareValues(object value1, object value2);

		int GetIndexOfItem(T item);

		int GetIndexOfSource(object source);

		int GetIndexOfValue(object value);

		bool GetItemSelected(T item);

		object GetSource(int index);

		object GetSource(T item);

		bool GetSourceSelected(T item);

		object GetValue(int index);

		void Lock(T item);

		void SetItemSelected(T item, bool value);

		void SetSourceSelected(T item, bool value);

		bool TryGetItem(int index, bool ensure, out T item);

		bool TryGetSelection(int index, bool ensure, out Selection<T> selection);
		
		bool TryGetSelection(object source, bool ensure, out Selection<T> selection);

		bool TryGetItemBySource(object source, bool ensure, out T item);

		void Unlock(T item);
		
		bool CanSelect(T item);
	}
}