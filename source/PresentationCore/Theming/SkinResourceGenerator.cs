// <copyright file="SkinResourceGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Zaaml.PresentationCore.Theming
{
	public abstract partial class SkinResourceGenerator : DependencyObject
	{
		private SkinDictionary _skinDictionary;

		private Dictionary<string, SkinResourceTemplate> Dictionary { get; } = new();

		public SkinDictionary SkinDictionary
		{
			get => _skinDictionary;
			internal set
			{
				if (ReferenceEquals(_skinDictionary, value))
					return;

				if (_skinDictionary != null)
					DetachSkinPrivate(_skinDictionary);

				_skinDictionary = value;

				if (_skinDictionary != null)
					AttachSkinPrivate(_skinDictionary);
			}
		}

		private void AttachSkinPrivate(SkinDictionary skinDictionary)
		{
		}

		internal SkinResourceGenerator Clone()
		{
			var clone = CreateInstance();

			clone.CopyFrom(this);

			return clone;
		}

		protected virtual void CopyFrom(SkinResourceGenerator generatorSource)
		{
			foreach (var kv in generatorSource)
				Add(kv);
		}

		protected abstract SkinResourceGenerator CreateInstance();

		private void DetachSkinPrivate(SkinDictionary skinDictionary)
		{
		}

		protected abstract IEnumerable<KeyValuePair<string, object>> GenerateCoreBase();

		internal IEnumerable<KeyValuePair<string, object>> GenerateInternal()
		{
			return GenerateCoreBase();
		}
	}

	public abstract class SkinResourceGenerator<TResource> : SkinResourceGenerator
	{
		protected abstract IEnumerable<KeyValuePair<string, TResource>> GenerateCore();

		protected sealed override IEnumerable<KeyValuePair<string, object>> GenerateCoreBase()
		{
			return GenerateCore().Select(kv => new KeyValuePair<string, object>(kv.Key, kv.Value));
		}
	}
}