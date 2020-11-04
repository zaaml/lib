// <copyright file="SkinDictionary.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.ComponentModel;
using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.Theming
{
	[TypeConverter(typeof(SkinDictionaryTypeConverter))]
	public sealed partial class SkinDictionary : SkinBase
	{
		#region Static Fields and Constants

		// Thread unsafe builder
		private static readonly List<string> Builder = new List<string>();

		#endregion

		#region Fields

		private SkinDictionaryCollection _basedOn;

		#endregion

		#region Ctors

		public SkinDictionary()
		{
			Processors = new SkinDictionaryProcessorCollection(this);
		}

		#endregion

		#region Properties

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
			get => _basedOn ?? (_basedOn = new SkinDictionaryCollection { Owner = this });
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

		public BasedOnFlags BasedOnFlags { get; set; } = BasedOnFlags.Inherit;

		internal SkinDictionaryCollection BasedOnInternal => _basedOn;

#if INTERACTIVITY_DEBUG
		public bool Debug { get; set; }
#endif

		private Dictionary<string, object> Dictionary { get; } = new Dictionary<string, object>();

		private string Key { get; set; }

		internal SkinDictionary Parent { get; private set; }

		internal override IEnumerable<KeyValuePair<string, object>> Resources => ShallowResources;

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

		#endregion

		#region  Methods

		protected override object GetValue(string key)
		{
			return this.GetValueOrDefault(key);
		}

		public override string ToString()
		{
			return ActualKey ?? "$";
		}

		#endregion
	}

	public enum BasedOnFlags
	{
		Inherit,
		Override
	}
}