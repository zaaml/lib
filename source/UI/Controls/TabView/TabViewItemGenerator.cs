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
			Implementation = new TemplatedGeneratorImplementation<TabViewItem>(this);
		}

		private TemplatedGeneratorImplementation<TabViewItem> Implementation { get; }

		public TabViewItemTemplate ItemTemplate
		{
			get => (TabViewItemTemplate) GetValue(ItemTemplateProperty);
			set => SetValue(ItemTemplateProperty, value);
		}

		protected override bool SupportsRecycling => true;

		protected override void AttachItem(TabViewItem item, object source)
		{
			Implementation.AttachItem(item, source);
		}

		protected override TabViewItem CreateItem(object source)
		{
			return Implementation.CreateItem(source);
		}

		protected override void DetachItem(TabViewItem item, object source)
		{
			Implementation.DetachItem(item, source);
		}

		protected override void DisposeItem(TabViewItem item, object source)
		{
			Implementation.DisposeItem(item, source);
		}

		private void OnItemTemplateChanged()
		{
			Implementation.ItemTemplate = ItemTemplate;
		}
	}
}