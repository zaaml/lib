﻿// <copyright file="GeneratorDataTemplateHelper.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.Data;

namespace Zaaml.UI.Controls.Core
{
	internal class GeneratorDataTemplateHelper<TItemBase, TItem> where TItemBase : FrameworkElement where TItem : TItemBase, new()
	{
		private DataTemplate _dataTemplate;

		private DataTemplateBindingHelper _dataTemplateBindingHelper;

		public DataTemplate DataTemplate
		{
			get => _dataTemplate;
			set
			{
				if (ReferenceEquals(_dataTemplate, value))
					return;

				_dataTemplate = value;
				_dataTemplateBindingHelper = new DataTemplateBindingHelper(_dataTemplate);
			}
		}

		public void AttachDataContext(TItemBase item, object source)
		{
			item.SetValue(FrameworkElement.DataContextProperty, source);

			_dataTemplateBindingHelper?.EnsureBinding(item);
		}

		public TItemBase Load(object source)
		{
			var itemTemplate = DataTemplate;

			if (itemTemplate == null)
				return new TItem();

			return (TItem) DataTemplate.LoadContent();
		}
	}

	internal sealed class DataTemplateBindingHelper
	{
		private readonly BindingTree _bindingTree;

		public DataTemplateBindingHelper(DataTemplate dataTemplate)
		{
			if (dataTemplate == null)
				return;

			_bindingTree = new BindingTree(dataTemplate.LoadContent());
		}

		public void EnsureBinding(DependencyObject dependencyObject)
		{
			_bindingTree?.EnsureBinding(dependencyObject);
		}

		private sealed class BindingTree
		{
			private readonly List<DependencyProperty> _boundProperties;
			private readonly Dictionary<DependencyProperty, BindingTree> _childNodes;

			public BindingTree(DependencyObject dependencyObject)
			{
				var lve = dependencyObject.GetLocalValueEnumerator();

				while (lve.MoveNext())
				{
					var entry = lve.Current;

					if (BindingOperations.IsDataBound(dependencyObject, entry.Property))
					{
						if (entry.Value is BindingExpression bindingExpression)
						{
							var parentBinding = bindingExpression.ParentBinding;

							if (parentBinding.Source == null && parentBinding.RelativeSource == null && parentBinding.ElementName == null)
							{
								_boundProperties ??= new List<DependencyProperty>();
								_boundProperties.Add(bindingExpression.TargetProperty);
							}
						}
					}
					else
					{
						if (entry.Value is DependencyObject childObject)
						{
							_childNodes ??= new Dictionary<DependencyProperty, BindingTree>();
							_childNodes.Add(entry.Property, new BindingTree(childObject));
						}
					}
				}
			}

			public void EnsureBinding(DependencyObject dependencyObject)
			{
				if (_boundProperties != null)
				{
					foreach (var dependencyProperty in _boundProperties)
						BindingUtil.EnsureBindingAttached(dependencyObject, dependencyProperty);
				}

				if (_childNodes != null)
				{
					foreach (var kv in _childNodes)
					{
						var childProperty = kv.Key;
						var childTree = kv.Value;

						if (dependencyObject.GetValue(childProperty) is DependencyObject childDependencyObject)
							childTree.EnsureBinding(childDependencyObject);
					}
				}
			}
		}
	}
}