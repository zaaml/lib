// <copyright file="IWindowElement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.UI.Windows
{
	internal interface IWindowElement
	{
		IWindow Window { get; set; }

		IEnumerable<IWindowElement> EnumerateWindowElements();
	}

	internal interface IWindowEventListener
	{
		void OnResizeFinished();
		void OnResizeStarted();
	}
}