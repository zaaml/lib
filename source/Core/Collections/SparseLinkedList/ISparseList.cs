// <copyright file="ISparseList.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Core.Collections
{
	internal interface ISparseList<T> : IList<T>
	{
		bool IsVoid { get; }
		void AddVoid();

		void AddVoidRange(long length);

		void InsertVoid(long index);

		void InsertVoidRange(long index, long length);

		void Void();

		void VoidAt(long index);

		void VoidRange(long index, long length);
	}
}