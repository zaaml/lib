// <copyright file="XamlConstants.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.MarkupExtensions;

namespace Zaaml.PresentationCore
{
	public enum FrameworkType
	{
		WPF,
		Silverlight
	}

	[Flags]
	public enum AllowedFramework
	{
		None = 0x0,
		WPF = 0x1,
		Silverlight = 0x2,
		All = WPF | Silverlight
	}

	public static class XamlConstants
	{
		public static readonly Size NanSize = new(double.NaN, double.NaN);
		public static readonly Size InfiniteSize = new(double.PositiveInfinity, double.PositiveInfinity);
		public static readonly Size ZeroSize = new(0.0, 0.0);
		public static readonly Rect ZeroRect = new(0.0, 0.0, 0.0, 0.0);
		public static readonly Point ZeroPoint = new(0.0, 0.0);

		public static readonly string XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
		public static readonly string XamlXNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";
		public static readonly string XamlMCNamespace = "http://schemas.openxmlformats.org/markup-compatibility/2006";
		public static readonly string XamlZMNamespace = "http://schemas.zaaml.com/xaml";
		public static readonly string XamlZMPrefix = "zm";

#if SILVERLIGHT
		public static readonly FrameworkType Framework = FrameworkType.Silverlight;
#else
		public static readonly FrameworkType Framework = FrameworkType.WPF;
#endif

		internal static readonly RelativeSource TemplatedParent = new(RelativeSourceMode.TemplatedParent);
		internal static readonly RelativeSource Self = new(RelativeSourceMode.Self);

		internal static readonly int LayoutComparisonPrecision = 4;

		internal static SolidColorBrush TransparentBrush = Colors.Transparent.ToSolidColorBrush().AsFrozen();
	}

	public enum KnownFrameworkType
	{
		DatePickerTextBox,
		DatePicker,
		GridSplitter,
		TabControl,
		TabItem,
		TabPanel,
		TreeView,
		TreeViewItem,
		Calendar,
		CalendarItem,
		CalendarButton,
		CalendarDayButton
	}

	public class KnownFrameworkTypeExtension : MarkupExtensionBase
	{
		private static readonly Dictionary<KnownFrameworkType, object> Dictionary = new();

		static KnownFrameworkTypeExtension()
		{
			Dictionary[KnownFrameworkType.DatePickerTextBox] = typeof(DatePickerTextBox);
			Dictionary[KnownFrameworkType.DatePicker] = typeof(DatePicker);
			Dictionary[KnownFrameworkType.GridSplitter] = typeof(GridSplitter);
			Dictionary[KnownFrameworkType.TabControl] = typeof(TabControl);
			Dictionary[KnownFrameworkType.TabItem] = typeof(TabItem);
			Dictionary[KnownFrameworkType.TabPanel] = typeof(TabPanel);
			Dictionary[KnownFrameworkType.TreeView] = typeof(TreeView);
			Dictionary[KnownFrameworkType.TreeViewItem] = typeof(TreeViewItem);
			Dictionary[KnownFrameworkType.Calendar] = typeof(Calendar);
			Dictionary[KnownFrameworkType.CalendarItem] = typeof(CalendarItem);
			Dictionary[KnownFrameworkType.CalendarButton] = typeof(CalendarButton);
			Dictionary[KnownFrameworkType.CalendarDayButton] = typeof(CalendarDayButton);
		}

		public KnownFrameworkType Type { get; set; }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return Dictionary.GetValueOrDefault(Type, () => Guid.NewGuid().ToString());
		}
	}

	public class StyleTypeKeyExtension : MarkupExtensionBase
	{
		public Type Type { get; set; }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			switch (XamlConstants.Framework)
			{
				case FrameworkType.WPF:
					return Type;
				case FrameworkType.Silverlight:
					return Type.Name;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}