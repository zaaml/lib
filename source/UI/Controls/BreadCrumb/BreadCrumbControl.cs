// <copyright file="BreadCrumbControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Zaaml.Core.Extensions;
using Zaaml.Core.Runtime;
using Zaaml.Core.Trees;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Runtime;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;
using Control = Zaaml.UI.Controls.Core.Control;

namespace Zaaml.UI.Controls.BreadCrumb
{
	[ContentProperty(nameof(ItemCollection))]
	public class BreadCrumbControl : Control, IBreadCrumbItemsOwner
	{
		private static readonly DependencyPropertyKey SelectedItemPropertyKey = DPM.RegisterReadOnly<BreadCrumbItem, BreadCrumbControl>
			("SelectedItem", b => b.OnSelectedItemChanged);

		public static readonly DependencyProperty ItemsIconVisibilityProperty = DPM.Register<ElementVisibility, BreadCrumbControl>
			("ItemsIconVisibility", ElementVisibility.Visible, b => b.OnItemsIconVisibilityChanged);

		public static readonly DependencyProperty SelectedItemProperty = SelectedItemPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey ItemCollectionPropertyKey = DPM.RegisterReadOnly<BreadCrumbItemCollection, BreadCrumbControl>
			("ItemCollectionPrivate");

		public static readonly DependencyProperty ItemCollectionProperty = ItemCollectionPropertyKey.DependencyProperty;

		public static readonly DependencyProperty IconProperty = DPM.Register<IconBase, BreadCrumbControl>
			("Icon", i => i.LogicalChildMentor.OnLogicalChildPropertyChanged);

		private static readonly DependencyPropertyKey HasItemsPropertyKey = DPM.RegisterReadOnly<bool, BreadCrumbControl>
			("HasItems", b => b.OnHasItemsChanged);

		public static readonly DependencyProperty HasItemsProperty = HasItemsPropertyKey.DependencyProperty;
		private static readonly ITreeEnumeratorAdvisor<BreadCrumbItem> BreadCrumbEnumeratorAdvisor = new DelegateTreeEnumeratorAdvisor<BreadCrumbItem>(b => b.ItemCollection.GetEnumerator());

		private BreadCrumbItem _currentMenuBreadCrumbItem;
		private Panel _hostPanel;

		public event RoutedPropertyChangedEventHandler<BreadCrumbItem> SelectedItemChanged;

		static BreadCrumbControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<BreadCrumbControl>();
		}

		public BreadCrumbControl()
		{
			this.OverrideStyleKey<BreadCrumbControl>();

			ItemCollection = new BreadCrumbItemCollection(this);
		}

		internal BreadCrumbItem CurrentMenuBreadCrumbItem
		{
			get => _currentMenuBreadCrumbItem;
			set
			{
				if (ReferenceEquals(_currentMenuBreadCrumbItem, value))
					return;

				try
				{
					CurrentMenuBreadCrumbItemChanging = true;

					if (_currentMenuBreadCrumbItem != null)
						_currentMenuBreadCrumbItem.IsMenuOpen = false;

					_currentMenuBreadCrumbItem = value;

					if (_currentMenuBreadCrumbItem != null)
						_currentMenuBreadCrumbItem.IsMenuOpen = true;
				}
				finally
				{
					CurrentMenuBreadCrumbItemChanging = false;
				}
			}
		}

		internal bool CurrentMenuBreadCrumbItemChanging { get; set; }
		
		public bool HasItems
		{
			get => (bool)GetValue(HasItemsProperty);
			private set => this.SetReadOnlyValue(HasItemsPropertyKey, value.Box());
		}

		public IconBase Icon
		{
			get => (IconBase)GetValue(IconProperty);
			set => SetValue(IconProperty, value);
		}

		public ElementVisibility ItemsIconVisibility
		{
			get => (ElementVisibility)GetValue(ItemsIconVisibilityProperty);
			set => SetValue(ItemsIconVisibilityProperty, value.Box());
		}

		public BreadCrumbItem SelectedItem
		{
			get => (BreadCrumbItem)GetValue(SelectedItemProperty);
			internal set => this.SetReadOnlyValue(SelectedItemPropertyKey, value);
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_hostPanel?.Children.Clear();
			_hostPanel = (Panel)GetTemplateChild("hostPanel");

			UpdateControl();
		}

		private void OnHasItemsChanged(bool oldValue, bool newValue)
		{
			if (newValue && SelectedItem == null)
				SelectedItem = ItemCollection.First();
			else if (newValue == false)
				SelectedItem = null;
		}

		protected virtual void OnItemAdded(BreadCrumbItem item)
		{
		}

		private void OnItemAddedInt(BreadCrumbItem item)
		{
			UpdateHasItems();
			OnItemAdded(item);
		}

		protected virtual void OnItemRemoved(BreadCrumbItem item)
		{
		}

		private void OnItemRemovedInt(BreadCrumbItem item)
		{
			UpdateHasItems();
			OnItemRemoved(item);
		}

		private void OnItemsIconVisibilityChanged()
		{
			foreach (var breadCrumbItem in BreadCrumbEnumeratorAdvisor.GetEnumerator(ItemCollection).Enumerate())
				breadCrumbItem.UpdateIconVisibility();
		}

		private void OnSelectedItemChanged(BreadCrumbItem oldItem, BreadCrumbItem newItem)
		{
			if (oldItem?.IsSelected == true)
				oldItem.IsSelected = false;

			UpdateControl();
			OnSelectionChanged(oldItem, newItem);
		}

		protected virtual void OnSelectionChanged(BreadCrumbItem oldItem, BreadCrumbItem newItem)
		{
			SelectedItemChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<BreadCrumbItem>(oldItem, newItem));
		}

		private void UpdateControl()
		{
			if (_hostPanel == null)
				return;

			_hostPanel.Children.Clear();

			var current = SelectedItem;

			while (current != null)
			{
				_hostPanel.Children.Insert(0, current);
				current = current.ParentItem;
			}
		}

		private void UpdateHasItems()
		{
			HasItems = ItemCollection.Count > 0;
		}

		public BreadCrumbItemCollection ItemCollection
		{
			get => (BreadCrumbItemCollection)GetValue(ItemCollectionProperty);
			private set => this.SetReadOnlyValue(ItemCollectionPropertyKey, value);
		}

		void IBreadCrumbItemsOwner.OnItemAdded(BreadCrumbItem item)
		{
			OnItemAddedInt(item);
		}

		void IBreadCrumbItemsOwner.OnItemRemoved(BreadCrumbItem item)
		{
			OnItemRemovedInt(item);
		}
	}

	internal interface IBreadCrumbItemsOwner
	{
		BreadCrumbItemCollection ItemCollection { get; }

		void OnItemAdded(BreadCrumbItem item);

		void OnItemRemoved(BreadCrumbItem item);
	}
}