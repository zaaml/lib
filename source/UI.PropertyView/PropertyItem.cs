// <copyright file="PropertyItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Zaaml.Core;

namespace Zaaml.UI.Controls.PropertyView
{
	public abstract class PropertyItem : PropertyItemBase
	{
		protected static readonly ReadOnlyCollection<PropertyItem> EmptyChildPropertyItems = new ReadOnlyCollection<PropertyItem>(new List<PropertyItem>());

		private IReadOnlyCollection<PropertyItem> _childPropertyItems;
		internal event EventHandler ValueChangedInternal;
		internal event EventHandler ValueUpdatedInternal;

		protected PropertyItem(PropertyItem parentItem)
		{
			ParentItem = parentItem;
		}

		public IReadOnlyCollection<PropertyItem> ChildPropertyItems => _childPropertyItems ??= IsComposite ? CreateChildPropertyItems() : EmptyChildPropertyItems;

		public override string Description => PropertyDescriptorBaseCore.Description;

		public override string DisplayName => PropertyDescriptorBaseCore.DisplayName;

		private bool IsComposite
		{
			get
			{
				var propertyType = PropertyDescriptorBaseCore.PropertyType;

				//return propertyType.IsValueType &&
				//       propertyType.IsPrimitive == false &&
				//       propertyType.IsEnum == false;
				
				if (propertyType == typeof(string))
					return false;

				return true;
			}
		}

		public abstract bool IsReadOnly { get; }

		public PropertyItem ParentItem { get; }

		protected abstract PropertyDescriptor PropertyDescriptorBaseCore { get; }

		internal PropertyDescriptor PropertyDescriptorBaseInternal => PropertyDescriptorBaseCore;

		internal abstract object RawValueInternal { get; set; }

		internal abstract Type ValueTypeInternal { get; }

		protected abstract IReadOnlyCollection<PropertyItem> CreateChildPropertyItems();

		private protected void OnValueChangedInternal()
		{
			ValueChangedInternal?.Invoke(this, EventArgs.Empty);
		}

		private protected void OnValueUpdatedInternal()
		{
			ValueUpdatedInternal?.Invoke(this, EventArgs.Empty);
		}

		public abstract void ResetValue();
	}

	public abstract class PropertyItem<T> : PropertyItem
	{
		public event EventHandler<ValueChangedEventArgs<T>> ValueChanged;

		private protected PropertyItem(PropertyItem parentItem) : base(parentItem)
		{
		}

		public override bool IsReadOnly
		{
			get
			{
				if (PropertyDescriptorInternal.IsReadOnly)
					return true;

				if (ParentItem != null && ParentItem.ValueTypeInternal.IsValueType && ParentItem.IsReadOnly)
					return true;

				return false;
			}
		}

		protected abstract PropertyDescriptor<T> PropertyDescriptorCore { get; }

		internal PropertyDescriptor<T> PropertyDescriptorInternal => PropertyDescriptorCore;

		internal override object RawValueInternal
		{
			get => Value;
			set => Value = (T) value;
		}

		public abstract T Value { get; set; }

		internal override Type ValueTypeInternal => typeof(T);

		protected override IReadOnlyCollection<PropertyItem> CreateChildPropertyItems()
		{
			var propertyObject = Value;
			var propertyDescriptors = PropertyDescriptorInternal.Provider.GetPropertyDescriptors(propertyObject);

			if (propertyDescriptors == null || propertyDescriptors.Count == 0)
				return EmptyChildPropertyItems;

			var propertyItems = new List<PropertyItem>();

			foreach (var propertyDescriptor in propertyDescriptors)
			{
				if (propertyDescriptor is IPropertyItemFactory<T> itemFactory)
				{
					propertyItems.Add(itemFactory.CreatePropertyItem(propertyObject, this));
				}
			}

			if (PropertyDescriptorInternal is ICollectionPropertyItemFactory<T> collectionPropertyItemFactory)
			{
				foreach (var propertyItem in collectionPropertyItemFactory.CreatePropertyItems(propertyObject, this))
					propertyItems.Add(propertyItem);
			}

			return new ReadOnlyCollection<PropertyItem>(propertyItems);
		}

		protected virtual void OnValueChanged(T oldValue, T newValue)
		{
			ValueChanged?.Invoke(this, new ValueChangedEventArgs<T>(oldValue, newValue));

			OnValueChangedInternal();
		}
	}

	public sealed class PropertyItem<TTarget, T> : PropertyItem<T>
	{
		public PropertyItem(TTarget propertyObject, PropertyDescriptor<TTarget, T> propertyDescriptor, PropertyItem parentItem) : base(parentItem)
		{
			PropertyObject = propertyObject;
			PropertyDescriptor = propertyDescriptor;

			if (ParentItem is PropertyItem<TTarget> parent && parent.ValueTypeInternal.IsValueType)
			{
				parent.ValueChanged += OnParentValueChanged;
				parent.ValueUpdatedInternal += OnParentValueUpdated;
			}
		}

		public PropertyDescriptor<TTarget, T> PropertyDescriptor { get; }

		protected override PropertyDescriptor PropertyDescriptorBaseCore => PropertyDescriptor;

		protected override PropertyDescriptor<T> PropertyDescriptorCore => PropertyDescriptor;

		public TTarget PropertyObject { get; private set; }

		public override T Value
		{
			get => PropertyDescriptor.GetValue(PropertyObject);
			set
			{
				var oldValue = Value;

				if (AreEqual(oldValue, value))
				{
					OnValueUpdatedInternal();

					return;
				}

				PropertyObject = PropertyDescriptor.SetValue(PropertyObject, value);

				if (ParentItem is PropertyItem<TTarget> parent && parent.ValueTypeInternal.IsValueType)
					parent.Value = PropertyObject;

				OnValueChanged(oldValue, value);
			}
		}

		private static bool AreEqual(T first, T second)
		{
			return Equals(first, second);
		}

		private void OnParentValueChanged(object sender, ValueChangedEventArgs<TTarget> e)
		{
			var oldValue = PropertyDescriptor.GetValue(PropertyObject);

			PropertyObject = e.NewValue;

			var newValue = PropertyDescriptor.GetValue(PropertyObject);

			if (AreEqual(oldValue, newValue) == false)
				OnValueChanged(oldValue, newValue);
			else
				OnValueUpdatedInternal();
		}

		private void OnParentValueUpdated(object sender, EventArgs e)
		{
			OnValueUpdatedInternal();
		}

		public override void ResetValue()
		{
			var oldValue = Value;

			PropertyObject = PropertyDescriptor.ResetValue(PropertyObject);

			if (ParentItem is PropertyItem<TTarget> parent && parent.ValueTypeInternal.IsValueType)
				parent.Value = PropertyObject;

			var value = Value;

			if (AreEqual(oldValue, value) == false)
				OnValueChanged(oldValue, value);
			else
				OnValueUpdatedInternal();
		}
	}
}