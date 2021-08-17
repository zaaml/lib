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

				public int Index { get; }

				internal string Name { get; set; }

				public Production WithPrecedence(Syntax syntax, int value, bool level = false)
				{
					var enterPrecedence = new EnterPrecedenceSymbol(syntax, value, level);
					var leavePrecedence = new LeavePrecedenceSymbol(syntax, value, level);

					InsertSymbolCore(0, enterPrecedence);
					AddSymbolCore(leavePrecedence);

					syntax.MaxPrecedenceValue = Math.Max(syntax.MaxPrecedenceValue, value);

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