// <copyright file="Automata.Production.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		protected class Production
		{
			protected Production(Entry entry)
			{
				Entries = new[] { entry };
			}

			public Production(IEnumerable<Entry> entries)
			{
				Entries = entries.ToArray();
			}

			public Entry[] Entries { get; }
		}
	}
}