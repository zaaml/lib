// <copyright file="RadioMenuItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Primitives;

namespace Zaaml.UI.Controls.Menu
{
	public class RadioMenuItem : ToggleMenuItem, IRadio
	{
		#region Static Fields and Constants

		private static readonly MenuRadioGroup GlobalRadioGroup = new MenuRadioGroup();

		private static readonly DependencyProperty RadioGroupAttachedProperty = DPM.RegisterAttached<MenuRadioGroup, RadioMenuItem>
			("RadioGroupAttached");

		#endregion

		#region Fields

		private event EventHandler IsSelectedChanged;

		#endregion

		#region Ctors

		static RadioMenuItem()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<RadioMenuItem>();
		}

		public RadioMenuItem()
		{
			this.OverrideStyleKey<RadioMenuItem>();
			IsSubmenuEnabled = false;
		}

		#endregion

		#region Properties

		private RadioGroup<RadioMenuItem> RadioGroup => GetRadioGroup() ?? GlobalRadioGroup;

		#endregion

		#region  Methods

		private RadioGroup<RadioMenuItem> GetRadioGroup()
		{
			return GetRadioGroup(Owner as FrameworkElement);
		}

		internal static RadioGroup<RadioMenuItem> GetRadioGroup(FrameworkElement element)
		{
			return element?.GetValueOrCreate(RadioGroupAttachedProperty, () => new MenuRadioGroup());
		}

		protected override void OnClick()
		{
			base.OnClick();

			UpdateRadioGroup();
		}

		internal override void OnIsCheckedChangedInternal(bool? oldValue, bool? newValue)
		{
			base.OnIsCheckedChangedInternal(oldValue, newValue);

			var oldIsSelected = oldValue == true;
			var newIsSelected = newValue == true;

			if (oldIsSelected != newIsSelected)
				IsSelectedChanged?.Invoke(this, EventArgs.Empty);
		}

		internal override void OnOwnerChangedInternal(IMenuItemOwner oldOwner, IMenuItemOwner newOwner)
		{
			base.OnOwnerChangedInternal(oldOwner, newOwner);
			UpdateRadioGroup();
		}

		private void UpdateRadioGroup()
		{
			if (IsChecked == true)
				RadioGroup.CurrentRadio = this;
		}

		#endregion

		#region Interface Implementations

		#region IRadio

		bool? IRadio.IsChecked
		{
			get => IsChecked;
			set => this.SetCurrentValueInternal(IsCheckedProperty, value);
		}

		#endregion

		#endregion

		#region  Nested Types

		private class MenuRadioGroup : RadioGroup<RadioMenuItem>
		{
		}

		#endregion
	}
}