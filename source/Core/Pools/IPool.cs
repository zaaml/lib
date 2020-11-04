// <copyright file="IPool.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core.Pools
{
	internal interface IPool<T>
	{
		#region Methods

		T Get();

		void Release(T item);

		#endregion
	}
}