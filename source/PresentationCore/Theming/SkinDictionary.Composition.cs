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
		#region Properties

		private IEnumerable<KeyValuePair<string, object>> ShallowResources => Dictionary;

		#endregion

		#region  Methods

		internal SkinDictionary AsFrozen()
		{
			var frozen = AsFrozenIntermediate();

			frozen.FreezeRelativeDeferred();
			frozen.FreezeBasedOnRecursive();
			frozen.FreezeProcessors();
			frozen.FreezeValues();

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

		private object CloneValue(object value)
		{
			var skinValue = value as SkinDictionary;

			if (skinValue != null)
				return skinValue.Clone();

			var processorValue = value as SkinDictionaryProcessor;

			if (processorValue != null)
				return processorValue.Clone();

			return value;
		}

		private void CopyResourcesFrom(SkinDictionary skinDictionary)
		{
			foreach (var keyValuePair in skinDictionary)
				AddCore(keyValuePair.Key, CloneValue(keyValuePair.Value));
		}

		private void FreezeBasedOn()
		{
			if (BasedOn.Count == 0)
				return;

			var basedOnCopy = BasedOn.ToList();

			BasedOn.Clear();

			if (BasedOnFlags == BasedOnFlags.Inherit)
			{
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
			else
			{
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
			}
		}

		private void FreezeBasedOnRecursive()
		{
			FreezeBasedOn();

			foreach (var keyValuePair in ShallowResources)
			{
				var skinValue = keyValuePair.Value as SkinDictionary;

				skinValue?.FreezeBasedOnRecursive();
			}
		}

		private void FreezeMerge(KeyValuePair<string, object> keyValuePair)
		{
			var key = keyValuePair.Key;
			var skinValue = keyValuePair.Value as SkinDictionary;

			if (skinValue == null)
				SetCore(keyValuePair);
			else
			{
				var skinValueFrozen = skinValue.AsFrozenIntermediate();

				object currentValue;

				if (TryGetValueCore(key, out currentValue) == false)
				{
					SetCore(key, skinValueFrozen);

					return;
				}

				var currentSkin = currentValue as SkinDictionary;

				if (currentSkin == null)
					SetCore(key, skinValueFrozen);
				else
				{
					currentSkin.BasedOnFlags = skinValueFrozen.BasedOnFlags;

					foreach (var basedOn in skinValueFrozen.BasedOn.Where(s => s.IsDeferred))
						if (basedOn.IsAbsoluteKey == false)
							currentSkin.BasedOn.Add(basedOn.Clone());

					foreach (var childKeyValuePair in skinValueFrozen)
						currentSkin.FreezeMerge(childKeyValuePair);
				}
			}
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

		internal KeyValuePair<string, object> FreezeResource(KeyValuePair<string, object> themeResourceKeyValuePair)
		{
			return themeResourceKeyValuePair;
		}

		private void FreezeValues()
		{
			foreach (var keyValuePair in ShallowResources.Select(UnwrapValue).ToList())
			{
				var skinValue = keyValuePair.Value as SkinDictionary;

				if (skinValue == null)
				{
					var frozenKeyValuePair = FreezeResource(keyValuePair);

					this[frozenKeyValuePair.Key] = frozenKeyValuePair.Value;
				}
				else
					skinValue.FreezeValues();
			}
		}
		
		private SkinDictionary ShallowClone()
		{
			var clone = new SkinDictionary();

			clone.ShallowCopyFrom(this);
			clone.CopyResourcesFrom(this);

			return clone;
		}

		private void ShallowCopyFrom(SkinDictionary skinDictionary)
		{
			Key = skinDictionary.Key;
			DeferredKey = skinDictionary.DeferredKey;
			BasedOnFlags = skinDictionary.BasedOnFlags;

#if INTERACTIVITY_DEBUG
			Debug = skinDictionary.Debug;
#endif
		}

		private static KeyValuePair<string, object> UnwrapValue(KeyValuePair<string, object> keyValuePair)
		{
			var themeResourceValue = keyValuePair.Value as ThemeResource;

			return themeResourceValue != null ? keyValuePair.WithValue(themeResourceValue.Value) : keyValuePair;
		}

		#endregion
	}
}