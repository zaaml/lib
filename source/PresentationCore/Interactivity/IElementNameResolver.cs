// <copyright file="IElementNameResolver.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;

namespace Zaaml.PresentationCore.Interactivity
{
	internal interface IElementNameResolver : IDisposable
	{
		#region Fields

		event EventHandler ElementResolved;

		#endregion

		#region Properties

		DependencyObject Element { get; }

		#endregion
	}
}