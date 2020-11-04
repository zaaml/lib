// <copyright file="TreeViewItemGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TreeView
{
	[ContentProperty(nameof(ItemTemplate))]
	public sealed class TreeViewItemGenerator : TreeViewItemGeneratorBase
	{
		public static readonly DependencyProperty ItemTemplateProperty = DPM.Register<TreeViewItemTemplate, TreeViewItemGenerator>
			("ItemTemplate", g => g.OnItemTemplateChanged);

		private readonly Dictionary<object, TreeViewItem> _explicitItemsDictionary = new Dictionary<object, TreeViewItem>();

		private ITreeViewAdvisor _advisor;

		public TreeViewItemGenerator()
		{
			Implementation = new TemplatedGeneratorImpl<TreeViewItem>(this);
		}

		private TemplatedGeneratorImpl<TreeViewItem> Implementation { get; }

		public TreeViewItemTemplate ItemTemplate
		{
			get => (TreeViewItemTemplate) GetValue(ItemTemplateProperty);
			set => SetValue(ItemTemplateProperty, value);
		}

		protected override bool SupportsRecycling => true;

		protected override void AttachItem(TreeViewItem item, object itemSource)
		{
			Implementation.AttachItem(item, itemSource);
		}

		protected override TreeViewItem CreateItem(object itemSource)
		{
			var treeViewItem = Implementation.CreateItem(itemSource);

			if (treeViewItem.Items.Count > 0)
				_explicitItemsDictionary[itemSource] = treeViewItem;

			return treeViewItem;
		}

		protected override void DetachItem(TreeViewItem item, object itemSource)
		{
			Implementation.DetachItem(item, itemSource);
		}

		protected override void DisposeItem(TreeViewItem item, object itemSource)
		{
			Implementation.DisposeItem(item, itemSource);

			_explicitItemsDictionary.Remove(item);
		}

		internal TreeViewItem GetExplicitItem(object itemSource)
		{
			return _explicitItemsDictionary.TryGetValue(itemSource, out var item) ? item : null;
		}

		protected override IEnumerable GetTreeNodes(object treeNodeData)
		{
			return _advisor?.GetNodes(treeNodeData) ?? Enumerable.Empty<object>();
		}

		protected override bool IsExpanded(object treeNodeData)
		{
			return _advisor?.IsExpanded(treeNodeData) ?? false;
		}

		private void OnItemTemplateChanged()
		{
			Implementation.ItemTemplate = ItemTemplate;

			_advisor = ItemTemplate != null ? new TreeViewItemTemplateAdvisor(this) : null;
		}
	}
}