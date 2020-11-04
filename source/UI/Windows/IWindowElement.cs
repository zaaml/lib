// <copyright file="IWindowElement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.UI.Windows
{
	internal interface IWindowElement
	{
		#region Properties

		IWindow Window { get; set; }

		#endregion

		#region  Methods

		IEnumerable<IWindowElement> EnumerateWindowElements();

		#endregion
	}

	internal interface IWindowEventListener
	{
		void OnResizeStarted();

		void OnResizeFinished();
	}
}