// <copyright file="AccordionViewControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Interfaces;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;

namespace Zaaml.UI.Controls.AccordionView
{
	[TemplateContractType(typeof(AccordionViewControlTemplateContract))]
	public class AccordionViewControl : ItemsControlBase<AccordionViewControl, AccordionViewItem, AccordionViewItemCollection, AccordionViewItemsPresenter, AccordionViewPanel>, IHeaderedIconContentItemsControl
	{
		public static readonly DependencyProperty SelectionModeProperty = DPM.Register<AccordionViewSelectionMode, AccordionViewControl>
			("SelectionMode");

		public static readonly DependencyProperty ItemGeneratorProperty = DPM.Register<AccordionViewItemGeneratorBase, AccordionViewControl>
			("ItemGenerator", a => a.OnItemGeneratorChanged);

		public static readonly DependencyProperty ItemContentTemplateProperty = DPM.Register<DataTemplate, AccordionViewControl>
			("ItemContentTemplate", a => a.DefaultGeneratorImplementation.OnItemContentTemplateChanged);

		public static readonly DependencyProperty ItemContentTemplateSelectorProperty = DPM.Register<DataTemplateSelector, AccordionViewControl>
			("ItemContentTemplateSelector", a => a.DefaultGeneratorImplementation.OnItemContentTemplateSelectorChanged);

		public static readonly DependencyProperty ItemContentStringFormatProperty = DPM.Register<string, AccordionViewControl>
			("ItemContentStringFormat", a => a.DefaultGeneratorImplementation.OnItemContentStringFormatChanged);

		public static readonly DependencyProperty ItemHeaderTemplateProperty = DPM.Register<DataTemplate, AccordionViewControl>
			("ItemHeaderTemplate", a => a.DefaultGeneratorImplementation.OnItemHeaderTemplateChanged);

		public static readonly DependencyProperty ItemHeaderTemplateSelectorProperty = DPM.Register<DataTemplateSelector, AccordionViewControl>
			("ItemHeaderTemplateSelector", a => a.DefaultGeneratorImplementation.OnItemHeaderTemplateSelectorChanged);

		public static readonly DependencyProperty ItemHeaderStringFormatProperty = DPM.Register<string, AccordionViewControl>
			("ItemHeaderStringFormat", a => a.DefaultGeneratorImplementation.OnItemHeaderStringFormatChanged);

		public static readonly DependencyProperty SourceCollectionProperty = DPM.Register<IEnumerable, AccordionViewControl>
			("SourceCollection", i => i.OnSourceCollectionPropertyChangedPrivate);

		public static readonly DependencyProperty ItemContentMemberProperty = DPM.Register<string, AccordionViewControl>
			("ItemContentMember", d => d.DefaultGeneratorImplementation.OnItemContentMemberChanged);

		public static readonly DependencyProperty ItemHeaderMemberProperty = DPM.Register<string, AccordionViewControl>
			("ItemHeaderMember", d => d.DefaultGeneratorImplementation.OnItemHeaderMemberChanged);

		public static readonly DependencyProperty ItemIconMemberProperty = DPM.Register<string, AccordionViewControl>
			("ItemIconMember", d => d.DefaultGeneratorImplementation.OnItemIconMemberChanged);

		private DelegateHeaderedIconContentItemGeneratorImplementation<AccordionViewItem, DefaultAccordionViewItemGenerator> _defaultGeneratorImplementation;

		static AccordionViewControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<AccordionViewControl>();
		}

		public AccordionViewControl()
		{
			this.OverrideStyleKey<AccordionViewControl>();
		}

		private AccordionViewItemsPresenter AccordionViewItemsPresenter => TemplateContract.ItemsPresenter;

		private AccordionViewItemGeneratorBase ActualGenerator => ItemGenerator ?? DefaultGenerator;

		private AccordionViewItemGeneratorBase DefaultGenerator => DefaultGeneratorImplementation.Generator;

