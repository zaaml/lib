// <copyright file="Grammar.Parser.PredicateSymbol.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class ParserGrammar
		{
			protected internal sealed class PredicateSymbol : Symbol
			{
				public PredicateSymbol(PredicateSyntax predicateSyntax)
				{
					PredicateSyntax = predicateSyntax;
				}

				public Parser<TToken>.PredicateEntry PredicateEntry => PredicateSyntax.PredicateEntry;

				public PredicateSyntax PredicateSyntax { get; }
			}

			protected internal sealed class DataSymbol<TData> : Symbol
			{
				public DataSymbol(TData data)
				{
					Data = data;
				}

				public TData Data { get; }
			}
		}
	}
}