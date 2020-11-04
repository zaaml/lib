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
	}
}