// <copyright file="ITreeEnumerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Core.Trees
{
	internal interface ITreeEnumerator<T> : IEnumerator<T>
	{
		AncestorsEnumerator<T> GetAncestorsEnumerator();
	}
}