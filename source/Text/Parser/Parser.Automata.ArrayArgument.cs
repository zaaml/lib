// <copyright file="Parser.Automata.ArrayArgument.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Reflection;
using System.Runtime.CompilerServices;

// ReSharper disable ForCanBeConvertedToForeach
// ReSharper disable StaticMemberInGenericType

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class ArrayArgument<TValue> : ProductionEntityArgument
			{
				public static readonly MethodInfo AddMethodInfo = typeof(ArrayArgument<TValue>).GetMethod(nameof(Add), BindingFlags.Instance | BindingFlags.Public);
				public static readonly MethodInfo ToArrayMethodInfo = typeof(ArrayArgument<TValue>).GetMethod(nameof(ToArray), BindingFlags.Instance | BindingFlags.Public);
				public static readonly MethodInfo ToArrayConvertMethodInfo = typeof(ArrayArgument<TValue>).GetMethod(nameof(ToArrayConvert), BindingFlags.Instance | BindingFlags.Public);
				public static readonly MethodInfo CopyToArrayMethodInfo = typeof(ArrayArgument<TValue>).GetMethod(nameof(CopyToArray), BindingFlags.Instance | BindingFlags.Public);
				public static readonly MethodInfo CopyToArrayConvertMethodInfo = typeof(ArrayArgument<TValue>).GetMethod(nameof(CopyToArrayConvert), BindingFlags.Instance | BindingFlags.Public);

				private static readonly TValue[] Empty = Array.Empty<TValue>();
				private int _count;
				private TValue[] _valueArray = new TValue[8];

				public ArrayArgument(ProductionEntity entity, ProductionArgument argument) : base(entity, argument)
				{
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public void Add(TValue value)
				{
					if (_count < _valueArray.Length)
						_valueArray[_count++] = value;
					else
					{
						Array.Resize(ref _valueArray, _count * 2);

						_valueArray[_count++] = value;
					}
				}

				public override object Build()
				{
					return ToArray();
				}

				public override void ConsumeValue(object value)
				{
					ConsumeValue((TValue)value);
				}

				public void ConsumeValue(TValue value)
				{
					if (_count < _valueArray.Length)
						_valueArray[_count] = value;
					else
					{
						Array.Resize(ref _valueArray, _count * 2);

						_valueArray[_count] = value;
					}

					_count++;
				}

				public TValue[] CopyToArray(TValue[] array, ref int index)
				{
					if (_count == 0)
						return array;

					Array.Copy(_valueArray, 0, array, index, _count);

					index += _count;

					Array.Clear(_valueArray, 0, _count);
					_count = 0;

					return array;
				}

				public TConvertValue[] CopyToArrayConvert<TConvertValue>(TConvertValue[] array, ref int index, Converter<TValue, TConvertValue> converter)
				{
					for (var i = 0; i < _count; i++)
						array[index++] = converter(_valueArray[i]);

					Array.Clear(_valueArray, 0, _count);
					_count = 0;

					return array;
				}

				public override int GetCount()
				{
					return _count;
				}

				public override void Reset()
				{
					Array.Clear(_valueArray, 0, _count);
					_count = 0;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public TValue[] ToArray()
				{
					if (_count == 0)
						return Empty;

					var result = new TValue[_count];

					Array.Copy(_valueArray, result, _count);
					_count = 0;

					return result;
				}

				public TConvertValue[] ToArrayConvert<TConvertValue>(Converter<TValue, TConvertValue> converter)
				{
					if (_count == 0)
						return Array.Empty<TConvertValue>();

					var result = new TConvertValue[_count];

					for (var i = 0; i < _count; i++)
						result[i] = converter(_valueArray[i]);

					Array.Clear(_valueArray, 0, _count);
					_count = 0;

					return result;
				}

				public override void TransferValue(ProductionEntityArgument argument)
				{
					var arrayArgument = (ArrayArgument<TValue>)argument;

					var array = arrayArgument._valueArray;
					var count = arrayArgument._count;

					arrayArgument._valueArray = _valueArray;
					arrayArgument._count = _count;

					_count = count;
					_valueArray = array;

					Array.Clear(_valueArray, 0, _count);
					_count = 0;
				}
			}

			private sealed class Argument<TValue> : ProductionEntityArgument
			{
				private static readonly BindingFlags BindingFlags = BindingFlags.Public | BindingFlags.Instance;
				private static readonly Type Type = typeof(Argument<TValue>);

				public static readonly FieldInfo ValueFieldInfo = Type.GetField(nameof(Value), BindingFlags);
				public static readonly FieldInfo CountFieldInfo = Type.GetField(nameof(Count), BindingFlags);
				public static readonly MethodInfo ConvertValueMethodInfo = Type.GetMethod(nameof(ConvertValue), BindingFlags);
				public static readonly MethodInfo CopyToArrayMethodInfo = Type.GetMethod(nameof(CopyToArray), BindingFlags);
				public static readonly MethodInfo CopyToArrayConvertMethodInfo = Type.GetMethod(nameof(CopyToArrayConvert), BindingFlags);
				public int Count;

				public TValue Value;


				public Argument(ProductionEntity entity, ProductionArgument argument) : base(entity, argument)
				{
					Count = 0;
				}

				public override object Build()
				{
					var result = Value;

					Count = 0;
					Value = default;

					return result;
				}

				public override void ConsumeValue(object value)
				{
					Count = 1;
					Value = (TValue)value;
				}

				public TConvertValue ConvertValue<TConvertValue>(Converter<TValue, TConvertValue> converter)
				{
					return Count == 1 ? converter(Value) : default;
				}

				public TValue[] CopyToArray(TValue[] array, ref int index)
				{
					if (Count == 0)
						return array;

					array[index++] = Value;

					Value = default;
					Count = 0;

					return array;
				}

				public TConvertValue[] CopyToArrayConvert<TConvertValue>(TConvertValue[] array, ref int index, Converter<TValue, TConvertValue> converter)
				{
					if (Count == 0)
						return array;

					array[index++] = converter(Value);

					Value = default;
					Count = 0;

					return array;
				}

				public override int GetCount()
				{
					return Count;
				}

				public override void Reset()
				{
					Count = 0;
					Value = default;
				}

				public override void TransferValue(ProductionEntityArgument argument)
				{
					var valueArgument = (Argument<TValue>)argument;

					valueArgument.Value = Value;
					valueArgument.Count = Count;

					Value = default;
					Count = 0;
				}
			}
		}
	}
}