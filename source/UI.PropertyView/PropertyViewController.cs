// <copyright file="PropertyViewController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using Zaaml.Core.Converters;
using Zaaml.UI.Controls.PropertyView.Editors;

namespace Zaaml.UI.Controls.PropertyView
{
	public class PropertyViewController
	{
		private static readonly MethodInfo GetStringConverterMethodInfo = typeof(PropertyViewController).GetMethod(nameof(GetStringConverter), BindingFlags.Instance | BindingFlags.NonPublic);
		private static readonly Dictionary<Type, PropertyStringConverter> ReadonlyConvertersDictionary = [];
		private static readonly Dictionary<Type, PropertyStringConverter> ConvertersDictionary = [];

		private static readonly Dictionary<Type, Type> ValueEditorDictionary = new()
		{
			{typeof(bool), typeof(PropertyBooleanEditor)},
			{typeof(string), typeof(PropertyTextEditor)},
			{typeof(FontWeight), typeof(PropertyFontWeightEditor)},
			{typeof(FontStretch), typeof(PropertyFontStretchEditor)},
			{typeof(FontStyle), typeof(PropertyFontStyleEditor)}
		};

		private PropertyDescriptorProvider _propertyDescriptorProvider;
		private object _selectedObject;

		static PropertyViewController()
		{
		}

		public PropertyViewController(PropertyViewControl propertyView)
		{
			PropertyView = propertyView;
		}

		private Dictionary<Type, Stack<PropertyEditor>> EditorsPoolDictionary { get; } = [];

		public IReadOnlyCollection<PropertyCategory> PropertyCategories { get; private set; }

		private PropertyDescriptorProvider PropertyDescriptorProvider => _propertyDescriptorProvider ??= CreatePropertyDescriptorProvider();

		public IReadOnlyCollection<PropertyItem> PropertyItems { get; private set; }

		public PropertyViewControl PropertyView { get; }

		public object SelectedObject
		{
			get => _selectedObject;
			internal set
			{
				if (ReferenceEquals(_selectedObject, value))
					return;

				_selectedObject = value;

				Update();
			}
		}

		private static PropertyEditor CreateEditor(Type editorType)
		{
			return (PropertyEditor) Activator.CreateInstance(editorType);
		}

		protected virtual PropertyDescriptorProvider CreatePropertyDescriptorProvider()
		{
			return PropertyDescriptorProvider.Instance;
		}

		private static PropertyStringConverter<T> CreateStringConverter<T>(PropertyDescriptor<T> propertyDescriptor)
		{
			var type = typeof(T);

			if (propertyDescriptor.IsReadOnly)
			{
				var fromConverter = PrimitiveConverterFactory.GetConverter<T, string>();

				if (fromConverter != null)
					return new PrimitivePropertyStringConverter<T>(fromConverter, null);

				if (type.IsValueType && type.Assembly.GetName().Name == "PresentationFramework")
					return new PrimitiveXamlPropertyStringConverter<T>();
			}
			else
			{
				var toConverter = PrimitiveConverterFactory.GetConverter<string, T>();
				var fromConverter = PrimitiveConverterFactory.GetConverter<T, string>();

				if (fromConverter != null && toConverter != null)
					return new PrimitivePropertyStringConverter<T>(fromConverter, toConverter);

				if (type.IsValueType && type.Assembly.GetName().Name == "PresentationFramework")
					return new PrimitiveXamlPropertyStringConverter<T>();
			}

			return null;
		}

		protected virtual string GetDisplayName<T>(T value)
		{
			return value.ToString();
		}

		private Stack<PropertyEditor> GetEditorsPool(Type editorType)
		{
			if (EditorsPoolDictionary.TryGetValue(editorType, out var editorsPool) == false)
				EditorsPoolDictionary[editorType] = editorsPool = new Stack<PropertyEditor>();

			return editorsPool;
		}

		private Type GetEditorType(PropertyItem propertyItem)
		{
			if (propertyItem.IsReadOnly)
				return typeof(PropertyDisplayOnlyEditor);

			var propertyType = propertyItem.ValueTypeInternal;

			if (ValueEditorDictionary.TryGetValue(propertyType, out var editorType))
				return editorType;

			if (propertyType.IsEnum)
				return typeof(PropertyEnumEditor<>).MakeGenericType(propertyType);

			if (GetStringConverterInternal(propertyItem) != null)
				return typeof(PropertyTextEditor);

			return typeof(PropertyDisplayOnlyEditor);
		}

		protected virtual PropertyStringConverter<T> GetStringConverter<T>(PropertyItem<T> propertyItem)
		{
			if (propertyItem.PropertyDescriptorBaseInternal.IsReadOnly)
			{
				if (ReadonlyConvertersDictionary.TryGetValue(typeof(T), out var converter) == false)
					ReadonlyConvertersDictionary[typeof(T)] = converter = CreateStringConverter(propertyItem.PropertyDescriptorInternal);

				return (PropertyStringConverter<T>) converter;
			}

			{
				if (ConvertersDictionary.TryGetValue(typeof(T), out var converter) == false)
					ConvertersDictionary[typeof(T)] = converter = CreateStringConverter(propertyItem.PropertyDescriptorInternal);

				return (PropertyStringConverter<T>) converter;
			}
		}

		internal PropertyStringConverter GetStringConverterInternal(PropertyItem propertyItem)
		{
			if (propertyItem.ValueTypeInternal == typeof(string))
				return DummyStringConverter.Instance;

			var genericMethod = GetStringConverterMethodInfo.MakeGenericMethod(propertyItem.ValueTypeInternal);

			return (PropertyStringConverter) genericMethod.Invoke(this, new object[] {propertyItem});
		}

		protected virtual PropertyEditor RentEditor(PropertyItem propertyItem)
		{
			var editorType = GetEditorType(propertyItem);

			if (editorType == null)
				return null;

			var editorsPool = GetEditorsPool(editorType);
			var editor = editorsPool.Count > 0 ? editorsPool.Pop() : CreateEditor(editorType);

			editor.Mount(propertyItem, this);

			return editor;
		}

		internal PropertyEditor RentEditorInternal(PropertyItem propertyItem)
		{
			return RentEditor(propertyItem);
		}

		protected virtual void ReturnEditor(PropertyEditor editor)
		{
			editor.Release();

			GetEditorsPool(editor.GetType()).Push(editor);
		}

		internal void ReturnEditorInternal(PropertyEditor editor)
		{
			ReturnEditor(editor);
		}

		private void Update()
		{
			try
			{
				PropertyCategories = null;
				PropertyItems = null;

				if (SelectedObject == null)
					return;

				var propertyItems = new List<PropertyItem>();
				var propertyCategories = new Dictionary<string, List<PropertyItem>>();
				var properties = PropertyDescriptorProvider.GetPropertiesInternal(SelectedObject);

				foreach (var property in properties)
				{
					var factory = (IPropertyItemFactory) property;
					var propertyItem = factory.CreatePropertyItem(SelectedObject, null);

					propertyItems.Add(propertyItem);

					var categoryName = property.Category ?? "General";

					if (propertyCategories.TryGetValue(categoryName, out var category) == false)
						propertyCategories[categoryName] = category = [];

					category.Add(propertyItem);
				}

				PropertyItems = new ReadOnlyCollection<PropertyItem>(propertyItems);
				PropertyCategories = new ReadOnlyCollection<PropertyCategory>(propertyCategories.Select(kv => new PropertyCategory(kv.Key, null, kv.Value)).ToList());
			}
			finally
			{
				PropertyView.UpdateProperties();
			}
		}
	}
}