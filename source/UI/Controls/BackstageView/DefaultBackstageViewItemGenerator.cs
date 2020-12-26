// <copyright file="DefaultBackstageViewItemGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.BackstageView
{
	internal sealed class DefaultBackstageViewItemGenerator : BackstageViewItemGeneratorBase, IDelegatedGenerator<BackstageViewItem>
	{
		#region  Methods

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

		#endregion

		#region Interface Implementations

		#region IDelegatedGenerator<BackstageViewItem>

		public IItemGenerator<BackstageViewItem> Implementation { get; set; }

		#endregion

		#endregion
	}

	internal class DefaultItemTemplateBackstageViewItemGenerator : DelegateHeaderedIconContentSelectableItemGeneratorImplementation<BackstageViewItem, DefaultBackstageViewItemGenerator>
	{
		public DefaultItemTemplateBackstageViewItemGenerator(BackstageViewControl backstageViewControl) : base(backstageViewControl)
		{
		}
	}
}