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
		private static readonly PropertyPath ListViewItemIsCheckedPropertyPath = new PropertyPath(ToggleSelectionListViewItem.IsCheckedInternalProperty);

		public ListViewCheckGlyph()
		{
		}

		public ListViewCheckGlyph(ListViewItem listViewItem)
		{
			ListViewItem = listViewItem;

			IsExplicit = true;
		}

		private bool IsExplicit { get; }

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
			{
				var propertyPath = newListViewItem is ToggleSelectionListViewItem ? ListViewItemIsCheckedPropertyPath : ListViewItemIsSelectedPropertyPath;

				SetBinding(IsCheckedProperty, new Binding {Path = propertyPath, Source = newListViewItem, Mode = BindingMode.TwoWay});
			}
		}

		protected override void OnLoaded()
		{
			base.OnLoaded();

			if (IsExplicit)
				return;

			var templatedParent = TemplatedParent;

			if (templatedParent is ContentPresenter contentPresenter)
			{
				if (contentPresenter.TemplatedParent is ListViewItem listViewItem)
					ListViewItem = listViewItem;
			}
			else if (templatedParent is ListViewItem listViewItem)
				ListViewItem = listViewItem;
			else
				ListViewItem = null;
		}

		protected override void OnUnloaded()
		{
			if (IsExplicit == false)
				ListViewItem = null;

			base.OnUnloaded();
		}
	}
}