﻿// <copyright file="IItemCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace Zaaml.UI.Controls.Core
{
	internal interface IItemCollection
	{
	}

	internal interface IItemCollection<TItem> : IItemCollection where TItem : FrameworkElement
	{
		#region Properties

		int ActualCount { get; }

		IEnumerable<TItem> ActualItems { get; }

		IEnumerable SourceCollection { get; set; }

		#endregion

		#region  Methods

		void BringIntoView(int index);

		TItem EnsureItem(int index);

		int GetIndexFromItem(TItem item);

		int GetIndexFromSource(object source);

		TItem GetItemFromIndex(int index);

		object GetSourceFromIndex(int index);

		void LockItem(TItem item);

		void UnlockItem(TItem item);

		#endregion
	}
}