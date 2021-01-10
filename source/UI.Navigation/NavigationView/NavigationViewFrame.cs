// <copyright file="NavigationViewFrame.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.NavigationView
{
	[TemplateContractType(typeof(NavigationViewFrameTemplateContract))]
	public class NavigationViewFrame : TemplateContractControl, ISelector<NavigationViewItem>
	{
		private static readonly DependencyProperty SelectedItemProperty = DPM.Register<NavigationViewItem, NavigationViewFrame>
			("SelectedItem", n => n.OnSelectedItemPropertyChangedPrivate, n => n.CoerceSelectedItem);

		private NavigationViewSelectorController _selectorController;

		static NavigationViewFrame()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<NavigationViewFrame>();
		}

		public NavigationViewFrame()
		{
			this.OverrideStyleKey<NavigationViewFrame>();
		}

		private ContentPresenter ContentPresenter => TemplateContract.ContentPresenter;

		private bool IsInitializing { get; set; }

		internal NavigationViewFrameItemCollection Items { get; } = new NavigationViewFrameItemCollection();

		private NavigationViewItem SelectedItem => (NavigationViewItem) GetValue(SelectedItemProperty);

		private NavigationViewSelectorController SelectorController => _selectorController ??= CreateSelectorControllerPrivate();

		private NavigationViewFrameTemplateContract TemplateContract => (NavigationViewFrameTemplateContract) TemplateContractInternal;

		internal void AttachItem(NavigationViewItem navigationViewItem)
		{
			Items.Add(navigationViewItem);

			SelectorController.AdvisorOnItemAttached(-1, navigationViewItem);

			SelectorController.EnsureSelection();
		}

		public override void BeginInit()
		{
			base.BeginInit();

			IsInitializing = true;
			SelectorController.BeginInit();

			SelectorController.AllowNullSelection = true;
			SelectorController.PreferSelection = false;
		}

		protected virtual bool CanSelectItem(NavigationViewItem navigationViewItem)
		{
			return true;
		}

		internal bool CanSelectItemInternal(NavigationViewItem navigationViewItem)
		{
			return CanSelectItem(navigationViewItem);
		}

		private NavigationViewItem CoerceSelectedItem(NavigationViewItem navigationItem)
		{
			return SelectorController.CoerceSelectedItem(navigationItem);
		}

		internal virtual NavigationViewSelectorController CreateSelectorController()
		{
			return new NavigationViewSelectorController(this);
		}

		private NavigationViewSelectorController CreateSelectorControllerPrivate()
		{
			var selectorController = CreateSelectorController();

			if (IsInitializing == false)
			{
				selectorController.AllowNullSelection = true;
				selectorController.PreferSelection = false;
			}

			return selectorController;
		}

		internal void DetachItem(NavigationViewItem navigationViewItem)
		{
			Items.Remove(navigationViewItem);
			SelectorController.AdvisorOnItemDetached(-1, navigationViewItem);
			SelectorController.EnsureSelection();
		}

		public override void EndInit()
		{
			IsInitializing = false;
			
			SelectorController.EndInit();

			base.EndInit();
		}

		public void NavigateTo(NavigationViewItem navigationViewItem)
		{
			if (navigationViewItem.Frame != this)
				Items.Add(navigationViewItem);

			SelectorController.SelectItem(navigationViewItem);
		}

		private void OnSelectedItemPropertyChangedPrivate(NavigationViewItem oldValue, NavigationViewItem newValue)
		{
			SelectorController.OnSelectedItemPropertyChanged(oldValue, newValue);
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			UpdateSelectedContent();
		}

		protected override void OnTemplateContractDetaching()
		{
			ContentPresenter.Content = null;

			base.OnTemplateContractDetaching();
		}

		internal void Select(NavigationViewItem navigationViewItem)
		{
			var currentSelectedItem = SelectorController.CurrentSelectedItem;

			if (ReferenceEquals(currentSelectedItem, navigationViewItem) == false)
				currentSelectedItem?.SetIsSelectedInternal(false);

			SelectorController.SelectItem(navigationViewItem);
		}

		private void UpdateSelectedContent()
		{
			if (ContentPresenter != null)
			{
				ContentPresenter.Content = SelectedItem?.EnsurePage();
			}
		}

		DependencyProperty ISelector<NavigationViewItem>.SelectedIndexProperty => null;

		DependencyProperty ISelector<NavigationViewItem>.SelectedItemProperty => SelectedItemProperty;

		DependencyProperty ISelector<NavigationViewItem>.SelectedSourceProperty => null;

		DependencyProperty ISelector<NavigationViewItem>.SelectedValueProperty => null;

		void ISelector<NavigationViewItem>.OnSelectedIndexChanged(int oldIndex, int newIndex)
		{
		}

		void ISelector<NavigationViewItem>.OnSelectedItemChanged(NavigationViewItem oldItem, NavigationViewItem newItem)
		{
			UpdateSelectedContent();
		}

		void ISelector<NavigationViewItem>.OnSelectedSourceChanged(object oldSource, object newSource)
		{
		}

		void ISelector<NavigationViewItem>.OnSelectedValueChanged(object oldValue, object newValue)
		{
		}

		void ISelector<NavigationViewItem>.OnSelectionChanged(Selection<NavigationViewItem> oldSelection, Selection<NavigationViewItem> newSelection)
		{
		}
	}
}