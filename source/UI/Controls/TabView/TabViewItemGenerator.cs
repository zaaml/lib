// <copyright file="TabViewItemGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TabView
{
	[ContentProperty(nameof(ItemTemplate))]
	public sealed class TabViewItemGenerator : TabViewItemGeneratorBase
	{
		public static readonly DependencyProperty ItemTemplateProperty = DPM.Register<TabViewItemTemplate, TabViewItemGenerator>
			("ItemTemplate", g => g.OnItemTemplateChanged);

		public TabViewItemGenerator()
		{
			Implementation = new TemplatedGeneratorImpl<TabViewItem>(this);
		}

		private TemplatedGeneratorImpl<TabViewItem> Implementation { get; }

		public TabViewItemTemplate ItemTemplate
		{
			get => (TabViewItemTemplate) GetValue(ItemTemplateProperty);
			set => SetValue(ItemTemplateProperty, value);
		}

		protected override bool SupportsRecycling => true;

		protected override void AttachItem(TabViewItem item, object itemSource)
		{
			Implementation.AttachItem(item, itemSource);
		}

		protected override TabViewItem CreateItem(object itemSource)
		{
			return Implementation.CreateItem(itemSource);
		}

		protected override void DetachItem(TabViewItem item, object itemSource)
		{
			Implementation.DetachItem(item, itemSource);
		}

		protected override void DisposeItem(TabViewItem item, object itemSource)
		{
			Implementation.DisposeItem(item, itemSource);
		}

		private void OnItemTemplateChanged()
		{
			Implementation.ItemTemplate = ItemTemplate;
		}
	}
}