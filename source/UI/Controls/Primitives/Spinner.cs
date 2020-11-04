// <copyright file="Spinner.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Primitives
{
	public abstract class Spinner : Control
	{
		#region Static Fields and Constants

		private static readonly DependencyPropertyKey ActualCanIncreasePropertyKey = DPM.RegisterReadOnly<bool, Spinner>
			("ActualCanIncrease");

		public static readonly DependencyProperty ActualCanIncreaseProperty = ActualCanIncreasePropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey ActualCanDecreasePropertyKey = DPM.RegisterReadOnly<bool, Spinner>
			("ActualCanDecrease");

		public static readonly DependencyProperty ActualCanDecreaseProperty = ActualCanDecreasePropertyKey.DependencyProperty;

		#endregion

		#region Fields

		public event EventHandler<SpinEventArgs> Spin;

		#endregion

		#region Properties

		public bool ActualCanDecrease
		{
			get => (bool) GetValue(ActualCanDecreaseProperty);
			set => this.SetReadOnlyValue(ActualCanDecreasePropertyKey, value);
		}

		public bool ActualCanIncrease
		{
			get => (bool) GetValue(ActualCanIncreaseProperty);
			set => this.SetReadOnlyValue(ActualCanIncreasePropertyKey, value);
		}

		#endregion

		#region  Methods

		protected virtual void OnSpin(SpinEventArgs e)
		{
			if (ValidateSpinDirection(e.Direction))
				Spin?.Invoke(this, e);
		}

		private bool ValidateSpinDirection(SpinDirection spinDirection)
		{
			switch (spinDirection)
			{
				case SpinDirection.Increase:
					return ActualCanIncrease;
				case SpinDirection.Decrease:
					return ActualCanDecrease;
				default:
					throw new ArgumentOutOfRangeException(nameof(spinDirection));
			}
		}

		#endregion
	}
}