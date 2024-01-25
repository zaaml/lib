// <copyright file="IconContentControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Runtime;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Interfaces;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;

namespace Zaaml.UI.Controls.Core
{
	[TemplateContractType(typeof(IconContentControlTemplateContract))]
	public class IconContentControl : TemplateContractContentControl, IIconOwner, IIconContentControl
	{
		public static readonly DependencyProperty IconProperty = DPM.Register<IconBase, IconContentControl>
			("Icon", null, i => i.LogicalChildMentor.OnLogicalChildPropertyChanged);

		public static readonly DependencyProperty IconDistanceProperty = DPM.Register<double, IconContentControl>
			("IconDistance", 4);

		public static readonly DependencyProperty IconDockProperty = DPM.Register<Dock, IconContentControl>
			("IconDock", Dock.Left);

		public static readonly DependencyProperty ShowContentProperty = DPM.Register<bool, IconContentControl>
			("ShowContent", true);

		public static readonly DependencyProperty ShowIconProperty = DPM.Register<bool, IconContentControl>
			("ShowIcon", true);

		public static readonly DependencyProperty VerticalIconAlignmentProperty = DPM.Register<VerticalAlignment, IconContentControl>
			("VerticalIconAlignment", VerticalAlignment.Center);

		public static readonly DependencyProperty HorizontalIconAlignmentProperty = DPM.Register<HorizontalAlignment, IconContentControl>
			("HorizontalIconAlignment", HorizontalAlignment.Center);

		static IconContentControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<IconContentControl>();
		}

		public IconContentControl()
		{
			this.OverrideStyleKey<IconContentControl>();
		}

		public HorizontalAlignment HorizontalIconAlignment
		{
			get => (HorizontalAlignment) GetValue(HorizontalIconAlignmentProperty);
			set => SetValue(HorizontalIconAlignmentProperty, value.Box());
		}

		public double IconDistance
		{
			get => (double) GetValue(IconDistanceProperty);
			set => SetValue(IconDistanceProperty, value);
		}

		public Dock IconDock
		{
			get => (Dock) GetValue(IconDockProperty);
			set => SetValue(IconDockProperty, value);
		}

		public bool ShowContent
		{
			get => (bool) GetValue(ShowContentProperty);
			set => SetValue(ShowContentProperty, value.Box());
		}

		public bool ShowIcon
		{
			get => (bool) GetValue(ShowIconProperty);
			set => SetValue(ShowIconProperty, value.Box());
		}

		public VerticalAlignment VerticalIconAlignment
		{
			get => (VerticalAlignment) GetValue(VerticalIconAlignmentProperty);
			set => SetValue(VerticalIconAlignmentProperty, value.Box());
		}

		IconBase IIconContentControl.Icon
		{
			get => Icon;
			set => Icon = value;
		}

		DependencyProperty IIconContentControl.IconProperty => IconProperty;

		public IconBase Icon
		{
			get => (IconBase) GetValue(IconProperty);
			set => SetValue(IconProperty, value);
		}
	}

	public class IconContentControlTemplateContract : TemplateContract
	{
	}
}