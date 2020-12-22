// <copyright file="DefaultSelectionItemItemGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Core
{
	internal class DefaultSelectionItemItemGenerator<TSelectionItem, TItem> : SelectionItemGeneratorBase<TSelectionItem, TItem>, IDelegatedGenerator<TSelectionItem>
		where TSelectionItem : SelectionItem<TItem>, new()
		where TItem : FrameworkElement
	{
		protected override void AttachItem(TSelectionItem item, object source)
		{
			Implementation.AttachItem(item, source);
		}

		protected override TSelectionItem CreateItem(object source)
		{
			return Implementation.CreateItem(source);
		}

		protected override void DetachItem(TSelectionItem item, object source)
		{
			Implementation.DetachItem(item, source);
		}

		protected override void DisposeItem(TSelectionItem item, object source)
		{
			Implementation.DisposeItem(item, source);
		}

		public IItemGenerator<TSelectionItem> Implementation { get; set; }
	}
}