// <copyright file="BreadCrumbItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.CommandCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.DropDown;

namespace Zaaml.UI.Controls.BreadCrumb
{
	[ContentProperty(nameof(Items))]
	public class BreadCrumbItem : HeaderedIconContentControl, IBreadCrumbItemsOwner
	{
		private static readonly DependencyPropertyKey ActualShowIconPropertyKey = DPM.RegisterReadOnly<bool, BreadCrumbItem>
			("ActualShowIcon");

		public static readonly DependencyProperty ActualShowIconProperty = ActualShowIconPropertyKey.DependencyProperty;

		public static readonly DependencyProperty IconVisibilityProperty = DPM.Register<ElementVisibility, BreadCrumbItem>
			("IconVisibility", ElementVisibility.Inherit, b => b.OnIconVisibilityChanged);

		public static readonly DependencyProperty IsMenuOpenProperty = DPM.Register<bool, BreadCrumbItem>
			("IsMenuOpen", false, b => b.OnIsMenuOpenChanged, b => b.OnCoerceIsMenuOpenChanged);

		private static readonly DependencyPropertyKey BreadCrumbControlPropertyKey = DPM.RegisterReadOnly<BreadCrumbControl, BreadCrumbItem>
			("BreadCrumbControl", b => b.OnBreadCrumbControlChanged);

		private static readonly DependencyPropertyKey ItemsPropertyKey = DPM.RegisterReadOnly<BreadCrumbItemCollection, BreadCrumbItem>
			("ItemsInt");

		private static readonly DependencyPropertyKey HasItemsPropertyKey = DPM.RegisterReadOnly<bool, BreadCrumbItem>
			("HasItems");

		private static readonly DependencyPropertyKey ParentItemPropertyKey = DPM.RegisterReadOnly<BreadCrumbItem, BreadCrumbItem>
			("ParentItem");

		public static readonly DependencyProperty IsSelectedProperty = DPM.Register<bool, BreadCrumbItem>
			("IsSelected", b => b.OnIsSelectedChanged);

		public static readonly DependencyProperty ParentItemProperty = ParentItemPropertyKey.DependencyProperty;
		public static readonly DependencyProperty HasItemsProperty = HasItemsPropertyKey.DependencyProperty;
		public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;
		public static readonly DependencyProperty BreadCrumbControlProperty = BreadCrumbControlPropertyKey.DependencyProperty;

		private IBreadCrumbItemsOwner _owner;
		private SplitButton _splitButton;
		public event PropertyChangedEventHandler PropertyChanged;

		static BreadCrumbItem()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<BreadCrumbItem>();
		}

		public BreadCrumbItem()
		{
			this.OverrideStyleKey<BreadCrumbItem>();

			SelectCommand = new RelayCommand(OnSelectCommandExecute, () => true);
			Items = new BreadCrumbItemCollection(this);
		}

		public bool ActualShowIcon
		{
			get => (bool) GetValue(ActualShowIconProperty);
			private set => this.SetReadOnlyValue(ActualShowIconPropertyKey, value);
		}

		public BreadCrumbControl BreadCrumbControl
		{
			get => (BreadCrumbControl) GetValue(BreadCrumbControlProperty);
			internal set => this.SetReadOnlyValue(BreadCrumbControlPropertyKey, value);
		}

		public bool HasItems
		{
			get => (bool) GetValue(HasItemsProperty);
			private set => this.SetReadOnlyValue(HasItemsPropertyKey, value);
		}

		public ElementVisibility IconVisibility
		{
			get => (ElementVisibility) GetValue(IconVisibilityProperty);
			set => SetValue(IconVisibilityProperty, value);
		}

		public bool IsMenuOpen
		{
			get => (bool) GetValue(IsMenuOpenProperty);
			set => SetValue(IsMenuOpenProperty, value);
		}

		public bool IsSelected
		{
			get => (bool) GetValue(IsSelectedProperty);
			set => SetValue(IsSelectedProperty, value);
		}

