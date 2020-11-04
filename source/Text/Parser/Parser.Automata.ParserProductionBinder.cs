// <copyright file="Parser.Automata.ParserProductionBinder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class ProductionInstanceBuilderPool
			{
				private readonly ParserProduction _production;
				private ProductionInstanceBuilder[] _poolArray;
				private int _tail;

				public ProductionInstanceBuilderPool(ParserProduction production)
				{
					_production = production;
					_poolArray = new ProductionInstanceBuilder[16];

					for (var i = 0; i < _poolArray.Length; i++)
						_poolArray[i] = new ProductionInstanceBuilder(_production, this);

					_tail = _poolArray.Length - 1;
				}


				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public ProductionInstanceBuilder Rent()
				{
					if (_tail == -1)
						Resize();

					var productionInstanceBuilder = _poolArray[_tail--];

					productionInstanceBuilder.Busy = true;

					return productionInstanceBuilder;
				}

				private void Resize()
				{
					var len = _poolArray.Length;

					Array.Resize(ref _poolArray, len * 2);

					for (var i = 0; i < len; i++)
						_poolArray[i] = new ProductionInstanceBuilder(_production, this);

					_tail = len - 1;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void Return(ProductionInstanceBuilder productionInstanceBuilder)
				{
					productionInstanceBuilder.Busy = false;

					_poolArray[++_tail] = productionInstanceBuilder;
				}
			}

			private sealed class ProductionInstanceBuilder
			{
				private readonly object _constValue;
				private readonly Func<ProductionInstanceBuilder, SyntaxFactory, object> _createInstanceDelegate;
				private readonly ProductionInstanceBuilderPool _pool;
				private readonly bool _tryReturn;
				public readonly ArgumentBuilder[] ArgumentBuilders;
				public readonly Argument[] Arguments;
				public readonly Argument LeftRecursionArgument;
				private int _consumeCount;
				public bool Busy;

				public ProductionInstanceBuilder(ParserProduction parserProduction, ProductionInstanceBuilderPool pool)
				{
					var binder = parserProduction.Binder;

					_pool = pool;
					_tryReturn = binder.TryReturn;

					if (binder.ConstValue != null)
					{
						Arguments = Array.Empty<Argument>();

						_constValue = binder.ConstValue;
						_createInstanceDelegate = (builder, factory) => builder._constValue;
					}
					else
					{
						var template = binder.Template;

						Arguments = new Argument[template.Length];
						ArgumentBuilders = binder.Template;

						for (var index = 0; index < template.Length; index++)
						{
							var argumentBuilder = template[index];

							if (argumentBuilder == null)
								continue;

							Arguments[index] = argumentBuilder.CreateArgument(this, index);
						}

						var flatIndex = parserProduction.LeftRecursionEntry?.ParserEntryData.FlatIndex ?? -1;

						LeftRecursionArgument = flatIndex != -1 ? Arguments[flatIndex] : null;

						_createInstanceDelegate = binder.CreateInstanceDelegate;
					}
				}

				public void ConsumeLexerEntry(int entryIndex)
				{
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public object CreateInstance(SyntaxFactory syntaxFactory)
				{
					if (_tryReturn && _consumeCount == 1)
					{
						var build = Arguments[0].Build();

						_consumeCount = 0;
						_pool.Return(this);

						return build;
					}

					var instance = _createInstanceDelegate(this, syntaxFactory);

					_consumeCount = 0;
					_pool.Return(this);

					return instance;
				}

				public void OnAfterConsumeValue(int entryIndex)
				{
					_consumeCount++;
				}

				public void Reset()
				{
					_consumeCount = 0;

					foreach (var argument in Arguments) 
						argument?.Reset();

					if (Busy)
						_pool.Return(this);
				}
			}

			private abstract class ParserProductionBinder
			{
				private static readonly FieldInfo ProductionInstanceBuilderArgumentsFieldInfo = typeof(ProductionInstanceBuilder).GetField(nameof(ProductionInstanceBuilder.Arguments), BindingFlags.Instance | BindingFlags.Public);
				private static readonly FieldInfo FactoryTargetFieldInfo = typeof(ParserProductionBinder).GetField(nameof(FactoryTarget), BindingFlags.Instance | BindingFlags.NonPublic);

				private Func<ProductionInstanceBuilder, SyntaxFactory, object> _createInstanceDelegate;

				protected object FactoryTarget;

				protected int[] Arguments { get; set; }

				protected ConstructorInfo ConstructorInfo { get; set; }

				public object ConstValue { get; protected set; }

				public Func<ProductionInstanceBuilder, SyntaxFactory, object> CreateInstanceDelegate => _createInstanceDelegate ??= BuildCreateInstanceDelegate();

				protected MethodInfo FactoryInfo { get; set; }

				public abstract bool IsConstValue { get; }

				protected bool Return { get; set; }

				public ArgumentBuilder[] Template { get; protected set; }

				public bool TryReturn { get; protected set; }

				private Func<ProductionInstanceBuilder, SyntaxFactory, object> BuildCreateInstanceDelegate()
				{
					var dynMethod = new DynamicMethod("CreateInstance", typeof(object), new[] {typeof(ParserProductionBinder), typeof(ProductionInstanceBuilder), typeof(SyntaxFactory)}, GetType(), true);
					var ilBuilder = dynMethod.GetILGenerator();
					var argumentsBuilderLocal = ilBuilder.DeclareLocal(typeof(Argument[]));
					var argumentLocal = ilBuilder.DeclareLocal(typeof(Argument));

					ilBuilder.Emit(OpCodes.Ldarg_1);
					ilBuilder.Emit(OpCodes.Ldfld, ProductionInstanceBuilderArgumentsFieldInfo);
					ilBuilder.Emit(OpCodes.Stloc, argumentsBuilderLocal);

					if (FactoryInfo != null)
					{
						if (FactoryTarget != null)
						{
							ilBuilder.Emit(OpCodes.Ldarg_0);
							ilBuilder.Emit(OpCodes.Ldfld, FactoryTargetFieldInfo);
						}

						ilBuilder.Emit(OpCodes.Ldarg_2);
					}

					foreach (var argument in Arguments)
					{
						// Push entries[argument]
						ilBuilder.Emit(OpCodes.Ldloc, argumentsBuilderLocal);
						ilBuilder.Emit(OpCodes.Ldc_I4, argument);
						ilBuilder.Emit(OpCodes.Ldelem_Ref);
						ilBuilder.Emit(OpCodes.Stloc, argumentLocal);

						var argumentBuilder = Template[argument];

						argumentBuilder.EmitPushResetArgument(argumentLocal, ilBuilder);
					}

					if (Return)
					{
					}
					else
					{
						if (ConstructorInfo != null)
							ilBuilder.Emit(OpCodes.Newobj, ConstructorInfo);
						else if (FactoryInfo != null)
							ilBuilder.Emit(OpCodes.Call, FactoryInfo);
						else
							throw new InvalidOperationException("Unknown binder");
					}

					ilBuilder.Emit(OpCodes.Ret);

					var delType = Expression.GetDelegateType(typeof(ProductionInstanceBuilder), typeof(SyntaxFactory), typeof(object));

					return (Func<ProductionInstanceBuilder, SyntaxFactory, object>) dynMethod.CreateDelegate(delType, this);
				}

				protected static ArgumentBuilder CreateArgumentBuilder(Type type, ParameterInfo parameterInfo, ParserEntryData flatEntryData)
				{
					var parserEntry = flatEntryData.ParserEntry;

					return parserEntry switch
					{
						IParserPredicate parserPredicateEntry => CreatePredicateArgumentBuilder(type, parserPredicateEntry),
						ParserSetMatchEntry parserSetMatchEntry => CreateLexerArgumentBuilder(type, null, parserSetMatchEntry),
						ParserSingleMatchEntry parserSingleMatchEntry => CreateLexerArgumentBuilder(type, null, parserSingleMatchEntry),
						ParserStateEntry parserStateEntry => CreateParserArgumentBuilder(type),

						_ => throw new ArgumentOutOfRangeException(nameof(parserEntry))
					};
				}

				private static ArgumentBuilder CreateLexerArgumentBuilder(Type type, Type converterType, MatchEntry matchEntry)
				{
					return new LocalLexerArgumentBuilder(type, matchEntry);
				}

				private static ArgumentBuilder CreateParserArgumentBuilder(Type type)
				{
					if (type.IsArray)
					{
						var valueListArgumentBuilderGenericType = typeof(ValueListArgumentBuilder<>);
						var valueListArgumentBuilderType = valueListArgumentBuilderGenericType.MakeGenericType(typeof(TGrammar), typeof(TToken), type.GetElementType());

						return (ArgumentBuilder) Activator.CreateInstance(valueListArgumentBuilderType);
					}

					var valueArgumentBuilderGenericType = typeof(ValueArgumentBuilder<>);
					var valueArgumentBuilderType = valueArgumentBuilderGenericType.MakeGenericType(typeof(TGrammar), typeof(TToken), type);

					return (ArgumentBuilder) Activator.CreateInstance(valueArgumentBuilderType);
				}

				private static ArgumentBuilder CreatePredicateArgumentBuilder(Type type, IParserPredicate parserPredicateEntry)
				{
					if (parserPredicateEntry.PredicateKind == ParserPredicateKind.SubParser)
						return CreateParserArgumentBuilder(type);

					if (parserPredicateEntry.PredicateKind == ParserPredicateKind.SubLexer)
					{
						var tokenType = parserPredicateEntry.GetType().GetGenericArguments().Last().GetGenericArguments().Single();

						return CreateSubLexerArgumentBuilder(tokenType, type);
					}

					throw new ArgumentOutOfRangeException();
				}

				private static ArgumentBuilder CreateSubLexerArgumentBuilder(Type subTokenType, Type type)
				{
					var argumentBuilderType = typeof(ExternalLexemeArgumentBuilder<>).MakeGenericType(typeof(TGrammar), typeof(TToken), subTokenType);
					var argumentBuilderCtor = argumentBuilderType.GetConstructors().Single();
					var argumentBuilder = (ArgumentBuilder) argumentBuilderCtor.Invoke(new object[] {type});

					return argumentBuilder;
				}
			}
		}
	}
}