// <copyright file="PropertyDisplayOnlyEditor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Editors.Core;

namespace Zaaml.UI.Controls.PropertyView.Editors
{
	public class PropertyDisplayOnlyEditor : PropertyEditor
	{
		private static readonly DependencyPropertyKey TextValuePropertyKey = DPM.RegisterReadOnly<string, PropertyDisplayOnlyEditor>
			("TextValue", string.Empty);

		public static readonly DependencyProperty TextValueProperty = TextValuePropertyKey.DependencyProperty;

		static PropertyDisplayOnlyEditor()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<PropertyDisplayOnlyEditor>();
		}

		public PropertyDisplayOnlyEditor()
		{
			this.OverrideStyleKey<PropertyDisplayOnlyEditor>();
		}

		protected override EditorBase EditorCore => null;

		public string TextValue
		{
			get => (string) GetValue(TextValueProperty);
			private set => this.SetReadOnlyValue(TextValuePropertyKey, value);
		}

		protected override void OnPropertyItemChanged(PropertyItem oldValue, PropertyItem newValue)
		{
			base.OnPropertyItemChanged(oldValue, newValue);

			UpdateTextValue();
		}

		protected override void OnPropertyItemValueChanged()
		{
			base.OnPropertyItemValueChanged();

			UpdateTextValue();
		}

		private void UpdateTextValue()
		{
			var propertyItem = PropertyItem;

			TextValue = propertyItem != null ? propertyItem.RawValueInternal?.ToString() : string.Empty;
		}
	}
}