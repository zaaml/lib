// <copyright file="ColorEditorControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.UI.Controls.ColorEditor
{
	[TemplateContractType(typeof(ColorEditorControlTemplateContract))]
	public class ColorEditorControl : ColorEditorControlBase
	{
		public static readonly DependencyProperty OriginalColorProperty = DPM.Register<Color, ColorEditorControl>
			("OriginalColor");

		static ColorEditorControl()
		{
			ControlUtils.OverrideIsTabStop<ColorEditorControl>(false);

			DefaultStyleKeyHelper.OverrideStyleKey<ColorEditorControl>();
		}

		public ColorEditorControl()
		{
			this.OverrideStyleKey<ColorEditorControl>();
		}

		private IEnumerable<ColorEditorControlBase> ColorEditors
		{
			get
			{
				if (IsTemplateAttached == false)
					yield break;

				yield return RectangleEditor;
				yield return SliderEditor;
			}
		}

		public Color OriginalColor
		{
			get => (Color) GetValue(OriginalColorProperty);
			set => SetValue(OriginalColorProperty, value);
		}

		public ColorRectangleEditorControl RectangleEditor => TemplateContract.RectangleEditor;

		public ColorSliderEditorControl SliderEditor => TemplateContract.SliderEditor;

		private ColorEditorControlTemplateContract TemplateContract => (ColorEditorControlTemplateContract) TemplateContractInternal;

		protected override void OnColorChanged(Color oldColor, Color newColor)
		{
			base.OnColorChanged(oldColor, newColor);

			if (IsTemplateAttached == false)
				return;

			if (IsChangeHandlerSuspended)
				return;

			foreach (var colorEditor in ColorEditors)
				colorEditor.SyncColor(EditorColor);
		}

		private void OnColorEditorChannelValueChanged(object sender, ColorChannelValueChangedEventArgs e)
		{
			if (IsChangeHandlerSuspended)
				return;

			var editor = (ColorEditorControlBase) sender;

			if (editor.IsChangeHandlerSuspendedInternal)
				return;

			SuspendChangeHandler();

			foreach (var colorEditor in ColorEditors)
			{
				if (ReferenceEquals(sender, colorEditor))
					continue;

				colorEditor.SetChannelValue(e.Channel, e.NewValue);
			}

			Color = editor.Color;

			ResumeChangeHandler();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			foreach (var colorEditor in ColorEditors)
			{
				colorEditor.ChannelValueChanged += OnColorEditorChannelValueChanged;

				colorEditor.SyncColor(EditorColor);
			}
		}

		protected override void OnTemplateContractDetaching()
		{
			foreach (var colorEditor in ColorEditors)
				colorEditor.ChannelValueChanged -= OnColorEditorChannelValueChanged;

			base.OnTemplateContractDetaching();
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			var measureOverride = base.MeasureOverride(availableSize);

			return measureOverride;
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			var arrangeOverride = base.ArrangeOverride(arrangeBounds);

			return arrangeOverride;
		}
	}
}