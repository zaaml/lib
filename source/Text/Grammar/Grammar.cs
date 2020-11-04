// <copyright file="Grammar.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Zaaml.Text
{
	internal abstract class Grammar
	{
		#region Static Fields and Constants

		private static readonly Dictionary<Type, Grammar> Grammars = new Dictionary<Type, Grammar>();

		#endregion

		#region Ctors

		protected Grammar()
		{
			Grammars[GetType()] = this;
		}

		#endregion

		#region Methods

		public static Grammar<TToken> Get<TToken>(Type grammarType) where TToken : unmanaged, Enum
		{
			RuntimeHelpers.RunClassConstructor(grammarType.TypeHandle);

			if (Grammars.TryGetValue(grammarType, out var grammarBase))
			{
				var grammar = (Grammar<TToken>) grammarBase;

				grammar.Build();

				return grammar;
			}

			{
				var grammar = (Grammar<TToken>) Activator.CreateInstance(grammarType, true);

				grammar.Build();

				Grammars[grammarType] = grammar;

				return grammar;
			}
		}

		public static TGrammar Get<TGrammar, TToken>() where TGrammar : Grammar<TToken> where TToken : unmanaged, Enum
		{
			return (TGrammar) Get<TToken>(typeof(TGrammar));
		}

		#endregion
	}

	internal abstract partial class Grammar<TToken> : Grammar where TToken : unmanaged, Enum
	{
		#region Static Fields and Constants

		// ReSharper disable once StaticMemberInGenericType
		private static readonly int EnumTokenCodeOffset;

		#endregion

		#region Fields

		private readonly Dictionary<string, ParserRule> _parserRuleDictionary = new Dictionary<string, ParserRule>();
		private readonly Dictionary<string, TokenFragment> _tokenFragmentDictionary = new Dictionary<string, TokenFragment>();
		private readonly Dictionary<string, TokenRule> _tokenRuleDictionary = new Dictionary<string, TokenRule>();
		private bool _isBuilt;
		private int _tokenCounter;

		#endregion

		#region Ctors

		static Grammar()
		{
			foreach (int value in Enum.GetValues(typeof(TToken)))
				EnumTokenCodeOffset = Math.Max(EnumTokenCodeOffset, value);

			EnumTokenCodeOffset++;
		}

		// ReSharper disable once EmptyConstructor
		internal Grammar()
		{
		}

		#endregion

		#region Properties

		protected EmptyEntry Empty => EmptyEntry.Instance;

		internal IEnumerable<ParserRule> ParserRules => _parserRuleDictionary.Values;

		internal IEnumerable<TokenFragment> TokenFragments => _tokenFragmentDictionary.Values;

		internal IEnumerable<TokenRule> TokenRules => _tokenRuleDictionary.Values;

		#endregion

		#region Methods

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
				return (int) (object) tokenRule.Token;

			return EnumTokenCodeOffset + _tokenCounter++;
		}

		#endregion

		#region Nested Types

		protected sealed class EmptyEntry
		{
			#region Static Fields and Constants

			internal static readonly EmptyEntry Instance = new EmptyEntry();

			#endregion

			#region Ctors

			private EmptyEntry()
			{
			}

			#endregion

			#region Methods

			public static implicit operator ParserProduction(EmptyEntry entry)
			{
				return ParserProduction.Empty;
			}

			#endregion
		}

		#endregion
	}

	internal abstract class Grammar<TToken, TNode> : Grammar<TToken> where TToken : unmanaged, Enum where TNode : class
	{
		#region Methods

		protected static ParserRule<TActualNode> CreateParserRule<TActualNode>([CallerMemberName] string name = null) where TActualNode : TNode
		{
			var parserRule = new ParserRule<TActualNode>
			{
				Name = name
			};

			CreatedParserRules.Add(parserRule);

			return parserRule;
		}

		#endregion

		#region Nested Types

		protected internal sealed class ParserRule<TActualNode> : ParserRule where TActualNode : TNode
		{
			#region Ctors

			internal ParserRule()
			{
			}

			#endregion

			#region Properties

			public new Grammar<TToken, TNode> Grammar => (Grammar<TToken, TNode>) base.Grammar;

			#endregion

			public void Bind<TResultNode>(ParserProduction parserProduction) where TResultNode : TActualNode
			{
				parserProduction.Name = typeof(TResultNode).Name;
				parserProduction.Binding = ConstructorParserProductionBinding.Bind<TResultNode>();

				Productions.Add(parserProduction);
			}

			public void BindReturn<TResultNode, TBaseNode>(Grammar<TToken, TBaseNode>.ParserRule<TResultNode> rule) where TResultNode : TBaseNode where TBaseNode : class
			{
				var transition = new ParserProduction(new ParserEntry[] { rule })
				{
					Name = typeof(TResultNode).Name,
					Binding = ConstructorParserProductionBinding.Bind<TResultNode>(),
					Unwrap = true
				};

				Productions.Add(transition);
			}
		}

		#endregion
	}

	internal abstract partial class Grammar<TToken, TNode, TSyntaxFactory> : Grammar<TToken> where TToken : unmanaged, Enum where TNode : class where TSyntaxFactory : SyntaxFactory<TNode>
	{
		protected static ParserRule<TActualNode> CreateParserRule<TActualNode>([CallerMemberName] string name = null) where TActualNode : TNode
		{
			var parserRule = new ParserRule<TActualNode>
			{
				Name = name
			};

			CreatedParserRules.Add(parserRule);

			return parserRule;
		}

		protected internal sealed partial  class ParserRule<TActualNode> : ParserRule where TActualNode : TNode
		{
			#region Ctors

			internal ParserRule()
			{
			}

			#endregion

			#region Properties

			public new Grammar<TToken, TNode, TSyntaxFactory> Grammar => (Grammar<TToken, TNode, TSyntaxFactory>)base.Grammar;

			#endregion
		}
	}
}