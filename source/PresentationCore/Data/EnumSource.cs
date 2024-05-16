// <copyright file="EnumSource.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Zaaml.PresentationCore.Data
{
	public sealed class EnumSourceCollection : IEnumerable<EnumSource>
	{
		private readonly ReadOnlyCollection<EnumSource> _readOnlyCollection;

		public EnumSourceCollection(Type enumType)
		{
			var enumSources = new List<EnumSource>();

			foreach (var name in Enum.GetNames(enumType))
				enumSources.Add(new EnumSource(name, Enum.Parse(enumType, name)));

			_readOnlyCollection = new ReadOnlyCollection<EnumSource>(enumSources);
		}

		public IEnumerator<EnumSource> GetEnumerator()
		{
			return _readOnlyCollection.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)_readOnlyCollection).GetEnumerator();
		}
	}

	public sealed class EnumSource<T> : EnumSourceBase where T : Enum
	{
		public EnumSource(string display, T value) : base(display)
		{
			Value = value;
		}

		public T Value { get; }
	}

	public sealed class EnumSource : EnumSourceBase
	{
		public EnumSource(string display, object value) : base(display)
		{
			Value = value;
		}

		public object Value { get; }
	}
}