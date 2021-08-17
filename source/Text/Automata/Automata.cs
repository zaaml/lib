// <copyright file="Automata.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Linq;
using Zaaml.Core;
using Zaaml.Core.Pools;

namespace Zaaml.Text
{
	internal abstract class Automata
	{
		protected Automata(AutomataManager manager)
		{
			if (manager == null)
				throw new ArgumentNullException(nameof(manager));
		}

		private protected enum ProcessKind
		{
			Process,
			SubProcess
		}
	}

	internal abstract partial class Automata<TInstruction, TOperand> : Automata where TInstruction : unmanaged where TOperand : IConvertible
	{
		protected static Func<TOperand, int> FromConverter;
		protected static Func<int, TOperand> ToConverter;

		// ReSharper disable once StaticMemberInGenericType
		protected static readonly Interval<int> InstructionsRange;

		static Automata()
		{
			FromConverter = ConvertFromOperandPrivate;
			ToConverter = ConvertToOperandPrivate;

			var operandType = typeof(TOperand);

			if (operandType.IsEnum && Enum.GetUnderlyingType(operandType) == typeof(int))
				InstructionsRange = new Interval<int>(0, Enum.GetValues(operandType).Cast<int>().Max());
			else
				InstructionsRange = new Interval<int>(0, int.MaxValue);
		}

		protected Automata(AutomataManager manager) : base(manager)
		{
			RegisterNode(UnexpectedNode.Instance);
			RegisterExecutionPath(ExecutionPath.Invalid);
			RegisterExecutionPathGroup(ExecutionPathGroup.Empty);
		}

		protected virtual bool AllowParallelInstructionReader => false;

		protected int ExecutionPathLookAheadLength { get; private set; }

		protected virtual bool ForceInlineAll => false;

		protected bool HasPredicates { get; private set; }

		protected virtual bool LookAheadEnabled => false;

		private int StackHashCodeDepthThreshold => 1;

		private static int ConvertFromOperand(TOperand operand)
		{
			return FromConverter(operand);
		}

		private static int ConvertFromOperandPrivate(TOperand operand)
		{
			var code = operand.ToInt32(CultureInfo.InvariantCulture);

			if (code <= 0)
				throw new InvalidOperationException("Operand code must be greater than 1.");

			return code;
		}

		private static TOperand ConvertToOperand(int operandCode)
		{
			return ToConverter(operandCode);
		}

		private static TOperand ConvertToOperandPrivate(int operandCode)
		{
			var operand = (TOperand)Convert.ChangeType(operandCode, typeof(TOperand));

			return operand;
		}

		private protected abstract class AutomataResult : IDisposable
		{
			private ReferenceCounter _referenceCounter;

			protected Process Process { get; private set; }

			protected void AddReferenceInternal()
			{
				Process.AddReference();

				_referenceCounter.AddReference();
			}

			protected abstract void DisposeCore();

			protected void MountInternal(Process process)
			{
				Process = process;

				process.AddReference();

				_referenceCounter.AddReference();
			}

			public void Dispose()
			{
				Process.ReleaseReference();

				if (_referenceCounter.ReleaseReference() == 0)
				{
					DisposeCore();
				}
			}

			internal AutomataResult Verify()
			{
				if (this is not ExceptionAutomataResult exceptionResult) 
					return this;

				var exception = exceptionResult.Exception;

				Dispose();

				throw exception;
			}
		}

		private protected sealed class SuccessAutomataResult : AutomataResult
		{
			private readonly IPool<SuccessAutomataResult> _pool;

			public SuccessAutomataResult(IPool<SuccessAutomataResult> pool)
			{
				_pool = pool;
			}

			public int InstructionPosition => Process.InstructionStreamPosition;

			protected override void DisposeCore()
			{
				_pool.Release(this);
			}

			public SuccessAutomataResult Mount(Process process)
			{
				MountInternal(process);

				return this;
			}
		}

		private protected sealed class ExceptionAutomataResult : AutomataResult
		{
			private readonly IPool<ExceptionAutomataResult> _pool;

			public ExceptionAutomataResult(IPool<ExceptionAutomataResult> pool)
			{
				_pool = pool;
			}

			public Exception Exception { get; private set; }

			protected override void DisposeCore()
			{
				Exception = null;

				_pool.Release(this);
			}

			public ExceptionAutomataResult Mount(Exception exception, Process process)
			{
				MountInternal(process);

				Exception = exception;

				return this;
			}
		}

		private protected sealed class ForkAutomataResult : AutomataResult
		{
			private readonly IPool<ForkAutomataResult> _pool;

			public ForkAutomataResult(IPool<ForkAutomataResult> pool)
			{
				_pool = pool;
			}

			public int InstructionStreamPosition { get; private set; }

			public ForkAutomataResult AddReference()
			{
				AddReferenceInternal();

				return this;
			}

			protected override void DisposeCore()
			{
				_pool.Release(this);
			}

			public ForkAutomataResult Mount(Process process)
			{
				MountInternal(process);

				InstructionStreamPosition = process.InstructionStreamPosition;

				return this;
			}

			public AutomataResult RunFirst()
			{
				return Process.ForkFinish();
			}

			public AutomataResult RunSecond()
			{
				return Process.ForkRunNext();
			}
		}
	}
}