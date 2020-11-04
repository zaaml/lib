// <copyright file="ToggleMenuItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Interfaces;
using Zaaml.UI.Controls.Primitives;

namespace Zaaml.UI.Controls.Menu
{
	public class ToggleMenuItem : MenuItem, IToggleButton
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty IsCheckedProperty = DPM.Register<bool?, ToggleMenuItem>
			("IsChecked", false, c => c.OnIsCheckedChangedPrivate);

		public static readonly DependencyProperty IsThreeStateProperty = DPM.Register<bool, ToggleMenuItem>
			("IsThreeState");

		#endregion

		#region Ctors

		static ToggleMenuItem()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ToggleMenuItem>();
		}

		public ToggleMenuItem()
		{
			this.OverrideStyleKey<ToggleMenuItem>();
			IsSubmenuEnabled = false;
		}

		#endregion

		#region  Methods

		protected virtual void OnChecked()
		{
			Checked?.Invoke(this, RoutedEventArgsFactory.Create(this));
		}

		protected override void OnClick()
		{
			this.OnToggle();

			switch (IsChecked)
			{
				case true:

					OnChecked();

					break;

				case false:

					OnUnchecked();

					break;

				default:

					OnIndeterminate();

					break;
			}

			base.OnClick();
		}

		protected virtual void OnIndeterminate()
		{
			Indeterminate?.Invoke(this, RoutedEventArgsFactory.Create(this));
		}

		protected virtual void OnIsCheckedChanged(bool? oldValue, bool? newValue)
		{
		}

		internal virtual void OnIsCheckedChangedInternal(bool? oldValue, bool? newValue)
		{
			OnIsCheckedChanged(oldValue, newValue);
		}

		private void OnIsCheckedChangedPrivate(bool? oldValue, bool? newValue)
		{
			OnIsCheckedChangedInternal(oldValue, newValue);

			IsSubmenuEnabled = IsChecked == true;

			UpdateHeaderState();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();
			UpdateActualIsChecked();
		}

		protected virtual void OnUnchecked()
		{
			Unchecked?.Invoke(this, RoutedEventArgsFactory.Create(this));
		}

		private void UpdateActualIsChecked()
		{
			HeaderPresenter.ActualIsChecked = IsChecked;
		}

		protected override void UpdateHeaderStateCore()
		{
			base.UpdateHeaderStateCore();

			UpdateActualIsChecked();
		}

		#endregion

		#region Interface Implementations

		#region IToggleButton

		public event RoutedEventHandler Checked;
		public event RoutedEventHandler Unchecked;
		public event RoutedEventHandler Indeterminate;

		public bool? IsChecked
		{
			get => (bool?) GetValue(IsCheckedProperty);
			set => SetValue(IsCheckedProperty, value);
		}

		public bool IsThreeState
		{
			get => this.GetValue<bool>(IsThreeStateProperty);
			set => SetValue(IsThreeStateProperty, value);
		}

		DependencyProperty IToggleButton.IsCheckedPropertyInt => IsCheckedProperty;

		#endregion

		#endregion
	}
}