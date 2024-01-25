// <copyright file="Grammar.Parser.Production.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class ParserGrammar
		{
			protected internal sealed class Production : GrammarProduction<Syntax, Production, Symbol>
			{
				public static readonly Production Empty = new(Array.Empty<Symbol>());

				public Production(IReadOnlyCollection<Symbol> symbols)
				{
					Index = RegisterParserSyntaxProduction(this);

					foreach (var symbol in symbols)
						AddSymbolCore(symbol);
				}
				
				public Production(params Symbol[] symbols)
				{
					Index = RegisterParserSyntaxProduction(this);

					foreach (var symbol in symbols)
						AddSymbolCore(symbol);
				}

				public int Index { get; }

				internal string Name { get; set; }

				public Production WithPrecedence(Syntax syntax, short value)
				{
					var enterPrecedence = new EnterPrecedenceSymbol(syntax, value, false);
					var leavePrecedence = new LeavePrecedenceSymbol(syntax, value, false);

					InsertSymbolCore(0, enterPrecedence);
					AddSymbolCore(leavePrecedence);

					return this;
				}

				public Production WithPrecedenceLevel(Syntax syntax)
				{
					var enterPrecedence = new EnterPrecedenceSymbol(syntax, short.MaxValue, true);
					var leavePrecedence = new LeavePrecedenceSymbol(syntax, short.MaxValue, true);

					InsertSymbolCore(0, enterPrecedence);
					AddSymbolCore(leavePrecedence);

					return this;
				}

				public ProductionNodeBinding ProductionBinding { get; internal set; }

				public override string ToString()
				{
					return Name;
				}

				public Production Clone()
				{
					var production = new Production(Symbols)
					{
						Name = Name,
						ProductionBinding = ProductionBinding
					};

					return production;
				}

				public Production Clone(IEnumerable<Symbol> symbols)
				{
					var production = new Production(symbols.ToArray())
					{
						Name = Name,
						ProductionBinding = ProductionBinding
					};

					return production;
				}
			}
		}
	}
}