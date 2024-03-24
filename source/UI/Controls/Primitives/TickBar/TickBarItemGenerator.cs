// <copyright file="TickBarItemGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Primitives.TickBar
{
	[ContentProperty(nameof(ItemTemplate))]
	public sealed class TickBarItemGenerator : TickBarItemGeneratorBase
	{
		public static readonly DependencyProperty ItemTemplateProperty = DPM.Register<TickBarItemTemplate, TickBarItemGenerator>
			("ItemTemplate", g => g.OnItemTemplateChanged);

		public TickBarItemGenerator()
		{
			Implementation = new TemplatedGeneratorImplementation<TickBarItem>(this);
		}

		private TemplatedGeneratorImplementation<TickBarItem> Implementation { get; }

		public TickBarItemTemplate ItemTemplate
		{
			get => (TickBarItemTemplate)GetValue(ItemTemplateProperty);
			set => SetValue(ItemTemplateProperty, value);
		}

		protected override bool SupportsRecycling => true;

		protected override void AttachItem(TickBarItem item, object source)
		{
			Implementation.AttachItem(item, source);
		}

		protected override TickBarItem CreateItem(object source)
		{
			return Implementation.CreateItem(source);
		}

		protected override void DetachItem(TickBarItem item, object source)
		{
			Implementation.DetachItem(item, source);
		}

		protected override void DisposeItem(TickBarItem item, object source)
		{
			Implementation.DisposeItem(item, source);
		}

		private void OnItemTemplateChanged()
		{
			Implementation.ItemTemplate = ItemTemplate;
		}
	}
}