// <copyright file="AutomataTelemetry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Zaaml.Text
{
	internal sealed class AutomataTelemetry
	{
		private readonly Dictionary<int, int> _backtrackingLengthCountDictionary = new();
		private Stopwatch _execution;
		private Stopwatch _simulation;

		public int ForkCount { get; private set; }

		public int BacktrackingCount { get; private set; }

		public IReadOnlyDictionary<int, int> BacktrackingLengthCountDictionary => _backtrackingLengthCountDictionary;

		public TimeSpan SimulationTime { get; private set; }

		public TimeSpan ExecutionTime { get; private set; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void Backtracking(int length)
		{
			if (_backtrackingLengthCountDictionary.TryGetValue(length, out var count) == false)
			{
				_backtrackingLengthCountDictionary[length] = 1;
			}
			else
			{
				_backtrackingLengthCountDictionary[length] = count + 1;
			}

			BacktrackingCount++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void Fork()
		{
			ForkCount++;
		}

		public override string ToString()
		{
			var stringBuilder = new StringBuilder();

			stringBuilder.AppendLine($"Fork: {ForkCount}, Backtracking: {BacktrackingCount}");
			stringBuilder.AppendLine();

			stringBuilder.AppendLine("Backtracking Details:");

			foreach (var kv in _backtrackingLengthCountDictionary.OrderByDescending(kv => kv.Key))
			{
				stringBuilder.AppendLine($"\t{kv.Key}: {kv.Value} ");
			}


			return stringBuilder.ToString();
		}

		public void StartSimulation()
		{
			_simulation = Stopwatch.StartNew();
		}

		public void StopSimulation()
		{
			_simulation.Stop();

			SimulationTime += _simulation?.Elapsed ?? TimeSpan.Zero;
		}

		public void StartExecution()
		{
			_execution = Stopwatch.StartNew();
		}

		public void StopExecution()
		{
			_execution.Stop();

			ExecutionTime += _execution?.Elapsed ?? TimeSpan.Zero;
		}
	}
}