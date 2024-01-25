// <copyright file="KeyChordBinding.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Input;

namespace Zaaml.PresentationCore.Input
{
	public class KeyChordBinding : InputBinding
	{
		public KeyChordBinding()
		{
		}

		public KeyChordBinding(ICommand command, KeyChordGesture gesture) : base(command, gesture)
		{
		}

		public override InputGesture Gesture
		{
			get => base.Gesture;
			set
			{
				if (value is not null and not KeyChordGesture)
					throw new InvalidOperationException();

				base.Gesture = value;
			}
		}
	}
}