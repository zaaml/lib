// <copyright file="HierarchyView.Test.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

#if TEST
using System;
using System.Linq;
using System.Text;
using Zaaml.Core.Extensions;
using Zaaml.Core.Trees;

namespace Zaaml.UI.Data.Hierarchy
{
	internal abstract partial class HierarchyView
	{
		[Flags]
		internal enum DumpOptions
		{
			Plain = 0,
			Padding = 1,
			LineNumbers = 2,
			ExpansionGlyph = 4,
			DebugInformation = 8,
			Full = Padding + LineNumbers + ExpansionGlyph + DebugInformation
		}
	}

	internal abstract partial class HierarchyView<THierarchy, TNodeCollection, TNode>
		where THierarchy : HierarchyView<THierarchy, TNodeCollection, TNode>
		where TNodeCollection : HierarchyNodeViewCollection<THierarchy, TNodeCollection, TNode>
		where TNode : HierarchyNodeView<THierarchy, TNodeCollection, TNode>
	{
		internal string Dump(DumpOptions dumpOptions)
		{
			var sb = new StringBuilder();
			var usePadding = (dumpOptions & DumpOptions.Padding) != 0;
			var useLineIndex = (dumpOptions & DumpOptions.LineNumbers) != 0;
			var useExpansionGlyph = (dumpOptions & DumpOptions.ExpansionGlyph) != 0;
			var useDebugInformation = (dumpOptions & DumpOptions.DebugInformation) != 0;

			for (var iNode = 0; iNode < VisibleFlatCount; iNode++)
			{
				var treeNode = GetNode(iNode);
				var padding = usePadding ? "".PadLeft(treeNode.Level * 4) : "";
				var lineIndex = useLineIndex ? iNode.ToString().PadRight(10) : "";
				var expansionGlyph = useExpansionGlyph ? treeNode.IsExpanded ? "[-] " : "[+] " : "";
				var debugInformation = useDebugInformation ? $"  FC: {treeNode.FlatCount}, VFC: {treeNode.VisibleFlatCount}" : "";

				sb.AppendLine(lineIndex + padding + expansionGlyph + treeNode.Data + debugInformation);
			}

			return sb.ToString();
		}

		internal void Verify()
		{
			var treeAdvisor = new DelegateTreeEnumeratorAdvisor<TNode>(n => n.Nodes?.GetEnumerator() ?? Enumerable.Empty<TNode>().GetEnumerator());
			var flatCount = Nodes.Sum(n => n.FlatCount) + Nodes.Count;
			var visibleCount = Nodes.Sum(n => n.VisibleFlatCount) + Nodes.Count;

			foreach (var node in TreeEnumerator.GetEnumerator(Nodes, treeAdvisor).Enumerate())
				node.Verify();

			if (FlatCount != flatCount)
				throw new Exception(nameof(FlatCount));

			if (VisibleFlatCount != visibleCount)
				throw new Exception(nameof(VisibleFlatCount));
		}
	}
}
#endif