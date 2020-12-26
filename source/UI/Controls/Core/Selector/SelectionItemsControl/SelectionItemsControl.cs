// <copyright file="SelectionItemsControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.Core
{
	public class SelectionItemsControl<TSelectionItem, TItem> : ItemsControlBase<SelectionItemsControl<TSelectionItem, TItem>, TSelectionItem, SelectionItemCollection<TSelectionItem, TItem>, SelectionItemsPresenter<TSelectionItem, TItem>,
		SelectionItemsPanel<TSelectionItem, TItem>>, IContentItemsControl
		where TSelectionItem : SelectionItem<TItem>, new()
		where TItem : FrameworkElement
	{
		public static readonly DependencyProperty ItemContentTemplateProperty = DPM.Register<DataTemplate, SelectionItemsControl<TSelectionItem, TItem>>
			("ItemContentTemplate", l => l.DefaultGeneratorImpl.OnItemContentTemplateChanged);

		public static readonly DependencyProperty ItemContentTemplateSelectorProperty = DPM.Register<DataTemplateSelector, SelectionItemsControl<TSelectionItem, TItem>>
			("ItemContentTemplateSelector", l => l.DefaultGeneratorImpl.OnItemContentTemplateSelectorChanged);

		public static readonly DependencyProperty ItemContentStringFormatProperty = DPM.Register<string, SelectionItemsControl<TSelectionItem, TItem>>
			("ItemContentStringFormat", l => l.DefaultGeneratorImpl.OnItemContentStringFormatChanged);

		public static readonly DependencyProperty SelectionCollectionProperty = DPM.Register<SelectionCollectionBase<TItem>, SelectionItemsControl<TSelectionItem, TItem>>
			("SelectionCollection", d => d.OnSelectionCollectionPropertyChangedPrivate);

		public static readonly DependencyProperty ContentModeProperty = DPM.Register<SelectionItemContentMode, SelectionItemsControl<TSelectionItem, TItem>>
			("ContentMode", SelectionItemContentMode.None, d => d.OnContentModePropertyChangedPrivate);

		public static readonly DependencyProperty ItemContentMemberProperty = DPM.Register<string, SelectionItemsControl<TSelectionItem, TItem>>
			("ItemContentMember", d => d.DefaultGeneratorImpl.OnItemContentMemberChanged);

		private DelegateContentItemGenerator<TSelectionItem, TItem> _defaultGeneratorImpl;

		private SelectionItemGeneratorBase<TSelectionItem, TItem> ActualGenerator => DefaultGenerator;

		public SelectionItemContentMode ContentMode
		{
			get => (SelectionItemContentMode) GetValue(ContentModeProperty);
			set => SetValue(ContentModeProperty, value);
		}

		private SelectionItemGeneratorBase<TSelectionItem, TItem> DefaultGenerator => DefaultGeneratorImpl.Generator;

		private DelegateContentItemGenerator<TSelectionItem, TItem> DefaultGeneratorImpl =>
			_defaultGeneratorImpl ??= new DelegateContentItemGenerator<TSelectionItem, TItem>(this);

		private protected virtual bool IsAttachDetachOverriden => false;

		internal bool IsAttachDetachOverridenInternal => IsAttachDetachOverriden;

		public SelectionCollectionBase<TItem> SelectionCollection
		{
			get => (SelectionCollectionBase<TItem>) GetValue(SelectionCollectionProperty);
			set => SetValue(SelectionCollectionProperty, value);
		}

		private protected virtual void AttachOverride(SelectionItem<TItem> selectionItem, Selection<TItem> selection)
		{
		}

		internal void AttachOverrideInternal(SelectionItem<TItem> selectionItem, Selection<TItem> selection)
		{
			AttachOverride(selectionItem, selection);
		}

		protected override SelectionItemCollection<TSelectionItem, TItem> CreateItemCollection()
		{
			return new SelectionItemCollection<TSelectionItem, TItem>(this)
			{
				Generator = ActualGenerator
			};
		}

		protected override TemplateContract CreateTemplateContract()
		{
			return new SelectionItemsControlTemplateContract<TSelectionItem, TItem>();
		}

		private protected virtual void DetachOverride(SelectionItem<TItem> selectionItem, Selection<TItem> selection)
		{
		}

		internal void DetachOverrideInternal(SelectionItem<TItem> selectionItem, Selection<TItem> selection)
		{
			DetachOverride(selectionItem, selection);
		}

		private void InvalidateItemsHost()
		{
			ItemsPresenter?.ItemsHostInternal?.InvalidateMeasure();
		}

		private void OnContentModePropertyChangedPrivate(SelectionItemContentMode oldValue, SelectionItemContentMode newValue)
		{
			DefaultGeneratorImpl.OnContentModeChanged();
		}

		private void OnSelectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			InvalidateItemsHost();
		}

		private void OnSelectionCollectionPropertyChangedPrivate(SelectionCollectionBase<TItem> oldValue, SelectionCollectionBase<TItem> newValue)
		{
			if (ReferenceEquals(oldValue, newValue))
				return;

			if (oldValue != null)
				oldValue.CollectionChanged -= OnSelectionCollectionChanged;

			if (newValue != null)
				newValue.CollectionChanged += OnSelectionCollectionChanged;

			SourceCollectionCore = newValue;

			InvalidateItemsHost();
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
	}
}