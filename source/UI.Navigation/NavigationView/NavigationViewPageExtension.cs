// <copyright file="NavigationViewPageExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Data;
using System.Windows.Markup;
using Zaaml.PresentationCore.MarkupExtensions;

namespace Zaaml.UI.Controls.NavigationView
{
	public sealed class NavigationViewPageExtension : MarkupExtensionBase
	{
		public Type Type { get; set; }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget targetProvider && targetProvider.TargetObject is NavigationViewItem navigationViewItem)
				return new Binding {Source = Type, BindsDirectlyToSource = true, Converter = new DeferredPageTypeConverter(navigationViewItem)}.ProvideValue(serviceProvider);

			return null;
		}
	}
}