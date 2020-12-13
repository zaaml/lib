// <copyright file="Automata.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Zaaml.Core;
using Zaaml.Core.Pools;

namespace Zaaml.Text
{
	internal abstract class Automata
	{
		#region Ctors

		protected Automata(AutomataManager manager)
		{
			if (manager == null)
				throw new ArgumentNullException(nameof(manager));
		}

		#endregion
	}

	internal abstract partial class Automata<TInstruction, TOperand> : Automata where TInstruction : unmanaged where TOperand : IConvertible
	{
		#region Static Fields and Constants

		protected static Func<TOperand, int> Converter;

		// ReSharper disable once StaticMemberInGenericType
		protected static readonly Interval<int> InstructionsRange;

		#endregion

		#region Fields

		private readonly Dictionary<Type, ExecutionPathMethodCollection> _executionPathMethodsDictionary = new Dictionary<Type, ExecutionPathMethodCollection>();
		private readonly Stack<Process> _processPool = new Stack<Process>();

		#endregion

		#region Ctors

		static Automata()
		{
			Converter = operand => operand.ToInt32(CultureInfo.InvariantCulture);

			var operandType = typeof(TOperand);

			if (operandType.IsEnum && Enum.GetUnderlyingType(operandType) == typeof(int))
				InstructionsRange = new Interval<int>(0, Enum.GetValues(operandType).Cast<int>().Max());
			else
				InstructionsRange = new Interval<int>(0, int.MaxValue);
		}

		protected Automata(AutomataManager manager) : base(manager)
		{
			Instance = this;
		}

		#endregion

		#region Properties

		protected virtual bool AllowParallelInstructionReader => false;

		protected virtual bool ForceInlineAll => false;

		private static Automata<TInstruction, TOperand> Instance { get; set; }

		protected virtual bool LookAheadEnabled => false;

		#endregion

		#region Methods

		private ExecutionPathMethodCollection BuildExecutionPathMethods(IILBuilder context, Automata<TInstruction, TOperand> automata)
		{
			return new ExecutionPathMethodCollection(context, automata);
		}

		private static int ConvertOperand(TOperand operand)
		{
			return Converter(operand);
		}

		private Process CreateProcess<TContext>(IInstructionReader instructionReader, TContext context) where TContext : AutomataContext
		{
			Build();

			Process process;

			lock (_processPool)
				process = _processPool.Count > 0 ? _processPool.Pop() : new Process(this);

			process.Initialize(instructionReader, context);

			return process;
		}

		private ExecutionPathMethodCollection GetExecutionMethods(AutomataContext context)
		{
			var builderType = context.ILBuilderTypeInternal;

			// ReSharper disable once InconsistentlySynchronizedField
			if (_executionPathMethodsDictionary.TryGetValue(builderType, out var executionPathMethods) == false)
			{
				lock (_executionPathMethodsDictionary)
				{
					if (_executionPathMethodsDictionary.TryGetValue(builderType, out executionPathMethods) == false)
					{
						_executionPathMethodsDictionary[builderType] = executionPathMethods = BuildExecutionPathMethods(context, this);
					}

					return executionPathMethods;
				}
			}

			return executionPathMethods;
		}

		private protected AutomataResult PartialRunCore<TContext>(IInstructionReader instructionReader, TContext context) where TContext : AutomataContext
		{
			var process = CreateProcess(instructionReader, context);

			return process.PartialRun();
		}

		private void ReleaseProcess(Process process)
		{
			lock (_processPool)
				_processPool.Push(process);
		}

		private protected void RunCore<TContext>(IInstructionReader instructionReader, TContext context) where TContext : AutomataContext
		{
			var process = CreateProcess(instructionReader, context);
			var result = process.Run();

			if (result is ExceptionAutomataResult exceptionResult)
			{
				var exception = exceptionResult.Exception;

				result.Dispose();

				throw exception;
			}

			result.Dispose();
		}

		#endregion

		#region Nested Types

		private sealed class ExecutionPathMethodCollection
		{
			#region Fields

