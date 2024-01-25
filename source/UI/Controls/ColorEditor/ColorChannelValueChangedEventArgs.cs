// <copyright file="ColorChannelValueChangedEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.ColorEditor
{
	public class ColorChannelValueChangedEventArgs : EventArgs
	{
		public ColorChannelValueChangedEventArgs(ColorChannel channel, double oldValue, double newValue)
		{
			Channel = channel;
			OldValue = oldValue;
			NewValue = newValue;
		}

		public ColorChannel Channel { get; }

		public double NewValue { get; }

		public double OldValue { get; }
	}
}