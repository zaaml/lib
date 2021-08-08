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
		private protected class ExecutionPathLookup
		{
			private const int FastLookupLimit = 1024;
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
				var executionPathGroups = GroupExecutionPaths(automata, executionPaths, out var epsilonGroup).ToList();

				if (executionPathGroups.Count == 0)
					_lookup = new AnyLookup(epsilonGroup);
				else if (executionPathGroups.Count == 1)
				{
					_lookup = executionPathGroups[0].Match switch
					{
						RangeMatchEntry rangeMatch => new SingleOperandRangeLookup(rangeMatch.MinOperand, rangeMatch.MaxOperand, executionPathGroups[0], epsilonGroup),
						SingleMatchEntry singleMatch => new SingleOperandLookup(singleMatch.Operand, executionPathGroups[0], epsilonGroup),
						_ => throw new ArgumentOutOfRangeException()
					};
				}
				else
				{
					var hasRanges = executionPathGroups.Any(g => g.Match is RangeMatchEntry);

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

			private static Interval<int> CreateRange(TOperand minimum, TOperand maximum)
			{
				return new Interval<int>(ConvertFromOperand(minimum), ConvertFromOperand(maximum));
			}

			private static Interval<int> CreateRange(PrimitiveMatchEntry operandMatch)
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
			public ExecutionPathGroup GetExecutionPathGroupFast(int operand)
			{
				return _fastLookup != null ? _fastLookup[operand] : _lookup.GetExecutionPathGroup(operand);
			}

			private IEnumerable<ExecutionPathGroup> Group(Automata<TInstruction, TOperand> automata, IEnumerable<GroupExecutionPathData> matchPaths, List<ExecutionPath> predicatePaths)
			{
				return matchPaths.GroupBy(p => ((SingleMatchEntry)p.Match).Operand).Select(g => new ExecutionPathGroup(automata, g.Key, g.Select(d => d.Path).Concat(predicatePaths)));
			}

			private IEnumerable<ExecutionPathGroup> GroupExecutionPaths(Automata<TInstruction, TOperand> automata, IEnumerable<ExecutionPath> executionPaths, out ExecutionPathGroup epsilonGroup)
			{
				var paths = new List<GroupExecutionPathData>();
				var epsilonPaths = new List<ExecutionPath>();
				var hasRanges = false;

				foreach (var executionPath in executionPaths)
				{
					if (executionPath.IsPredicate || executionPath.OutputReturn || executionPath.OutputEnd)
					{
						epsilonPaths.Add(executionPath);

						continue;
					}

					var match = executionPath.Match;

					if (match is SetMatchEntry setMatch)
					{
						foreach (var primitiveOperandMatch in setMatch.Matches)
						{
							paths.Add(new GroupExecutionPathData(primitiveOperandMatch, executionPath));
							hasRanges |= primitiveOperandMatch is RangeMatchEntry;
						}
					}
					else
					{
						var primitiveOperandMatch = (PrimitiveMatchEntry)match;

						paths.Add(new GroupExecutionPathData(primitiveOperandMatch, executionPath));
						hasRanges |= primitiveOperandMatch is RangeMatchEntry;
					}
				}

				epsilonGroup = epsilonPaths.Count > 0 ? new ExecutionPathGroup(automata, epsilonPaths) : ExecutionPathGroup.Empty;

				return hasRanges ? GroupWithRanges(automata, paths, epsilonPaths) : Group(automata, paths, epsilonPaths);
			}

			private static IEnumerable<ExecutionPathGroup> GroupWithRanges(Automata<TInstruction, TOperand> automata, IEnumerable<GroupExecutionPathData> matchPaths, List<ExecutionPath> predicatePaths)
			{
				var rangeItems = new List<IntervalItem<ExecutionPath, int>>();

				foreach (var executionPath in matchPaths)
				{
					switch (executionPath.Match)
					{
						case SingleMatchEntry singleMatch:
							rangeItems.Add(new IntervalItem<ExecutionPath, int>(executionPath.Path, CreateRange(singleMatch.Operand, singleMatch.Operand)));

							break;
						case RangeMatchEntry rangeMatch:
							rangeItems.Add(new IntervalItem<ExecutionPath, int>(executionPath.Path, rangeMatch.IntRange));

							break;
					}
				}

				var split = Interval.Split(rangeItems.Distinct());
				var dictionary = new Dictionary<Interval<int>, List<ExecutionPath>>();

				foreach (var rangeItem in split)
					dictionary.GetValueOrCreate(rangeItem.Interval, () => new List<ExecutionPath>()).Add(rangeItem.Item);

				return dictionary.Select(kv => new ExecutionPathGroup(automata, new RangeMatchEntry(kv.Key), kv.Value.Concat(predicatePaths)));
			}

			private readonly struct GroupExecutionPathData
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
				ExecutionPathGroup GetExecutionPathGroup(int operand);
			}

			private sealed class DictionaryLookup : ILookup
			{
				private readonly HybridDictionaryCacheEx<ExecutionPathGroup> _dictionary = new();
				private readonly ExecutionPathGroup _emptyGroup;
				private readonly int _maxValue;
				private readonly int _minValue;

				public DictionaryLookup(IEnumerable<ExecutionPathGroup> executionPaths, ExecutionPathGroup emptyGroup)
				{
					_emptyGroup = emptyGroup;
					foreach (var executionPath in executionPaths)
					{
						var singleMatch = (SingleMatchEntry)executionPath.Match;
						var operand = ConvertFromOperand(singleMatch.Operand);

						_minValue = Math.Min(_minValue, operand);
						_maxValue = Math.Max(_maxValue, operand);
						_dictionary.SetValue(operand, executionPath);
					}
				}

				public ExecutionPathGroup GetExecutionPathGroup(int operand)
				{
					if (operand < _minValue || operand > _maxValue)
						return ExecutionPathGroup.Empty;

					return _dictionary.TryGetValue(operand, out var executionPathGroup) ? executionPathGroup : _emptyGroup;
				}
			}

			private sealed class RangeTreeLookup : ILookup
			{
				private readonly HybridDictionaryCacheEx<ExecutionPathGroup> _dictionary = new();
				private readonly ExecutionPathGroup _epsilonGroup;
				private readonly IntervalTree<ExecutionPathGroup, int> _intervalTree;
				private readonly int _maxValue;
				private readonly int _minValue;

				public RangeTreeLookup(IReadOnlyCollection<ExecutionPathGroup> executionPathGroups, ExecutionPathGroup epsilonGroup)
				{
					_epsilonGroup = epsilonGroup;

					foreach (var executionPathGroup in executionPathGroups)
					{
						var range = CreateRange(executionPathGroup.Match);

						_minValue = Math.Min(_minValue, range.Minimum);
						_maxValue = Math.Max(_maxValue, range.Maximum);
					}

					_intervalTree = IntervalTree.Build(executionPathGroups, e => CreateRange(e.Match));
				}

				public ExecutionPathGroup GetExecutionPathGroup(int operand)
				{
					if (operand < _minValue || operand > _maxValue)
						return ExecutionPathGroup.Empty;

					lock (_dictionary)
					{
						if (_dictionary.TryGetValue(operand, out var executionPathGroup))
							return executionPathGroup;

						executionPathGroup = _intervalTree.SearchFirstOrDefault(operand) ?? _epsilonGroup;

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

				public SingleOperandLookup(TOperand operand, ExecutionPathGroup executionPath, ExecutionPathGroup epsilonGroup)
				{
					_operand = ConvertFromOperand(operand);
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

				public SingleOperandRangeLookup(TOperand from, TOperand to, ExecutionPathGroup executionPath, ExecutionPathGroup epsilonGroup)
				{
					_executionPath = executionPath;
					_epsilonGroup = epsilonGroup;
					_minValue = ConvertFromOperand(from);
					_maxValue = ConvertFromOperand(to);
				}

				public ExecutionPathGroup GetExecutionPathGroup(int operand)
				{
					return operand >= _minValue && operand <= _maxValue ? _executionPath : _epsilonGroup;
				}
			}
		}
	}
}