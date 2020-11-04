// <copyright file="CollectionPropertyItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.UI.Controls.PropertyView
{
	public sealed class CollectionPropertyItem<TCollection, TItem> : PropertyItem<TItem> where TCollection : ICollection<TItem>
	{
		private TItem _value;

		internal CollectionPropertyItem(TCollection propertyObject, TItem item, int index, CollectionItemPropertyDescriptor<TCollection, TItem> propertyDescriptor, PropertyItem parentItem) : base(parentItem)
		{
			PropertyObject = propertyObject;
			PropertyDescriptor = propertyDescriptor;
			_value = item;
			Index = index;
		}

		public override string DisplayName => $"{base.DisplayName} {Index}";

		public int Index { get; }

		public CollectionItemPropertyDescriptor<TCollection, TItem> PropertyDescriptor { get; }

		protected override PropertyDescriptor PropertyDescriptorBaseCore => PropertyDescriptor;

		protected override PropertyDescriptor<TItem> PropertyDescriptorCore => PropertyDescriptor;

		public TCollection PropertyObject { get; }

		public override TItem Value
		{
			get => _value;
			set
			{
				if (Equals(Value, value))
					return;

				PropertyDescriptor.SetItem(PropertyObject, Index, value);

				_value = value;
			}
		}

		public override void ResetValue()
		{
			Value = default;
		}
	}
}