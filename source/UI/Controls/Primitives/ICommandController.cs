// <copyright file="ICommandController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Primitives
{
	internal interface ICommandController
	{
		#region  Methods

		void OnCommandChanged();

		void OnCommandParameterChanged();

		void OnCommandTargetChanged();

		#endregion
	}
}