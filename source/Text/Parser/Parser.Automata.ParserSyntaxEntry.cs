// <copyright file="Parser.Automata.ParserSyntaxEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class ParserSyntaxEntry : SyntaxEntry, IParserEntry
			{
				public ParserSyntaxEntry(Grammar<TGrammar, TToken>.ParserGrammar.NodeSymbol grammarSymbol, Syntax syntax) : base(syntax)
				{
					GrammarSymbol = grammarSymbol;
					Fragment = false;
				}

				public ParserSyntaxEntry(Grammar<TGrammar, TToken>.ParserGrammar.FragmentSymbol grammarSymbol, Syntax syntax) : base(syntax)
				{
					GrammarSymbol = grammarSymbol;
					Fragment = true;
				}

				public ParserSyntaxEntry(Grammar<TGrammar, TToken>.ParserGrammar.TokenSymbol grammarSymbol, Syntax syntax) : base(syntax)
				{
					GrammarSymbol = grammarSymbol;
					Fragment = true;
				}

				protected override string DebuggerDisplay
				{
					get
					{
						if (Fragment)
							return $"({string.Join(" | ", Syntax.Productions.Cast<ParserProduction>().Select(p => p.EntriesDebuggerDisplay))})";

						return base.DebuggerDisplay;
					}
				}

				public bool Fragment { get; }

				public Grammar<TGrammar, TToken>.ParserGrammar.Symbol GrammarSymbol { get; }

				public ProductionArgument ProductionArgument { get; set; }

				public Entry Clone()
				{
					if (Fragment)
						return new ParserSyntaxEntry((Grammar<TGrammar, TToken>.ParserGrammar.FragmentSymbol)GrammarSymbol, Syntax)
						{
							Source = this
						};

					return new ParserSyntaxEntry((Grammar<TGrammar, TToken>.ParserGrammar.NodeSymbol)GrammarSymbol, Syntax)
					{
						Source = this
					};
				}

				public IParserEntry Source { get; private set; }
			}
		}
	}
}