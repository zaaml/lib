// <copyright file="ExplicitTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	public sealed class ExplicitTrigger : PopupTrigger
	{
		public static readonly DependencyProperty IsOpenProperty = DPM.Register<bool, ExplicitTrigger>
			("IsOpen", default, d => d.OnIsOpenPropertyChangedPrivate);

		public bool IsOpen
		{
			get => (bool)GetValue(IsOpenProperty);
			set => SetValue(IsOpenProperty, value.Box());
		}

		private void OnIsOpenPropertyChangedPrivate(bool oldValue, bool newValue)
		{
			if (newValue)
				Open();
			else
				Close();
		}
	}
}