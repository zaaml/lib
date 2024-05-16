// <copyright file="EnumSourceCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Zaaml.PresentationCore.Data
{
	public class EnumSourceCollection<T> : IEnumerable<EnumSource<T>> where T : Enum
	{
		private readonly ReadOnlyCollection<EnumSource<T>> _readOnlyCollection;

		public EnumSourceCollection()
		{
			var enumType = typeof(T);
			var enumSources = new List<EnumSource<T>>();

			foreach (var name in Enum.GetNames(enumType))
				enumSources.Add(new EnumSource<T>(name, (T)Enum.Parse(enumType, name)));

			_readOnlyCollection = new ReadOnlyCollection<EnumSource<T>>(enumSources);
		}

		public IEnumerator<EnumSource<T>> GetEnumerator()
		{
			return _readOnlyCollection.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)_readOnlyCollection).GetEnumerator();
		}
	}
}