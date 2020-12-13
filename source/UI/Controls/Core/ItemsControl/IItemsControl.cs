// <copyright file="IItemsControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Interfaces;

namespace Zaaml.UI.Controls.Core
{
	internal interface IItemsControl : IControl
	{
		#region Properties

		bool HasItems { get; }

		#endregion

		#region  Methods

		void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args);

		void OnSourceChanged();

		#endregion
	}

	internal interface IItemsControl<TItem> : IItemsControl, ILogicalOwner where TItem : FrameworkElement
	{
		#region Properties

		IEnumerable<TItem> ActualItems { get; }

		#endregion

		#region  Methods

		void OnItemAttached(TItem item);

		void OnItemAttaching(TItem item);

		void OnItemDetached(TItem item);

		void OnItemDetaching(TItem item);

		#endregion
	}
}