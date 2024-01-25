// <copyright file="OverflowItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.UI.Controls.Core;
using Control = System.Windows.Controls.Control;

namespace Zaaml.UI.Controls.Primitives.Overflow
{
	public sealed class OverflowItem<TItem> : FixedTemplateContentControl<OverflowPanel, TItem>, IOverflowItem
		where TItem : Control, IOverflowableItem
	{
		internal OverflowItem(OverflowItemController<TItem> controller, OverflowItemKind kind)
		{
			Controller = controller;
			Kind = kind;
		}

		internal OverflowItemController<TItem> Controller { get; }

		protected override bool IsLogicalParent => false;

		internal TItem Item => Controller.Item;

		internal OverflowItemKind Kind { get; }

		protected override void ApplyTemplateOverride()
		{
			base.ApplyTemplateOverride();

			TemplateRoot.Item = this;
		}

		protected override void UndoTemplateOverride()
		{
			TemplateRoot.Item = null;

			base.UndoTemplateOverride();
		}

		Size IOverflowItem.MeasurePanel(Size availableSize)
		{
			var item = Item;

			if (item == null)
				return XamlConstants.ZeroSize;

			if (Kind == OverflowItemKind.Overflow && Item.IsOverflow == false)
				return XamlConstants.ZeroSize;

			var fre = (FrameworkElement)item;

			fre.Measure(availableSize);

			return fre.DesiredSize;
		}
	}
}