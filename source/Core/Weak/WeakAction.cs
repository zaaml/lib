// <copyright file="WeakAction.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

#if SILVERLIGHT
using System.Linq.Expressions;
#endif

namespace Zaaml.Core.Weak
{
	internal class WeakAction : WeakActionBase
	{
		#region Ctors

		public WeakAction(Action action) : base(action.Target, action.Method)
		{
		}

		#endregion

		#region  Methods

		public static Action ConvertWeak(Action action)
		{
			var weakAction = new WeakAction(action);

			return () => weakAction.Invoke();
		}

		public static Action<T> ConvertWeak<T>(Action<T> action)
		{
			var weakAction = new WeakAction<T>(action);

			return a => weakAction.Invoke(a);
		}

		public static Action<T1, T2> ConvertWeak<T1, T2>(Action<T1, T2> action)
		{
			var weakAction = new WeakAction<T1, T2>(action);

			return (a1, a2) => weakAction.Invoke(a1, a2);
		}

		public static Action<T1, T2, T3> ConvertWeak<T1, T2, T3>(Action<T1, T2, T3> action)
		{
			var weakAction = new WeakAction<T1, T2, T3>(action);

			return (a1, a2, a3) => weakAction.Invoke(a1, a2, a3);
		}

		public static Action<T1, T2, T3, T4> ConvertWeak<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action)
		{
			var weakAction = new WeakAction<T1, T2, T3, T4>(action);

			return (a1, a2, a3, a4) => weakAction.Invoke(a1, a2, a3, a4);
		}

		public static WeakAction Create(Action action)
		{
			return new WeakAction(action);
		}

		public void Invoke()
		{
			InvokeImpl(null);
		}

		#endregion
	}

	internal class WeakAction<T> : WeakActionBase
	{
		#region Ctors

		public WeakAction(Action<T> action) : base(action.Target, action.Method)
		{
		}

		#endregion

		#region  Methods

		public static WeakAction<T> Create(Action<T> action)
		{
			return new WeakAction<T>(action);
		}

		public void Invoke(T arg0)
		{
			InvokeImpl(new object[] { arg0 });
		}

		#endregion
	}

	internal class WeakAction<T1, T2> : WeakActionBase
	{
		#region Ctors

		public WeakAction(Action<T1, T2> action) : base(action.Target, action.Method)
		{
		}

		#endregion

		#region  Methods

		public static WeakAction<T1, T2> Create(Action<T1, T2> action)
		{
			return new WeakAction<T1, T2>(action);
		}

		public void Invoke(T1 arg1, T2 arg2)
		{
			InvokeImpl(new object[] { arg1, arg2 });
		}

		#endregion
	}

	internal class WeakAction<T1, T2, T3> : WeakActionBase
	{
		#region Ctors

		public WeakAction(Action<T1, T2, T3> action) : base(action.Target, action.Method)
		{
		}

		#endregion

		#region  Methods

		public static WeakAction<T1, T2, T3> Create(Action<T1, T2, T3> action)
		{
			return new WeakAction<T1, T2, T3>(action);
		}

		public void Invoke(T1 arg1, T2 arg2, T3 arg3)
		{
			InvokeImpl(new object[] { arg1, arg2, arg3 });
		}

		#endregion
	}

	internal class WeakAction<T1, T2, T3, T4> : WeakActionBase
	{
		#region Ctors

		public WeakAction(Action<T1, T2, T3, T4> action) : base(action.Target, action.Method)
		{
		}

		#endregion

		#region  Methods

		public static WeakAction<T1, T2, T3, T4> Create(Action<T1, T2, T3, T4> action)
		{
			return new WeakAction<T1, T2, T3, T4>(action);
		}

		public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			InvokeImpl(new object[] { arg1, arg2, arg3, arg4 });
		}

		#endregion
	}
}