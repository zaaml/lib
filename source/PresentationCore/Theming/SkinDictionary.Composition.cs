// <copyright file="SkinDictionary.Composition.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.Theming
{
	public partial class SkinDictionary
	{
		private IEnumerable<KeyValuePair<string, object>> ShallowResources => Dictionary;

		internal SkinDictionary AsFrozen()
		{
			var frozen = AsFrozenIntermediate();

			frozen.FreezePreProcessors();

			frozen.FreezeRelativeDeferred();
			frozen.FreezeBasedOnRecursive();
			frozen.FreezeGenerators();
			frozen.FreezeValues();

			frozen.FreezePostProcessors();

			return frozen;
		}

		private SkinDictionary AsFrozenIntermediate()
		{
			var clone = Clone();

			clone.FreezeBasedOnRecursive();

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
				SkinDictionaryProcessor processorValue => processorValue.Clone(),
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
			if (skinDictionary._generators == null) 
				return;

			foreach (var generator in skinDictionary.Generators)
				Generators.Add(generator.Clone());
		}

		private void CopyProcessorsFrom(SkinDictionary skinDictionary)
		{
			if (skinDictionary._preProcessors != null)
			{
				foreach (var processor in skinDictionary.PreProcessors)
					PreProcessors.Add(processor.Clone());
			}

			if (skinDictionary._postProcessors != null)
			{
				foreach (var processor in skinDictionary.PostProcessors)
					PostProcessors.Add(processor.Clone());
			}
		}

		private void FreezeBasedOn()
		{
			if (BasedOn.Count == 0)
				return;

			var basedOnCopy = BasedOn.ToList();

			BasedOn.Clear();

			var shallowResources = ShallowResources.ToList();

			Clear();

			foreach (var basedOn in basedOnCopy)
			{
				if (basedOn.IsDeferred && basedOn.IsAbsoluteKey == false)
				{
					BasedOn.Add(basedOn);

					continue;
				}

				var frozenBasedOn = basedOn.AsFrozenIntermediate();

				foreach (var keyValuePair in frozenBasedOn.ShallowResources)
					FreezeMerge(keyValuePair);
			}

			foreach (var keyValuePair in shallowResources)
				FreezeMerge(keyValuePair);
		}

		private void FreezeBasedOnRecursive()
		{
			FreezeBasedOn();

			foreach (var keyValuePair in ShallowResources)
			{
				if (keyValuePair.Value is SkinDictionary skinValue)
					skinValue.FreezeBasedOnRecursive();
			}
		}

		private void SetMergeCore(string key, object value)
		{
			SetCore(key, value);
		}

		private void FreezeMerge(KeyValuePair<string, object> keyValuePair)
		{
			var key = keyValuePair.Key;

			if (keyValuePair.Value is SkinDictionary skinValue)
			{
				var skinValueFrozen = skinValue.AsFrozenIntermediate();

				if (TryGetValueCore(key, out var currentValue) == false)
				{
					SetMergeCore(key, skinValueFrozen);

					return;
				}

				if (currentValue is SkinDictionary currentSkin)
				{
					currentSkin.CopyProcessorsFrom(skinValueFrozen);
					currentSkin.CopyGeneratorsFrom(skinValueFrozen);

					foreach (var basedOn in skinValueFrozen.BasedOn.Where(s => s.IsDeferred))
						if (basedOn.IsAbsoluteKey == false)
							currentSkin.BasedOn.Add(basedOn.Clone());

					foreach (var childKeyValuePair in skinValueFrozen)
						currentSkin.FreezeMerge(childKeyValuePair);
				}
				else
					SetMergeCore(key, skinValueFrozen);
			}
			else
				SetMergeCore(keyValuePair.Key, keyValuePair.Value);
		}

		private void FreezeRelativeDeferred()
		{
			foreach (var keyValuePair in ShallowResources)
			{
				var skinValue = keyValuePair.Value as SkinDictionary;

				if (skinValue == null)
					continue;

				if (skinValue.BasedOn.Count > 0)
				{
					var basedOnCopy = skinValue.BasedOn.ToList();

					skinValue.BasedOn.Clear();

					foreach (var skinResources in basedOnCopy)
					{
						var actual = skinValue.GetValueOrDefault(skinResources.DeferredKey) as SkinDictionary;

						if (actual == null)
							continue;

						skinValue.BasedOn.Add(actual);
					}
				}

				skinValue.FreezeRelativeDeferred();
			}
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
			clone.CopyProcessorsFrom(this);

			return clone;
		}

		private void ShallowCopyFrom(SkinDictionary skinDictionary)
		{
			Key = skinDictionary.Key;
			DeferredKey = skinDictionary.DeferredKey;
			
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