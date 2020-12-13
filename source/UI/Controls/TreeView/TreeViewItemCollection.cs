// <copyright file="TreeViewItemCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;
using NativeControl = System.Windows.Controls.Control;

namespace Zaaml.UI.Controls.TreeView
{
	public abstract class TreeViewItemCollectionBase<TControl> : ItemCollectionBase<TControl, TreeViewItem>
		where TControl : NativeControl
	{
		protected TreeViewItemCollectionBase(TControl control) : base(control)
		{
		}

		protected override ItemGenerator<TreeViewItem> DefaultGenerator { get; } = new TreeViewItemGenerator();

		internal TreeViewItemGeneratorBase Generator
		{
			get => (TreeViewItemGeneratorBase) GeneratorCore;
			set => GeneratorCore = value;
		}
	}

	public sealed class TreeViewItemRootCollection : TreeViewItemCollectionBase<TreeViewControl>
	{
		public TreeViewItemRootCollection(TreeViewControl treeViewControl) : base(treeViewControl)
		{
		}

		internal TreeViewControl TreeViewControl => Control;

		internal override VirtualItemCollection<TreeViewItem> VirtualCollection => Control.VirtualItemCollection;
	}

	public sealed class TreeViewItemCollection : TreeViewItemCollectionBase<TreeViewItem>
	{
		private bool _isExpanded;

		internal TreeViewItemCollection(TreeViewItem treeViewItem) : base(treeViewItem)
		{
		}

		internal bool IsExpanded
		{
			get => _isExpanded;
			set
			{
				if (_isExpanded == value)
					return;

				_isExpanded = value;

				if (IsLogicalParent == false)
					return;

				if (value)
				{
					foreach (var treeViewItem in ActualItemsInternal)
						DetachLogicalCore(treeViewItem);
				}
				else
				{
					foreach (var treeViewItem in ActualItemsInternal)
						AttachLogicalCore(treeViewItem);
				}
			}
		}

		private TreeViewControl TreeViewControl => TreeViewItem.TreeViewControl;

		private TreeViewItem TreeViewItem => Control;

		internal override VirtualItemCollection<TreeViewItem> VirtualCollection => TreeViewControl?.VirtualItemCollection;

		internal override void OnCollectionChangedInternal()
		{
			base.OnCollectionChangedInternal();

			TreeViewItem.UpdateHasItemsInternal();
		}

		internal override void OnSourceChangedInternal()
		{
			base.OnSourceChangedInternal();

			TreeViewItem.UpdateHasItemsInternal();
		}
	}
}