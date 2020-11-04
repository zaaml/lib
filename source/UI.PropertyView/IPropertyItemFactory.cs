// <copyright file="IPropertyItemFactory.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.PropertyView
{
	internal interface IPropertyItemFactory
	{
		PropertyItem CreatePropertyItem(object propertyObject, PropertyItem parentItem);
	}

	internal interface IPropertyItemFactory<in TTarget>
	{
		PropertyItem CreatePropertyItem(TTarget propertyObject, PropertyItem parentItem);
	}
}