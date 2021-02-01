// <copyright file="SkinDictionary.Processor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Zaaml.PresentationCore.Theming
{
	public partial class SkinDictionary
	{
		private SkinDictionaryProcessorCollection _postProcessors;
		private SkinDictionaryProcessorCollection _preProcessors;

		public SkinDictionaryProcessorCollection PostProcessors => _postProcessors ??= new SkinDictionaryProcessorCollection(this);

		public SkinDictionaryProcessorCollection PreProcessors => _preProcessors ??= new SkinDictionaryProcessorCollection(this);

		private void ApplyPreProcessors()
		{
			if (_preProcessors == null)
				return;

			foreach (var processor in _preProcessors)
				processor.ProcessInternal();
		}

		private void ApplyPostProcessors()
		{
			if (_postProcessors == null)
				return;

			foreach (var processor in _postProcessors)
				processor.ProcessInternal();
		}

		private void FreezePreProcessors()
		{
			ApplyPreProcessors();

			foreach (var skinDictionary in Flatten().Select(kv => kv.Value).OfType<SkinDictionary>())
				skinDictionary.ApplyPreProcessors();
		}

		private void FreezePostProcessors()
		{
			ApplyPostProcessors();

			foreach (var skinDictionary in Flatten().Select(kv => kv.Value).OfType<SkinDictionary>())
				skinDictionary.ApplyPostProcessors();
		}
	}

	public abstract class SkinDictionaryProcessor : DependencyObject
	{
		private SkinDictionary _skinDictionary;

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

		internal SkinDictionaryProcessor Clone()
		{
			var clone = CreateInstance();

			clone.CopyFrom(this);

			return clone;
		}

		protected virtual void CopyFrom(SkinDictionaryProcessor processorSource)
		{
		}

		protected abstract SkinDictionaryProcessor CreateInstance();

		private void DetachSkinPrivate(SkinDictionary skinDictionary)
		{
		}

		protected abstract void ProcessCore();

		internal void ProcessInternal()
		{
			ProcessCore();
		}
	}

	public sealed class SkinDictionaryClear : SkinDictionaryProcessor
	{
		protected override SkinDictionaryProcessor CreateInstance()
		{
			return new SkinDictionaryClear();
		}

		protected override void ProcessCore()
		{
			SkinDictionary.Clear();
		}
	}

	public sealed class SkinDictionaryProcessorCollection : Collection<SkinDictionaryProcessor>
	{
		internal SkinDictionaryProcessorCollection(SkinDictionary skinDictionary)
		{
			SkinDictionary = skinDictionary;
		}

		public SkinDictionary SkinDictionary { get; }

		protected override void ClearItems()
		{
			foreach (var processor in this)
				processor.SkinDictionary = null;

			base.ClearItems();
		}

		protected override void InsertItem(int index, SkinDictionaryProcessor item)
		{
			base.InsertItem(index, item);

			item.SkinDictionary = SkinDictionary;
		}

		protected override void RemoveItem(int index)
		{
			var processor = this[index];

			processor.SkinDictionary = null;

			base.RemoveItem(index);
		}

		protected override void SetItem(int index, SkinDictionaryProcessor item)
		{
			var processor = this[index];

			processor.SkinDictionary = null;

			base.SetItem(index, item);

			processor.SkinDictionary = SkinDictionary;
		}
	}
}