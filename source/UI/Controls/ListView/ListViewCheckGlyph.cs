// <copyright file="ListViewCheckGlyph.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Primitives;

namespace Zaaml.UI.Controls.ListView
{
	public class ListViewCheckGlyph : CheckGlyph
	{
		private static readonly DependencyPropertyKey ListViewItemPropertyKey = DPM.RegisterReadOnly<ListViewItem, ListViewCheckGlyph>
			("ListViewItem", g => g.OnListViewItemPropertyChangedPrivate);

		public static readonly DependencyProperty ListViewItemProperty = ListViewItemPropertyKey.DependencyProperty;
		
		private static readonly PropertyPath ListViewItemIsSelectedPropertyPath = new PropertyPath(ListViewItem.IsSelectedProperty);

		public ListViewItem ListViewItem
		{
			get => (ListViewItem) GetValue(ListViewItemProperty);
			private set => this.SetReadOnlyValue(ListViewItemPropertyKey, value);
		}

		private void OnListViewItemPropertyChangedPrivate(ListViewItem oldListViewItem, ListViewItem newListViewItem)
		{
			if (ReferenceEquals(oldListViewItem, newListViewItem))
				return;
			
			if (oldListViewItem != null)
				ClearValue(IsCheckedProperty);

			if (newListViewItem != null) 
				SetBinding(IsCheckedProperty, new Binding {Path = ListViewItemIsSelectedPropertyPath, Source = newListViewItem, Mode = BindingMode.TwoWay});
		}

		protected override void OnLoaded()
		{
			base.OnLoaded();

			var parent = TemplatedParent;

			if (parent is ContentPresenter contentPresenter)
			{
				if (contentPresenter.TemplatedParent is ListViewItem listViewItem)
					ListViewItem = listViewItem;
			}
			else if (parent is ListViewItem listViewItem)
				ListViewItem = listViewItem;
			else
				ListViewItem = null;
		}

		protected override void OnUnloaded()
		{
			ListViewItem = null;

			base.OnUnloaded();
		}
	}
}