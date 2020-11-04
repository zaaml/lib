// <copyright file="Grammar.ParserFragment.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		#region Nested Types

		protected internal sealed partial class ParserFragment : ParserPrimitiveEntry
		{
			internal ParserFragment(bool inline)
			{
				Inline = inline;
			}

			#region Properties

			public ParserProductionCollection Productions { get; } = new ParserProductionCollection();
			
			public bool Inline { get; }

			#endregion

			public void AddProduction(ParserProduction production)
			{
				Productions.Add(production);
			}
		}

		#endregion
	}
}