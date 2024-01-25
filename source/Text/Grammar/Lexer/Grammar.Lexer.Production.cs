// <copyright file="Grammar.Lexer.Production.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Zaaml.Core.Converters;

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class LexerGrammar
		{
			protected internal sealed class Production : GrammarProduction<Syntax, Production, Symbol>
			{
				public Production(IReadOnlyCollection<Symbol> symbols)
				{
					Index = RegisterProduction(this);

					foreach (var symbol in symbols)
						AddSymbolCore(symbol);
				}

				public int Index { get; }

				public TToken Token { get; private set; }

				public int TokenCode { get; private set; }

				internal void Bind(TToken token)
				{
					Token = token;
					TokenCode = (int)EnumConverter<TToken>.Convert(token);
				}
			}
		}
	}
}