		private DelegateHeaderedIconContentItemGeneratorImplementation<AccordionViewItem, DefaultAccordionViewItemGenerator> DefaultGeneratorImplementation =>
			_defaultGeneratorImplementation ??= new DelegateHeaderedIconContentItemGeneratorImplementation<AccordionViewItem, DefaultAccordionViewItemGenerator>(this);

		public AccordionViewItemGeneratorBase ItemGenerator
		{
			get => (AccordionViewItemGeneratorBase)GetValue(ItemGeneratorProperty);
			set => SetValue(ItemGeneratorProperty, value);
		}

		public AccordionViewSelectionMode SelectionMode
		{
			get => (AccordionViewSelectionMode)GetValue(SelectionModeProperty);
			set => SetValue(SelectionModeProperty, value);
		}

		public IEnumerable SourceCollection
		{
			get => (IEnumerable)GetValue(SourceCollectionProperty);
			set => SetValue(SourceCollectionProperty, value);
		}

		private AccordionViewControlTemplateContract TemplateContract => (AccordionViewControlTemplateContract)TemplateContractCore;

		protected override AccordionViewItemCollection CreateItemCollection()
		{
			return new AccordionViewItemCollection(this)
			{
				Generator = ActualGenerator
			};
		}

		internal override void OnItemAttachedInternal(AccordionViewItem item)
		{
			item.AccordionViewControl = this;

			base.OnItemAttachedInternal(item);
		}

		internal override void OnItemDetachedInternal(AccordionViewItem item)
		{
			base.OnItemDetachedInternal(item);

			item.AccordionViewControl = null;
		}

		internal virtual void OnItemGeneratorChanged(AccordionViewItemGeneratorBase oldGenerator, AccordionViewItemGeneratorBase newGenerator)
		{
			ItemCollection.Generator = ActualGenerator;
		}

		private void OnSourceCollectionPropertyChangedPrivate(IEnumerable oldSource, IEnumerable newSource)
		{
			SourceCollectionCore = newSource;
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			AccordionViewItemsPresenter.AccordionViewControl = this;
		}

		protected override void OnTemplateContractDetaching()
		{
			AccordionViewItemsPresenter.AccordionViewControl = null;

			base.OnTemplateContractDetaching();
		}

		public string ItemIconMember
		{
			get => (string)GetValue(ItemIconMemberProperty);
			set => SetValue(ItemIconMemberProperty, value);
		}

		IIconSelector IIconContentItemsControl.ItemIconSelector => null;

		public string ItemHeaderMember
		{
			get => (string)GetValue(ItemHeaderMemberProperty);
			set => SetValue(ItemHeaderMemberProperty, value);
		}

		public string ItemContentMember
		{
			get => (string)GetValue(ItemContentMemberProperty);
			set => SetValue(ItemContentMemberProperty, value);
		}

		public string ItemContentStringFormat
		{
			get => (string)GetValue(ItemContentStringFormatProperty);
			set => SetValue(ItemContentStringFormatProperty, value);
		}

		public DataTemplate ItemContentTemplate
		{
			get => (DataTemplate)GetValue(ItemContentTemplateProperty);
			set => SetValue(ItemContentTemplateProperty, value);
		}

		public DataTemplateSelector ItemContentTemplateSelector
		{
			get => (DataTemplateSelector)GetValue(ItemContentTemplateSelectorProperty);
			set => SetValue(ItemContentTemplateSelectorProperty, value);
		}

		public string ItemHeaderStringFormat
		{
			get => (string)GetValue(ItemHeaderStringFormatProperty);
			set => SetValue(ItemHeaderStringFormatProperty, value);
		}

		public DataTemplateSelector ItemHeaderTemplateSelector
		{
			get => (DataTemplateSelector)GetValue(ItemHeaderTemplateSelectorProperty);
			set => SetValue(ItemHeaderTemplateSelectorProperty, value);
		}

		public DataTemplate ItemHeaderTemplate
		{
			get => (DataTemplate)GetValue(ItemHeaderTemplateProperty);
			set => SetValue(ItemHeaderTemplateProperty, value);
		}
	}
}