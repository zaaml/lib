// <copyright file="ItemTemplateSelector.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Zaaml.Core.Utils;

namespace Zaaml.UI.Controls.TableView
{
	[ContentProperty(nameof(ItemTemplateCollection))]
	public class ItemTemplateSelector<TItemTemplate> : DataTemplateSelector
		where TItemTemplate : DataTemplate
	{
		public ItemTemplateCollection<TItemTemplate> ItemTemplateCollection { get; } = [];

		public sealed override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			return SelectTemplateCore(item);
		}

		protected virtual TItemTemplate SelectTemplateCore(object source)
		{
			if (source == null)
				return null;

			var sourceType = source.GetType();
			var selectedTypeDistance = int.MaxValue;

			TItemTemplate selectedTemplate = null;

			foreach (var dataTemplate in ItemTemplateCollection)
			{
				if (dataTemplate.DataType is not Type dataTemplateType || !dataTemplateType.IsAssignableFrom(sourceType))
					continue;

				var typeDistance = RuntimeUtils.GetTypeInheritanceDistance(dataTemplateType, sourceType);

				if (typeDistance < selectedTypeDistance)
				{
					selectedTypeDistance = typeDistance;
					selectedTemplate = dataTemplate;
				}
			}

			return selectedTemplate;
		}
	}

	public sealed class ItemTemplateCollection<TItemTemplate> : ObservableCollection<TItemTemplate>
		where TItemTemplate : DataTemplate
	{
	}
}