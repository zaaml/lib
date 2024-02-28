// <copyright file="ItemsControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Panels.Core;
using NativeControl = System.Windows.Controls.Control;
using Style = System.Windows.Style;

namespace Zaaml.UI.Controls.Core
{
	[TemplateContractType(typeof(ItemsControlTemplateContract))]
	public class ItemsControl : ItemsControlBase<ItemsControl, NativeControl, ItemCollection, ItemsPresenter, ItemsPanel<NativeControl>>
	{
		public static readonly DependencyProperty ItemsPresenterTemplateProperty = DPM.Register<ControlTemplate, ItemsControl>
			("ItemsPresenterTemplate");

		public static readonly DependencyProperty SourceCollectionProperty = DPM.Register<IEnumerable, ItemsControl>
			("SourceCollection", i => i.OnSourceCollectionPropertyChangedPrivate);

		public static readonly DependencyProperty ItemStyleProperty = DPM.Register<Style, ItemsControl>
			("ItemStyle", i => i.OnItemStyleChanged);

		static ItemsControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ItemsControl>();
		}

		public ItemsControl()
		{
			this.OverrideStyleKey<ItemsControl>();

			Generator = new ItemsGeneratorInt(this);
		}

		private ItemGenerator<NativeControl> Generator { get; }

		public ControlTemplate ItemsPresenterTemplate
		{
			get => (ControlTemplate)GetValue(ItemsPresenterTemplateProperty);
			set => SetValue(ItemsPresenterTemplateProperty, value);
		}

		public Style ItemStyle
		{
			get => (Style)GetValue(ItemStyleProperty);
			set => SetValue(ItemStyleProperty, value);
		}

		public IEnumerable SourceCollection
		{
			get => (IEnumerable)GetValue(SourceCollectionProperty);
			set => SetValue(SourceCollectionProperty, value);
		}

		protected virtual void AttachItem(FrameworkElement item, object source)
		{
			if (item is ContentPresenter contentPresenter)
				contentPresenter.Content = source;
		}

		protected virtual NativeControl CreateItem(object source)
		{
			return source as NativeControl ?? new ContentControl();
		}

		protected override ItemCollection CreateItemCollection()
		{
			return new ItemCollection(this)
			{
				Generator = Generator
			};
		}

		protected virtual void DetachItem(FrameworkElement item, object source)
		{
			var contentPresenter = item as ContentPresenter;

			contentPresenter?.ClearValue(System.Windows.Controls.ContentPresenter.ContentProperty);
		}

		protected virtual void DisposeItem(FrameworkElement item, object source)
		{
		}

		internal override void OnItemAttachedInternal(NativeControl item)
		{
			base.OnItemAttachedInternal(item);

			item.ApplyItemStyle(StyleProperty, ItemStyle);
		}

		internal override void OnItemDetachedInternal(NativeControl item)
		{
			base.OnItemDetachedInternal(item);

			item.UndoItemStyle(StyleProperty, ItemStyle);
		}

		private void OnItemStyleChanged(Style oldStyle, Style newStyle)
		{
			this.ChangeStyle(StyleProperty, oldStyle, newStyle);
		}

		private void OnSourceCollectionPropertyChangedPrivate(IEnumerable oldSource, IEnumerable newSource)
		{
			SourceCollectionCore = newSource;
		}

		private sealed class ItemsGeneratorInt : ItemGenerator<NativeControl>
		{
			private readonly ItemsControl _itemsControl;

			public ItemsGeneratorInt(ItemsControl itemsControl)
			{
				_itemsControl = itemsControl;
			}

			protected override void AttachItem(NativeControl item, object source)
			{
				_itemsControl.AttachItem(item, source);
			}

			protected override NativeControl CreateItem(object source)
			{
				return _itemsControl.CreateItem(source);
			}

			protected override void DetachItem(NativeControl item, object source)
			{
				_itemsControl.DetachItem(item, source);
			}

			protected override void DisposeItem(NativeControl item, object source)
			{
				_itemsControl.DisposeItem(item, source);
			}
		}
	}

	public class ItemsControlTemplateContract : ItemsControlBaseTemplateContract<ItemsPresenter>
	{
	}
}