		internal IBreadCrumbItemsOwner Owner
		{
			get => _owner;
			set
			{
				if (ReferenceEquals(_owner, value))
					return;

				_owner = value;

				if (value is BreadCrumbItem breadCrumbItem)
					ParentItem = breadCrumbItem;

				if (value is BreadCrumbControl breadCrumbControl)
					BreadCrumbControl = breadCrumbControl;
			}
		}

		public BreadCrumbItem ParentItem
		{
			get => (BreadCrumbItem) GetValue(ParentItemProperty);
			private set => this.SetReadOnlyValue(ParentItemPropertyKey, value);
		}

		public ICommand SelectCommand { get; }

		private SplitButton SplitButton
		{
			get => _splitButton;
			set
			{
				if (ReferenceEquals(_splitButton, value))
					return;

				if (_splitButton != null)
					_splitButton.Click -= SplitButtonOnClick;

				_splitButton = value;

				if (_splitButton != null)
					_splitButton.Click += SplitButtonOnClick;
			}
		}

		private void OnBreadCrumbControlChanged(BreadCrumbControl oldControl, BreadCrumbControl newControl)
		{
			if (IsSelected)
			{
				if (newControl != null)
					newControl.SelectedItem = this;

				if (oldControl != null && ReferenceEquals(oldControl.SelectedItem, this))
					oldControl.SelectedItem = null;
			}

			foreach (var item in Items)
				item.BreadCrumbControl = newControl;

			UpdateIconVisibility();
		}

		private object OnCoerceIsMenuOpenChanged(object o)
		{
			if ((bool) o && HasItems == false)
				return false;

			return o;
		}

		private void OnIconVisibilityChanged()
		{
			UpdateIconVisibility();
		}

		private void OnIsMenuOpenChanged()
		{
			if (BreadCrumbControl == null || BreadCrumbControl.CurrentMenuBreadCrumbItemChanging)
				return;

			if (IsMenuOpen)
				BreadCrumbControl.CurrentMenuBreadCrumbItem = this;
			else if (ReferenceEquals(BreadCrumbControl.CurrentMenuBreadCrumbItem, this))
				BreadCrumbControl.CurrentMenuBreadCrumbItem = null;
		}

		private void OnIsSelectedChanged(bool oldValue, bool newValue)
		{
			if (newValue)
			{
				var breadCrumbControl = BreadCrumbControl;

				if (breadCrumbControl != null)
					breadCrumbControl.SelectedItem = this;
			}
		}

		protected virtual void OnItemAdded(BreadCrumbItem item)
		{
		}

		private void OnItemAddedInt(BreadCrumbItem item)
		{
			item.BreadCrumbControl = BreadCrumbControl;

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

			item.BreadCrumbControl = null;
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);

			if (HasItems == false || BreadCrumbControl?.CurrentMenuBreadCrumbItem == null)
				return;

			BreadCrumbControl.CurrentMenuBreadCrumbItem = this;
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void OnSelectCommandExecute()
		{
			IsSelected = true;
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			SplitButton = (SplitButton) GetTemplateChild("SplitButton");
		}

		protected override void OnTemplateContractDetaching()
		{
			SplitButton = null;

			base.OnTemplateContractDetaching();
		}

		private void SplitButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
		{
			BreadCrumbControl.SelectedItem = this;
		}

		private void UpdateHasItems()
		{
			HasItems = Items.Count > 0;
		}

		internal void UpdateIconVisibility()
		{
			var iconVisibility = IconVisibility;
			var breadCrumbControl = BreadCrumbControl;

			if (iconVisibility == ElementVisibility.Inherit && breadCrumbControl != null)
				iconVisibility = breadCrumbControl.ItemsIconVisibility;

			ActualShowIcon = VisibilityUtils.EvaluateElementVisibility(iconVisibility, Visibility.Visible) == Visibility.Visible;
		}

		public BreadCrumbItemCollection Items
		{
			get => (BreadCrumbItemCollection) GetValue(ItemsProperty);
			private set => this.SetReadOnlyValue(ItemsPropertyKey, value);
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
}