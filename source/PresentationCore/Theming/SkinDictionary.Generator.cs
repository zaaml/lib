// <copyright file="SkinDictionary.Generator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;

namespace Zaaml.PresentationCore.Theming
{
	public partial class SkinDictionary
	{
		private SkinResourceGeneratorCollection _generators;

		public SkinResourceGeneratorCollection Generators => _generators ??= new SkinResourceGeneratorCollection(this);

		private void ApplyGenerators()
		{
			if (_generators == null)
				return;

			foreach (var generator in _generators)
			{
				foreach (var keyValuePair in generator.GenerateInternal())
				{
					if (ContainsKey(keyValuePair.Key) && generator.AllowOverwrite == false)
						continue;

					this[keyValuePair.Key] = keyValuePair.Value;
				}
			}
		}

		private void FreezeGenerators()
		{
			ApplyGenerators();

			foreach (var skinDictionary in Flatten().Select(kv => kv.Value).OfType<SkinDictionary>())
				skinDictionary.ApplyGenerators();
		}
	}
}