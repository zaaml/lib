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

		public static readonly DependencyProperty ItemTemplateSelectorProperty = DPM.Register<TreeViewItemTemplateSelector, TreeViewItemGenerator>
			("ItemTemplateSelector", g => g.OnItemTemplateSelectorChanged);

		private readonly Dictionary<object, TreeViewItem> _explicitItemsDictionary = new();

		private ITreeViewAdvisor _advisor;

		public TreeViewItemGenerator()
		{
			Implementation = new TemplatedGeneratorImplementation<TreeViewItem>(this);
		}

		private TemplatedGeneratorImplementation<TreeViewItem> Implementation { get; }

		public TreeViewItemTemplate ItemTemplate
		{
			get => (TreeViewItemTemplate)GetValue(ItemTemplateProperty);
			set => SetValue(ItemTemplateProperty, value);
		}

		public TreeViewItemTemplateSelector ItemTemplateSelector
		{
			get => (TreeViewItemTemplateSelector)GetValue(ItemTemplateSelectorProperty);
			set => SetValue(ItemTemplateSelectorProperty, value);
		}

		protected override bool SupportsRecycling => true;

		protected override void AttachItem(TreeViewItem item, object source)
		{
			Implementation.AttachItem(item, source);
		}

		protected override TreeViewItem CreateItem(object source)
		{
			var treeViewItem = Implementation.CreateItem(source);

			if (treeViewItem.ItemCollection.Count > 0)
				_explicitItemsDictionary[source] = treeViewItem;

			return treeViewItem;
		}

		protected override void DetachItem(TreeViewItem item, object source)
		{
			Implementation.DetachItem(item, source);
		}

		protected override void DisposeItem(TreeViewItem item, object source)
		{
			Implementation.DisposeItem(item, source);

			_explicitItemsDictionary.Remove(item);
		}

		internal TreeViewItem GetExplicitItem(object source)
		{
			return _explicitItemsDictionary.TryGetValue(source, out var item) ? item : null;
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

			UpdateAdvisor();
		}

		private void UpdateAdvisor()
		{
			_advisor = ItemTemplate != null || ItemTemplateSelector != null ? new TreeViewItemTemplateAdvisor(this) : null;
		}

		private void OnItemTemplateSelectorChanged()
		{
			Implementation.ItemTemplateSelector = ItemTemplateSelector;

			UpdateAdvisor();
		}
	}
}