// <copyright file="EnumSource.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Zaaml.PresentationCore.MarkupExtensions;

namespace Zaaml.PresentationCore
{
	public class EnumSourceCollection<T> : IEnumerable<EnumSource<T>> where T : Enum
	{
		private readonly ReadOnlyCollection<EnumSource<T>> _readOnlyCollection;

		public EnumSourceCollection()
		{
			var enumType = typeof(T);
			var enumSources = new List<EnumSource<T>>();

			foreach (var name in Enum.GetNames(enumType))
				enumSources.Add(new EnumSource<T>(name, (T) Enum.Parse(enumType, name)));

			_readOnlyCollection = new ReadOnlyCollection<EnumSource<T>>(enumSources);
		}

		public IEnumerator<EnumSource<T>> GetEnumerator()
		{
			return _readOnlyCollection.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable) _readOnlyCollection).GetEnumerator();
		}
	}

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
			return ((IEnumerable) _readOnlyCollection).GetEnumerator();
		}
	}

	public abstract class EnumSourceBase
	{
		protected EnumSourceBase(string display)
		{
			Display = display;
		}

		public string Display { get; }
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

	public sealed class EnumSourceCollectionExtension : MarkupExtensionBase
	{
		public Type EnumType { get; set; }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return new EnumSourceCollection(EnumType);
		}
	}
}