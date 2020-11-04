// <copyright file="OverflowItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Panels.Core;
using Control = System.Windows.Controls.Control;

namespace Zaaml.UI.Controls.Primitives.Overflow
{
	public sealed class OverflowPanel : Panel
	{
		#region Properties

		internal IOverflowItem Item { get; set; }

		#endregion

		#region  Methods

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			return Item?.MeasurePanel(availableSize) ?? XamlConstants.ZeroSize;
		}

		#endregion
	}

	internal interface IOverflowItem
	{
		#region  Methods

		Size MeasurePanel(Size availableSize);

		#endregion
	}


	public sealed class OverflowItem<TItem> : FixedTemplateContentControl<OverflowPanel, TItem>, IOverflowItem
		where TItem : Control, IOverflowableItem
	{
		#region Ctors

		internal OverflowItem(OverflowItemController<TItem> controller, OverflowItemKind kind)
		{
			Controller = controller;
			Kind = kind;
		}

		#endregion

		#region Properties

		internal OverflowItemController<TItem> Controller { get; }

		protected override bool IsLogicalParent => false;

		internal TItem Item => Controller.Item;

		internal OverflowItemKind Kind { get; }

		#endregion

		#region  Methods

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

		#endregion

		#region Interface Implementations

		#region IOverflowItem

		Size IOverflowItem.MeasurePanel(Size availableSize)
		{
			var item = Item;

			if (item == null)
				return XamlConstants.ZeroSize;

			if (Kind == OverflowItemKind.Overflow && Item.IsOverflow == false)
				return XamlConstants.ZeroSize;

			var fre = (FrameworkElement) item;

			fre.Measure(availableSize);

			return fre.DesiredSize;
		}

		#endregion

		#endregion
	}
}