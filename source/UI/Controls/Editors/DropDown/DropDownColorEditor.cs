// <copyright file="DropDownColorEditor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Media;
using Zaaml.Core;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.ColorEditor;

namespace Zaaml.UI.Controls.Editors.DropDown
{
	[TemplateContractType(typeof(DropDownColorEditorTemplateContract))]
	public class DropDownColorEditor : DropDownEditorBase
	{
		public static readonly DependencyProperty IsTextEditableProperty = DPM.Register<bool, DropDownColorEditor>
			("IsTextEditable");

		public static readonly DependencyProperty ColorProperty = DPM.Register<Color, DropDownColorEditor>
			("Color", d => d.OnColorPropertyChangedPrivate);

		public event EventHandler<ValueChangedEventArgs<Color>> ColorChanged;


		public Color Color
		{
			get => (Color)GetValue(ColorProperty);
			set => SetValue(ColorProperty, value);
		}

		private DropDownColorSelector DropDownColorSelector => TemplateContract.DropDownColorSelector;

		public bool IsTextEditable
		{
			get => (bool)GetValue(IsTextEditableProperty);
			set => SetValue(IsTextEditableProperty, value);
		}

		private DropDownColorEditorTemplateContract TemplateContract => (DropDownColorEditorTemplateContract)TemplateContractCore;

		private void OnColorPropertyChangedPrivate(Color oldColor, Color newColor)
		{
			ColorChanged?.Invoke(this, new ValueChangedEventArgs<Color>(oldColor, newColor));
		}
	}

	public class DropDownColorEditorTemplateContract : DropDownEditorBaseTemplateContract
	{
		[TemplateContractPart(Required = true)]
		public DropDownColorSelector DropDownColorSelector { get; private set; }
	}
}