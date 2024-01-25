// <copyright file="Automata.PredicateEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected abstract class PredicateEntryBase : PrimitiveEntry
		{
			protected PredicateEntryBase(string predicateName = null)
			{
				PredicateName = predicateName;
			}

			public string PredicateName { get; }

			internal abstract bool ConsumeResult { get; }

			protected override string DebuggerDisplay => "Predicate";

			internal virtual bool PopResult => ConsumeResult;

			internal virtual PredicateEntryBase GetActualPredicateEntry() => this;

			protected abstract PredicateResult Pass(AutomataContext context);

			internal PredicateResult PassInternal(AutomataContext context)
			{
				return Pass(context);
			}
		}

		private protected class PredicateEntry : PredicateEntryBase
		{
			public PredicateEntry(Func<AutomataContext, PredicateResult> predicate, string predicateName = null) 
				: base(predicateName)
			{
				Predicate = predicate;
			}

			public PredicateEntry(Func<AutomataContext, bool> predicate)
			{
				Predicate = c => predicate(c) ? PredicateResult.True : PredicateResult.False;
			}

			internal override bool ConsumeResult => false;

			protected Func<AutomataContext, PredicateResult> Predicate { get; }

			protected override PredicateResult Pass(AutomataContext context)
			{
				return Predicate(context);
			}
		}

		private protected class PredicateEntry<TResult> : PredicateEntryBase
		{
			public PredicateEntry(Func<AutomataContext, PredicateResult<TResult>> predicate)
			{
				Predicate = predicate;
			}

			internal override bool ConsumeResult => true;

			protected Func<AutomataContext, PredicateResult<TResult>> Predicate { get; }

			protected override PredicateResult Pass(AutomataContext context)
			{
				return Predicate(context);
			}
		}

		protected class PredicateResult
		{
			internal static readonly PredicateResult True = new();
			internal static readonly PredicateResult False = null;

			internal virtual Type ResultType => typeof(object);

			internal virtual void Dispose()
			{
			}

			internal virtual object GetResult() => null;

			internal virtual bool IsFork() => false;
		}

		protected abstract class PredicateResult<TResult> : PredicateResult
		{
			internal override Type ResultType => typeof(TResult);

			internal override object GetResult()
			{
				return GetResultCore();
			}

			protected abstract TResult GetResultCore();
		}

		protected sealed class LocalPredicateResult<TResult> : PredicateResult<TResult>
		{
			private readonly TResult _result;

			public LocalPredicateResult(TResult result)
			{
				_result = result;
			}

			protected override TResult GetResultCore()
			{
				return _result;
			}
		}

		private interface IForkPredicateResult
		{
			PredicateEntryBase First { get; }

			PredicateEntryBase Second { get; }
		}

		private protected abstract class ForkPredicateResult : PredicateResult, IForkPredicateResult
		{
			internal override bool IsFork()
			{
				return true;
			}

			public PredicateEntryBase First { get; protected set; }

			public PredicateEntryBase Second { get; protected set; }
		}

		private protected abstract class ForkPredicateResult<TResult> : PredicateResult<TResult>, IForkPredicateResult
		{
			protected override TResult GetResultCore()
			{
				throw new NotSupportedException();
			}

			internal override bool IsFork()
			{
				return true;
			}

			public PredicateEntryBase First { get; protected set; }

			public PredicateEntryBase Second { get; protected set; }
		}
	}
}