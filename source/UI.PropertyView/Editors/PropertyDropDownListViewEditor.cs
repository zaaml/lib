// <copyright file="PropertyDropDownListViewEditor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Editors.Core;
using Zaaml.UI.Controls.Editors.DropDown;
using Zaaml.UI.Controls.ListView;

namespace Zaaml.UI.Controls.PropertyView.Editors
{
	[TemplateContractType(typeof(PropertyDropDownListViewEditorTemplateContract))]
	public abstract class PropertyDropDownListViewEditor : PropertyEditor
	{
		static PropertyDropDownListViewEditor()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<PropertyDropDownListViewEditor>();
		}

		protected PropertyDropDownListViewEditor()
		{
			this.OverrideStyleKey<PropertyDropDownListViewEditor>();
		}

		protected DropDownListViewEditor Editor => TemplateContract.Editor;

		protected override EditorBase EditorCore => Editor;

		protected ListViewControl ListView => TemplateContract.ListView;

		private PropertyDropDownListViewEditorTemplateContract TemplateContract => (PropertyDropDownListViewEditorTemplateContract)TemplateContractCore;
	}

	public abstract class PropertyDropDownListViewEditor<T> : PropertyDropDownListViewEditor
	{
		private bool _suspendListViewSelectionChangedHandler;
		private bool _suspendValueChangedHandler;

		protected virtual bool DefaultIsTextEditable => false;

		protected abstract IReadOnlyCollection<PropertyListViewItemSource<T>> Items { get; }

		protected abstract PropertyListViewItemSource<T> GetItemByValue(T value);

		protected void OnItemsChanged()
		{
			if (ListView == null)
				return;

			ListView.SourceCollection = Items;
		}

		private void OnListViewSelectionChanged(object sender, SelectionChangedEventArgs<ListViewItem> e)
		{
			if (_suspendListViewSelectionChangedHandler)
				return;

			UpdateValue();
		}

		protected override void OnPropertyItemChanged(PropertyItem oldValue, PropertyItem newValue)
		{
			base.OnPropertyItemChanged(oldValue, newValue);

			UpdateListViewValue();
		}

		protected override void OnPropertyItemValueChanged()
		{
			base.OnPropertyItemValueChanged();

			if (_suspendValueChangedHandler)
				return;

			UpdateListViewValue();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			Editor.IsTextEditable = DefaultIsTextEditable;

			ListView.SourceCollection = Items;
			ListView.ItemsFilter = new PropertyListViewItemTextFilter();
			ListView.SelectionChanged += OnListViewSelectionChanged;

			UpdateListViewValue();
		}

		protected override void OnTemplateContractDetaching()
		{
			ListView.SelectionChanged -= OnListViewSelectionChanged;
			ListView.SourceCollection = null;
			ListView.ItemsFilter = null;

			base.OnTemplateContractDetaching();
		}

		private void UpdateListViewValue()
		{
			if (ListView == null)
				return;

			var propertyItem = PropertyItem;

			if (propertyItem == null)
				return;

			try
			{
				_suspendListViewSelectionChangedHandler = true;

				var value = ((PropertyItem<T>)propertyItem).Value;

				ListView.SelectedSource = GetItemByValue(value);
			}
			finally
			{
				_suspendListViewSelectionChangedHandler = false;
			}
		}

		private void UpdateValue()
		{
			try
			{
				_suspendValueChangedHandler = true;

				if (PropertyItem is PropertyItem<T> propertyItem && ListView.SelectedSource is PropertyListViewItemSource<T> propertyValueSource)
					propertyItem.Value = propertyValueSource.Value;
			}
			finally
			{
				_suspendValueChangedHandler = false;
			}
		}
	}

	public class PropertyDropDownListViewEditorTemplateContract : PropertyEditorTemplateContract
	{
		[TemplateContractPart(Required = true)]
		public DropDownListViewEditor Editor { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public ListViewControl ListView { get; [UsedImplicitly] private set; }
	}
}