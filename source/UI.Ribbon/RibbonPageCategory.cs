// <copyright file="RibbonPageCategory.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Panels;

namespace Zaaml.UI.Controls.Ribbon
{
	[ContentProperty(nameof(ItemCollection))]
	[TemplateContractType(typeof(RibbonPageCategoryTemplateContract))]
	public class RibbonPageCategory : ItemsControlBase<RibbonPageCategory, RibbonPage, RibbonPageCollection, RibbonPagesPresenter, RibbonPagesPanel>, ISelector<RibbonPage>
	{
		private static readonly DependencyPropertyKey RibbonPropertyKey = DPM.RegisterReadOnly<RibbonControl, RibbonPageCategory>
			("Ribbon", c => c.OnRibbonChanged);

		public static readonly DependencyProperty RibbonProperty = RibbonPropertyKey.DependencyProperty;

		public static readonly DependencyProperty HeaderProperty = DPM.Register<string, RibbonPageCategory>
			("Header");

		public static readonly DependencyProperty IsContextualProperty = DPM.Register<bool, RibbonPageCategory>
			("IsContextual", false);

		private Thickness _pagesPadding;
		private Size _pagesSize;

		static RibbonPageCategory()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<RibbonPageCategory>();
		}

		public RibbonPageCategory()
		{
			this.OverrideStyleKey<RibbonPageCategory>();
		}

		internal RibbonPageCategory(RibbonControl ribbonControl)
		{
			Selector = ribbonControl;
		}

		public string Header
		{
			get => (string) GetValue(HeaderProperty);
			set => SetValue(HeaderProperty, value);
		}

		public bool IsContextual
		{
			get => (bool) GetValue(IsContextualProperty);
			set => SetValue(IsContextualProperty, value);
		}

		internal Thickness PagesPadding
		{
			get => _pagesPadding;
			set
			{
				if (_pagesPadding.IsCloseTo(value))
					return;

				_pagesPadding = value;

				InvalidateMeasure();
			}
		}

		internal Size PagesSize
		{
			get => _pagesSize;
			set
			{
				if (_pagesSize.IsCloseTo(value))
					return;

				_pagesSize = value;

				InvalidateMeasure();
			}
		}

		public RibbonControl Ribbon
		{
			get => this.GetValue<RibbonControl>(RibbonProperty);
			internal set => this.SetReadOnlyValue(RibbonPropertyKey, value);
		}

		private ISelector<RibbonPage> Selector { get; }

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			var rect = arrangeBounds.Rect().GetInflated(PagesPadding.Negate());
			var implementationRoot = this.GetImplementationRoot();

			if (implementationRoot == null)
				base.ArrangeOverride(rect.Size());
			else
				implementationRoot.Arrange(rect);

			return arrangeBounds;
		}

		protected override RibbonPageCollection CreateItemCollection()
		{
			return new RibbonPageCollection(this)
			{
				ItemsHost = new RibbonPagesHost()
			};
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			var sizeWidth = PagesSize.Width;

			availableSize.Width = sizeWidth;

			var measureOverride = base.MeasureOverride(availableSize);

			measureOverride.Width = sizeWidth;

			return measureOverride;
		}

		internal override void OnItemAttachedInternal(RibbonPage item)
		{
			Ribbon?.Pages.Add(item);

			item.PageCategory = this;

			base.OnItemAttachedInternal(item);
		}

		internal override void OnItemDetachedInternal(RibbonPage item)
		{
			base.OnItemDetachedInternal(item);

			item.PageCategory = null;

			Ribbon?.Pages.Remove(item);
		}

		private void OnRibbonChanged(RibbonControl oldRibbon, RibbonControl newRibbon)
		{
			oldRibbon?.Pages.RemoveRange(ItemCollection);
			newRibbon?.Pages.AddRange(ItemCollection);
		}

		internal void UpdatePagesSize()
		{
			var pagesSize = PagesSize;
			var pagesPadding = new Thickness();
			var orientedSize = new OrientedSize(Orientation.Horizontal);
			var first = true;

			foreach (var item in ItemCollection.ActualItemsInternal)
			{
				var desiredOriented = item.GetDesiredOrientedSize(Orientation.Horizontal);

				orientedSize = orientedSize.StackSize(desiredOriented);

				if (first)
				{
					pagesPadding.Left = item.Margin.Left;
					first = false;
				}

				pagesPadding.Right = item.Margin.Right;
			}

			pagesSize.Width = orientedSize.Width;

			PagesPadding = pagesPadding;
			PagesSize = pagesSize;
		}

		public DependencyProperty SelectedIndexProperty => Selector.SelectedIndexProperty;

		public DependencyProperty SelectedItemProperty => Selector.SelectedItemProperty;

		public DependencyProperty SelectedSourceProperty => Selector.SelectedSourceProperty;

		public DependencyProperty SelectedValueProperty => Selector.SelectedValueProperty;

		public object GetValue(RibbonPage item, object source)
		{
			return Selector.GetValue(item, source);
		}

		public void OnSelectedIndexChanged(int oldIndex, int newIndex)
		{
			Selector.OnSelectedIndexChanged(oldIndex, newIndex);
		}

		public void OnSelectedItemChanged(RibbonPage oldItem, RibbonPage newItem)
		{
			Selector.OnSelectedItemChanged(oldItem, newItem);
		}

		public void OnSelectedSourceChanged(object oldSource, object newSource)
		{
			Selector.OnSelectedSourceChanged(oldSource, newSource);
		}

		public void OnSelectedValueChanged(object oldValue, object newValue)
		{
			Selector.OnSelectedValueChanged(oldValue, newValue);
		}

		public void OnSelectionChanged(Selection<RibbonPage> oldSelection, Selection<RibbonPage> newSelection)
		{
			Selector.OnSelectionChanged(oldSelection, newSelection);
		}

		private class RibbonPagesHost : IItemsHost<RibbonPage>
		{
			private RibbonPagesItemCollection ItemCollection { get; } = new RibbonPagesItemCollection();

			public ItemHostCollection<RibbonPage> Items => ItemCollection;
			
			public void BringIntoView(BringIntoViewRequest<RibbonPage> request)
			{
				throw new System.NotSupportedException();
			}

			public void EnqueueBringIntoView(BringIntoViewRequest<RibbonPage> request)
			{
				throw new System.NotSupportedException();
			}

			ItemLayoutInformation IItemsHost<RibbonPage>.GetLayoutInformation(int index)
			{
				return ItemLayoutInformation.Empty;
			}

			ItemLayoutInformation IItemsHost<RibbonPage>.GetLayoutInformation(RibbonPage item)
			{
				return ItemLayoutInformation.Empty;
			}
		}

		private class RibbonPagesItemCollection : ItemHostCollection<RibbonPage>
		{
			protected override void SyncCore(SyncAction syncAction, SyncActionData syncActionData)
			{
			}
		}
	}

	public sealed class RibbonPageCategoryTemplateContract : ItemsControlBaseTemplateContract<RibbonPagesPresenter>
	{
	}

	internal sealed class RibbonControlSelectorAdvisor : ItemCollectionSelectorAdvisor<RibbonPageCategory, RibbonPage>
	{
		public RibbonControlSelectorAdvisor(RibbonControl ribbonControl, RibbonPageCategory ribbonPageCategory) : base(ribbonPageCategory, ribbonControl.Pages)
		{
		}

		public override bool GetItemSelected(RibbonPage item)
		{
			return item.IsSelected;
		}

		public override void SetItemSelected(RibbonPage item, bool value)
		{
			item.SetCurrentValueInternal(RibbonPage.IsSelectedProperty, value);
		}
	}
}