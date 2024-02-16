// <copyright file="DelegatePropertyChangeProvider.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Zaaml.Core;

namespace Zaaml.PresentationCore.PropertyCore
{
	internal abstract class DelegatePropertyChangeProviderBase : IDisposable
	{
		private static readonly ConditionalWeakTable<object, List<IDisposable>> StrongDisposables = new();

		protected DelegatePropertyChangeProviderBase(IPropertyChangeProvider provider)
		{
			Provider = provider;
		}

		protected IPropertyChangeProvider Provider { get; }

		protected static void AddStrongDisposable(object target, IDisposable disposable)
		{
			var disposables = StrongDisposables.GetOrCreateValue(target);

			disposables?.Add(disposable);
		}

		protected static void RemoveStrongDisposable(object target, IDisposable disposable)
		{
			if (StrongDisposables.TryGetValue(target, out var disposables))
				disposables?.Remove(disposable);
		}

		public void Dispose()
		{
			Provider.Dispose();

			var source = Provider.Source;

			if (source != null)
				RemoveStrongDisposable(source, this);
		}
	}

	internal class DelegatePropertyChangeProvider<TValue> : DelegatePropertyChangeProviderBase
	{
		public DelegatePropertyChangeProvider(IPropertyChangeProvider provider, Action<object, TValue, TValue> action) : base(provider)
		{
			Provider.PropertyChanged += (sender, args) => action(sender, (TValue)args.OldValue, (TValue)args.NewValue);
		}

		public DelegatePropertyChangeProvider(IPropertyChangeProvider provider, Action<TValue, TValue> action) : base(provider)
		{
			Provider.PropertyChanged += (_, args) => action((TValue)args.OldValue, (TValue)args.NewValue);
		}

		public DelegatePropertyChangeProvider(IPropertyChangeProvider provider, Action<IDisposable, TValue, TValue> action, bool strongDisposable) : base(provider)
		{
			Provider.PropertyChanged += (_, args) => action(this, (TValue)args.OldValue, (TValue)args.NewValue);

			if (strongDisposable)
				AddStrongDisposable(Provider.Source, this);
		}
	}

	internal class DelegatePropertyChangeProvider : DelegatePropertyChangeProviderBase
	{
		public DelegatePropertyChangeProvider(IPropertyChangeProvider provider, Action<object, object, object> action) : base(provider)
		{
			Provider.PropertyChanged += (sender, args) => action(sender, args.OldValue, args.NewValue);
		}

		public DelegatePropertyChangeProvider(IPropertyChangeProvider provider, Action<object, object> action) : base(provider)
		{
			Provider.PropertyChanged += (_, args) => action(args.OldValue, args.NewValue);
		}

		public DelegatePropertyChangeProvider(IPropertyChangeProvider provider, Action<IDisposable, object, object> action, bool strongDisposable) : base(provider)
		{
			Provider.PropertyChanged += (_, args) => action(this, args.OldValue, args.NewValue);

			if (strongDisposable)
				AddStrongDisposable(Provider.Source, this);
		}

		public DelegatePropertyChangeProvider(IPropertyChangeProvider provider, Action<PropertyValueChangedEventArgs> action) : base(provider)
		{
			Provider.PropertyChanged += (_, args) => action(args);
		}

		public DelegatePropertyChangeProvider(IPropertyChangeProvider provider, Action<IDisposable, PropertyValueChangedEventArgs> action, bool strongDisposable) : base(provider)
		{
			Provider.PropertyChanged += (_, args) => action(this, args);

			if (strongDisposable)
				AddStrongDisposable(Provider.Source, this);
		}
	}
}