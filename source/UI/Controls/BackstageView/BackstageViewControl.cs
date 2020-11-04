// <copyright file="BackstageViewControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Zaaml.Core;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.BackstageView
{
	[ContentProperty(nameof(Items))]
	[TemplateContractType(typeof(BackstageViewControlTemplateContract))]
	public class BackstageViewControl : IndexedSelectorBase<BackstageViewControl, BackstageViewItem, BackstageViewItemCollection, BackstageViewItemsPresenter, BackstageViewItemsPanel>, IHeaderedContentItemsControl
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty IsOpenProperty = DPM.Register<bool, BackstageViewControl>
			("IsOpen", b => b.OnIsOpenChanged);

		public static readonly DependencyProperty OpenDelayProperty = DPM.Register<Duration, BackstageViewControl>
			("OpenDelay", b => b.OnOpenDelayChanged);

		public static readonly DependencyProperty CloseDelayProperty = DPM.Register<Duration, BackstageViewControl>
			("CloseDelay", b => b.OnCloseDelayChanged);

		public static readonly DependencyProperty ItemGeneratorProperty = DPM.Register<BackstageViewItemGeneratorBase, BackstageViewControl>
			("ItemGenerator", a => a.OnItemGeneratorChanged);

		public static readonly DependencyProperty ItemContentTemplateProperty = DPM.Register<DataTemplate, BackstageViewControl>
			("ItemContentTemplate", a => a.DefaultGeneratorImpl.OnItemContentTemplateChanged);

		public static readonly DependencyProperty ItemContentTemplateSelectorProperty = DPM.Register<DataTemplateSelector, BackstageViewControl>
			("ItemContentTemplateSelector", a => a.DefaultGeneratorImpl.OnItemContentTemplateSelectorChanged);

		public static readonly DependencyProperty ItemContentStringFormatProperty = DPM.Register<string, BackstageViewControl>
			("ItemContentStringFormat", a => a.DefaultGeneratorImpl.OnItemContentStringFormatChanged);

		public static readonly DependencyProperty ItemHeaderTemplateProperty = DPM.Register<DataTemplate, BackstageViewControl>
			("ItemHeaderTemplate", a => a.DefaultGeneratorImpl.OnItemHeaderTemplateChanged);

		public static readonly DependencyProperty ItemHeaderTemplateSelectorProperty = DPM.Register<DataTemplateSelector, BackstageViewControl>
			("ItemHeaderTemplateSelector", a => a.DefaultGeneratorImpl.OnItemHeaderTemplateSelectorChanged);

		public static readonly DependencyProperty ItemHeaderStringFormatProperty = DPM.Register<string, BackstageViewControl>
			("ItemHeaderStringFormat", a => a.DefaultGeneratorImpl.OnItemHeaderStringFormatChanged);

		public static readonly DependencyProperty ItemsSourceProperty = DPM.Register<IEnumerable, BackstageViewControl>
			("ItemsSource", i => i.OnItemsSourceChangedPrivate);

		#endregion

		#region Type: Fields

		private readonly DelayStateTrigger _delayTrigger;
		private readonly BackstageViewHost _host;
		private DelegateHeaderedContentItemGeneratorImpl<BackstageViewItem, DefaultBackstageViewItemGenerator> _defaultGeneratorImpl;
		public event EventHandler IsOpenChanged;

		#endregion

		#region Ctors

		static BackstageViewControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<BackstageViewControl>();
		}

		public BackstageViewControl()
		{
			this.OverrideStyleKey<BackstageViewControl>();

			_host = new BackstageViewHost(this);

			_delayTrigger = new DelayStateTrigger(OpenPopupHost, TimeSpan.Zero, ClosePopupHost, TimeSpan.Zero);
		}

		#endregion

		#region Properties

		private BackstageViewItemGeneratorBase ActualGenerator => ItemGenerator ?? DefaultGenerator;

		public Duration CloseDelay
		{
			get => (Duration) GetValue(CloseDelayProperty);
			set => SetValue(CloseDelayProperty, value);
		}

		private BackstageViewContentPresenter ContentPresenter => TemplateContract.ContentPresenter;

		private BackstageViewItemGeneratorBase DefaultGenerator => DefaultGeneratorImpl.Generator;

		private DelegateHeaderedContentItemGeneratorImpl<BackstageViewItem, DefaultBackstageViewItemGenerator> DefaultGeneratorImpl => _defaultGeneratorImpl ??= new DelegateHeaderedContentItemGeneratorImpl<BackstageViewItem, DefaultBackstageViewItemGenerator>(this);

		public bool IsOpen
		{
			get => (bool) GetValue(IsOpenProperty);
			set => SetValue(IsOpenProperty, value);
		}

		public BackstageViewItemGeneratorBase ItemGenerator
		{
			get => (BackstageViewItemGeneratorBase) GetValue(ItemGeneratorProperty);
			set => SetValue(ItemGeneratorProperty, value);
		}

		public IEnumerable ItemsSource
		{
			get => (IEnumerable) GetValue(ItemsSourceProperty);
			set => SetValue(ItemsSourceProperty, value);
		}

		public Duration OpenDelay
		{
			get => (Duration) GetValue(OpenDelayProperty);
			set => SetValue(OpenDelayProperty, value);
		}

		private BackstageViewControlTemplateContract TemplateContract => (BackstageViewControlTemplateContract) TemplateContractInternal;

		#endregion

		#region  Methods

		public void Activate(BackstageViewItem backstageViewItem)
		{
			if (ReferenceEquals(SelectedItem, backstageViewItem) == false)
				SelectorController.SelectedItem = backstageViewItem;
		}

		private void ClosePopupHost()
		{
			_host.IsOpen = false;
		}

		protected override BackstageViewItemCollection CreateItemCollection()
		{
			return new BackstageViewItemCollection(this)
			{
				Generator = ActualGenerator
			};
		}

		private void OnCloseDelayChanged()
		{
			_delayTrigger.CloseDelay = CloseDelay.HasTimeSpan ? CloseDelay.TimeSpan : TimeSpan.Zero;
		}

		private void OnIsOpenChanged()
		{
			if (IsOpen)
				_delayTrigger.InvokeOpen();
			else
				_delayTrigger.InvokeClose();

			IsOpenChanged?.Invoke(this, EventArgs.Empty);
		}

		internal override void OnItemAttachedInternal(BackstageViewItem item)
		{
			item.BackstageViewControl = this;

			base.OnItemAttachedInternal(item);
		}

		internal override void OnItemDetachedInternal(BackstageViewItem item)
		{
			base.OnItemDetachedInternal(item);

			item.BackstageViewControl = null;
		}

		internal virtual void OnItemGeneratorChanged(BackstageViewItemGeneratorBase oldGenerator, BackstageViewItemGeneratorBase newGenerator)
		{
			Items.Generator = ActualGenerator;
		}

		private void OnItemsSourceChangedPrivate(IEnumerable oldSource, IEnumerable newSource)
		{
			ItemsSourceCore = newSource;
		}

		private void OnOpenDelayChanged()
		{
			_delayTrigger.OpenDelay = OpenDelay.HasTimeSpan ? OpenDelay.TimeSpan : TimeSpan.Zero;
		}

		protected override void OnSelectionChanged(Selection<BackstageViewItem> oldSelection, Selection<BackstageViewItem> newSelection)
		{
			base.OnSelectionChanged(oldSelection, newSelection);

			UpdateSelectedContent();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			ItemsPresenter.BackstageViewControl = this;
			ContentPresenter.BackstageViewControl = this;

			UpdateSelectedContent();
		}

		protected override void OnTemplateContractDetaching()
		{
			ItemsPresenter.BackstageViewControl = null;
			ContentPresenter.BackstageViewControl = null;

			base.OnTemplateContractDetaching();
		}

		private void OpenPopupHost()
		{
			_host.IsOpen = true;
		}

		internal void SelectItemInternal(BackstageViewItem backstageViewItem)
		{
			SelectorController.SelectedItem = backstageViewItem;
		}

		protected virtual void UpdateSelectedContent()
		{
			if (ContentPresenter == null)
				return;

			ContentPresenter.Content = SelectedItem?.ContentHost;
		}

		#endregion

		#region Interface Implementations

		#region IContentItemsControl

		public string ItemContentStringFormat
		{
			get => (string) GetValue(ItemContentStringFormatProperty);
			set => SetValue(ItemContentStringFormatProperty, value);
		}

		public DataTemplate ItemContentTemplate
		{
			get => (DataTemplate) GetValue(ItemContentTemplateProperty);
			set => SetValue(ItemContentTemplateProperty, value);
		}

		public DataTemplateSelector ItemContentTemplateSelector
		{
			get => (DataTemplateSelector) GetValue(ItemContentTemplateSelectorProperty);
			set => SetValue(ItemContentTemplateSelectorProperty, value);
		}

		#endregion

		#region IHeaderedContentItemsControl

		public string ItemHeaderStringFormat
		{
			get => (string) GetValue(ItemHeaderStringFormatProperty);
			set => SetValue(ItemHeaderStringFormatProperty, value);
		}

		public DataTemplateSelector ItemHeaderTemplateSelector
		{
			get => (DataTemplateSelector) GetValue(ItemHeaderTemplateSelectorProperty);
			set => SetValue(ItemHeaderTemplateSelectorProperty, value);
		}

		public DataTemplate ItemHeaderTemplate
		{
			get => (DataTemplate) GetValue(ItemHeaderTemplateProperty);
			set => SetValue(ItemHeaderTemplateProperty, value);
		}

		#endregion

		#endregion
	}

	public class BackstageViewControlTemplateContract : ItemsControlBaseTemplateContract<BackstageViewItemsPresenter>
	{
		#region Properties

		[TemplateContractPart(Required = true)]
		public BackstageViewContentPresenter ContentPresenter { get; [UsedImplicitly] private set; }

		#endregion
	}
}
