// <copyright file="Automata.Production.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		protected class Production
		{
			#region Ctors

			protected Production(Entry entry)
			{
				Entries = new[] {entry};
			}

			public Production(IEnumerable<Entry> entries)
			{
				Entries = entries.ToArray();
			}

			#endregion

			#region Properties

			public Entry[] Entries { get; internal set; }

			#endregion
		}

		#endregion
	}
}