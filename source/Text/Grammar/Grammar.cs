// <copyright file="Grammar.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Zaaml.Core.Reflection;

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
		private Func<IServiceProvider, Lexer<TGrammar, TToken>> _lexerFactoryDelegate;
		private Func<IServiceProvider, Parser<TGrammar, TToken>> _parserFactoryDelegate;

		// ReSharper disable once EmptyConstructor
		internal Grammar(TToken undefinedToken, LexerGrammar lexerGrammar, ParserGrammar parserGrammar)
		{
			UndefinedToken = undefinedToken;
			LexerGrammarInstance = lexerGrammar;
			ParserGrammarInstance = parserGrammar;
		}

		public LexerGrammar LexerGrammarInstance { get; }

		public ParserGrammar ParserGrammarInstance { get; }

		public TToken UndefinedToken { get; }

		private Func<IServiceProvider, Lexer<TGrammar, TToken>> CreateLexerFactoryDelegate()
		{
			var grammarType = GetType();
			var lexerFactoryMethodInfo = grammarType.GetMethod("LexerFactory", BF.SPNP);

			if (lexerFactoryMethodInfo == null)
				throw new InvalidOperationException("Can not find LexerFactory");

#if NET5_0_OR_GREATER
			return lexerFactoryMethodInfo.CreateDelegate<Func<IServiceProvider, Lexer<TGrammar, TToken>>>();
#else
			return (Func<IServiceProvider, Lexer<TGrammar, TToken>>)lexerFactoryMethodInfo.CreateDelegate(typeof(Func<IServiceProvider, Lexer<TGrammar, TToken>>));
#endif
		}

		private Func<IServiceProvider, Parser<TGrammar, TToken>> CreateParserFactoryDelegate()
		{
			var grammarType = GetType();
			var parserFactoryMethodInfo = grammarType.GetMethod("ParserFactory", BF.SPNP);

			if (parserFactoryMethodInfo == null)
				throw new InvalidOperationException("Can not find ParserFactory");

#if NET5_0_OR_GREATER
			return parserFactoryMethodInfo.CreateDelegate<Func<IServiceProvider, Parser<TGrammar, TToken>>>();

#else
			return (Func<IServiceProvider, Parser<TGrammar, TToken>>)parserFactoryMethodInfo.CreateDelegate(typeof(Func<IServiceProvider, Parser<TGrammar, TToken>>));			
#endif
		}

		internal Func<IServiceProvider, Lexer<TGrammar, TToken>> GetLexerFactory()
		{
			return _lexerFactoryDelegate ??= CreateLexerFactoryDelegate();
		}

		internal Func<IServiceProvider, Parser<TGrammar, TToken>> GetParserFactory()
		{
			return _parserFactoryDelegate ??= CreateParserFactoryDelegate();
		}
	}


	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class ExternalParserAttribute : Attribute
	{
		public ExternalParserAttribute(Type parserType)
		{
			ParserType = parserType;
		}

		public Type ParserType { get; }
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class ExternalLexerAttribute : Attribute
	{
		public ExternalLexerAttribute(Type lexerType)
		{
			LexerType = lexerType;
		}

		public Type LexerType { get; }
	}
}