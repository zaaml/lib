// <copyright file="IHeaderedContentControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;

namespace Zaaml.UI.Controls.Interfaces
{
	internal interface IHeaderedContentControl
	{
		object Header { get; set; }

		DependencyProperty HeaderProperty { get; }

		string HeaderStringFormat { get; set; }

		DependencyProperty HeaderStringFormatProperty { get; }

		DataTemplate HeaderTemplate { get; set; }

		DependencyProperty HeaderTemplateProperty { get; }

		DataTemplateSelector HeaderTemplateSelector { get; set; }

		DependencyProperty HeaderTemplateSelectorProperty { get; }
	}

	internal interface IHeaderedIconContentControl : IIconContentControl
	{
		object Header { get; set; }

		DependencyProperty HeaderProperty { get; }

		string HeaderStringFormat { get; set; }

		DependencyProperty HeaderStringFormatProperty { get; }

		DataTemplate HeaderTemplate { get; set; }

		DependencyProperty HeaderTemplateProperty { get; }

		DataTemplateSelector HeaderTemplateSelector { get; set; }

		DependencyProperty HeaderTemplateSelectorProperty { get; }
	}

	internal interface ISelectableItem
	{
		DependencyProperty ValueProperty { get; }

		DependencyProperty SelectionProperty { get; }
	}
	
	internal interface ISelectableIconContentItem: IIconContentControl, ISelectableItem
	{
	}
	
	internal interface ISelectableHeaderedIconContentItem: IHeaderedIconContentControl, ISelectableItem
	{
	}

	internal interface IContentItemsControl
	{
		string ItemContentMember { get; }

		string ItemContentStringFormat { get; }
		
		DataTemplate ItemContentTemplate { get; }

		DataTemplateSelector ItemContentTemplateSelector { get; }
	}

	internal interface IIconContentItemsControl : IContentItemsControl
	{
		string ItemIconMember { get; }

		IIconSelector ItemIconSelector { get; }
	}

	internal interface IHeaderedIconContentItemsControl : IIconContentItemsControl
	{
		string ItemHeaderMember { get; }

		string ItemHeaderStringFormat { get; }
		
		DataTemplate ItemHeaderTemplate { get; }

		DataTemplateSelector ItemHeaderTemplateSelector { get; }
	}

	internal interface ISelectableItemsControl
	{
		string ItemValueMember { get; }
		
		string ItemSelectionMember { get;}
	}
	
	internal interface IHeaderedIconContentSelectorControl : IHeaderedIconContentItemsControl, ISelectableItemsControl
	{
	}
	
	internal interface IIconContentSelectorControl : IIconContentItemsControl, ISelectableItemsControl
	{
	}
}