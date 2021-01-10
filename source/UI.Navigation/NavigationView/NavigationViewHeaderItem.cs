// <copyright file="NavigationViewHeaderItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Panels.Flexible;

namespace Zaaml.UI.Controls.NavigationView
{
	[ContentProperty(nameof(Header))]
	public class NavigationViewHeaderItem : NavigationViewItemBase
	{
		public static readonly DependencyProperty HeaderProperty = DPM.Register<object, NavigationViewHeaderItem>
			("Header", default, d => d.OnHeaderPropertyChangedPrivate);

		public static readonly DependencyProperty LengthProperty = DPM.Register<FlexLength, NavigationViewHeaderItem>
			("Length", FlexLength.Auto, d => d.OnLengthPropertyChangedPrivate);

		static NavigationViewHeaderItem()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<NavigationViewHeaderItem>();
		}

		public NavigationViewHeaderItem()
		{
			this.OverrideStyleKey<NavigationViewHeaderItem>();
		}

		public object Header
		{
			get => GetValue(HeaderProperty);
			set => SetValue(HeaderProperty, value);
		}

		[TypeConverter(typeof(FlexLengthTypeConverter))]
		public FlexLength Length
		{
			get => (FlexLength) GetValue(LengthProperty);
			set => SetValue(LengthProperty, value);
		}

		private void OnHeaderPropertyChangedPrivate(object oldValue, object newValue)
		{
		}

		private void OnLengthPropertyChangedPrivate(FlexLength oldValue, FlexLength newValue)
		{
		}
	}
}