// <copyright file="DelegateHeaderedIconContentSelectableItemGeneratorImpl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.Core
{
	internal class DelegateHeaderedIconContentSelectableItemGeneratorImplementation<TItem, TDefaultGenerator> : DelegateHeaderedIconContentItemGeneratorImplementation<TItem, TDefaultGenerator>
		where TItem : FrameworkElement, ISelectableHeaderedIconContentItem, new()
		where TDefaultGenerator : ItemGenerator<TItem>, IDelegatedGenerator<TItem>, new()
	{
		public DelegateHeaderedIconContentSelectableItemGeneratorImplementation(IHeaderedIconContentSelectorControl iconContentSelectorControl) : base(iconContentSelectorControl)
		{
			SelectableGeneratorImplementation = new DefaultSelectableIconContentItemGeneratorImplementation<TItem>(iconContentSelectorControl.ItemValueMember, iconContentSelectorControl.ItemSelectionMember, this);
		}

		public DefaultSelectableIconContentItemGeneratorImplementation<TItem> SelectableGeneratorImplementation { get; }

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