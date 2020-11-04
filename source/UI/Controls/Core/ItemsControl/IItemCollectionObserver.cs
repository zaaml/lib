// <copyright file="IItemCollectionObserver.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Specialized;
using System.Windows;

namespace Zaaml.UI.Controls.Core
{
	internal interface IItemCollectionObserver<in T> where T : FrameworkElement
	{
		#region  Methods

		void OnCollectionChanged(NotifyCollectionChangedEventArgs args);

		void OnItemAttached(int index, T item);

		void OnItemAttaching(int index, T item);

		void OnItemDetached(int index, T item);

		void OnItemDetaching(int index, T item);

		void OnSourceCollectionChanged(NotifyCollectionChangedEventArgs args);

		#endregion
	}
}