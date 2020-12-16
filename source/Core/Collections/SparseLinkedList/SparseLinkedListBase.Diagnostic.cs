// <copyright file="SparseLinkedListBase.Diagnostic.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Text;

namespace Zaaml.Core.Collections
{
	internal partial class SparseLinkedListBase<T>
	{
		[UsedImplicitly]
		internal string Dump()
		{
			var sb = new StringBuilder();
			NodeBase node = HeadNode;

			while (node != null)
			{
				if (ReferenceEquals(node, HeadNode) == false)
					sb.Append("  ");

				sb.Append(node);

				node = node.Next;
			}

			return sb.ToString();
		}
	}
}