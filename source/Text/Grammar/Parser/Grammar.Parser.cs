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
		#region Static Fields and Constants

		private static Type _grammarType;

		#endregion

		#region Methods

		protected static ParserFragment CreateParserFragment()
		{
			var parserFragment = new ParserFragment(false);

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

		protected static SubLexerEntry<TSubToken> SubLexer<TSubToken>(Grammar<TSubToken>.TokenRule subLexerRule) where TSubToken : unmanaged, Enum
		{
			return new SubLexerEntry<TSubToken>(subLexerRule);
		}

		protected static SubParserEntry<TSubToken, TSubNode, TSubNodeBase> SubParser<TSubToken, TSubNode, TSubNodeBase>(Grammar<TSubToken, TSubNodeBase>.ParserRule<TSubNode> subParserRule)
			where TSubToken : unmanaged, Enum where TSubNode : TSubNodeBase where TSubNodeBase : class
		{
			return new SubParserEntry<TSubToken, TSubNode, TSubNodeBase>(subParserRule);
		}

		protected static SubParserEntry<TSubToken> SubParser<TSubToken>(Grammar<TSubToken>.ParserRule subParserRule) where TSubToken : unmanaged, Enum
		{
			return new SubParserEntry<TSubToken>(subParserRule);
		}

		#endregion
	}
}