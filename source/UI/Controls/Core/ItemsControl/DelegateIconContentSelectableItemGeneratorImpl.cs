// <copyright file="DelegateIconContentSelectableItemGeneratorImpl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.Core
{
	internal class DelegateIconContentSelectableItemGeneratorImpl<TItem, TDefaultGenerator> : DelegateIconContentItemGeneratorImpl<TItem, TDefaultGenerator>
		where TItem : FrameworkElement, ISelectableIconContentItem, new()
		where TDefaultGenerator : ItemGenerator<TItem>, IDelegatedGenerator<TItem>, new()
	{
		public DelegateIconContentSelectableItemGeneratorImpl(IIconContentSelectorControl iconContentSelectorControl) : base(iconContentSelectorControl)
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