// <copyright file="BackgroundPatternBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Primitives
{
	public abstract class BackgroundPatternBase : InheritanceContextObject
	{
		public event EventHandler PatternChanged;

		protected abstract UIElement CreatePatternElementCore();

		internal UIElement CreatePatternElementInternal()
		{
			return CreatePatternElementCore();
		}

		protected virtual void OnPatternChanged()
		{
			PatternChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}