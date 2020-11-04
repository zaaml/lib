// <copyright file="IconContentControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Primitives.ContentPrimitives;

namespace Zaaml.UI.Controls.Core
{
	[TemplateContractType(typeof(IconContentControlTemplateContract))]
	public class IconContentControl : TemplateContractContentControl, IIconOwner
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
			set => SetValue(ShowContentProperty, value);
		}

		public bool ShowIcon
		{
			get => (bool) GetValue(ShowIconProperty);
			set => SetValue(ShowIconProperty, value);
		}

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