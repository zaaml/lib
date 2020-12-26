// <copyright file="DropDownListViewSelectionPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;
using System.Windows;
using Zaaml.Core;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.ListView;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;

namespace Zaaml.UI.Controls.DropDown
{
	public class DropDownListViewSelectionPresenter : DropDownSelectionPresenterBase<DropDownListViewSelectionItem, ListViewItem>
	{
		private static readonly DependencyPropertyKey ListViewControlPropertyKey = DPM.RegisterReadOnly<ListViewControl, DropDownListViewSelectionPresenter>
			("ListViewControl", d => d.OnListViewControlPropertyChangedPrivate);

		private static readonly DependencyPropertyKey DropDownListViewControlPropertyKey = DPM.RegisterReadOnly<DropDownListViewControl, DropDownListViewSelectionPresenter>
			("DropDownListViewControl", d => d.OnDropDownListViewControlPropertyChangedPrivate);

		public static readonly DependencyProperty DropDownListViewControlProperty = DropDownListViewControlPropertyKey.DependencyProperty;

		public static readonly DependencyProperty ListViewControlProperty = ListViewControlPropertyKey.DependencyProperty;

		static DropDownListViewSelectionPresenter()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<DropDownListViewSelectionPresenter>();
		}

		public DropDownListViewSelectionPresenter()
		{
			this.OverrideStyleKey<DropDownListViewSelectionPresenter>();
		}

		public DropDownListViewControl DropDownListViewControl
		{
			get => (DropDownListViewControl) GetValue(DropDownListViewControlProperty);
			internal set => this.SetReadOnlyValue(DropDownListViewControlPropertyKey, value);
		}

		private protected override bool IsAttachDetachOverriden => IsDefault;

		internal bool IsDefault { get; set; }

		public ListViewControl ListViewControl
		{
			get => (ListViewControl) GetValue(ListViewControlProperty);
			private set => this.SetReadOnlyValue(ListViewControlPropertyKey, value);
		}

		private protected override void AttachOverride(SelectionItem<ListViewItem> selectionItem, Selection<ListViewItem> selection)
		{
			selectionItem.ItemsControl = this;
			selectionItem.Selection = selection;

			var defaultGeneratorImplementation = ListViewControl.DefaultGeneratorImplementationInternal;
			var contentBinding = defaultGeneratorImplementation.ItemContentMemberBindingInternal;
			var iconBinding = defaultGeneratorImplementation.ItemIconMemberBindingInternal;
			var listViewItem = selection.Item;

			selectionItem.DataContext = selection.Source;

			if (listViewItem != null && ItemCollectionBase.GetInItemCollection(listViewItem))
			{
				selectionItem.Content = listViewItem.Content;
				selectionItem.Icon = listViewItem.Icon;
			}
			else
			{
				if (contentBinding != null)
					ItemGenerator<SelectionItem<ListViewItem>>.InstallBinding(selectionItem, IconContentPresenter.ContentProperty, contentBinding);
				else
					selectionItem.Content = selection.Source;

				if (iconBinding != null)
					ItemGenerator<SelectionItem<ListViewItem>>.InstallBinding(selectionItem, IconPresenterBase.IconProperty, iconBinding);
			}
		}

		private protected override void DetachOverride(SelectionItem<ListViewItem> selectionItem, Selection<ListViewItem> selection)
		{
			selectionItem.ItemsControl = null;
			selectionItem.Selection = Selection<ListViewItem>.Empty;

			selectionItem.ClearValue(DataContextProperty);
			selectionItem.ClearValue(IconContentPresenter.ContentProperty);
			selectionItem.ClearValue(IconPresenterBase.IconProperty);
		}

		private void OnDropDownListViewControlPropertyChangedPrivate(DropDownListViewControl oldValue, DropDownListViewControl newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			if (oldValue != null)
				oldValue.ListViewControlChanged -= OnListViewControlChanged;

			if (newValue != null)
				newValue.ListViewControlChanged += OnListViewControlChanged;

			UpdateListViewControl();
		}

		private void OnListViewControlChanged(object sender, ValueChangedEventArgs<ListViewControl> e)
		{
			UpdateListViewControl();
		}

		private void OnListViewControlPropertyChangedPrivate(ListViewControl oldValue, ListViewControl newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			SelectionCollection = newValue?.SelectionCollection;
		}

		private void UpdateListViewControl()
		{
			ListViewControl = DropDownListViewControl?.ListViewControl;
		}
	}

	public class DropDownListViewSelectionItem : SelectionItem<ListViewItem>
	{
		private protected override bool IsLast
		{
			get
			{
				var selectionPresenter = (DropDownListViewSelectionPresenter) ItemsControl;

				return ReferenceEquals(Selection.Source, selectionPresenter?.SelectionCollection.LastOrDefault().Source);
			}
		}
	}

	public class DropDownListViewSelectionItemsPresenter : SelectionItemsPresenter<DropDownListViewSelectionItem, ListViewItem>
	{
	}

	public class DropDownListViewSelectionItemsPanel : SelectionItemsPanel<DropDownListViewSelectionItem, ListViewItem>
	{
	}
}