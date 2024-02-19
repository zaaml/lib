// <copyright file="DataTreeItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Zaaml.Core;

namespace Zaaml.UI.Test.Data.Hierarchy
{
	internal class DataTreeItem
	{
		public DataTreeItem(DataTree tree, DataTreeItem parent, int localIndex, int globalIndex)
		{
			Tree = tree;
			Parent = parent;
			LocalIndex = localIndex;
			Level = parent?.Level + 1 ?? 0;
			GlobalIndex = globalIndex;
		}

		public string Caption { get; set; }

		public List<DataTreeItem> Children { get; } = new();

		public int GlobalIndex { get; }

		public bool IsExpanded { get; set; }

		public int Level { get; }

		public int LocalIndex { get; }

		[PublicAPI]
		public DataTreeItem Parent { get; }

		[PublicAPI]
		public DataTree Tree { get; }

		public override string ToString()
		{
			return Caption;
		}
	}
}