			private readonly IILBuilder _ilBuilder;
			private readonly Automata<TInstruction, TOperand> _automata;
			private ExecutionPathMethod[] _executionPaths;

			#endregion

			#region Ctors

			public ExecutionPathMethodCollection(IILBuilder ilBuilder, Automata<TInstruction, TOperand> automata)
			{
				_ilBuilder = ilBuilder;
				_automata = automata;

				var length = 128;

				while (length < automata._executionPathRegistry.Count) 
					length *= 2;

				_executionPaths = new ExecutionPathMethod[length];

				for (var i = 0; i < length; i++)
					_executionPaths[i] = new ExecutionPathMethod(_ilBuilder, automata, i);
			}

			#endregion

			#region Methods

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public ExecutionPathMethod GetExecutionPathMethod(ExecutionPath executionPath)
			{
				return _executionPaths[executionPath.Id];
			}

			public void ResizeExecutionPaths(ExecutionPath executionPath)
			{
				if (executionPath.Id < _executionPaths.Length)
					return;

				var nextLength = _executionPaths.Length * 2;

				while (executionPath.Id >= nextLength)
					nextLength *= 2;

				Array.Resize(ref _executionPaths, nextLength);

				for (var i = 0; i < _executionPaths.Length; i++) 
					_executionPaths[i] ??= new ExecutionPathMethod(_ilBuilder, _automata, i);
			}

			#endregion
		}

		private interface IILBuilder
		{
			#region Methods

			Func<Process, AutomataStack, object[], Node> BuildMain(ExecutionPath executionPath, out object[] closure);

			Func<Process, AutomataStack, object[], Node> BuildParallel(ExecutionPath executionPath, out object[] closure);

			#endregion
		}

		private protected abstract class AutomataResult : IDisposable
		{
			private ReferenceCounter _referenceCounter = new ReferenceCounter();

			#region Properties

			protected Process Process { get; private set; }

			#endregion

			#region Methods

			protected abstract void DisposeCore();

			protected void AddReferenceInternal()
			{
				Process.AddReference();
				_referenceCounter.AddReference();
			}

			protected void MountInternal(Process process)
			{
				Process = process;
				process.AddReference();
				_referenceCounter.AddReference();
			}

			#endregion

			#region Interface Implementations

			#region IDisposable

			public void Dispose()
			{
				Process.ReleaseReference();

				if (_referenceCounter.ReleaseReference() == 0)
				{
					DisposeCore();
				}
			}

			#endregion

			#endregion
		}

		private protected sealed class SuccessAutomataResult : AutomataResult
		{
			#region Fields

			private readonly IPool<SuccessAutomataResult> _pool;

			#endregion

			#region Ctors

			public SuccessAutomataResult(IPool<SuccessAutomataResult> pool)
			{
				_pool = pool;
			}

			#endregion

			#region Properties

			public int InstructionPosition => Process.InstructionStreamPosition;

			#endregion

			#region Methods

			protected override void DisposeCore()
			{
				_pool.Release(this);
			}

			public SuccessAutomataResult Mount(Process process)
			{
				MountInternal(process);

				return this;
			}

			#endregion
		}

		private protected sealed class ExceptionAutomataResult : AutomataResult
		{
			#region Fields

			private readonly IPool<ExceptionAutomataResult> _pool;

			#endregion

			#region Ctors

			public ExceptionAutomataResult(IPool<ExceptionAutomataResult> pool)
			{
				_pool = pool;
			}

			#endregion

			#region Properties

			public Exception Exception { get; private set; }

			#endregion

			#region Methods

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

			#endregion
		}

		private protected sealed class ForkAutomataResult : AutomataResult
		{
			#region Fields

			private readonly IPool<ForkAutomataResult> _pool;

			#endregion

			#region Ctors

			public ForkAutomataResult(IPool<ForkAutomataResult> pool)
			{
				_pool = pool;
			}

			#endregion

			#region Properties

			public int InstructionStreamPosition { get; private set; }

			#endregion

			#region Methods

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

			#endregion
		}

		#endregion
	}
}