// <copyright file="ITreeEnumeratorAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Core.Trees
{
	internal interface ITreeEnumeratorAdvisor<T>
	{
		IEnumerator<T> GetChildren(T node);
	}
}