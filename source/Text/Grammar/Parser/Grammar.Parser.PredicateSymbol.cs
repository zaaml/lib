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
				public PredicateSymbol(Parser<TToken>.PredicateEntry predicate)
				{
					Predicate = predicate;
				}

				public Parser<TToken>.PredicateEntry Predicate { get; }
			}

			protected internal abstract class PrecedenceSymbol : Symbol
			{
				public Syntax Syntax { get; }

				public int Value { get; }

				public bool Level { get; }

				protected PrecedenceSymbol(Syntax syntax, int value, bool level)
				{
					Syntax = syntax;
					Value = value;
					Level = level;
				}
			}

			protected internal sealed class EnterPrecedenceSymbol : PrecedenceSymbol
			{
				public EnterPrecedenceSymbol(Syntax syntax, int value, bool level) : base(syntax, value, level)
				{
				}
			}

			protected internal sealed class LeavePrecedenceSymbol : PrecedenceSymbol
			{
				public LeavePrecedenceSymbol(Syntax syntax, int value, bool level) : base(syntax, value, level)
				{
				}
			}
		}
	}
}