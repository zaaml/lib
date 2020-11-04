// <copyright file="Parser.Automata.ProductionBuilder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

#pragma warning disable 414

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private abstract class Argument
			{
				public ProductionInstanceBuilder InstanceBuilder { get; }

				public int Index { get; }

				public ArgumentBuilder ArgumentBuilder { get; }

				protected Argument(ProductionInstanceBuilder instanceBuilder, int index)
				{
					InstanceBuilder = instanceBuilder;
					Index = index;
					ArgumentBuilder = InstanceBuilder.ArgumentBuilders[index];
				}

				public abstract object Build();

				public abstract void Reset();

				public virtual void ConsumeParserValue(object value)
				{
				}

				public void ConsumeLexerEntry(ValueParserAutomataContext valueParserAutomataContext)
				{
				}
			}

			private abstract class ArgumentBuilder
			{
				public abstract Argument CreateArgument(ProductionInstanceBuilder instanceBuilder, int index);

				public abstract void EmitPushResetArgument(LocalBuilder argumentLocal, ILGenerator il);
			}

			private class ValueListArgument<TResult> : Argument
			{
				private static readonly TResult[] Empty = Array.Empty<TResult>();
				private int _count;
				private TResult[] _result = new TResult[8];

				public ValueListArgument(ProductionInstanceBuilder instanceBuilder, int index) : base(instanceBuilder, index)
				{
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void Add(TResult value)
				{
					if (_count < _result.Length)
						_result[_count++] = value;
					else
					{
						Array.Resize(ref _result, _count * 2);

						_result[_count++] = value;
					}
				}

				public override object Build()
				{
					return ToArray();
				}

				public override void Reset()
				{
					Array.Clear(_result, 0, _count);

					_count = 0;
				}

				public override void ConsumeParserValue(object value)
				{
					ConsumeValue((TResult)value);
				}

				protected void ConsumeValue(TResult value)
				{
					if (_count < _result.Length)
						_result[_count] = value;
					else
					{
						Array.Resize(ref _result, _count * 2);

						_result[_count] = value;
					}

					_count++;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public TResult[] ToArray()
				{
					if (_count == 0)
						return Empty;

					var result = new TResult[_count];

					Array.Copy(_result, result, _count);

					_count = 0;

					return result;
				}
			}

			private abstract class ParserArgumentBuilder : ArgumentBuilder
			{
				public abstract void EmitConsumeValue(ILBuilderContext ilBuilderContext);
			}

			private sealed class ValueArgumentBuilder<TResult> : ParserArgumentBuilder
			{
				private static readonly FieldInfo ValueArgumentResultFieldInfo = typeof(ValueArgument).GetField(nameof(ValueArgument.Result), BindingFlags.Public | BindingFlags.Instance);

				public override Argument CreateArgument(ProductionInstanceBuilder instanceBuilder, int index)
				{
					return new ValueArgument(instanceBuilder, index);
				}

				public override void EmitConsumeValue(ILBuilderContext ilBuilderContext)
				{
					ilBuilderContext.IL.Emit(OpCodes.Stfld, ValueArgumentResultFieldInfo);
				}

				public override void EmitPushResetArgument(LocalBuilder argumentLocal, ILGenerator il)
				{
					il.Emit(OpCodes.Ldloc, argumentLocal);
					il.Emit(OpCodes.Ldfld, ValueArgumentResultFieldInfo);
					il.Emit(OpCodes.Ldloc, argumentLocal);
					il.Emit(OpCodes.Ldnull);
					il.Emit(OpCodes.Stfld, ValueArgumentResultFieldInfo);
				}

				private sealed class ValueArgument : Argument
				{
					public TResult Result;

					public ValueArgument(ProductionInstanceBuilder instanceBuilder, int index) : base(instanceBuilder, index)
					{
					}

					public override object Build()
					{
						var result = Result;

						Result = default;

						return result;
					}

					public override void Reset()
					{
						Result = default;
					}

					public override void ConsumeParserValue(object value)
					{
						Result = (TResult) value;
					}
				}
			}

			private sealed class ValueListArgumentBuilder<TResult> : ParserArgumentBuilder
			{
				private static readonly MethodInfo ListTResultAddMethodInfo = typeof(ValueListArgument<TResult>).GetMethod(nameof(ValueListArgument<TResult>.Add), BindingFlags.Instance | BindingFlags.Public);
				private static readonly MethodInfo ListTResultToArrayMethodInfo = typeof(ValueListArgument<TResult>).GetMethod(nameof(ValueListArgument<TResult>.ToArray), BindingFlags.Instance | BindingFlags.Public);

				public override Argument CreateArgument(ProductionInstanceBuilder instanceBuilder, int index)
				{
					return new ValueListArgument<TResult>(instanceBuilder, index);
				}

				public override void EmitConsumeValue(ILBuilderContext ilBuilderContext)
				{
					ilBuilderContext.IL.Emit(OpCodes.Call, ListTResultAddMethodInfo);
				}

				public override void EmitPushResetArgument(LocalBuilder argumentLocal, ILGenerator il)
				{
					il.Emit(OpCodes.Ldloc, argumentLocal);
					il.Emit(OpCodes.Call, ListTResultToArrayMethodInfo);
				}
			}
		}
	}
}