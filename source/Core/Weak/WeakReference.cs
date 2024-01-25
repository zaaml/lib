// <copyright file="WeakReference.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Core.Weak
{
	internal class WeakReference<T>
	{
		private readonly WeakReference _reference;

		public WeakReference(T target) : this(target, false)
		{
		}

		public WeakReference(T target, bool trackResurrection)
		{
			_reference = new WeakReference(target, trackResurrection);
		}

		public bool IsAlive => _reference.IsAlive;

		public T Target
		{
			get => (T)_reference.Target;
			set => _reference.Target = value;
		}
	}
}