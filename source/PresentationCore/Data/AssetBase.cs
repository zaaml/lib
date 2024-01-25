// <copyright file="AssetBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.ComponentModel;

namespace Zaaml.PresentationCore.Data
{
	public class AssetBase : InheritanceContextObject, ISupportInitialize
	{
		protected bool Initialized => State == InitializationState.Initialized;

		protected bool Initializing => State == InitializationState.Initializing;

		private InitializationState State { get; set; } = InitializationState.Uninitialized;

		protected virtual void BeginInitCore()
		{
		}

		protected virtual void EndInitCore()
		{
		}

		void ISupportInitialize.BeginInit()
		{
			State = InitializationState.Initializing;

			BeginInitCore();
		}

		void ISupportInitialize.EndInit()
		{
			State = InitializationState.Initialized;

			EndInitCore();
		}

		private enum InitializationState
		{
			Uninitialized,
			Initializing,
			Initialized,
		}
#if INTERACTIVITY_DEBUG
		public bool Debug { get; set; }

		public string DebugName { get; set; }
#endif
	}
}