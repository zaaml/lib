// <copyright file="RadioGroup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.Core;
using Zaaml.PresentationCore.Behaviors.Selectable;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Primitives
{
	internal interface IRadio
	{
		#region Properties

		bool? IsChecked { get; set; }

		#endregion
	}

	internal class RadioGroup<TRadio> : DependencyObject where TRadio : IRadio
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty CurrentRadioProperty = DPM.Register<TRadio, RadioGroup<TRadio>>
			("CurrentRadio", s => s.OnCurrentRadioChanged);

		#endregion

		#region Fields

		private readonly BoolSuspender _skipChangedSuspender = new BoolSuspender();

		public event EventHandler<SelectedItemChangedEventArgs> SelectedRadioChanged;

		#endregion

		#region Properties

		public TRadio CurrentRadio
		{
			get => (TRadio) GetValue(CurrentRadioProperty);
			set => SetValue(CurrentRadioProperty, value);
		}

		#endregion

		#region  Methods

		protected virtual void OnCurrentRadioChanged(TRadio oldValue, TRadio newValue)
		{
			if (_skipChangedSuspender.IsSuspended)
				return;

			if (oldValue != null)
				oldValue.IsChecked = false;

			if (newValue != null)
				newValue.IsChecked = true;

			SelectedRadioChanged?.Invoke(this, new SelectedItemChangedEventArgs(oldValue, newValue));
		}

		internal void SetCurrentRadio(TRadio value)
		{
			using (BoolSuspender.Suspend(_skipChangedSuspender))
				CurrentRadio = value;
		}

		#endregion
	}
}