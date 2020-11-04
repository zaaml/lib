// <copyright file="IDeepCloneable.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interactivity
{
	internal interface IDeepCloneable
	{
		#region  Methods

		object DeepClone();

		#endregion
	}

	internal interface IDeepCloneable<out T>
	{
		#region  Methods

		T DeepClone();

		#endregion
	}
}