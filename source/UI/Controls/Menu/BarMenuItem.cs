// <copyright file="BarMenuItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Menu
{
	[ContentProperty(nameof(Content))]
	public class BarMenuItem : HeaderedMenuItem
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty ContentProperty = DPM.Register<object, BarMenuItem>
			("Content", m => m.OnContentChanged);

		public static readonly DependencyProperty ContentTemplateProperty = DPM.Register<DataTemplate, BarMenuItem>
			("ContentTemplate", m => m.OnContentTemplateChanged);

		#endregion

		#region Ctors

		static BarMenuItem()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<BarMenuItem>();
		}

		public BarMenuItem()
		{
			this.OverrideStyleKey<BarMenuItem>();

			ContentControl = new ContentControl
			{
				HorizontalContentAlignment = HorizontalAlignment.Stretch,
				VerticalContentAlignment = VerticalAlignment.Stretch
			};

			AddLogicalChild(ContentControl);
		}

		#endregion

		#region Properties

		public object Content
		{
			get => GetValue(ContentProperty);
			set => SetValue(ContentProperty, value);
		}

		private ContentControl ContentControl { get; }

		public DataTemplate ContentTemplate
		{
			get => (DataTemplate) GetValue(ContentTemplateProperty);
			set => SetValue(ContentTemplateProperty, value);
		}

		internal override FrameworkElement Submenu => SubmenuElement;

		protected override FrameworkElement SubmenuElement => ContentControl;

		#endregion

		#region  Methods

		private void OnContentChanged()
		{
			ContentControl.Content = Content;
			HasSubmenu = Content != null || ContentTemplate != null;
		}

		private void OnContentTemplateChanged()
		{
			ContentControl.ContentTemplate = ContentTemplate;
		}

		#endregion
	}
}