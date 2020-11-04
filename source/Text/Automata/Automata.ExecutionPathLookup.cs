// <copyright file="Automata.ExecutionPathLookup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Zaaml.Core;
using Zaaml.Core.Collections;
using Zaaml.Core.Extensions;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		private class ExecutionPathLookup
		{
			#region Static Fields and Constants

			private const int FastLookupLimit = 1024;
			public static readonly ExecutionPathLookup Empty = new ExecutionPathLookup(Enumerable.Empty<ExecutionPath>());
			private static readonly ExecutionPathGroup[] EmptyFastLookup;

			#endregion

			#region Fields

			private readonly ExecutionPathGroup[] _fastLookup;
			private readonly ILookup _lookup;

			#endregion

			#region Ctors

			static ExecutionPathLookup()
			{
				if (InstructionsRange.Maximum >= FastLookupLimit)
					return;

				EmptyFastLookup = CreateFastLookupArray();
			}

			public ExecutionPathLookup(IEnumerable<ExecutionPath> executionPaths)
			{
				var executionPathGroups = GroupExecutionPaths(executionPaths, out var predicateGroup).ToList();
				var emptyGroup = predicateGroup ?? ExecutionPathGroup.Empty;

				if (executionPathGroups.Count == 0)
					_lookup = new AnyLookup(emptyGroup);
				else if (executionPathGroups.Count == 1)
				{
					var match = executionPathGroups[0].Match;

					if (match is RangeMatchEntry rangeMatch)
						_lookup = new SingleOperandRangeLookup(rangeMatch.MinOperand, rangeMatch.MaxOperand, executionPathGroups[0], emptyGroup);
					else
						_lookup = new SingleOperandLookup(((SingleMatchEntry) match).Operand, executionPathGroups[0], emptyGroup);
				}
				else
				{
					var hasRanges = executionPathGroups.Any(g => g.Match is RangeMatchEntry);

					_lookup = hasRanges ? new RangeTreeLookup(executionPathGroups, emptyGroup) : (ILookup) new DictionaryLookup(executionPathGroups, emptyGroup);
				}

				if (predicateGroup != null)
					_lookup = new PredicateLookup(_lookup, predicateGroup);

				if (InstructionsRange.Maximum >= FastLookupLimit)
					return;

				//_lookup = new FastLookup(_lookup);

				for (var i = 0; i < InstructionsRange.Maximum + 1; i++)
				{
					var executionPathGroup = _lookup.GetExecutionPathGroup(i);

					if (ReferenceEquals(executionPathGroup, ExecutionPathGroup.Empty))
						continue;

					_fastLookup ??= CreateFastLookupArray();
					_fastLookup[i] = executionPathGroup;
				}

				_fastLookup ??= EmptyFastLookup;
			}

			#endregion

			#region Methods

			private static ExecutionPathGroup[] CreateFastLookupArray()
			{
				var fastLookup = new ExecutionPathGroup[InstructionsRange.Maximum + 1];

				for (var i = 0; i < fastLookup.Length; i++)
					fastLookup[i] = ExecutionPathGroup.Empty;

				return fastLookup;
			}

			private static Range<int> CreateRange(TOperand minimum, TOperand maximum)
			{
				return new Range<int>(ConvertOperand(minimum), ConvertOperand(maximum));
			}

			private static Range<int> CreateRange(PrimitiveMatchEntry operandMatch)
			{
				return operandMatch switch
				{
					RangeMatchEntry rangeMatch => rangeMatch.IntRange,
					SingleMatchEntry singleMatch => CreateRange(singleMatch.Operand, singleMatch.Operand),
					_ => throw new InvalidOperationException()
				};
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ExecutionPathGroup GetExecutionPathGroup(int operand)
			{
				return _lookup.GetExecutionPathGroup(operand);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ExecutionPathGroup GetExecutionPathGroup()
			{
				if (_lookup is PredicateLookup predicateLookup)
					return predicateLookup.GetExecutionPathGroup();

				return ExecutionPathGroup.Empty;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ExecutionPathGroup GetExecutionPathGroupFast(int operand)
			{
				return _fastLookup != null ? _fastLookup[operand] : _lookup.GetExecutionPathGroup(operand);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ExecutionPathGroup GetExecutionPathGroupFast()
			{
				if (_lookup is PredicateLookup predicateLookup)
					return predicateLookup.GetExecutionPathGroup();

				return ExecutionPathGroup.Empty;
			}

			private static IEnumerable<ExecutionPathGroup> Group(IEnumerable<GroupExecutionPathData> matchPaths, List<ExecutionPath> predicatePaths)
			{
				return matchPaths.GroupBy(p => ((SingleMatchEntry) p.Match).Operand).Select(g => new ExecutionPathGroup(g.Key, g.Select(d => d.Path).Concat(predicatePaths)));
			}

			private static IEnumerable<ExecutionPathGroup> GroupExecutionPaths(IEnumerable<ExecutionPath> executionPaths, out ExecutionPathGroup predicateGroup)
			{
				var paths = new List<GroupExecutionPathData>();
				var predicatePaths = new List<ExecutionPath>();
				var hasRanges = false;

				foreach (var executionPath in executionPaths)
				{
					if (executionPath.IsPredicate)
					{
						predicatePaths.Add(executionPath);

						continue;
					}

					var match = executionPath.Match;

					if (match is SetMatchEntry setMatch)
					{
						foreach (var primitiveOperandMatch in setMatch.Matches)
						{
							paths.Add(new GroupExecutionPathData(primitiveOperandMatch, executionPath)); //new ExecutionPath(executionPath.PathSourceNode, executionPath.Nodes, primitiveOperandMatch) {Index = executionPath.Index});
							hasRanges |= primitiveOperandMatch is RangeMatchEntry;
						}
					}
					else
					{
						var primitiveOperandMatch = (PrimitiveMatchEntry) match;

						paths.Add(new GroupExecutionPathData(primitiveOperandMatch, executionPath));
						hasRanges |= primitiveOperandMatch is RangeMatchEntry;
					}
				}

				predicateGroup = predicatePaths.Count > 0 ? new ExecutionPathGroup(predicatePaths) : null;

				return hasRanges ? GroupWithRanges(paths, predicatePaths) : Group(paths, predicatePaths);
			}

			private static IEnumerable<ExecutionPathGroup> GroupWithRanges(IEnumerable<GroupExecutionPathData> matchPaths, List<ExecutionPath> predicatePaths)
			{
				var rangeItems = new List<RangeItem<ExecutionPath, int>>();

				foreach (var executionPath in matchPaths)
				{
					switch (executionPath.Match)
					{
						case SingleMatchEntry singleMatch:
							rangeItems.Add(new RangeItem<ExecutionPath, int>(executionPath.Path, CreateRange(singleMatch.Operand, singleMatch.Operand)));

							break;
						case RangeMatchEntry rangeMatch:
							rangeItems.Add(new RangeItem<ExecutionPath, int>(executionPath.Path, rangeMatch.IntRange));

							break;
					}
				}

				var split = Core.Range.Split(rangeItems.Distinct());
				var dictionary = new Dictionary<Range<int>, List<ExecutionPath>>();

				foreach (var rangeItem in split)
					dictionary.GetValueOrCreate(rangeItem.Range, () => new List<ExecutionPath>()).Add(rangeItem.Item);

				return dictionary.Select(kv => new ExecutionPathGroup(new RangeMatchEntry(kv.Key), kv.Value.Concat(predicatePaths)));
			}

			#endregion

			#region Nested Types

			private struct GroupExecutionPathData
			{
				public GroupExecutionPathData(PrimitiveMatchEntry match, ExecutionPath path)
				{
					Match = match;
					Path = path;
				}

				public readonly PrimitiveMatchEntry Match;
				public readonly ExecutionPath Path;
			}

			private interface ILookup
			{
				#region Methods

				ExecutionPathGroup GetExecutionPathGroup(int operand);

				#endregion
			}

			//private sealed class FastLookup : ILookup
			//{
			//	#region Fields

			//	private readonly ExecutionPathGroup[] _fastLookup;

			//	#endregion

			//	#region Ctors

			//	public FastLookup(ILookup lookup)
			//	{
			//		for (var i = 0; i < InstructionsRange.Maximum + 1; i++)
			//		{
			//			var executionPathGroup = lookup.GetExecutionPathGroup(i);

			//			if (ReferenceEquals(executionPathGroup, ExecutionPathGroup.Empty))
			//				continue;

			//			_fastLookup ??= new ExecutionPathGroup[InstructionsRange.Maximum + 1];
			//			_fastLookup[i] = executionPathGroup;
			//		}

			//		_fastLookup ??= EmptyFastLookup;
			//	}

			//	#endregion

			//	#region Interface Implementations

			//	#region Automata<TInstruction,TOperand>.ExecutionPathLookup.ILookup

			//	[MethodImpl(MethodImplOptions.AggressiveInlining)]
			//	public ExecutionPathGroup GetExecutionPathGroup(int operand)
			//	{
			//		return _fastLookup[operand];
			//	}

			//	#endregion

			//	#endregion
			//}

			private sealed class DictionaryLookup : ILookup
			{
				#region Fields

				private readonly HybridDictionaryCacheEx<ExecutionPathGroup> _dictionary = new HybridDictionaryCacheEx<ExecutionPathGroup>();
				private readonly ExecutionPathGroup _emptyGroup;
				private readonly int _maxValue;
				private readonly int _minValue;

				#endregion

				#region Ctors

				public DictionaryLookup(IEnumerable<ExecutionPathGroup> executionPaths, ExecutionPathGroup emptyGroup)
				{
					_emptyGroup = emptyGroup;
					foreach (var executionPath in executionPaths)
					{
						var singleMatch = (SingleMatchEntry) executionPath.Match;
						var operand = ConvertOperand(singleMatch.Operand);

						_minValue = Math.Min(_minValue, operand);
						_maxValue = Math.Max(_maxValue, operand);
						_dictionary.SetValue(operand, executionPath);
					}
				}

				#endregion

				#region Interface Implementations

				#region Automata<TInstruction,TOperand>.ExecutionPathLookup.ILookup

				public ExecutionPathGroup GetExecutionPathGroup(int operand)
				{
					if (operand < _minValue || operand > _maxValue)
						return ExecutionPathGroup.Empty;

					return _dictionary.TryGetValue(operand, out var executionPathGroup) ? executionPathGroup : _emptyGroup;
				}

				#endregion

				#endregion
			}

			private sealed class RangeTreeLookup : ILookup
			{
				#region Fields

				private readonly HybridDictionaryCacheEx<ExecutionPathGroup> _dictionary = new HybridDictionaryCacheEx<ExecutionPathGroup>();
				private readonly ExecutionPathGroup _emptyGroup;
				private readonly int _maxValue;
				private readonly int _minValue;
				private readonly RangeTree<ExecutionPathGroup, int> _rangeTree;

				#endregion

				#region Ctors

				public RangeTreeLookup(System.Collections.Generic.IReadOnlyCollection<ExecutionPathGroup> executionPathGroups, ExecutionPathGroup emptyGroup)
				{
					_emptyGroup = emptyGroup;

					foreach (var executionPathGroup in executionPathGroups)
					{
						var range = CreateRange(executionPathGroup.Match);

						_minValue = Math.Min(_minValue, range.Minimum);
						_maxValue = Math.Max(_maxValue, range.Maximum);
					}

					_rangeTree = RangeTree.Build(executionPathGroups, e => CreateRange(e.Match));
				}

				#endregion

				#region Interface Implementations

				#region Automata<TInstruction,TOperand>.ExecutionPathLookup.ILookup

				public ExecutionPathGroup GetExecutionPathGroup(int operand)
				{
					if (operand < _minValue || operand > _maxValue)
						return ExecutionPathGroup.Empty;

					if (_dictionary.TryGetValue(operand, out var executionPathGroup))
						return executionPathGroup;

					lock (_dictionary)
					{
						if (_dictionary.TryGetValue(operand, out executionPathGroup))
							return executionPathGroup;

						executionPathGroup = _rangeTree.QueryFirstOrDefault(operand) ?? _emptyGroup;

						_dictionary.SetValue(operand, executionPathGroup);
					}

					return executionPathGroup;
				}

				#endregion

				#endregion
			}

			private sealed class SingleOperandLookup : ILookup
			{
				#region Fields

				private readonly ExecutionPathGroup _emptyGroup;
				private readonly ExecutionPathGroup _executionPath;
				private readonly int _operand;

				#endregion

				#region Ctors

				public SingleOperandLookup(TOperand operand, ExecutionPathGroup executionPath, ExecutionPathGroup emptyGroup)
				{
					_operand = ConvertOperand(operand);
					_executionPath = executionPath;
					_emptyGroup = emptyGroup;
				}

				#endregion

				#region Interface Implementations

				#region Automata<TInstruction,TOperand>.ExecutionPathLookup.ILookup

				public ExecutionPathGroup GetExecutionPathGroup(int operand)
				{
					return _operand == operand ? _executionPath : _emptyGroup;
				}

				#endregion

				#endregion
			}

			private sealed class AnyLookup : ILookup
			{
				#region Fields

				private readonly ExecutionPathGroup _executionPathGroup;

				#endregion

				#region Ctors

				public AnyLookup(ExecutionPathGroup executionPathGroup)
				{
					_executionPathGroup = executionPathGroup;
				}

				#endregion

				#region Interface Implementations

				#region Automata<TInstruction,TOperand>.ExecutionPathLookup.ILookup

				public ExecutionPathGroup GetExecutionPathGroup(int operand)
				{
					return _executionPathGroup;
				}

				#endregion

				#endregion
			}

			private sealed class PredicateLookup : ILookup
			{
				#region Fields

				private readonly ILookup _matchLookup;
				private readonly ExecutionPathGroup _predicateGroup;

				#endregion

				#region Ctors

				public PredicateLookup(ILookup matchLookup, ExecutionPathGroup predicateGroup)
				{
					_matchLookup = matchLookup;
					_predicateGroup = predicateGroup;
				}

				#endregion

				#region Methods

				public ExecutionPathGroup GetExecutionPathGroup()
				{
					return _predicateGroup;
				}

				#endregion

				#region Interface Implementations

				#region Automata<TInstruction,TOperand>.ExecutionPathLookup.ILookup

				public ExecutionPathGroup GetExecutionPathGroup(int operand)
				{
					return _matchLookup.GetExecutionPathGroup(operand);
				}

				#endregion

				#endregion
			}

			private sealed class SingleOperandRangeLookup : ILookup
			{
				#region Fields

				private readonly ExecutionPathGroup _emptyGroup;

				private readonly ExecutionPathGroup _executionPath;
				private readonly int _maxValue;
				private readonly int _minValue;

				#endregion

				#region Ctors

				public SingleOperandRangeLookup(TOperand from, TOperand to, ExecutionPathGroup executionPath, ExecutionPathGroup emptyGroup)
				{
					_executionPath = executionPath;
					_emptyGroup = emptyGroup;
					_minValue = ConvertOperand(from);
					_maxValue = ConvertOperand(to);
				}

				#endregion

				#region Interface Implementations

				#region Automata<TInstruction,TOperand>.ExecutionPathLookup.ILookup

				public ExecutionPathGroup GetExecutionPathGroup(int operand)
				{
					return operand >= _minValue && operand <= _maxValue ? _executionPath : _emptyGroup;
				}

				#endregion

				#endregion
			}

			#endregion
		}

		#endregion
	}
}