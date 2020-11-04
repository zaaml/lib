// <copyright file="Parser.Automata.SubParserInvokeInfo.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		#region Nested Types

		private sealed partial class ParserAutomata
		{
			#region Nested Types

			private class SubParserInvokeInfo<TSubGrammar, TSubToken> where TSubGrammar : Grammar<TSubToken> where TSubToken : unmanaged, Enum
			{
				#region Fields

				public readonly Lexer<TSubToken> Lexer;
				public readonly Parser<TSubGrammar, TSubToken> Parser;
				public readonly ParserPredicateEntry PredicateEntry;
				public readonly Grammar<TSubToken>.ParserRule Rule;

				#endregion

				#region Ctors

				public SubParserInvokeInfo(Grammar<TToken>.SubParserEntry<TSubToken> parserSubAutomataEntry)
				{
					Rule = parserSubAutomataEntry.SubParserRule;
					Parser = (Parser<TSubGrammar, TSubToken>) Rule.Grammar.GetType().GetProperties().SingleOrDefault(p => typeof(Parser<TSubGrammar, TSubToken>).IsAssignableFrom(p.PropertyType))?.GetValue(null);
					Lexer = (Lexer<TSubToken>) Rule.Grammar.GetType().GetProperties().SingleOrDefault(p => typeof(Lexer<TSubToken>).IsAssignableFrom(p.PropertyType))?.GetValue(null);

					if (Parser == null)
						throw new InvalidOperationException("Parser instance is null.");

					if (Lexer == null)
						throw new InvalidOperationException("Lexer instance is null.");

					PredicateEntry = new ParserPredicateEntry(parserSubAutomataEntry, SubParse);
				}

				#endregion

				#region Methods

				private PredicateResult SubParse(AutomataContext automataContext)
				{
					var parserContext = (ParserAutomataContext) automataContext;

					return parserContext.CallSubParser(this);
				}

				#endregion
			}

			private class SubParserInvokeInfo<TSubGrammar, TSubToken, TSubNode, TSubNodeBase> where TSubGrammar : Grammar<TSubToken, TSubNodeBase> where TSubToken : unmanaged, Enum where TSubNode : TSubNodeBase where TSubNodeBase : class
			{
				#region Fields

				public readonly Lexer<TSubToken> Lexer;
				public readonly Parser<TSubGrammar, TSubToken, TSubNodeBase> Parser;
				public readonly ParserPredicateEntry<TSubNode> PredicateEntry;
				public readonly Grammar<TSubToken>.ParserRule Rule;

				#endregion

				#region Ctors

				public SubParserInvokeInfo(Grammar<TToken>.SubParserEntry<TSubToken, TSubNode, TSubNodeBase> parserSubAutomataEntry)
				{
					Rule = parserSubAutomataEntry.SubParserRule;
					Parser = (Parser<TSubGrammar, TSubToken, TSubNodeBase>) Rule.Grammar.GetType().GetProperties().SingleOrDefault(p => typeof(Parser<TSubGrammar, TSubToken, TSubNodeBase>).IsAssignableFrom(p.PropertyType))?.GetValue(null);

					Lexer = (Lexer<TSubToken>) Rule.Grammar.GetType().GetProperties().SingleOrDefault(p => typeof(Lexer<TSubToken>).IsAssignableFrom(p.PropertyType))?.GetValue(null);

					if (Parser == null)
						throw new InvalidOperationException("Parser instance is null.");

					if (Lexer == null)
						throw new InvalidOperationException("Lexer instance is null.");

					PredicateEntry = new ParserPredicateEntry<TSubNode>(parserSubAutomataEntry, SubParse, ParserPredicateKind.SubParser);
				}

				#endregion

				#region Methods

				private PredicateResult<TSubNode> SubParse(AutomataContext automataContext)
				{
					var parserContext = (ParserAutomataContext) automataContext;

					return parserContext.CallValueSubParser(this);
				}

				#endregion
			}

			#endregion
		}

		#endregion
	}
}