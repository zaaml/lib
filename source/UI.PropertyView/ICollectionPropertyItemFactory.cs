// <copyright file="ICollectionPropertyItemFactory.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.UI.Controls.PropertyView
{
	internal interface ICollectionPropertyItemFactory<in TCollection>
	{
		IEnumerable<PropertyItem> CreatePropertyItems(TCollection propertyObject, PropertyItem parentItem);
	}
}