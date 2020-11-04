// <copyright file="PropertyDescriptorCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Zaaml.UI.Controls.PropertyView
{
	public sealed class PropertyDescriptorCollection : ReadOnlyCollection<PropertyDescriptor>
	{
		public PropertyDescriptorCollection(IList<PropertyDescriptor> list) : base(list)
		{
		}
	}
}