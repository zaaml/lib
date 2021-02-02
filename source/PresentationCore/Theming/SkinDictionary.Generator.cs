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

		private bool HasGenerators => _generators != null;

		private void ApplyGenerators()
		{
			if (HasGenerators == false)
				return;

			foreach (var generator in Generators)
			{
				foreach (var keyValuePair in generator.GenerateInternal())
				{
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