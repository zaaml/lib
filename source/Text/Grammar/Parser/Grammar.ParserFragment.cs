// <copyright file="Grammar.ParserFragment.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		protected internal sealed partial class ParserFragment : ParserPrimitiveEntry
		{
			internal ParserFragment(bool inline)
			{
				Inline = inline;
			}

			public bool Inline { get; }

			public ParserProductionCollection Productions { get; } = new ParserProductionCollection();

			public void AddProduction(ParserProduction production)
			{
				Productions.Add(production);
			}
		}
	}
}