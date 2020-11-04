// <copyright file="DropDownControlHostHelper.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;

namespace Zaaml.UI.Controls.DropDown
{
	internal static class DropDownControlHostHelper
	{
		public static void OnDropDownControlChanged<T>(T owner, PopupControlBase oldControl, PopupControlBase newControl) where T : FrameworkElement, IDropDownControlHost
		{
			if (owner.PopupHost == null)
				SetOwnerLogicalChild(owner, newControl);
			else
				owner.PopupHost.DropDownControl = newControl;
		}

		public static void OnHostAttached<T>(T owner) where T : FrameworkElement, IDropDownControlHost
		{
			owner.LogicalChild = null;

			if (owner.PopupHost != null)
				owner.PopupHost.DropDownControl = owner.PopupControl;
		}

		public static void OnHostDetaching<T>(T owner) where T : FrameworkElement, IDropDownControlHost
		{
			owner.PopupHost.DropDownControl = null;

			if (owner.PopupControl != null)
				SetOwnerLogicalChild(owner, owner.PopupControl);
		}

		private static void SetOwnerLogicalChild<T>(T owner, PopupControlBase dropDownControl) where T : FrameworkElement, IDropDownControlHost
		{
			var depObj = dropDownControl as DependencyObject;

			owner.LogicalChild = depObj?.GetLogicalParent() == null ? dropDownControl : null;
		}
	}
}