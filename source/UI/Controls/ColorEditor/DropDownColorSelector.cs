// <copyright file="DropDownColorSelector.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Media;
using Zaaml.Core;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.DropDown;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;

namespace Zaaml.UI.Controls.ColorEditor
{
	[TemplateContractType(typeof(DropDownColorSelectorTemplateContract))]
	public class DropDownColorSelector : DropDownControlBase
	{
		public static readonly DependencyProperty ShowTextValueProperty = DPM.Register<bool, DropDownColorSelector>
			("ShowTextValue", false);

		public static readonly DependencyProperty IsTextEditableProperty = DPM.Register<bool, DropDownColorSelector>
			("IsTextEditable", false);

		public static readonly DependencyProperty ShowTransparentPatternProperty = DPM.Register<bool, DropDownColorSelector>
			("ShowTransparentPattern", true);

		public static readonly DependencyProperty ColorProperty = DPM.Register<Color, DropDownColorSelector>
			("Color", Colors.Black, d => d.OnColorPropertyChangedPrivate);

		public static readonly DependencyProperty ShowDropDownButtonProperty = DPM.Register<bool, DropDownColorSelector>
			("ShowDropDownButton", true);

		static DropDownColorSelector()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<DropDownColorSelector>();
		}

		public DropDownColorSelector()
		{
			this.OverrideStyleKey<DropDownColorSelector>();
		}

		public Color Color
		{
			get => (Color)GetValue(ColorProperty);
			set => SetValue(ColorProperty, value);
		}

		private ColorEditorControl ColorEditor => TemplateContract.ColorEditor;

		public bool IsTextEditable
		{
			get => (bool)GetValue(IsTextEditableProperty);
			set => SetValue(IsTextEditableProperty, value.Box());
		}

		private PopupBar PopupBar => TemplateContract.PopupBar;

		protected override PopupControlBase PopupControlCore => PopupBar;

		public bool ShowDropDownButton
		{
			get => (bool)GetValue(ShowDropDownButtonProperty);
			set => SetValue(ShowDropDownButtonProperty, value);
		}

		public bool ShowTextValue
		{
			get => (bool)GetValue(ShowTextValueProperty);
			set => SetValue(ShowTextValueProperty, value.Box());
		}

		public bool ShowTransparentPattern
		{
			get => (bool)GetValue(ShowTransparentPatternProperty);
			set => SetValue(ShowTransparentPatternProperty, value.Box());
		}

		private DropDownColorSelectorTemplateContract TemplateContract => (DropDownColorSelectorTemplateContract)TemplateContractCore;

		private protected override void OnClosedCore()
		{
			if (ColorEditor != null)
				Color = ColorEditor.Color;

			base.OnClosedCore();
		}

		private void OnColorPropertyChangedPrivate(Color oldValue, Color newValue)
		{
			SyncColorEditor();
		}

		private protected override void OnOpenedCore()
		{
			if (ColorEditor != null)
			{
				ColorEditor.Color = Color;
				ColorEditor.OriginalColor = Color;
			}

			base.OnOpenedCore();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			SyncColorEditor();
		}

		private void SyncColorEditor()
		{
			if (ColorEditor == null || IsDropDownOpen == false)
				return;

			ColorEditor.Color = Color;
			ColorEditor.OriginalColor = Color;
		}
	}

	public class DropDownColorSelectorTemplateContract : TemplateContract
	{
		[TemplateContractPart(Required = false)]
		public ColorEditorControl ColorEditor { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = false)]
		public PopupBar PopupBar { get; [UsedImplicitly] private set; }
	}
}