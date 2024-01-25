// <copyright file="SkinDictionary.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Zaaml.Core.Extensions;
using Zaaml.Core.Trees;

namespace Zaaml.PresentationCore.Theming
{
	[TypeConverter(typeof(SkinDictionaryTypeConverter))]
	public sealed partial class SkinDictionary : ISkinResourceProvider
	{
		// Thread unsafe builder
		private static readonly List<string> Builder = new();

		private SkinDictionaryCollection _basedOn;

		internal string ActualKey
		{
			get
			{
				if (Parent == null)
					return null;

				var builder = Builder;

				builder.Clear();
				builder.Add(Key);

				var current = Parent;

				while (current.Parent != null)
				{
					builder.Add(current.Key);
					current = current.Parent;
				}

				builder.Reverse();

				var actualKey = new ThemeResourceKey(builder).Key;

				return actualKey;
			}
		}

		[TypeConverter(typeof(SkinDictionaryCollectionTypeConverter))]
		public SkinDictionaryCollection BasedOn
		{
			get => _basedOn ??= new SkinDictionaryCollection { Owner = this };
			set
			{
				if (ReferenceEquals(_basedOn, value))
					return;

				if (_basedOn != null)
					_basedOn.Owner = null;

				_basedOn = value;

				if (_basedOn != null)
					_basedOn.Owner = this;
			}
		}

		internal SkinDictionaryCollection BasedOnInternal => _basedOn;

		private Dictionary<string, object> Dictionary { get; } = new();

		private string Key { get; set; }

		internal SkinDictionary Parent { get; private set; }

		internal SkinDictionary Root
		{
			get
			{
				var current = this;

				while (current.Parent != null)
					current = current.Parent;

				return current;
			}
		}

		internal bool ResolveDependencies(ISkinResourceProvider skinResourceValueProvider)
		{
			var result = true;

			TreeEnumerator.Visit(this, SkinDictionaryTreeAdvisor, s =>
			{
				if (s.BasedOnInternal == null || s.BasedOnInternal.Count == 0)
					return;

				for (var index = 0; index < s.BasedOn.Count; index++)
				{
					var basedOn = s.BasedOn[index];

					if (basedOn.IsDeferred == false || basedOn.IsAbsoluteKey == false)
						continue;

					if (skinResourceValueProvider.TryGetValue(basedOn.DeferredKey, out var resolved) == false)
					{
						result = false;

						continue;
					}

					if (resolved is SkinDictionary resolvedSkin)
						s.BasedOn[index] = resolvedSkin;
					else
						result = false;
				}
			});

			return result;
		}

		internal IEnumerable<string> EnumerateDependencies()
		{
			return TreeEnumerator
				.GetEnumerable(this, SkinDictionaryTreeAdvisor)
				.SelectMany(s => s.BasedOn)
				.Where(s => s.IsDeferred && s.IsAbsoluteKey)
				.Select(s => s.DeferredKey);
		}

		public override string ToString()
		{
			if (IsDeferred)
				return $"Deferred: {DeferredKey}";

			return ActualKey ?? "$";
		}

		private sealed class SkinImpl : SkinBase
		{
			private readonly SkinDictionary _skinDictionary;

			public SkinImpl(SkinDictionary skinDictionary)
			{
				_skinDictionary = skinDictionary;
			}

			internal override IEnumerable<KeyValuePair<string, object>> Resources => _skinDictionary.ShallowResources;

			protected override object GetValue(string key)
			{
				return _skinDictionary.GetValueOrDefault(key);
			}
		}

#if INTERACTIVITY_DEBUG
		public bool Debug { get; set; }

		internal void Break()
		{
			if (Debug == false)
				return;

			//System.Diagnostics.Debug.WriteLine("Debug");
		}
#endif
	}
}