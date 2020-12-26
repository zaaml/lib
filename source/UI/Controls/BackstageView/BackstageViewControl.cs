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
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.BackstageView
{
	[ContentProperty(nameof(ItemCollection))]
	[TemplateContractType(typeof(BackstageViewControlTemplateContract))]
	public class BackstageViewControl : IndexedSelectorBase<BackstageViewControl, BackstageViewItem, BackstageViewItemCollection, BackstageViewItemsPresenter, BackstageViewItemsPanel>, IHeaderedIconContentSelectorControl
	{
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

		public static readonly DependencyProperty SourceCollectionProperty = DPM.Register<IEnumerable, BackstageViewControl>
			("SourceCollection", i => i.OnSourceCollectionPropertyChangedPrivate);

		public static readonly DependencyProperty ItemContentMemberProperty = DPM.Register<string, BackstageViewControl>
			("ItemContentMember", d => d.DefaultGeneratorImpl.OnItemContentMemberChanged);

		public static readonly DependencyProperty ItemHeaderMemberProperty = DPM.Register<string, BackstageViewControl>
			("ItemHeaderMember", d => d.DefaultGeneratorImpl.OnItemHeaderMemberChanged);

		public static readonly DependencyProperty ItemIconMemberProperty = DPM.Register<string, BackstageViewControl>
			("ItemIconMember", d => d.DefaultGeneratorImpl.OnItemIconMemberChanged);

		public static readonly DependencyProperty ItemValueMemberProperty = DPM.Register<string, BackstageViewControl>
			("ItemValueMember", d => d.DefaultGeneratorImpl.SelectableGeneratorImplementation.OnItemValueMemberChanged);

		public static readonly DependencyProperty ItemSelectionMemberProperty = DPM.Register<string, BackstageViewControl>
			("ItemSelectionMember", d => d.DefaultGeneratorImpl.SelectableGeneratorImplementation.OnItemSelectionMemberChanged);
		
		private readonly DelayStateTrigger _delayTrigger;
		private readonly BackstageViewHost _host;
		private DefaultItemTemplateBackstageViewItemGenerator _defaultGeneratorImpl;
		public event EventHandler IsOpenChanged;

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

		private BackstageViewItemGeneratorBase ActualGenerator => ItemGenerator ?? DefaultGenerator;

		public Duration CloseDelay
		{
			get => (Duration) GetValue(CloseDelayProperty);
			set => SetValue(CloseDelayProperty, value);
		}

		private BackstageViewContentPresenter ContentPresenter => TemplateContract.ContentPresenter;

		private BackstageViewItemGeneratorBase DefaultGenerator => DefaultGeneratorImpl.Generator;

		private DefaultItemTemplateBackstageViewItemGenerator DefaultGeneratorImpl =>
			_defaultGeneratorImpl ??= new DefaultItemTemplateBackstageViewItemGenerator(this);

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

		public Duration OpenDelay
		{
			get => (Duration) GetValue(OpenDelayProperty);
			set => SetValue(OpenDelayProperty, value);
		}

		public IEnumerable SourceCollection
		{
			get => (IEnumerable) GetValue(SourceCollectionProperty);
			set => SetValue(SourceCollectionProperty, value);
		}

		private BackstageViewControlTemplateContract TemplateContract => (BackstageViewControlTemplateContract) TemplateContractInternal;

		public void Activate(BackstageViewItem backstageViewItem)
		{
			if (ReferenceEquals(SelectedItem, backstageViewItem) == false)
				SelectorController.SelectItem(backstageViewItem);
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

		protected override bool GetIsSelected(BackstageViewItem item)
		{
			return item.IsSelected;
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
			ItemCollection.Generator = ActualGenerator;
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

		private void OnSourceCollectionPropertyChangedPrivate(IEnumerable oldSource, IEnumerable newSource)
		{
			SourceCollectionCore = newSource;
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
			SelectorController.SelectItem(backstageViewItem);
		}

		protected override void SetIsSelected(BackstageViewItem item, bool value)
		{
			item.SetCurrentValueInternal(BackstageViewItem.IsSelectedProperty, value);
		}

		protected virtual void UpdateSelectedContent()
		{
			if (ContentPresenter == null)
				return;

			ContentPresenter.Content = SelectedItem?.ContentHost;
		}

		public string ItemSelectionMember
		{
			get => (string) GetValue(ItemSelectionMemberProperty);
			set => SetValue(ItemSelectionMemberProperty, value);
		}

		public string ItemValueMember
		{
			get => (string) GetValue(ItemValueMemberProperty);
			set => SetValue(ItemValueMemberProperty, value);
		}

		public string ItemIconMember
		{
			get => (string) GetValue(ItemIconMemberProperty);
			set => SetValue(ItemIconMemberProperty, value);
		}

		public string ItemHeaderMember
		{
			get => (string) GetValue(ItemHeaderMemberProperty);
			set => SetValue(ItemHeaderMemberProperty, value);
		}

		public string ItemContentMember
		{
			get => (string) GetValue(ItemContentMemberProperty);
			set => SetValue(ItemContentMemberProperty, value);
		}

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
	}

	public class BackstageViewControlTemplateContract : IndexedSelectorBaseTemplateContract<BackstageViewItemsPresenter>
	{
		[TemplateContractPart(Required = true)]
		public BackstageViewContentPresenter ContentPresenter { get; [UsedImplicitly] private set; }
	}
}