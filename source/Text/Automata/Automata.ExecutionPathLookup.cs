// <copyright file="Automata.ExecutionPathLookup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Zaaml.Core;
using Zaaml.Core.Collections;
using Zaaml.Core.Collections.Pooled;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected class ExecutionPathLookup
		{
			private const int FastLookupLimit = 64;
			public static readonly ExecutionPathLookup Empty = new();
			private static readonly ExecutionPathGroup[] EmptyFastLookup;

			private readonly ExecutionPathGroup[] _fastLookup;
			private readonly ILookup _lookup;

			static ExecutionPathLookup()
			{
				if (InstructionsRange.Maximum >= FastLookupLimit)
					return;

				EmptyFastLookup = CreateFastLookupArray();
			}

			private ExecutionPathLookup()
			{
				_lookup = new AnyLookup(ExecutionPathGroup.Empty);
			}

			public ExecutionPathLookup(Automata<TInstruction, TOperand> automata, IEnumerable<ExecutionPath> executionPaths)
			{
				var executionPathGroups = GroupExecutionPaths(automata, executionPaths, out var epsilonGroup);

				if (executionPathGroups.Count == 0)
					_lookup = new AnyLookup(epsilonGroup);
				else if (executionPathGroups.Count == 1)
				{
					var executionPathGroup = executionPathGroups[0];
					var range = executionPathGroup.Range;

					if (range.IsEmpty)
					{
						throw new ArgumentOutOfRangeException();
					}

					_lookup = range.Minimum == range.Maximum
						? new SingleOperandLookup(range.Minimum, executionPathGroup, epsilonGroup)
						: new SingleOperandRangeLookup(range, executionPathGroup, epsilonGroup);
				}
				else
				{
					var hasRanges = executionPathGroups.Any(g => g.Range.Minimum != g.Range.Maximum);

					_lookup = hasRanges ? new RangeTreeLookup(executionPathGroups, epsilonGroup) : new DictionaryLookup(executionPathGroups, epsilonGroup);
				}

				if (epsilonGroup != null)
					_lookup = new EpsilonLookup(_lookup, epsilonGroup);

				if (InstructionsRange.Maximum >= FastLookupLimit)
					return;

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

			private static ExecutionPathGroup[] CreateFastLookupArray()
			{
				var fastLookup = new ExecutionPathGroup[InstructionsRange.Maximum + 1];

				for (var i = 0; i < fastLookup.Length; i++)
					fastLookup[i] = ExecutionPathGroup.Empty;

				return fastLookup;
			}

			private static Range<int> CreateRange(TOperand minimum, TOperand maximum)
			{
				return new Range<int>(ConvertFromOperand(minimum), ConvertFromOperand(maximum));
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ExecutionPathGroup GetExecutionPathGroup(int operand)
			{
				return _lookup.GetExecutionPathGroup(operand);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ExecutionPathGroup GetExecutionPathGroupFast(int operand)
			{
				return _fastLookup != null ? _fastLookup[operand] : _lookup.GetExecutionPathGroup(operand);
			}

			private static List<ExecutionPathGroup> Group(Automata<TInstruction, TOperand> automata, IEnumerable<GroupExecutionPathData> matchPaths, List<ExecutionPath> predicatePaths)
			{
				return matchPaths.SelectMany(p => p.Ranges.Select(r => new { Operand = r.Minimum, p.Path }))
					.GroupBy(t => t.Operand)
					.Select(g => new ExecutionPathGroup(automata, new Range<int>(g.Key, g.Key), g.Select(d => d.Path).Concat(predicatePaths))).ToList();
			}

			private List<ExecutionPathGroup> GroupExecutionPaths(Automata<TInstruction, TOperand> automata, IEnumerable<ExecutionPath> executionPaths, out ExecutionPathGroup epsilonGroup)
			{
				var paths = new List<GroupExecutionPathData>();
				var epsilonPaths = new List<ExecutionPath>();
				var hasRanges = false;

				foreach (var executionPath in executionPaths)
				{
					if (executionPath.IsEpsilon)
					{
						epsilonPaths.Add(executionPath);

						continue;
					}

					var match = executionPath.Match;

					if (match is not PrimitiveMatchEntry primitiveMatchEntry)
						throw new InvalidOperationException();

					var groupExecutionPathData = new GroupExecutionPathData(MergeRanges(FlattenRanges(primitiveMatchEntry)), executionPath);

					paths.Add(groupExecutionPathData);

					foreach (var range in groupExecutionPathData.Ranges)
						hasRanges |= range.Minimum != range.Maximum;
				}

				epsilonGroup = epsilonPaths.Count > 0 ? new ExecutionPathGroup(automata, epsilonPaths) : ExecutionPathGroup.Empty;

				return hasRanges ? GroupWithRanges(automata, paths, epsilonPaths) : Group(automata, paths, epsilonPaths);
			}

			private static IEnumerable<Range<int>> FlattenRanges(PrimitiveMatchEntry matchEntry)
			{
				switch (matchEntry)
				{
					case OperandMatchEntry operandMatchEntry:
						yield return new Range<int>(operandMatchEntry.IntOperand, operandMatchEntry.IntOperand);

						break;
					case RangeMatchEntry rangeMatchEntry:
						yield return rangeMatchEntry.Range;

						break;
					case SetMatchEntry setMatchEntry:
					{
						foreach (var range in setMatchEntry.Matches.SelectMany(FlattenRanges))
							yield return range;

						break;
					}
				}
			}

			private static Range<int>[] MergeRanges(IEnumerable<Range<int>> ranges)
			{
				var list = PooledList<Range<int>>.RentList();

				list.AddRange(ranges);
				list.Sort(RangeComparer.Instance);

				var result = MergeRangesImpl(list).ToArray();

				PooledList<Range<int>>.ReturnList(list);

				return result;
			}

			private static IEnumerable<Range<int>> MergeRangesImpl(IEnumerable<Range<int>> ranges)
			{
				var prev = Range<int>.Empty;

				foreach (var range in ranges)
				{
					if (prev.IsEmpty)
					{
						prev = range;

						continue;
					}

					var prevSpan = prev.AsIntSpan();
					var span = range.AsIntSpan();

					if (prevSpan.JoinsWith(span) || prevSpan.IntersectsWith(span))
					{
						prevSpan = prevSpan.Bounds(span);
						prev = prevSpan.AsRange();

						continue;
					}

					yield return prev;

					prev = range;
				}

				if (prev.IsEmpty == false)
					yield return prev;
			}

			private static List<ExecutionPathGroup> GroupWithRanges(Automata<TInstruction, TOperand> automata, IEnumerable<GroupExecutionPathData> matchPaths, List<ExecutionPath> predicatePaths)
			{
				var rangeItems = new List<RangeItem<ExecutionPath, int>>();

				foreach (var groupData in matchPaths)
				{
					foreach (var range in groupData.Ranges)
						rangeItems.Add(new RangeItem<ExecutionPath, int>(groupData.Path, range));
				}

				if (rangeItems.Count == 1)
				{
					var r = rangeItems[0];
					var executionPaths = new List<ExecutionPath> { r.Item };

					executionPaths.AddRange(predicatePaths);

					return new List<ExecutionPathGroup> { new(automata, r.Range, executionPaths) };
				}

				using var merger = new IntSpanItemMerger<RangeItem<List<ExecutionPath>, int>, List<ExecutionPath>>(rangeItems.Distinct().Select(r => new RangeItem<List<ExecutionPath>, int>(new List<ExecutionPath> { r.Item }, r.Range)),
					new RangeItem<List<ExecutionPath>, int>(default, Range<int>.Empty),
					r => r.Range.AsIntSpan(),
					r => r.Item,
					(span, path) => new RangeItem<List<ExecutionPath>, int>(path, span.AsRange()),
					pl => pl.SelectMany(l => l).Distinct().ToList(),
					true
				);

				return merger.AsEnumerable().Select(r => new ExecutionPathGroup(automata, r.Range, r.Item.Concat(predicatePaths))).ToList();
			}

			private sealed class RangeComparer : IComparer<Range<int>>
			{
				public static readonly RangeComparer Instance = new();

				private RangeComparer()
				{
				}

				public int Compare(Range<int> x, Range<int> y)
				{
					var minimumComparison = x.Minimum.CompareTo(y.Minimum);

					if (minimumComparison != 0)
						return minimumComparison;

					return x.Maximum.CompareTo(y.Maximum);
				}
			}

			private readonly struct GroupExecutionPathData
			{
				public GroupExecutionPathData(Range<int>[] ranges, ExecutionPath path)
				{
					Ranges = ranges;
					Path = path;
				}

				public readonly Range<int>[] Ranges;
				public readonly ExecutionPath Path;
			}

			private interface ILookup
			{
				ExecutionPathGroup GetExecutionPathGroup(int operand);
			}

			private sealed class DictionaryLookup : ILookup
			{
				//private readonly HybridDictionary<ExecutionPathGroup> _dictionary = new();
				private readonly Dictionary<int, ExecutionPathGroup> _dictionary = new();
				private readonly ExecutionPathGroup _epsilonGroup;
				private readonly int _maxValue;
				private readonly int _minValue;

				public DictionaryLookup(IEnumerable<ExecutionPathGroup> executionPathGroups, ExecutionPathGroup epsilonGroup)
				{
					_epsilonGroup = epsilonGroup;

					foreach (var executionPathGroup in executionPathGroups)
					{
						var range = executionPathGroup.Range;

						if (range.IsEmpty)
							throw new InvalidOperationException();

						if (range.Minimum != range.Maximum)
							throw new InvalidOperationException();

						var operand = range.Minimum;

						_minValue = Math.Min(_minValue, operand);
						_maxValue = Math.Max(_maxValue, operand);
						_dictionary.Add(operand, executionPathGroup);
					}
				}

				public ExecutionPathGroup GetExecutionPathGroup(int operand)
				{
					if (operand < _minValue || operand > _maxValue)
						return _epsilonGroup;

					return _dictionary.TryGetValue(operand, out var executionPathGroup) ? executionPathGroup : _epsilonGroup;
				}
			}

			private sealed class RangeTreeLookup : ILookup
			{
				private readonly HybridDictionary<ExecutionPathGroup> _dictionary = new();
				private readonly ExecutionPathGroup _epsilonGroup;
				private readonly int _maxValue;
				private readonly int _minValue;
				private readonly RangeTree<ExecutionPathGroup, int> _rangeTree;

				public RangeTreeLookup(IReadOnlyCollection<ExecutionPathGroup> executionPathGroups, ExecutionPathGroup epsilonGroup)
				{
					_epsilonGroup = epsilonGroup;

					foreach (var executionPathGroup in executionPathGroups)
					{
						var range = executionPathGroup.Range;

						_minValue = Math.Min(_minValue, range.Minimum);
						_maxValue = Math.Max(_maxValue, range.Maximum);
					}

					_rangeTree = RangeTree.Build(executionPathGroups, e => e.Range);
				}

				public ExecutionPathGroup GetExecutionPathGroup(int operand)
				{
					if (operand < _minValue || operand > _maxValue)
						return _epsilonGroup;

					lock (_dictionary)
					{
						if (_dictionary.TryGetValue(operand, out var executionPathGroup))
							return executionPathGroup;

						executionPathGroup = _rangeTree.SearchFirstOrDefault(operand) ?? _epsilonGroup;

						_dictionary.SetValue(operand, executionPathGroup);

						return executionPathGroup;
					}
				}
			}

			private sealed class SingleOperandLookup : ILookup
			{
				private readonly ExecutionPathGroup _epsilonGroup;
				private readonly ExecutionPathGroup _executionPath;
				private readonly int _operand;

				public SingleOperandLookup(int operand, ExecutionPathGroup executionPath, ExecutionPathGroup epsilonGroup)
				{
					_operand = operand;
					_executionPath = executionPath;
					_epsilonGroup = epsilonGroup;
				}

				public ExecutionPathGroup GetExecutionPathGroup(int operand)
				{
					return _operand == operand ? _executionPath : _epsilonGroup;
				}
			}

			private sealed class AnyLookup : ILookup
			{
				private readonly ExecutionPathGroup _executionPathGroup;

				public AnyLookup(ExecutionPathGroup executionPathGroup)
				{
					_executionPathGroup = executionPathGroup;
				}

				public ExecutionPathGroup GetExecutionPathGroup(int operand)
				{
					return _executionPathGroup;
				}
			}

			private sealed class EpsilonLookup : ILookup
			{
				private readonly ExecutionPathGroup _epsilonGroup;
				private readonly ILookup _matchLookup;

				public EpsilonLookup(ILookup matchLookup, ExecutionPathGroup epsilonGroup)
				{
					_matchLookup = matchLookup;
					_epsilonGroup = epsilonGroup;
				}

				public ExecutionPathGroup GetExecutionPathGroup(int operand)
				{
					return operand == 0 ? _epsilonGroup : _matchLookup.GetExecutionPathGroup(operand);
				}
			}

			private sealed class SingleOperandRangeLookup : ILookup
			{
				private readonly ExecutionPathGroup _epsilonGroup;

				private readonly ExecutionPathGroup _executionPath;
				private readonly int _maxValue;
				private readonly int _minValue;

				public SingleOperandRangeLookup(Range<int> range, ExecutionPathGroup executionPath, ExecutionPathGroup epsilonGroup)
				{
					_executionPath = executionPath;
					_epsilonGroup = epsilonGroup;
					_minValue = range.Minimum;
					_maxValue = range.Maximum;
				}

				public ExecutionPathGroup GetExecutionPathGroup(int operand)
				{
					return operand >= _minValue && operand <= _maxValue ? _executionPath : _epsilonGroup;
				}
			}
		}
	}
}