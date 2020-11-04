// <copyright file="ThemeTemplateLoader.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Theming
{
	internal static class ThemeTemplateLoader
	{
		#region Static Fields and Constants

		private static readonly ThemeLoaderPanel LoaderPanel;
		private static readonly Popup LoaderPopup;

		#endregion

		#region Ctors

		static ThemeTemplateLoader()
		{
			LoaderPopup = new Popup();
			LoaderPanel = new ThemeLoaderPanel();

			LoaderPopup.Child = LoaderPanel;
		}

		#endregion

		#region  Methods

		private static bool ApplyTemplateImpl(FrameworkElement frameworkElement)
		{
#if SILVERLIGHT
			var control = frameworkElement as Control;
			if (control != null)
				return control.ApplyTemplate();

			frameworkElement.Measure(XamlConstants.ZeroSize);
			frameworkElement.InvalidateMeasure();

			return true;
#else
			return frameworkElement.ApplyTemplate();
#endif
		}

		public static bool ApplyThemeTemplate(FrameworkElement element)
		{
			if (element.GetVisualParent() != null)
				return ApplyTemplateImpl(element);

			LoaderPanel.Children.Add(element);
			LoaderPopup.IsOpen = true;

			var result = ApplyTemplateImpl(element);
			LoaderPopup.IsOpen = false;
			LoaderPanel.Children.Remove(element);


			return result;
		}

		#endregion

		#region  Nested Types

		private class ThemeLoaderPanel : Panel
		{
		}

		#endregion
	}
}