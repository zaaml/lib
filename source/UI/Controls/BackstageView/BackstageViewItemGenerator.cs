// <copyright file="BackstageViewItemGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.BackstageView
{
	[ContentProperty(nameof(ItemTemplate))]
	public sealed class BackstageViewItemGenerator : BackstageViewItemGeneratorBase
	{
		public static readonly DependencyProperty ItemTemplateProperty = DPM.Register<BackstageViewItemTemplate, BackstageViewItemGenerator>
			("ItemTemplate", g => g.OnItemTemplateChanged);

		public BackstageViewItemGenerator()
		{
			Implementation = new TemplatedGeneratorImplementation<BackstageViewItem>(this);
		}

		private TemplatedGeneratorImplementation<BackstageViewItem> Implementation { get; }

		public BackstageViewItemTemplate ItemTemplate
		{
			get => (BackstageViewItemTemplate) GetValue(ItemTemplateProperty);
			set => SetValue(ItemTemplateProperty, value);
		}

		protected override bool SupportsRecycling => true;

		protected override void AttachItem(BackstageViewItem item, object source)
		{
			Implementation.AttachItem(item, source);
		}

		protected override BackstageViewItem CreateItem(object source)
		{
			return Implementation.CreateItem(source);
		}

		protected override void DetachItem(BackstageViewItem item, object source)
		{
			Implementation.DetachItem(item, source);
		}

		protected override void DisposeItem(BackstageViewItem item, object source)
		{
			Implementation.DisposeItem(item, source);
		}

		private void OnItemTemplateChanged()
		{
			Implementation.ItemTemplate = ItemTemplate;
		}
	}
}