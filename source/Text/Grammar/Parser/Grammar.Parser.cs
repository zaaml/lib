// <copyright file="Grammar.Parser.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Zaaml.Text
{
	internal partial class Grammar<TToken> where TToken : unmanaged, Enum
	{
		private static Type _grammarType;

		protected static ParserFragment CreateParserFragment([CallerMemberName] string name = null)
		{
			var parserFragment = new ParserFragment(false)
			{
				Name = name
			};

			CreatedParserFragments.Add(parserFragment);

			return parserFragment;
		}

		protected static ParserRule CreateParserRule([CallerMemberName] string name = null)
		{
			var parserRule = new ParserRule
			{
				Name = name
			};

			CreatedParserRules.Add(parserRule);

			return parserRule;
		}

		private static Type GetGrammarType()
		{
			if (_grammarType != null)
				return _grammarType;

			var stackTrace = new StackTrace();

			for (var i = 0; i < stackTrace.FrameCount; i++)
			{
				var frame = stackTrace.GetFrame(i);
				var methodBase = frame.GetMethod();
				var ci = methodBase as ConstructorInfo;

				if (ci == null)
					continue;

				var declaringType = ci.DeclaringType;

				if (declaringType == null)
					continue;

				if (!typeof(Grammar<TToken>).IsAssignableFrom(declaringType) || declaringType.TypeInitializer != ci)
					continue;

				_grammarType = declaringType;

				break;
			}

			return _grammarType;
		}

		protected static ExternalLexerEntry<TExternalToken> ExternalLexer<TExternalToken>(Grammar<TExternalToken>.TokenRule externalLexerRule) where TExternalToken : unmanaged, Enum
		{
			return new ExternalLexerEntry<TExternalToken>(externalLexerRule);
		}

		protected static ExternalParserEntry<TExternalToken, TExternalNode, TExternalNodeBase> ExternalParser<TExternalToken, TExternalNode, TExternalNodeBase>(Grammar<TExternalToken, TExternalNodeBase>.ParserRule<TExternalNode> externalParserRule)
			where TExternalToken : unmanaged, Enum where TExternalNode : TExternalNodeBase where TExternalNodeBase : class
		{
			return new ExternalParserEntry<TExternalToken, TExternalNode, TExternalNodeBase>(externalParserRule);
		}

		protected static ExternalParserEntry<TExternalToken> ExternalParser<TExternalToken>(Grammar<TExternalToken>.ParserRule externalParserRule) where TExternalToken : unmanaged, Enum
		{
			return new ExternalParserEntry<TExternalToken>(externalParserRule);
		}
	}
}