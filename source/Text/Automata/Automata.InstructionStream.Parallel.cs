// <copyright file="Automata.InstructionStream.Parallel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		private protected partial class InstructionStream
		{
			#region Static Fields and Constants

			private const int PageSizeFactor = 1;
			private const int ParallelStartThreshold = 2 * PageSizeFactor;
			private const int QueueReadThreshold = 3 * PageSizeFactor;
			private const int QueueWriteThreshold = 9 * PageSizeFactor;

			#endregion

			#region Fields

			private bool _allowParallel;
			private ConcurrentQueue<InstructionPageStruct> _instructionPageQueue;
			private bool _readerBusy;
			private bool _readerFinished;

			#endregion

			#region Methods

			private void DisposeParallel()
			{
				_allowParallel = false;
				_instructionPageQueue = null;
				_readerBusy = false;
				_readerFinished = false;
			}

			private void MountParallel(Automata<TInstruction, TOperand> automata)
			{
				//_allowParallel = true;
				_allowParallel = automata.AllowParallelInstructionReader;
			}

			private void ParallelTask()
			{
				try
				{
					Monitor.Enter(InstructionReader);

					if (_readerFinished)
						return;

					for (var i = 0; i < QueueWriteThreshold && _readerFinished == false; i++)
					{
						var lexemesPage = ReadPageCore();

						if (Exception != null)
							break;

						_instructionPageQueue.Enqueue(lexemesPage);
					}
				}
				finally
				{
					Monitor.Exit(InstructionReader);
					_readerBusy = false;
				}
			}

			private bool ReadNextPageParallel(out TInstruction[] instructionsBuffer, out int[] operandsBuffer, out int instructionPageInstructionsCount)
			{
				instructionsBuffer = null;
				operandsBuffer = null;
				instructionPageInstructionsCount = -1;

				if (_instructionPageQueue == null)
					return false;

				if (_instructionPageQueue.TryDequeue(out var instructionPage) == false)
				{
					if (Monitor.TryEnter(InstructionReader))
					{
						try
						{
							instructionPage = ReadPageCore();
						}
						finally
						{
							Monitor.Exit(InstructionReader);
						}
					}
					else
					{
						var spinWait = new SpinWait();

						while (_instructionPageQueue.TryDequeue(out instructionPage) == false || _readerFinished || Exception != null)
							spinWait.SpinOnce();
					}
				}

				if (Exception != null)
					throw Exception;

				operandsBuffer = instructionPage.OperandsBuffer;
				instructionsBuffer = instructionPage.InstructionsBuffer;

				Finished = instructionPage.InstructionsCount < PageSize;

				RunParallel();

				instructionPageInstructionsCount = instructionPage.InstructionsCount;

				return true;
			}

			private void RunParallel()
			{
				if (_readerFinished || _readerBusy || _instructionPageQueue.Count > QueueReadThreshold)
					return;

				_readerBusy = true;

				Task.Run(ParallelTask);
			}

			private void TryRunParallel()
			{
				if (_allowParallel == false || PageCount != ParallelStartThreshold)
					return;

				_instructionPageQueue ??= new ConcurrentQueue<InstructionPageStruct>();

				RunParallel();
			}

			#endregion
		}

		#endregion
	}
}