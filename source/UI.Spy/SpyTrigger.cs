// <copyright file="SpyTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Spy
{
	public class SpyTrigger : InheritanceContextObject
	{
		private static readonly DependencyPropertyKey IsOpenPropertyKey = DPM.RegisterReadOnly<bool, SpyTrigger>
			("IsOpen", d => d.OnIsOpenPropertyChangedPrivate);

		public static readonly DependencyProperty IsOpenProperty = IsOpenPropertyKey.DependencyProperty;

		public event EventHandler IsOpenChanged;

		public bool IsOpen
		{
			get => (bool) GetValue(IsOpenProperty);
			protected set => this.SetReadOnlyValue(IsOpenPropertyKey, value);
		}

		private void OnIsOpenPropertyChangedPrivate(bool oldValue, bool newValue)
		{
			IsOpenChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}