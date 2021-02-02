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
		private IEnumerable<KeyValuePair<string, object>> ShallowResources => Dictionary;

		internal SkinDictionary AsFrozen()
		{
			var frozen = AsFrozenIntermediate(false);

			frozen.FreezeRelativeBasedOn();
			frozen.FreezeGenerators();
			frozen.FreezeValues();

			return frozen;
		}

		private void FreezeRelativeBasedOn()
		{
			FreezeBasedOnRecursive(true);
		}

		private SkinDictionary AsFrozenIntermediate(bool relatives)
		{
			var clone = Clone();

			clone.FreezeBasedOnRecursive(relatives);

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

		private void CopyResourcesFrom(SkinDictionary skinDictionary)
		{
			foreach (var keyValuePair in skinDictionary)
				AddCore(keyValuePair.Key, CloneValue(keyValuePair.Value));
		}

		private void CopyGeneratorsFrom(SkinDictionary skinDictionary)
		{
			if (skinDictionary.HasGenerators == false) 
				return;

			foreach (var generator in skinDictionary.Generators)
				Generators.Add(generator.Clone());
		}

		private void FreezeBasedOn(bool processRelative)
		{
			if (BasedOn.Count == 0)
				return;

			var basedOnCopy = BasedOn.ToList();

			BasedOn.Clear();

			var shallowResources = ShallowResources.ToList();

			Clear();

			foreach (var basedOn in basedOnCopy)
			{
				var actualBasedOn = basedOn;

				if (basedOn.IsDeferred && basedOn.IsAbsoluteKey == false)
				{
					if (processRelative == false || Parent == null || Parent.TryGetValueRelative(basedOn.DeferredKey, out var relativeBasedOn) == false)
					{
						BasedOn.Add(basedOn);

						continue;
					}

					actualBasedOn = (SkinDictionary) relativeBasedOn;
				}

				var frozenBasedOn = actualBasedOn.AsFrozenIntermediate(processRelative);

				foreach (var keyValuePair in frozenBasedOn.ShallowResources)
					FreezeMerge(keyValuePair, frozenBasedOn, processRelative);
			}

			foreach (var keyValuePair in shallowResources)
				FreezeMerge(keyValuePair, this, processRelative);
		}

		private void FreezeBasedOnRecursive(bool processRelative)
		{
			FreezeBasedOn(processRelative);

			foreach (var keyValuePair in ShallowResources)
			{
				if (keyValuePair.Value is SkinDictionary skinValue)
					skinValue.FreezeBasedOnRecursive(processRelative);
			}
		}

		private Dictionary<string, ushort> _priorityDictionary;
		private Dictionary<string, ushort> PriorityDictionary => _priorityDictionary ??= new Dictionary<string, ushort>();

		private ushort GetKeyPriority(string key)
		{
			if (_priorityDictionary == null)
				return Priority;

			ushort keyPriority = _priorityDictionary.TryGetValue(key, out var priority) ? priority : 0;

			return Math.Max(Priority, keyPriority);
		}

		public ushort Priority { get; set; }
	
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

		private void FreezeMerge(KeyValuePair<string, object> keyValuePair, SkinDictionary sourceDictionary, bool processRelative)
		{
			var key = keyValuePair.Key;

			if (keyValuePair.Value is SkinDictionary skinValue)
			{
				var skinValueFrozen = skinValue.AsFrozenIntermediate(processRelative);

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
						currentSkin.FreezeMerge(childKeyValuePair, skinValueFrozen, processRelative);
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
	}
}