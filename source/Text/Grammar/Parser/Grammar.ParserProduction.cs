// <copyright file="Grammar.ParserProduction.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Reflection;

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		#region Nested Types

		public sealed class ConstructorParserProductionBinding: ParserProductionBinding
		{
			public Type NodeType { get; }

			private ConstructorParserProductionBinding(Type nodeType)
			{
				NodeType = nodeType;
			}

			public static ParserProductionBinding Bind<TNode>()
			{
				return new ConstructorParserProductionBinding(typeof(TNode));
			}
		}

		public sealed class SyntaxFactoryParserProductionBinding : ParserProductionBinding
		{
			public Type NodeType { get; }

			public Type SyntaxFactoryType { get; }

			public MethodInfo Method { get; }

			public object MethodTarget { get; }
			
			private SyntaxFactoryParserProductionBinding(Type nodeType, Type syntaxFactoryType, Delegate expression)
			{
				NodeType = nodeType;
				SyntaxFactoryType = syntaxFactoryType;

				MethodTarget = expression.Target;
				Method = expression.Method;
			}

			public static ParserProductionBinding Bind<TNode, TSyntaxFactory>(Delegate expression) where TSyntaxFactory : SyntaxFactory<TNode>
			{
				return new SyntaxFactoryParserProductionBinding(typeof(TNode), typeof(TSyntaxFactory), expression);
			}
		}

		public abstract class ParserProductionBinding
		{
		}

		protected internal sealed class ParserProduction
		{
			#region Static Fields and Constants

			public static readonly ParserProduction Empty = new ParserProduction();

			#endregion

			#region Ctors

			public ParserProduction(ParserEntry[] entries)
			{
				Entries = entries;
			}

			private ParserProduction()
			{
				Entries = new ParserEntry[0];
			}

			#endregion

			#region Properties

			public ParserEntry[] Entries { get; }

			internal string Name { get; set; }

			public ParserProductionBinding Binding { get; internal set; }

			internal bool Unwrap { get; set; }

			#endregion

			#region Methods

			public static implicit operator ParserProduction(Parser<TToken>.PredicateEntry parserPredicateEntry)
			{
				return new ParserProduction(new ParserEntry[] {new ParserPredicate(parserPredicateEntry)});
			}

			public static implicit operator ParserProduction(Parser<TToken>.ActionEntry parserActionEntry)
			{
				return new ParserProduction(new ParserEntry[] {new ParserAction(parserActionEntry)});
			}

			public static implicit operator ParserProduction(TokenRule tokenRule)
			{
				return new ParserProduction(new ParserEntry[] {tokenRule});
			}

			public static implicit operator ParserProduction(TokenInterProductionBuilder builder)
			{
				return new ParserProduction(new[] {builder.AsParserEntry()});
			}

			public static implicit operator ParserProduction(TokenInterProductionCollectionBuilder builder)
			{
				return new ParserProduction(new[] {builder.AsFragment().CreateParserEntry()});
			}

			public static implicit operator ParserProduction(TokenInterEntry entry)
			{
				return new ParserProduction(new[] {entry.CreateParserEntry()});
			}

			public override string ToString()
			{
				return Name;
			}

			#endregion
		}

		#endregion
	}
}