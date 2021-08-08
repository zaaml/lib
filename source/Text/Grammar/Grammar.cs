// <copyright file="Grammar.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Zaaml.Text
{
	internal abstract class Grammar
	{
		private static readonly Dictionary<Type, Grammar> Grammars = new();

		protected Grammar()
		{
			Grammars[GetType()] = this;
		}

		public static Grammar<TToken> Get<TToken>(Type grammarType) where TToken : unmanaged, Enum
		{
			RuntimeHelpers.RunClassConstructor(grammarType.TypeHandle);

			if (Grammars.TryGetValue(grammarType, out var grammarBase))
			{
				var grammar = (Grammar<TToken>)grammarBase;

				grammar.Build();

				return grammar;
			}

			{
				var grammar = (Grammar<TToken>)Activator.CreateInstance(grammarType, true);

				grammar.Build();

				Grammars[grammarType] = grammar;

				return grammar;
			}
		}

		public static TGrammar Get<TGrammar, TToken>() where TGrammar : Grammar<TToken> where TToken : unmanaged, Enum
		{
			return (TGrammar)Get<TToken>(typeof(TGrammar));
		}
	}

	internal abstract partial class Grammar<TToken> : Grammar where TToken : unmanaged, Enum
	{
		// ReSharper disable once StaticMemberInGenericType
		private static readonly int EnumTokenCodeOffset;

		private readonly Dictionary<string, ParserRule> _parserRuleDictionary = new();
		private readonly Dictionary<string, TokenFragment> _tokenFragmentDictionary = new();
		private readonly Dictionary<string, TokenRule> _tokenRuleDictionary = new();
		private bool _isBuilt;
		private int _tokenCounter;

		static Grammar()
		{
			foreach (int value in Enum.GetValues(typeof(TToken)))
				EnumTokenCodeOffset = Math.Max(EnumTokenCodeOffset, value);

			EnumTokenCodeOffset++;
		}

		// ReSharper disable once EmptyConstructor
		internal Grammar(TToken undefinedToken)
		{
			UndefinedToken = undefinedToken;
		}

		protected EmptyEntry Empty => EmptyEntry.Instance;

		internal IEnumerable<ParserRule> ParserRules => _parserRuleDictionary.Values;

		internal IEnumerable<TokenFragment> TokenFragments => _tokenFragmentDictionary.Values;

		internal IEnumerable<TokenRule> TokenRules => _tokenRuleDictionary.Values;
		public TToken UndefinedToken { get; }

		internal void Build()
		{
			if (_isBuilt)
				return;

			_isBuilt = true;

			foreach (var tokenFragment in CreatedTokenFragments)
			{
				tokenFragment.TokenCode = GetTokenCode(tokenFragment);

				_tokenFragmentDictionary.Add(tokenFragment.Name, tokenFragment);
			}

			foreach (var tokenRule in CreatedTokenRules)
			{
				if (tokenRule?.Pattern == null)
					continue;

				tokenRule.TokenCode = GetTokenCode(tokenRule);

				_tokenRuleDictionary.Add(tokenRule.Name, tokenRule);
			}

			foreach (var parserRule in CreatedParserRules)
			{
				_parserRuleDictionary.Add(parserRule.Name, parserRule);
			}
		}

		private int GetTokenCode(object tokenEntry)
		{
			if (tokenEntry is TokenRule tokenRule)
				return (int)(object)tokenRule.Token;

			// All instruction codes must be greater than zero. Zero instruction code - the end instruction.
			return 1 + EnumTokenCodeOffset + _tokenCounter++;
		}

		protected sealed class EmptyEntry
		{
			internal static readonly EmptyEntry Instance = new();

			private EmptyEntry()
			{
			}

			public static implicit operator ParserProduction(EmptyEntry entry)
			{
				return ParserProduction.Empty;
			}
		}
	}

	internal abstract class Grammar<TToken, TNode> : Grammar<TToken> where TToken : unmanaged, Enum where TNode : class
	{
		protected Grammar(TToken undefinedToken) : base(undefinedToken)
		{
		}

		protected static ParserRule<TActualNode> CreateParserRule<TActualNode>([CallerMemberName] string name = null) where TActualNode : TNode
		{
			var parserRule = new ParserRule<TActualNode>
			{
				Name = name
			};

			CreatedParserRules.Add(parserRule);

			return parserRule;
		}

		protected internal sealed class ParserRule<TActualNode> : ParserRule where TActualNode : TNode
		{
			internal ParserRule()
			{
			}

			public new Grammar<TToken, TNode> Grammar => (Grammar<TToken, TNode>)base.Grammar;

			public override Type NodeType => typeof(TActualNode);

			public void Bind<TResultNode>(ParserProduction parserProduction) where TResultNode : TActualNode
			{
				parserProduction.Name = typeof(TResultNode).Name;
				parserProduction.ProductionBinding = ConstructorBinding.Bind<TResultNode>();

				Productions.Add(parserProduction);
			}

			public void BindReturn<TResultNode, TBaseNode>(Grammar<TToken, TBaseNode>.ParserRule<TResultNode> rule) where TResultNode : TBaseNode where TBaseNode : class
			{
				var transition = new ParserProduction(new ParserEntry[] { rule })
				{
					Name = typeof(TResultNode).Name,
					ProductionBinding = ConstructorBinding.Bind<TResultNode>(),
					Unwrap = true
				};

				Productions.Add(transition);
			}
		}
	}

	internal abstract partial class Grammar<TToken, TNode, TSyntaxFactory> : Grammar<TToken> where TToken : unmanaged, Enum where TNode : class where TSyntaxFactory : SyntaxFactory<TNode>
	{
		protected Grammar(TToken undefinedToken) : base(undefinedToken)
		{
		}

		protected static ParserRule<TActualNode> CreateParserRule<TActualNode>([CallerMemberName] string name = null) where TActualNode : TNode
		{
			var parserRule = new ParserRule<TActualNode>
			{
				Name = name
			};

			CreatedParserRules.Add(parserRule);

			return parserRule;
		}

		protected internal sealed partial class ParserRule<TActualNode> : ParserRule where TActualNode : TNode
		{
			internal ParserRule()
			{
			}

			public new Grammar<TToken, TNode, TSyntaxFactory> Grammar => (Grammar<TToken, TNode, TSyntaxFactory>)base.Grammar;

			public override Type NodeType => typeof(TActualNode);
		}
	}
}