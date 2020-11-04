// <copyright file="ApplicationMenu.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Menu
{
	public abstract class ApplicationMenu : Control
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty IsOpenProperty = DPM.Register<bool, ApplicationMenu>
			("IsOpen", m => m.OnIsOpenChangedPrivate);

		#endregion

		#region Fields

		public event EventHandler IsOpenChanged;

		#endregion

		#region Properties

		public bool IsOpen
		{
			get => (bool) GetValue(IsOpenProperty);
			set => SetValue(IsOpenProperty, value);
		}

		#endregion

		#region  Methods

		private void OnIsOpenChangedPrivate()
		{
			OnOnIsOpenChangedInternal();

			IsOpenChanged?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void OnOnIsOpenChanged()
		{
		}

		internal virtual void OnOnIsOpenChangedInternal()
		{
			OnOnIsOpenChanged();
		}

		#endregion
	}
}