// <copyright file="ListViewItemCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
	public sealed class ListViewItemCollection : ItemCollectionBase<ListViewControl, ListViewItem>
	{
		internal ListViewItemCollection(ListViewControl listViewControl) : base(listViewControl)
		{
		}

		protected override ItemGenerator<ListViewItem> DefaultGenerator { get; } = new ListViewItemGenerator();

		internal ListViewItemGeneratorBase Generator
		{
			get => (ListViewItemGeneratorBase) GeneratorCore;
			set => GeneratorCore = value;
		}

		internal override VirtualItemCollection<ListViewItem> VirtualCollection => Control.VirtualItemCollection;

		private protected override void OnItemAdded(ListViewItem item)
		{
			base.OnItemAdded(item);

			Control.OnItemAttachedCollection(item);
		}

		private protected override void OnItemRemoved(ListViewItem item)
		{
			base.OnItemRemoved(item);

			Control.OnItemDetachedCollection(item);
		}
	}
}