// <copyright file="Automata.PredicateEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		protected abstract class PredicateEntryBase : PrimitiveEntry
		{
			#region Properties

			internal abstract bool ConsumeResult { get; }

			protected override string DebuggerDisplay => "Predicate";

			internal virtual bool PopResult => ConsumeResult;

			#endregion

			#region Methods

			internal virtual PredicateEntryBase GetActualPredicateEntry() => this;

			protected abstract PredicateResult Pass(AutomataContext context);

			internal PredicateResult PassInternal(AutomataContext context)
			{
				return Pass(context);
			}

			#endregion
		}

		protected class PredicateEntry : PredicateEntryBase
		{
			#region Ctors

			public PredicateEntry(Func<AutomataContext, PredicateResult> predicate)
			{
				Predicate = predicate;
			}

			public PredicateEntry(Func<AutomataContext, bool> predicate)
			{
				Predicate = c => predicate(c) ? PredicateResult.True : PredicateResult.False;
			}

			#endregion

			#region Properties

			internal override bool ConsumeResult => false;

			private Func<AutomataContext, PredicateResult> Predicate { get; }

			#endregion

			#region Methods

			protected override PredicateResult Pass(AutomataContext context)
			{
				return Predicate(context);
			}

			#endregion
		}

		protected class PredicateEntry<TResult> : PredicateEntryBase
		{
			#region Ctors

			public PredicateEntry(Func<AutomataContext, PredicateResult<TResult>> predicate)
			{
				Predicate = predicate;
			}

			#endregion

			#region Properties

			internal override bool ConsumeResult => true;

			private Func<AutomataContext, PredicateResult<TResult>> Predicate { get; }

			#endregion

			#region Methods

			protected override PredicateResult Pass(AutomataContext context)
			{
				return Predicate(context);
			}

			#endregion
		}

		protected class PredicateResult
		{
			#region Static Fields and Constants

			internal static readonly PredicateResult True = new PredicateResult();
			internal static readonly PredicateResult False = null;

			#endregion

			#region Properties

			internal virtual Type ResultType => typeof(object);

			#endregion

			#region Methods

			internal virtual void Dispose()
			{
			}

			internal virtual object GetResult() => null;

			internal virtual bool IsFork() => false;

			#endregion
		}

		protected abstract class PredicateResult<TResult> : PredicateResult
		{
			#region Properties

			internal override Type ResultType => typeof(TResult);

			#endregion

			#region Methods

			internal override object GetResult()
			{
				return GetResultCore();
			}

			protected abstract TResult GetResultCore();

			#endregion
		}

		protected sealed class LocalPredicateResult<TResult> : PredicateResult<TResult>
		{
			#region Fields

			private readonly TResult _result;

			#endregion

			#region Ctors

			public LocalPredicateResult(TResult result)
			{
				_result = result;
			}

			#endregion

			#region Methods

			protected override TResult GetResultCore()
			{
				return _result;
			}

			#endregion
		}

		private interface IForkPredicateResult
		{
			#region Properties

			PredicateEntryBase First { get; }

			PredicateEntryBase Second { get; }

			#endregion
		}

		protected abstract class ForkPredicateResult : PredicateResult, IForkPredicateResult
		{
			#region Methods

			internal override bool IsFork()
			{
				return true;
			}

			#endregion

			#region Interface Implementations

			#region Automata<TInstruction,TOperand>.IForkPredicateResult

			public PredicateEntryBase First { get; protected set; }

			public PredicateEntryBase Second { get; protected set; }

			#endregion

			#endregion
		}

		protected abstract class ForkPredicateResult<TResult> : PredicateResult<TResult>, IForkPredicateResult
		{
			#region Methods

			protected override TResult GetResultCore()
			{
				throw new NotSupportedException();
			}

			internal override bool IsFork()
			{
				return true;
			}

			#endregion

			#region Interface Implementations

			#region Automata<TInstruction,TOperand>.IForkPredicateResult

			public PredicateEntryBase First { get; protected set; }

			public PredicateEntryBase Second { get; protected set; }

			#endregion

			#endregion
		}

		#endregion
	}
}