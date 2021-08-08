// <copyright file="Automata.SubGraphBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private readonly List<SubGraph> _subGraphRegistry = new();

		private int RegisterSubGraph(SubGraph subGraph)
		{
			var id = _subGraphRegistry.Count;

			_subGraphRegistry.Add(subGraph);

			return id;
		}
	}
}