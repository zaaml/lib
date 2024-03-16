// <copyright file="PropertyBrushEditor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;
using Zaaml.Core;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Editors.Core;
using Zaaml.UI.Controls.Editors.DropDown;

namespace Zaaml.UI.Controls.PropertyView.Editors
{
	[TemplateContractType<PropertyBrushEditorTemplateContract>]
	public class PropertyBrushEditor : PropertyEditor
	{
		private bool _suspendValueChangedHandler;
		protected DropDownColorEditor Editor => TemplateContract.Editor;

		protected override EditorBase EditorCore => Editor;

		private PropertyBrushEditorTemplateContract TemplateContract => (PropertyBrushEditorTemplateContract)TemplateContractCore;

		private void OnEditorColorChanged(object sender, ValueChangedEventArgs<Color> e)
		{
			if (_suspendValueChangedHandler)
				return;

			UpdateValue();
		}

		protected override void OnPropertyItemChanged(PropertyItem oldValue, PropertyItem newValue)
		{
			base.OnPropertyItemChanged(oldValue, newValue);

			UpdateBrush();
		}

		protected override void OnPropertyItemValueChanged()
		{
			base.OnPropertyItemValueChanged();

			UpdateBrush();
		}

		protected override void OnPropertyItemValueUpdated()
		{
			base.OnPropertyItemValueUpdated();

			UpdateBrush();
		}


		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			Editor.ColorChanged += OnEditorColorChanged;

			UpdateBrush();
		}

		protected override void OnTemplateContractDetaching()
		{
			Editor.ColorChanged -= OnEditorColorChanged;

			base.OnTemplateContractDetaching();
		}

		private void UpdateBrush()
		{
			if (Editor == null)
				return;

			var propertyItem = PropertyItem;

			if (propertyItem == null)
				return;

			try
			{
				_suspendValueChangedHandler = true;

				var value = ((PropertyItem<Brush>)propertyItem).Value;

				if (value is SolidColorBrush solidColorBrush)
					Editor.Color = solidColorBrush.Color;
				else
					Editor.Color = Colors.Transparent;
			}
			finally
			{
				_suspendValueChangedHandler = false;
			}
		}

		private void UpdateValue()
		{
			try
			{
				_suspendValueChangedHandler = true;

				if (PropertyItem is PropertyItem<Brush> propertyItem)
				{
					var brush = propertyItem.Value;

					if (brush is SolidColorBrush solidColorBrush && brush.IsFrozen == false)
					{
						solidColorBrush.Color = Editor.Color;
					}
					else
					{
						propertyItem.Value = new SolidColorBrush(Editor.Color);
					}
				}
			}
			finally
			{
				_suspendValueChangedHandler = false;
			}
		}
	}

	public class PropertyBrushEditorTemplateContract : PropertyEditorTemplateContract
	{
		[TemplateContractPart(Required = true)]
		public DropDownColorEditor Editor { get; [UsedImplicitly] private set; }
	}
}