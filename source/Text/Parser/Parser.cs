// <copyright file="Parser.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TToken> : ParserBase where TToken : unmanaged, Enum
	{
	}

	internal abstract partial class Parser<TGrammar, TToken> : Parser<TToken> where TGrammar : Grammar<TToken> where TToken : unmanaged, Enum
	{
		#region Fields

		private ParserAutomata _automata;

		#endregion

		#region Properties

		protected virtual bool AllowParallel => false;

		private ParserAutomata Automata => _automata ??= AutomataManager.Get<ParserAutomata>();

		#endregion

		#region Methods

		//protected TNode ParseCore<TNode>(Visitor<TNode> visitor, Grammar<TToken>.ParserRuleBase parserRule, LexemeStream<TToken> lexemeStream)
		//{
		//	return ParseInternal(visitor, parserRule, lexemeStream);
		//}

		//protected TResult ParseCore<TResult>(Grammar<TToken>.ParserRuleBase parserRule, LexemeStream<TToken> lexemeStream)
		//{
		//	return ParseInternal<TResult>(parserRule, lexemeStream);
		//}

		private protected TResult ParseInternal<TResult>(Visitor<TResult> visitor, Grammar<TToken>.ParserRule parserRule, LexemeSource<TToken> lexemeSource)
		{
			return Automata.Parse(visitor, parserRule, lexemeSource, CreateContext(lexemeSource), this);
		}

		private protected TResult ParseInternal<TResult>(Grammar<TToken>.ParserRule parserRule, LexemeSource<TToken> lexemeSource)
		{
			return Automata.Parse<TResult>(parserRule, lexemeSource, CreateContext(lexemeSource), this);
		}

		private protected void ParseInternal(Grammar<TToken>.ParserRule parserRule, LexemeSource<TToken> lexemeSource)
		{
			Automata.Parse(parserRule, lexemeSource, CreateContext(lexemeSource), this);
		}

		#endregion
	}

	internal abstract class Parser<TGrammar, TToken, TNode> : Parser<TGrammar, TToken> where TGrammar : Grammar<TToken, TNode> where TToken : unmanaged, Enum where TNode : class
	{
		#region Methods

		protected TResult ParseCore<TResult, TRuleNode>(Visitor<TResult> visitor, Grammar<TToken, TNode>.ParserRule<TRuleNode> parserRule, LexemeSource<TToken> lexemeSource) where TRuleNode : TNode
		{
			return ParseInternal(visitor, parserRule, lexemeSource);
		}

		protected TActualNode ParseCore<TActualNode>(Grammar<TToken, TNode>.ParserRule<TActualNode> parserRule, LexemeSource<TToken> lexemeSource) where TActualNode : TNode
		{
			return (TActualNode) ParseInternal<TNode>(parserRule, lexemeSource);
		}

		protected TBaseNode ParseCore<TBaseNode, TActualNode>(Grammar<TToken, TNode>.ParserRule<TActualNode> parserRule, LexemeSource<TToken> lexemeSource) where TActualNode : TBaseNode where TBaseNode : TNode
		{
			return (TBaseNode) ParseInternal<TNode>(parserRule, lexemeSource);
		}

		#endregion
	}

	internal abstract class Parser<TGrammar, TToken, TNode, TSyntaxFactory> : Parser<TGrammar, TToken> 
		where TGrammar : Grammar<TToken, TNode, TSyntaxFactory> 
		where TToken : unmanaged, Enum 
		where TNode : class 
		where TSyntaxFactory : SyntaxFactory<TNode>, new()
	{
		#region Methods

		protected TResult ParseCore<TResult, TRuleNode>(Visitor<TResult> visitor, Grammar<TToken, TNode, TSyntaxFactory>.ParserRule<TRuleNode> parserRule, LexemeSource<TToken> lexemeSource) where TRuleNode : TNode
		{
			return ParseInternal(visitor, parserRule, lexemeSource);
		}

		protected TActualNode ParseCore<TActualNode>(Grammar<TToken, TNode, TSyntaxFactory>.ParserRule<TActualNode> parserRule, LexemeSource<TToken> lexemeSource) where TActualNode : TNode
		{
			return (TActualNode) ParseInternal<TNode>(parserRule, lexemeSource);
		}

		protected TBaseNode ParseCore<TBaseNode, TActualNode>(Grammar<TToken, TNode, TSyntaxFactory>.ParserRule<TActualNode> parserRule, LexemeSource<TToken> lexemeSource) where TActualNode : TBaseNode where TBaseNode : TNode
		{
			return (TBaseNode) ParseInternal<TNode>(parserRule, lexemeSource);
		}

		private protected override SyntaxFactory CreateSyntaxFactoryInternal()
		{
			return CreateSyntaxFactory();
		}

		protected virtual TSyntaxFactory CreateSyntaxFactory()
		{
			return new TSyntaxFactory();
		}

		#endregion
	}
}