// <copyright file="SkinDictionary.Composition.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Zaaml.PresentationCore.Theming
{
	public partial class SkinDictionary
	{
		private Dictionary<string, ushort> _priorityDictionary;

		public ushort Priority { get; set; }

		private Dictionary<string, ushort> PriorityDictionary => _priorityDictionary ??= new Dictionary<string, ushort>();

		internal IEnumerable<KeyValuePair<string, object>> ShallowResources => Dictionary;

		internal SkinDictionary AsFrozen(ISkinResourceProvider skinResourceProvider = null)
		{
			var frozen = AsFrozenIntermediate(new FreezeContext(skinResourceProvider, false));

			frozen.FreezeBasedOnRecursive(new FreezeContext(skinResourceProvider, true));
			frozen.FreezeGenerators();
			frozen.FreezeValues();

			return frozen;
		}

		private SkinDictionary AsFrozenIntermediate(FreezeContext freezeContext)
		{
			var clone = Clone();

			clone.FreezeBasedOnRecursive(freezeContext);

			return clone;
		}

		private SkinDictionary Clone()
		{
			var clone = ShallowClone();

			foreach (var basedOn in BasedOn)
				clone.BasedOn.Add(basedOn.Clone());

			return clone;
		}

		private static object CloneValue(object value)
		{
			return value switch
			{
				SkinDictionary skinValue => skinValue.Clone(),
				_ => value
			};
		}

		private void CopyGeneratorsFrom(SkinDictionary skinDictionary)
		{
			if (skinDictionary.HasGenerators == false)
				return;

			foreach (var generator in skinDictionary.Generators)
				Generators.Add(generator.Clone());
		}

		private void CopyResourcesFrom(SkinDictionary skinDictionary)
		{
			foreach (var keyValuePair in skinDictionary)
				AddCore(keyValuePair.Key, CloneValue(keyValuePair.Value));
		}

		private void FreezeBasedOn(FreezeContext freezeContext)
		{
			if (BasedOn.Count == 0)
				return;

			var basedOnCopy = BasedOn.ToList();
			var processRelative = freezeContext.ResolveRelativeDependencies;

			BasedOn.Clear();

			var shallowResources = ShallowResources.ToList();

			Clear();

			foreach (var basedOn in basedOnCopy)
			{
				var actualBasedOn = basedOn;

				if (basedOn.IsDeferred)
				{
					if (basedOn.IsAbsoluteKey == false)
					{
						if (processRelative == false || Parent == null || Parent.TryGetValueRelative(basedOn.DeferredKey, out var relativeBasedOn) == false)
						{
							BasedOn.Add(basedOn);

							continue;
						}

						actualBasedOn = (SkinDictionary)relativeBasedOn;
					}
					else
					{
						if (freezeContext.SkinResourceProvider == null || freezeContext.SkinResourceProvider.TryGetValue(basedOn.DeferredKey, out var absoluteBasedOn) == false)
						{
							BasedOn.Add(basedOn);

							continue;
						}

						actualBasedOn = (SkinDictionary)absoluteBasedOn;
					}
				}

				var frozenBasedOn = actualBasedOn.AsFrozenIntermediate(freezeContext);

				foreach (var keyValuePair in frozenBasedOn.ShallowResources)
					FreezeMerge(keyValuePair, frozenBasedOn, freezeContext);
			}

			foreach (var keyValuePair in shallowResources)
				FreezeMerge(keyValuePair, this, freezeContext);
		}

		private void FreezeBasedOnRecursive(FreezeContext freezeContext)
		{
			FreezeBasedOn(freezeContext);

			foreach (var keyValuePair in ShallowResources)
			{
				if (keyValuePair.Value is SkinDictionary skinValue)
					skinValue.FreezeBasedOnRecursive(freezeContext);
			}
		}

		private void FreezeMerge(KeyValuePair<string, object> keyValuePair, SkinDictionary sourceDictionary, FreezeContext freezeContext)
		{
			var key = keyValuePair.Key;

			if (keyValuePair.Value is SkinDictionary skinValue)
			{
				var skinValueFrozen = skinValue.AsFrozenIntermediate(freezeContext);

				if (TryGetValueCore(key, out var currentValue) == false)
				{
					SetMergeCore(key, skinValueFrozen, sourceDictionary);

					return;
				}

				if (currentValue is SkinDictionary currentSkin)
				{
					currentSkin.CopyGeneratorsFrom(skinValueFrozen);
#if INTERACTIVITY_DEBUG
					currentSkin.Debug |= skinValueFrozen.Debug;
#endif

					foreach (var basedOn in skinValueFrozen.BasedOn.Where(s => s.IsDeferred && s.IsAbsoluteKey == false))
						currentSkin.BasedOn.Add(basedOn.Clone());

					foreach (var childKeyValuePair in skinValueFrozen)
						currentSkin.FreezeMerge(childKeyValuePair, skinValueFrozen, freezeContext);
				}
				else
					SetMergeCore(key, skinValueFrozen, sourceDictionary);
			}
			else
				SetMergeCore(keyValuePair.Key, keyValuePair.Value, sourceDictionary);
		}

		internal static KeyValuePair<string, object> FreezeResource(KeyValuePair<string, object> themeResourceKeyValuePair)
		{
			return themeResourceKeyValuePair;
		}

		private void FreezeValues()
		{
			foreach (var keyValuePair in ShallowResources.Select(UnwrapValue).ToList())
			{
				if (keyValuePair.Value is SkinDictionary skinValue)
					skinValue.FreezeValues();
				else
				{
					var frozenKeyValuePair = FreezeResource(keyValuePair);

					this[frozenKeyValuePair.Key] = frozenKeyValuePair.Value;
				}
			}
		}

		private ushort GetKeyPriority(string key)
		{
			if (_priorityDictionary == null)
				return Priority;

			var keyPriority = _priorityDictionary.TryGetValue(key, out var priority) ? priority : (ushort)0;

			return Math.Max(Priority, keyPriority);
		}

		private void SetMergeCore(string key, object value, SkinDictionary sourceDictionary)
		{
			ushort currentPriority = 0;

			if (TryGetValueCore(key, out var currentValue))
			{
				currentPriority = GetKeyPriority(key);

				if (sourceDictionary.Priority < currentPriority)
					return;
			}

			SetCore(key, value);

			if (sourceDictionary.Priority > currentPriority)
				PriorityDictionary[key] = sourceDictionary.Priority;
		}

		private SkinDictionary ShallowClone()
		{
			var clone = new SkinDictionary();

			clone.ShallowCopyFrom(this);
			clone.CopyResourcesFrom(this);
			clone.CopyGeneratorsFrom(this);

			return clone;
		}

		private void ShallowCopyFrom(SkinDictionary skinDictionary)
		{
			Key = skinDictionary.Key;
			DeferredKey = skinDictionary.DeferredKey;
			Priority = skinDictionary.Priority;

#if INTERACTIVITY_DEBUG
			Debug = skinDictionary.Debug;
#endif
		}

		private static KeyValuePair<string, object> UnwrapValue(KeyValuePair<string, object> keyValuePair)
		{
			return keyValuePair.Value is ThemeResource themeResourceValue ? keyValuePair.WithValue(themeResourceValue.Value) : keyValuePair;
		}

		private readonly struct FreezeContext
		{
			public FreezeContext(ISkinResourceProvider skinResourceProvider, bool resolveRelativeDependencies)
			{
				SkinResourceProvider = skinResourceProvider;
				ResolveRelativeDependencies = resolveRelativeDependencies;
			}

			public readonly ISkinResourceProvider SkinResourceProvider;
			public readonly bool ResolveRelativeDependencies;
		}
	}
}