// <copyright file="ColorSliderEditorControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Zaaml.Core;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.UI.Controls.ColorEditor
{
	[TemplateContractType(typeof(ColorSliderEditorControlTemplateContract))]
	public class ColorSliderEditorControl : ColorEditorControlBase
	{
		static ColorSliderEditorControl()
		{
			ControlUtils.OverrideIsTabStop<ColorSliderEditorControl>(false);

			DefaultStyleKeyHelper.OverrideStyleKey<ColorSliderEditorControl>();
		}

		public ColorSliderEditorControl()
		{
			this.OverrideStyleKey<ColorSliderEditorControl>();
		}

		private ColorChannelEditorControl AlphaChannelEditor => TemplateContract.AlphaChannelEditor;

		private ColorChannelEditorControl BlueChannelEditor => TemplateContract.BlueChannelEditor;

		private IEnumerable<ColorChannelEditorControl> ChannelEditors
		{
			get
			{
				if (IsTemplateAttached == false)
					yield break;

				yield return AlphaChannelEditor;
				yield return RedChannelEditor;
				yield return GreenChannelEditor;
				yield return BlueChannelEditor;
				yield return HueChannelEditor;
				yield return SaturationChannelEditor;
				yield return ValueChannelEditor;
			}
		}

		private ColorChannelEditorControl GreenChannelEditor => TemplateContract.GreenChannelEditor;

		private ColorChannelEditorControl HueChannelEditor => TemplateContract.HueChannelEditor;

		private ColorChannelEditorControl RedChannelEditor => TemplateContract.RedChannelEditor;

		private ColorChannelEditorControl SaturationChannelEditor => TemplateContract.SaturationChannelEditor;

		private ColorSliderEditorControlTemplateContract TemplateContract => (ColorSliderEditorControlTemplateContract) TemplateContractInternal;

		private ColorChannelEditorControl ValueChannelEditor => TemplateContract.ValueChannelEditor;

		protected override void OnChannelValueChanged(ColorChannel channel, double oldValue, double newValue)
		{
			base.OnChannelValueChanged(channel, oldValue, newValue);

			UpdateChannelEditorsColor();
		}

		protected override void OnColorChanged(Color oldColor, Color newColor)
		{
			base.OnColorChanged(oldColor, newColor);

			UpdateChannelEditorsColor();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			UpdateChannelEditorsColor();
		}

		internal override void SyncColor(EditorColorStruct color)
		{
			base.SyncColor(color);

			UpdateChannelEditorsColor();
		}

		private void UpdateChannelEditorsColor()
		{
			SuspendChangeHandler();

			foreach (var channelEditor in ChannelEditors)
				channelEditor.EditorColor = EditorColor;

			ResumeChangeHandler();
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			var measureOverride = base.MeasureOverride(availableSize);

			return measureOverride;
		}
	}

	public class ColorSliderEditorControlTemplateContract : TemplateContract
	{
		[TemplateContractPart(Required = true)]
		public ColorChannelEditorControl AlphaChannelEditor { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public ColorChannelEditorControl BlueChannelEditor { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public ColorChannelEditorControl GreenChannelEditor { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public ColorChannelEditorControl HueChannelEditor { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public ColorChannelEditorControl RedChannelEditor { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public ColorChannelEditorControl SaturationChannelEditor { get; [UsedImplicitly] private set; }

		[TemplateContractPart(Required = true)]
		public ColorChannelEditorControl ValueChannelEditor { get; [UsedImplicitly] private set; }
	}
}