// <copyright file="Grammar.Parser.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{


		public partial class ParserGrammar
		{
			private static readonly Dictionary<string, NodeSyntax> NodeDictionary = [];
			private static readonly Dictionary<string, FragmentSyntax> FragmentDictionary = [];
			private static readonly Dictionary<int, Production> ProductionDictionary = [];

			internal IEnumerable<NodeSyntax> NodeSyntaxCollection => NodeDictionary.Values;

			internal IEnumerable<FragmentSyntax> SyntaxFragmentCollection => FragmentDictionary.Values;

			public void Seal()
			{
			}

			protected static ExternalTokenSymbol<TExternalGrammar, TExternalToken> ExternalLexer<TExternalGrammar, TExternalToken>(Grammar<TExternalGrammar, TExternalToken>.LexerGrammar.TokenSyntax externalToken)
				where TExternalGrammar : Grammar<TExternalGrammar, TExternalToken>
				where TExternalToken : unmanaged, Enum
			{
				return new ExternalTokenSymbol<TExternalGrammar, TExternalToken>(externalToken);
			}

			private protected static void RegisterFragmentSyntax(FragmentSyntax fragment)
			{
				FragmentDictionary.Add(fragment.Name, fragment);
			}

			private protected static void RegisterNodeSyntax(NodeSyntax node)
			{
				NodeDictionary.Add(node.Name, node);
			}

			private protected static int RegisterParserSyntaxProduction(Production parserSyntaxProduction)
			{
				var index = ProductionDictionary.Count;

				ProductionDictionary.Add(index, parserSyntaxProduction);

				return index;
			}
		}
	}
}