// <copyright file="EventDelegateFactory.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Zaaml.Core.Reflection
{
  internal static class EventDelegateFactory
  {
    #region  Methods

    public static Delegate CreateDelegate(this EventInfo eventInfo, Action onEvent)
    {
      return new DelegateWrapper(onEvent).CreateDelegateImpl(eventInfo);
    }

		public static Delegate CreateDelegate(this EventInfo eventInfo, Action<object> onEvent)
		{
			return new DelegateWrapper(onEvent).CreateDelegateImpl(eventInfo);
		}

		public static Delegate CreateDelegate(this EventInfo eventInfo, Action<object, object> onEvent)
		{
			return new DelegateWrapper(onEvent).CreateDelegateImpl(eventInfo);
		}

		public static Delegate CreateDelegate(this EventInfo eventInfo, Action<object, object, object> onEvent)
		{
			return new DelegateWrapper(onEvent).CreateDelegateImpl(eventInfo);
		}

		public static Delegate CreateDelegate(this EventInfo eventInfo, Action<object, object, object, object> onEvent)
		{
			return new DelegateWrapper(onEvent).CreateDelegateImpl(eventInfo);
		}

		public static Delegate CreateDelegate(this EventInfo eventInfo, Action<object, object, object, object, object> onEvent)
		{
			return new DelegateWrapper(onEvent).CreateDelegateImpl(eventInfo);
		}

		public static Delegate CreateDelegate(this EventInfo eventInfo, Action<object, object, object, object, object, object> onEvent)
		{
			return new DelegateWrapper(onEvent).CreateDelegateImpl(eventInfo);
		}

		public static Delegate CreateDelegate(this EventInfo eventInfo, Action<object, object, object, object, object, object, object> onEvent)
		{
			return new DelegateWrapper(onEvent).CreateDelegateImpl(eventInfo);
		}

		public static Delegate CreateDelegate(this EventInfo eventInfo, Action<object, object, object, object, object, object, object, object> onEvent)
		{
			return new DelegateWrapper(onEvent).CreateDelegateImpl(eventInfo);
		}

		public static Delegate CreateDelegate(this EventInfo eventInfo, Action<object, object, object, object, object, object, object, object, object> onEvent)
		{
			return new DelegateWrapper(onEvent).CreateDelegateImpl(eventInfo);
		}

		public static Delegate CreateDelegate(this EventInfo eventInfo, Action<object, object, object, object, object, object, object, object, object, object> onEvent)
		{
			return new DelegateWrapper(onEvent).CreateDelegateImpl(eventInfo);
		}

		public static Delegate CreateDelegate(this EventInfo eventInfo, Action<object, object, object, object, object, object, object, object, object, object, object> onEvent)
		{
			return new DelegateWrapper(onEvent).CreateDelegateImpl(eventInfo);
		}

		public static Delegate CreateDelegate(this EventInfo eventInfo, Action<object, object, object, object, object, object, object, object, object, object, object, object> onEvent)
		{
			return new DelegateWrapper(onEvent).CreateDelegateImpl(eventInfo);
		}

		public static Delegate CreateDelegate(this EventInfo eventInfo, Action<object, object, object, object, object, object, object, object, object, object, object, object, object> onEvent)
		{
			return new DelegateWrapper(onEvent).CreateDelegateImpl(eventInfo);
		}

		public static Delegate CreateDelegate(this EventInfo eventInfo, Action<object, object, object, object, object, object, object, object, object, object, object, object, object, object> onEvent)
		{
			return new DelegateWrapper(onEvent).CreateDelegateImpl(eventInfo);
		}

		public static Delegate CreateDelegate(this EventInfo eventInfo, Action<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object> onEvent)
		{
			return new DelegateWrapper(onEvent).CreateDelegateImpl(eventInfo);
		}

		public static Delegate CreateDelegate(this EventInfo eventInfo, Action<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object> onEvent)
		{
			return new DelegateWrapper(onEvent).CreateDelegateImpl(eventInfo);
		}

		#endregion

		#region  Nested Types

		private class DelegateWrapper
    {
      #region Static Fields and Constants

      private static readonly List<MethodInfo> MethodInfos;

      #endregion

      #region Fields

      private readonly object _onAction;

      #endregion

      #region Ctors

      static DelegateWrapper()
      {
        var type = typeof(DelegateWrapper);
        const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic;

        MethodInfos = new List<MethodInfo>();

        for (var i = 0; i < 17; i++)
          MethodInfos.Add(type.GetMethod("OnEvent", bindingFlags, null, Enumerable.Range(0, i).Select(j => typeof(object)).ToArray(), null));
      }

      public DelegateWrapper(Action onAction)
      {
        _onAction = onAction;
      }

			public DelegateWrapper(Action<object> onAction)
			{
				_onAction = onAction;
			}

			public DelegateWrapper(Action<object, object> onAction)
			{
				_onAction = onAction;
			}

			public DelegateWrapper(Action<object, object, object> onAction)
			{
				_onAction = onAction;
			}

			public DelegateWrapper(Action<object, object, object, object> onAction)
			{
				_onAction = onAction;
			}

			public DelegateWrapper(Action<object, object, object, object, object> onAction)
			{
				_onAction = onAction;
			}

			public DelegateWrapper(Action<object, object, object, object, object, object> onAction)
			{
				_onAction = onAction;
			}

			public DelegateWrapper(Action<object, object, object, object, object, object, object> onAction)
			{
				_onAction = onAction;
			}

			public DelegateWrapper(Action<object, object, object, object, object, object, object, object> onAction)
			{
				_onAction = onAction;
			}

			public DelegateWrapper(Action<object, object, object, object, object, object, object, object, object> onAction)
			{
				_onAction = onAction;
			}

			public DelegateWrapper(Action<object, object, object, object, object, object, object, object, object, object> onAction)
			{
				_onAction = onAction;
			}

			public DelegateWrapper(Action<object, object, object, object, object, object, object, object, object, object, object> onAction)
			{
				_onAction = onAction;
			}

			public DelegateWrapper(Action<object, object, object, object, object, object, object, object, object, object, object, object> onAction)
			{
				_onAction = onAction;
			}

			public DelegateWrapper(Action<object, object, object, object, object, object, object, object, object, object, object, object, object> onAction)
			{
				_onAction = onAction;
			}

			public DelegateWrapper(Action<object, object, object, object, object, object, object, object, object, object, object, object, object, object> onAction)
			{
				_onAction = onAction;
			}

			public DelegateWrapper(Action<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object> onAction)
			{
				_onAction = onAction;
			}

			public DelegateWrapper(Action<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object> onAction)
			{
				_onAction = onAction;
			}

			#endregion

			#region  Methods

			public Delegate CreateDelegateImpl(EventInfo eventInfo)
      {
        var invokeMethod = eventInfo.EventHandlerType.GetMethod("Invoke");
        var paramsCount = invokeMethod.GetParameters().Length;

        if (invokeMethod.ReturnType != typeof(void)) return null;

        return paramsCount < MethodInfos.Count ? Delegate.CreateDelegate(eventInfo.EventHandlerType, this, MethodInfos[paramsCount], false) : null;
      }

      [UsedImplicitly]
      private void OnEvent()
      {
	      var action = ((Action)_onAction);
	      action();
      }

      [UsedImplicitly]
      private void OnEvent(object a1)
      {
	      var action = (Action<object>)_onAction;
	      action(a1);
      }

      [UsedImplicitly]
      private void OnEvent(object a1, object a2)
      {
				var action = (Action<object, object>)_onAction;
				action(a1, a2);
      }

      [UsedImplicitly]
      private void OnEvent(object a1, object a2, object a3)
      {
				var action = (Action<object, object, object>)_onAction;
	      action(a1, a2, a3);
      }

      [UsedImplicitly]
      private void OnEvent(object a1, object a2, object a3, object a4)
      {
				var action = (Action<object, object, object, object>)_onAction;
				action(a1, a2, a3, a4);
			}

      [UsedImplicitly]
      private void OnEvent(object a1, object a2, object a3, object a4, object a5)
      {
				var action = (Action<object, object, object, object, object>)_onAction;
				action(a1, a2, a3, a4, a5);
			}

      [UsedImplicitly]
      private void OnEvent(object a1, object a2, object a3, object a4, object a5, object a6)
      {
				var action = (Action<object, object, object, object, object, object>)_onAction;
				action(a1, a2, a3, a4, a5, a6);
			}

      [UsedImplicitly]
      private void OnEvent(object a1, object a2, object a3, object a4, object a5, object a6, object a7)
      {
				var action = (Action<object, object, object, object, object, object, object>)_onAction;
				action(a1, a2, a3, a4, a5, a6, a7);
			}

      [UsedImplicitly]
      private void OnEvent(object a1, object a2, object a3, object a4, object a5, object a6, object a7, object a8)
      {
				var action = (Action<object, object, object, object, object, object, object, object>)_onAction;
				action(a1, a2, a3, a4, a5, a6, a7, a8);
			}

      [UsedImplicitly]
      private void OnEvent(object a1, object a2, object a3, object a4, object a5, object a6, object a7, object a8, object a9)
      {
				var action = (Action<object, object, object, object, object, object, object, object, object>)_onAction;
				action(a1, a2, a3, a4, a5, a6, a7, a8, a9);
			}

      [UsedImplicitly]
      private void OnEvent(object a1, object a2, object a3, object a4, object a5, object a6, object a7, object a8, object a9, object a10)
      {
				var action = (Action<object, object, object, object, object, object, object, object, object, object>)_onAction;
				action(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10);
			}

      [UsedImplicitly]
      private void OnEvent(object a1, object a2, object a3, object a4, object a5, object a6, object a7, object a8, object a9, object a10, object a11)
      {
				var action = (Action<object, object, object, object, object, object, object, object, object, object, object>)_onAction;
				action(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11);
			}

      [UsedImplicitly]
      private void OnEvent(object a1, object a2, object a3, object a4, object a5, object a6, object a7, object a8, object a9, object a10, object a11, object a12)
      {
				var action = (Action<object, object, object, object, object, object, object, object, object, object, object, object>)_onAction;
				action(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12);
			}

      [UsedImplicitly]
      private void OnEvent(object a1, object a2, object a3, object a4, object a5, object a6, object a7, object a8, object a9, object a10, object a11, object a12, object a13)
      {
				var action = (Action<object, object, object, object, object, object, object, object, object, object, object, object, object>)_onAction;
				action(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13);
			}

      [UsedImplicitly]
      private void OnEvent(object a1, object a2, object a3, object a4, object a5, object a6, object a7, object a8, object a9, object a10, object a11, object a12, object a13, object a14)
      {
				var action = (Action<object, object, object, object, object, object, object, object, object, object, object, object, object, object>)_onAction;
				action(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14);
			}

			[UsedImplicitly]
      private void OnEvent(object a1, object a2, object a3, object a4, object a5, object a6, object a7, object a8, object a9, object a10, object a11, object a12, object a13, object a14, object a15)
      {
				var action = (Action<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>)_onAction;
				action(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15);
			}

      [UsedImplicitly]
      private void OnEvent(object a1, object a2, object a3, object a4, object a5, object a6, object a7, object a8, object a9, object a10, object a11, object a12, object a13, object a14, object a15, object a16)
      {
				var action = (Action<object, object, object, object, object, object, object, object, object, object, object, object, object, object, object, object>)_onAction;
				action(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16);
			}

      #endregion
    }

    #endregion
  }
}