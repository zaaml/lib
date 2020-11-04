// <copyright file="ICopyMembers.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interactivity
{
	internal interface ICopyMembers<in T>
	{
		#region  Methods

		void CopyMembers(T source);

		#endregion
	}
}