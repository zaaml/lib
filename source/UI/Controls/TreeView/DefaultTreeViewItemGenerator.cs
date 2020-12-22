// <copyright file="DefaultTreeViewItemGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TreeView
{
	internal sealed class DefaultTreeViewItemGenerator : TreeViewItemGeneratorBase, IDelegatedGenerator<TreeViewItem>
	{
		protected override bool SupportsRecycling => true;

		protected override void AttachItem(TreeViewItem item, object source)
		{
			Implementation.AttachItem(item, source);
		}

		protected override TreeViewItem CreateItem(object source)
		{
			return Implementation.CreateItem(source);
		}

		protected override void DetachItem(TreeViewItem item, object source)
		{
			Implementation.DetachItem(item, source);
		}

		protected override void DisposeItem(TreeViewItem item, object source)
		{
			Implementation.DisposeItem(item, source);
		}

		protected override IEnumerable GetTreeNodes(object treeNodeData)
		{
			return ((DefaultItemTemplateTreeViewItemGenerator) Implementation).GetTreeNodes(treeNodeData);
		}

		protected override bool IsExpanded(object treeNodeData)
		{
			return false;
		}

		public IItemGenerator<TreeViewItem> Implementation { get; set; }
	}

	internal class DefaultItemTemplateTreeViewItemGenerator : DelegateIconContentSelectableItemGeneratorImpl<TreeViewItem, DefaultTreeViewItemGenerator>
	{
		private readonly TreeNodesEvaluator _treeNodesEvaluator = new TreeNodesEvaluator();
		private string _itemSourceCollectionMember;

		public DefaultItemTemplateTreeViewItemGenerator(TreeViewControl treeViewControl) : base(treeViewControl)
		{
		}

		public string ItemSourceCollectionMember
		{
			get => _itemSourceCollectionMember;
			set
			{
				if (string.Equals(_itemSourceCollectionMember, value))
					return;

				Generator.OnGeneratorChangingInt();

				_itemSourceCollectionMember = value;
				CreateItemSourceCollectionMemberBinding();

				Generator.OnGeneratorChangedInt();
			}
		}

		private Binding ItemSourceCollectionMemberBinding { get; set; }

		private Binding ItemSourceCollectionMemberEvaluatorBinding { get; set; }

		public override void AttachItem(TreeViewItem item, object source)
		{
			base.AttachItem(item, source);

			ItemGenerator<TreeViewItem>.InstallBinding(item, TreeViewItem.SourceCollectionProperty, ItemSourceCollectionMemberBinding);
		}

		private void CreateItemSourceCollectionMemberBinding()
		{
			ItemSourceCollectionMemberBinding = _itemSourceCollectionMember != null ? new Binding(_itemSourceCollectionMember) : null;
			ItemSourceCollectionMemberEvaluatorBinding = _itemSourceCollectionMember != null ? new Binding($"Source.{_itemSourceCollectionMember}") {FallbackValue = null, RelativeSource = RelativeSource.Self} : null;

			if (ItemSourceCollectionMemberEvaluatorBinding != null)
				_treeNodesEvaluator.SetBinding(TreeNodesEvaluator.SourceCollectionProperty, ItemSourceCollectionMemberEvaluatorBinding);
			else
				_treeNodesEvaluator.ClearValue(TreeNodesEvaluator.SourceCollectionProperty);
		}

		public override void DetachItem(TreeViewItem item, object source)
		{
			ItemGenerator<TreeViewItem>.UninstallBinding(item, TreeViewItem.SourceCollectionProperty, ItemSourceCollectionMemberBinding);

			base.DetachItem(item, source);
		}

		public IEnumerable GetTreeNodes(object treeNodeData)
		{
			if (ItemSourceCollectionMemberEvaluatorBinding == null)
				return null;

			_treeNodesEvaluator.Source = treeNodeData;

			var treeNodes = _treeNodesEvaluator.SourceCollection;

			_treeNodesEvaluator.Source = null;

			return treeNodes;
		}

		public void OnItemSourceCollectionMemberChanged(string oldSourceCollectionMember, string newSourceCollectionMember)
		{
			ItemSourceCollectionMember = newSourceCollectionMember;
		}

		private sealed class TreeNodesEvaluator : DependencyObject
		{
			public static readonly DependencyProperty SourceProperty = DPM.Register<object, TreeNodesEvaluator>
				("Source");

			public static readonly DependencyProperty SourceCollectionProperty = DPM.Register<IEnumerable, TreeNodesEvaluator>
				("SourceCollection");

			public object Source
			{
				set => SetValue(SourceProperty, value);
			}

			public IEnumerable SourceCollection => (IEnumerable) GetValue(SourceCollectionProperty);
		}
	}
}