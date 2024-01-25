// <copyright file="PropertyDescriptorCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Zaaml.UI.Controls.PropertyView
{
	public sealed class PropertyDescriptorCollection : ReadOnlyCollection<PropertyDescriptor>
	{
#if NET8_0_OR_GREATER
		public new static readonly PropertyDescriptorCollection Empty = new(Array.Empty<PropertyDescriptor>());
#else
		public static readonly PropertyDescriptorCollection Empty = new(Array.Empty<PropertyDescriptor>());
#endif

		public PropertyDescriptorCollection(IList<PropertyDescriptor> list) : base(list)
		{
		}
	}
}