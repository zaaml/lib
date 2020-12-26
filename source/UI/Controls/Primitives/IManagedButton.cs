// <copyright file="IManagedButton.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Controls;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.Primitives
{
	internal interface IManagedButton : ICommandControl
	{
		#region Properties

		bool CanClick { get; }

		ClickMode ClickMode { get; }

		bool IsMouseOver { get; }

		bool IsPressed { get; set; }

		bool InvokeCommandBeforeClick { get; }
		
		#endregion

		#region  Methods

		void OnClick();
		
		void OnPreClick();
		
		void OnPostClick();

		void FocusControl();

		#endregion
	}
}