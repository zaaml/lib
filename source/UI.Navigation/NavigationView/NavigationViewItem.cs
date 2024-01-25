// <copyright file="NavigationViewItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.NavigationView
{
	[ContentProperty(nameof(Page))]
	[TemplateContractType(typeof(NavigationViewItemTemplateContract))]
	public partial class NavigationViewItem : NavigationViewHeaderedIconItem
	{
		public static readonly DependencyProperty PageProperty = DPM.Register<NavigationViewPage, NavigationViewItem>
			("Page", n => n.OnPagePropertyChangedPrivate);

		public static readonly DependencyProperty IsSelectedProperty = DPM.Register<bool, NavigationViewItem>
			("IsSelected", i => i.OnIsSelectedPropertyChangedPrivate, i => i.OnCoerceSelection);

		public static readonly DependencyProperty FrameProperty = DPM.Register<NavigationViewFrame, NavigationViewItem>
			("Frame", null, d => d.OnFramePropertyChangedPrivate);

		public event EventHandler IsSelectedChanged;

		static NavigationViewItem()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<NavigationViewItem>();
		}

		public NavigationViewItem()
		{
			this.OverrideStyleKey<NavigationViewItem>();
		}

		internal bool ActualCanSelect => CanSelect && Frame?.CanSelectItemInternal(this) != false;

		protected virtual bool CanSelect => IsInitialized == false || Frame != null;

		[TypeConverter(typeof(NavigationViewFrameConverter))]
		public NavigationViewFrame Frame
		{
			get => (NavigationViewFrame) GetValue(FrameProperty);
			set => SetValue(FrameProperty, value);
		}

		private bool IsDeferredPageLoaded { get; set; }

		private protected override bool IsPressedVisualState => IsMouseCaptured;

		public bool IsSelected
		{
			get => (bool) GetValue(IsSelectedProperty);
			set => SetValue(IsSelectedProperty, value.Box());
		}

		private protected override bool IsSelectedVisualState => IsSelected;

		private NavigationViewPage LoadedPage { get; set; }

		[TypeConverter(typeof(NavigationViewPageConverter))]
		public NavigationViewPage Page
		{
			get => (NavigationViewPage) GetValue(PageProperty);
			set => SetValue(PageProperty, value);
		}

		internal NavigationViewPage EnsurePage()
		{
			if (IsDeferredPageLoaded)
				return Page;

			IsDeferredPageLoaded = true;

			this.ReadLocalBindingExpression(PageProperty)?.UpdateTarget();

			return Page;
		}

		internal NavigationViewPage LoadPage(Type pageType)
		{
			if (IsDeferredPageLoaded == false)
				return null;

			LoadedPage ??= (NavigationViewPage) Activator.CreateInstance(pageType);

			return LoadedPage;
		}

		private protected override void OnClickCore()
		{
			base.OnClickCore();

			if (ActualCanSelect)
				SelectInternal();
		}

		private object OnCoerceSelection(object isSelectedObject)
		{
			var isSelected = (bool) isSelectedObject;

			if (isSelected && CanSelect == false)
				return BooleanBoxes.False;

			return isSelectedObject;
		}

		private void OnFramePropertyChangedPrivate(NavigationViewFrame oldValue, NavigationViewFrame newValue)
		{
			oldValue?.DetachItem(this);
			newValue?.AttachItem(this);

			if (IsSelected) 
				Frame?.Select(this);
		}

		protected virtual void OnIsSelectedChanged()
		{
			var selected = IsSelected;

			if (selected)
				RaiseSelectedEvent();
			else
				RaiseUnselectedEvent();

			if (selected == IsSelected)
				IsSelectedChanged?.Invoke(this, EventArgs.Empty);
		}

		private void OnIsSelectedPropertyChangedPrivate()
		{
			var selected = IsSelected;

			if (selected)
			{
				EnsurePage();

				Frame?.Select(this);
			}

			OnIsSelectedChanged();

			UpdateVisualState(true);
		}

		private void OnPagePropertyChangedPrivate(NavigationViewPage oldPage, NavigationViewPage newPage)
		{
			if (oldPage != null)
				oldPage.NavigationViewItem = null;

			if (newPage != null)
				newPage.NavigationViewItem = this;
		}

		internal void SelectInternal()
		{
			SetIsSelectedInternal(true);
		}

		internal void SetIsSelectedInternal(bool value)
		{
			this.SetCurrentValueInternal(IsSelectedProperty, value.Box());
		}
	}
}