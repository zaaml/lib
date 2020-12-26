// <copyright file="TreeViewItemTemplateAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.UI.Controls.Core;
using NativeContentControl = System.Windows.Controls.ContentControl;

namespace Zaaml.UI.Controls.TreeView
{
	internal sealed class TreeViewItemTemplateAdvisor : DependencyObject, ITreeViewAdvisor
	{
		private readonly Dictionary<Type, Evaluator> _evaluatorDictionary;
		private readonly TreeViewItemGenerator _generator;
		private readonly Binding _isExpandedBinding;
		private readonly Binding _sourceBinding;
		private readonly bool _staticIsExpanded;
		private readonly IEnumerable _staticSource;
		private readonly TreeViewItem _treeViewItem;

		public TreeViewItemTemplateAdvisor(TreeViewItemGenerator generator)
		{
			_generator = generator;

			_treeViewItem = (TreeViewItem) _generator.ItemTemplate.LoadContent();

			_treeViewItem.ClearValue(IconContentControl.IconProperty);
			_treeViewItem.ClearValue(NativeContentControl.ContentProperty);

			_sourceBinding = _treeViewItem.ReadLocalBinding(TreeViewItem.SourceCollectionProperty);
			_isExpandedBinding = _treeViewItem.ReadLocalBinding(TreeViewItem.IsExpandedProperty);

			if (_isExpandedBinding == null)
				_staticIsExpanded = _treeViewItem.IsExpanded;

			if (_sourceBinding == null)
				_staticSource = _treeViewItem.SourceCollection;

			if (_sourceBinding == null && _isExpandedBinding == null)
				return;

			_evaluatorDictionary = new Dictionary<Type, Evaluator>();
		}

		private Evaluator GetEvaluator(Type type)
		{
			if (_evaluatorDictionary == null)
				return new Evaluator(DataContextMemberEvaluator<object>.Empty, DataContextMemberEvaluator<bool>.Empty);

			if (_evaluatorDictionary.TryGetValue(type, out var evaluator))
				return evaluator;

			evaluator = new Evaluator(new DataContextMemberEvaluator<object>(_sourceBinding, type), new DataContextMemberEvaluator<bool>(_isExpandedBinding, type));

			_evaluatorDictionary.Add(type, evaluator);

			return evaluator;
		}

		IEnumerable ITreeViewAdvisor.GetNodes(object treeNodeData)
		{
			if (treeNodeData == null)
				return null;

			if (_treeViewItem.ItemCollection.Count > 0)
				return _generator.GetExplicitItem(treeNodeData)?.ItemCollection;

			var evaluator = GetEvaluator(treeNodeData.GetType());

			if (evaluator.SourceEvaluator.IsEmpty == false)
				// ReSharper disable once ImpureMethodCallOnReadonlyValueField
				return evaluator.SourceEvaluator.GetValue(treeNodeData) as IEnumerable;

			if (_sourceBinding == null)
				return _staticSource;

			_treeViewItem.DataContext = treeNodeData;

			BindingUtil.EnsureBindingAttached(_treeViewItem, TreeViewItem.SourceCollectionProperty);

			var itemsSource = _treeViewItem.SourceCollection;

			_treeViewItem.DataContext = null;

			return itemsSource;
		}

		bool ITreeViewAdvisor.IsExpanded(object treeNodeData)
		{
			if (treeNodeData == null)
				return false;

			var evaluator = GetEvaluator(treeNodeData.GetType());

			if (evaluator.IsExpandedEvaluator.IsEmpty == false)
			{
				// ReSharper disable once ImpureMethodCallOnReadonlyValueField
				var value = evaluator.IsExpandedEvaluator.GetValue(treeNodeData);

				return value as bool? ?? false;
			}

			if (_isExpandedBinding == null)
				return _staticIsExpanded;

			_treeViewItem.DataContext = treeNodeData;

			BindingUtil.EnsureBindingAttached(_treeViewItem, TreeViewItem.IsExpandedProperty);

			var isExpanded = _treeViewItem.IsExpanded;

			_treeViewItem.DataContext = null;

			return isExpanded;
		}

		private readonly struct Evaluator
		{
			public Evaluator(DataContextMemberEvaluator<object> sourceEvaluator, DataContextMemberEvaluator<bool> isExpandedEvaluator)
			{
				SourceEvaluator = sourceEvaluator;
				IsExpandedEvaluator = isExpandedEvaluator;
			}

			public readonly DataContextMemberEvaluator<object> SourceEvaluator;

			public readonly DataContextMemberEvaluator<bool> IsExpandedEvaluator;
		}
	}
}