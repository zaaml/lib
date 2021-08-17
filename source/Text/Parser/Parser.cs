// <copyright file="Parser.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TToken> : ParserBase where TToken : unmanaged, Enum
	{
	}

	internal partial class Parser<TGrammar, TToken> : Parser<TToken> 
		where TGrammar : Grammar<TGrammar, TToken> 
		where TToken : unmanaged, Enum
	{
		private ParserAutomata _automata;

		protected virtual bool AllowParallel => false;

		private ParserAutomata Automata => _automata ??= AutomataManager.Get<ParserAutomata>();

		//protected TNode ParseCore<TNode>(Visitor<TNode> visitor, Grammar<TToken>.ParserRuleBase parserRule, LexemeStream<TToken> lexemeStream)
		//{
		//	return ParseInternal(visitor, parserRule, lexemeStream);
		//}

		//protected TResult ParseCore<TResult>(Grammar<TToken>.ParserRuleBase parserRule, LexemeStream<TToken> lexemeStream)
		//{
		//	return ParseInternal<TResult>(parserRule, lexemeStream);
		//}

		private protected TResult ParseInternal<TResult>(Visitor<TResult> visitor, Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax parserRule, LexemeSource<TToken> lexemeSource)
		{
			return Automata.Parse(visitor, parserRule, lexemeSource, CreateContext(lexemeSource), this);
		}

		private protected TResult ParseInternal<TResult>(Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax parserRule, LexemeSource<TToken> lexemeSource)
		{
			return Automata.Parse<TResult>(parserRule, lexemeSource, CreateContext(lexemeSource), this);
		}

		private protected void ParseInternal(Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax parserRule, LexemeSource<TToken> lexemeSource)
		{
			Automata.Parse(parserRule, lexemeSource, CreateContext(lexemeSource), this);
		}
		
		protected TResult ParseCore<TResult, TNode>(Visitor<TResult> visitor, Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax<TNode> parserRule, LexemeSource<TToken> lexemeSource) where TNode : class
		{
			return ParseInternal(visitor, parserRule, lexemeSource);
		}

		protected TNode ParseCore<TNode>(Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax<TNode> parserRule, LexemeSource<TToken> lexemeSource) where TNode : class 
		{
			return ParseInternal<TNode>(parserRule, lexemeSource);
		}

		public readonly struct SyntaxNodeParser<TNode> where TNode : class
		{
			public Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax<TNode> Syntax { get; }
			
			public Lexer<TGrammar, TToken> Lexer { get; }

			public Parser<TGrammar, TToken> Parser { get; }

			public SyntaxNodeParser(Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax<TNode> syntax, Lexer<TGrammar, TToken> lexer, Parser<TGrammar, TToken> parser)
			{
				Syntax = syntax;
				Lexer = lexer;
				Parser = parser;
			}

			public TNode Parse(string nodeString)
			{
				using var lexemeStream = Lexer.GetLexemeSource(nodeString);

				return Parser.ParseCore(Syntax, lexemeStream);
			}
		}
	}


	//internal abstract class Parser<TGrammar, TToken, TNode, TSyntaxFactory> : Parser<TGrammar, TToken>
	//	where TGrammar : Grammar<TGrammar, TToken, TNode, TSyntaxFactory>
	//	where TToken : unmanaged, Enum
	//	where TNode : class
	//	where TSyntaxFactory : SyntaxFactory<TNode>, new()
	//{
	//	protected virtual TSyntaxFactory CreateSyntaxFactory()
	//	{
	//		return new TSyntaxFactory();
	//	}

	//	private protected override SyntaxFactory CreateSyntaxFactoryInternal()
	//	{
	//		return CreateSyntaxFactory();
	//	}

	//	protected TResult ParseCore<TResult, TRuleNode>(Visitor<TResult> visitor, Grammar<TGrammar, TToken, TNode, TSyntaxFactory>.Parser.ParserSyntaxNode<TRuleNode> parserRule, LexemeSource<TToken> lexemeSource) where TRuleNode : TNode
	//	{
	//		return ParseInternal(visitor, parserRule, lexemeSource);
	//	}

	//	protected TActualNode ParseCore<TActualNode>(Grammar<TGrammar, TToken, TNode, TSyntaxFactory>.Parser.ParserSyntaxNode<TActualNode> parserRule, LexemeSource<TToken> lexemeSource) where TActualNode : TNode
	//	{
	//		return (TActualNode)ParseInternal<TNode>(parserRule, lexemeSource);
	//	}

	//	protected TBaseNode ParseCore<TBaseNode, TActualNode>(Grammar<TGrammar, TToken, TNode, TSyntaxFactory>.Parser.ParserSyntaxNode<TActualNode> parserRule, LexemeSource<TToken> lexemeSource) where TActualNode : TBaseNode where TBaseNode : TNode
	//	{
	//		return (TBaseNode)ParseInternal<TNode>(parserRule, lexemeSource);
	//	}
	//}
}