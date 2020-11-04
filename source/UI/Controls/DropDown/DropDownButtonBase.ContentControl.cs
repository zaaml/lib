// <copyright file="DropDownButtonBase.ContentControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.DropDown
{
	[ContentProperty(nameof(Content))]
	public abstract partial class DropDownButtonBase : IContentControl
	{
		public static readonly DependencyProperty ContentProperty = DPM.Register<object, DropDownButtonBase>
			("Content");

		public static readonly DependencyProperty ContentTemplateProperty = DPM.Register<DataTemplate, DropDownButtonBase>
			("ContentTemplate");

		public static readonly DependencyProperty ContentStringFormatProperty = DPM.Register<string, DropDownButtonBase>
			("ContentStringFormat");

		public static readonly DependencyProperty ContentTemplateSelectorProperty = DPM.Register<DataTemplateSelector, DropDownButtonBase>
			("ContentTemplateSelector");

		DependencyProperty IContentControl.ContentProperty => ContentProperty;

		DependencyProperty IContentControl.ContentTemplateProperty => ContentTemplateProperty;

		DependencyProperty IContentControl.ContentStringFormatProperty => ContentStringFormatProperty;

		DependencyProperty IContentControl.ContentTemplateSelectorProperty => ContentTemplateSelectorProperty;

		public object Content
		{
			get => GetValue(ContentProperty);
			set => SetValue(ContentProperty, value);
		}

		public string ContentStringFormat
		{
			get => (string) GetValue(ContentStringFormatProperty);
			set => SetValue(ContentStringFormatProperty, value);
		}

		public DataTemplate ContentTemplate
		{
			get => (DataTemplate) GetValue(ContentTemplateProperty);
			set => SetValue(ContentTemplateProperty, value);
		}

		public DataTemplateSelector ContentTemplateSelector
		{
			get => (DataTemplateSelector) GetValue(ContentTemplateSelectorProperty);
			set => SetValue(ContentTemplateSelectorProperty, value);
		}
	}
}