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

		public static Grammar<TGrammar, TToken> Get<TGrammar, TToken>()
			where TGrammar : Grammar<TGrammar, TToken>
			where TToken : unmanaged, Enum
		{
			var grammarType = typeof(TGrammar);

			RuntimeHelpers.RunClassConstructor(grammarType.TypeHandle);

			if (Grammars.TryGetValue(grammarType, out var grammarBase))
				return (TGrammar)grammarBase;

			{
				var grammar = (Grammar<TGrammar, TToken>)Activator.CreateInstance(grammarType, true);

				Grammars[grammarType] = grammar;

				return grammar;
			}
		}
	}

	internal abstract class Grammar<TGrammar> : Grammar
		where TGrammar : Grammar
	{
	}

	internal abstract partial class Grammar<TGrammar, TToken> : Grammar<TGrammar>
		where TGrammar : Grammar<TGrammar, TToken>
		where TToken : unmanaged, Enum
	{
		// ReSharper disable once EmptyConstructor
		internal Grammar(TToken undefinedToken, LexerGrammar lexer, ParserGrammar parser)
		{
			UndefinedToken = undefinedToken;
		}

		public TToken UndefinedToken { get; }
	}


	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class ExternalParserAttribute : Attribute
	{
		public Type ParserType { get; }

		public ExternalParserAttribute(Type parserType)
		{
			ParserType = parserType;
		}
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class ExternalLexerAttribute : Attribute
	{
		public Type LexerType { get; }

		public ExternalLexerAttribute(Type lexerType)
		{
			LexerType = lexerType;
		}
	}
}