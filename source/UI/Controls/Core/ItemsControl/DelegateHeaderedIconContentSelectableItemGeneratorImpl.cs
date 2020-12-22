// <copyright file="DelegateHeaderedIconContentSelectableItemGeneratorImpl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.Core
{
	internal class DelegateHeaderedIconContentSelectableItemGeneratorImpl<TItem, TDefaultGenerator> : DelegateHeaderedIconContentItemGeneratorImpl<TItem, TDefaultGenerator>
		where TItem : FrameworkElement, ISelectableHeaderedIconContentItem, new()
		where TDefaultGenerator : ItemGenerator<TItem>, IDelegatedGenerator<TItem>, new()
	{
		public DelegateHeaderedIconContentSelectableItemGeneratorImpl(IHeaderedIconContentSelectorControl iconContentSelectorControl) : base(iconContentSelectorControl)
		{
			SelectableGeneratorImplementation = new DefaultSelectableIconContentItemGeneratorImpl<TItem>(iconContentSelectorControl.ItemValueMember, iconContentSelectorControl.ItemSelectionMember, this);
		}

		public DefaultSelectableIconContentItemGeneratorImpl<TItem> SelectableGeneratorImplementation { get; }

		public override void AttachItem(TItem item, object source)
		{
			base.AttachItem(item, source);

			SelectableGeneratorImplementation.AttachItem(item, source);
		}

		public override void DetachItem(TItem item, object source)
		{
			SelectableGeneratorImplementation.DetachItem(item, source);

			base.DetachItem(item, source);
		}
	}
}