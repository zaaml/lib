// <copyright file="ILogicalOwner.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;

namespace Zaaml.PresentationCore
{
	internal interface ILogicalOwner
	{
		#region  Methods

		void AddLogicalChild(object child);

		void RemoveLogicalChild(object child);

		IEnumerator BaseLogicalChildren { get; }

		#endregion
	}
}