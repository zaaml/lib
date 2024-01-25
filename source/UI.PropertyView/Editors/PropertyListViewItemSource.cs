// <copyright file="PropertyListViewItemSource.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.UI.Controls.ListView;

namespace Zaaml.UI.Controls.PropertyView.Editors
{
	public abstract class PropertyListViewItemSource
	{
		protected PropertyListViewItemSource(string displayName)
		{
			DisplayName = displayName;
		}

		public string DisplayName { get; }

		public static PropertyListViewItemSource<T> Create<T>(T value, string displayName)
		{
			return new PropertyListViewItemSource<T>(value, displayName);
		}
	}

	public class PropertyListViewItemSource<T> : PropertyListViewItemSource
	{
		public PropertyListViewItemSource(T value, string displayName) : base(displayName)
		{
			Value = value;
		}

		public T Value { get; }
	}

	public sealed class PropertyListViewItemTextFilter : ListViewItemTextFilterBase<PropertyListViewItemSource>
	{
		protected override bool Pass(ListViewControl listViewControl, PropertyListViewItemSource item)
		{
			return item.DisplayName.StartsWith(FilterText, StringComparison.OrdinalIgnoreCase);
		}
	}
}