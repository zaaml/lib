// <copyright file="ContextMenuService.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;

namespace Zaaml.UI.Controls.Menu
{
	public static class ContextMenuService
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty ContextMenuProperty = DPM.RegisterAttached<ContextMenu>
		("ContextMenu", typeof(ContextMenuService),
			DPM.StaticCallback<FrameworkElement, ContextMenu>(OnContextMenuPropertyChanged));

		public static readonly DependencyProperty ContextMenuSelectorProperty = DPM.RegisterAttached<ContextMenuSelector>
		("ContextMenuSelector", typeof(ContextMenuService),
			DPM.StaticCallback<FrameworkElement, ContextMenuSelector>(OnContextMenuSelectorPropertyChanged));

		#endregion

		#region  Methods

		public static ContextMenu GetContextMenu(FrameworkElement element)
		{
			return element.GetValue<ContextMenu>(ContextMenuProperty);
		}

		public static ContextMenuSelector GetContextMenuSelector(FrameworkElement element)
		{
			return element.GetValue<ContextMenuSelector>(ContextMenuSelectorProperty);
		}

		private static void OnContextMenuPropertyChanged(FrameworkElement frameworkElement, ContextMenu oldMenu, ContextMenu newMenu)
		{
			SharedItemHelper.Share(frameworkElement, oldMenu, newMenu);
			PopupControlService.OnPopupControllerSelectorChanged(frameworkElement, oldMenu?.PopupController, newMenu?.PopupController);
		}

		private static void OnContextMenuSelectorPropertyChanged(FrameworkElement frameworkElement, ContextMenuSelector oldMenuSelector, ContextMenuSelector newMenuSelector)
		{
			SharedItemHelper.Share(frameworkElement, oldMenuSelector, newMenuSelector);
			PopupControlService.OnPopupControllerSelectorChanged(frameworkElement, oldMenuSelector, newMenuSelector);
		}

		public static void SetContextMenu(FrameworkElement element, ContextMenu value)
		{
			element.SetValue(ContextMenuProperty, value);
		}

		public static void SetContextMenuSelector(FrameworkElement element, ContextMenuSelector value)
		{
			element.SetValue(ContextMenuSelectorProperty, value);
		}

		#endregion
	}
}