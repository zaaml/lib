// <copyright file="ListGridView.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.ListView
{
	[ContentProperty(nameof(Columns))]
	public class ListGridView : ListViewBase
	{
		private static readonly DependencyPropertyKey ColumnsPropertyKey = DPM.RegisterReadOnly<ListGridViewColumnCollection, ListGridView>
			("ColumnsPrivate");

		private static readonly DependencyPropertyKey ListViewControlPropertyKey = DPM.RegisterReadOnly<ListViewControl, ListGridView>
			("ListViewControl", d => d.OnListViewControlPropertyChangedPrivate);

		public static readonly DependencyProperty ListViewControlProperty = ListViewControlPropertyKey.DependencyProperty;

		public static readonly DependencyProperty ColumnsProperty = ColumnsPropertyKey.DependencyProperty;

		public ListGridViewColumnCollection Columns => this.GetValueOrCreate(ColumnsPropertyKey, () => new ListGridViewColumnCollection(this));

		internal ListViewItemCellColumnController ItemCellColumnController { get; private set; }

		public ListViewControl ListViewControl
		{
			get => (ListViewControl) GetValue(ListViewControlProperty);
			internal set => this.SetReadOnlyValue(ListViewControlPropertyKey, value);
		}

		private void OnListViewControlPropertyChangedPrivate(ListViewControl oldValue, ListViewControl newValue)
		{
			ItemCellColumnController = newValue != null ? new ListViewItemCellColumnController(newValue) : null;
		}
	}
}