// <copyright file="ITreeAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;

namespace Zaaml.UI.Test.Data.Hierarchy
{
	internal interface ITreeAdvisor
	{
		IEnumerable GetNodes(object treeNodeData);

		bool IsExpanded(object treeNodeData);
	}
}