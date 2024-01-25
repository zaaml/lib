﻿// <copyright file="ResourceItemBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore
{
	public abstract class ResourceItemBase<TValue> : InheritanceContextObject
	{
		public static readonly DependencyProperty ValueProperty = DPM.Register<object, ResourceItemBase<TValue>>
			("Value");

		public TValue Value
		{
			get => (TValue)GetValue(ValueProperty);
			set => SetValue(ValueProperty, value);
		}
	}
}