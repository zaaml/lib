// <copyright file="DropDownListViewSelectionPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.Core;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Editors.Core;
using Zaaml.UI.Controls.ListView;

namespace Zaaml.UI.Controls.DropDown
{
	public class DropDownListViewSelectionPresenter : DropDownSelectionPresenterBase<DropDownListViewSelectionItem, ListViewItem>
	{
		private static readonly DependencyPropertyKey ListViewControlPropertyKey = DPM.RegisterReadOnly<ListViewControl, DropDownListViewSelectionPresenter>
			("ListViewControl", default, d => d.OnListViewControlPropertyChangedPrivate);

		private static readonly DependencyPropertyKey DropDownListViewControlPropertyKey = DPM.RegisterReadOnly<DropDownListViewControl, DropDownListViewSelectionPresenter>
			("DropDownListViewControl", default, d => d.OnDropDownListViewControlPropertyChangedPrivate);

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

		public ListViewControl ListViewControl
		{
			get => (ListViewControl) GetValue(ListViewControlProperty);
			private set => this.SetReadOnlyValue(ListViewControlPropertyKey, value);
		}

		private void OnDropDownListViewControlPropertyChangedPrivate(DropDownListViewControl oldValue, DropDownListViewControl newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			if (oldValue != null)
			{
				oldValue.EditingEnded -= OnEditingEnded;
				oldValue.EditingStarted -= OnEditingStarted;
				oldValue.ListViewControlChanged -= OnListViewControlChanged;
			}

			if (newValue != null)
			{
				newValue.EditingEnded += OnEditingEnded;
				newValue.EditingStarted += OnEditingStarted;
				newValue.ListViewControlChanged += OnListViewControlChanged;
			}

			UpdateListViewControl();
		}

		private void OnEditingEnded(object sender, EditingEndedEventArgs e)
		{
			UpdateContent();
		}

		private void OnEditingStarted(object sender, EventArgs e)
		{
		}

		private void OnListViewControlChanged(object sender, ValueChangedEventArgs<ListViewControl> e)
		{
			UpdateListViewControl();
		}

		private void OnListViewControlPropertyChangedPrivate(ListViewControl oldValue, ListViewControl newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			if (oldValue != null)
				oldValue.SelectionChanged -= OnListViewSelectionChanged;

			if (newValue != null)
				newValue.SelectionChanged += OnListViewSelectionChanged;

			SelectionCollection = newValue?.SelectionCollection;
		}

		private void OnListViewSelectionChanged(object sender, SelectionChangedEventArgs<ListViewItem> e)
		{
			UpdateContent();
		}

		private void UpdateContent()
		{
		}

		private void UpdateListViewControl()
		{
			ListViewControl = DropDownListViewControl?.ListViewControl;
		}
	}
	
	public class DropDownListViewSelectionItem : SelectionItem<ListViewItem>
	{
	}

	public class DropDownListViewSelectionItemsPresenter : SelectionItemsPresenter<DropDownListViewSelectionItem, ListViewItem>
	{
	}

	public class DropDownListViewSelectionItemsPanel : SelectionItemsPanel<DropDownListViewSelectionItem, ListViewItem>
	{
	}
}