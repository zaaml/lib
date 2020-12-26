// <copyright file="DefaultItemTextFilter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Zaaml.PresentationCore.Data;

namespace Zaaml.UI.Controls.Core
{
	internal abstract class DefaultItemTextFilter<TItem>
	{
		protected abstract string ContentMember { get; }
		private Dictionary<Tuple<Type, string>, MemberEvaluator> ContentMemberEvaluators { get; } = new Dictionary<Tuple<Type, string>, MemberEvaluator>();

		private static bool FilterContent(object content, string filterText)
		{
			var stringContent = content as string;

			return stringContent == null || stringContent.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) != -1;
		}

		protected abstract object GetItemContent(TItem item);

		public bool Pass(object itemSource, string filterText)
		{
			if (string.IsNullOrEmpty(filterText))
				return true;

			if (itemSource == null)
				return true;

			if (itemSource is TItem item)
				return FilterContent(GetItemContent(item), filterText);

			var itemContentMember = ContentMember;

			if (itemContentMember != null)
			{
				var tuple = new Tuple<Type, string>(itemSource.GetType(), itemContentMember);

				if (ContentMemberEvaluators.TryGetValue(tuple, out var evaluator) == false)
					ContentMemberEvaluators[tuple] = evaluator = new MemberEvaluator(itemContentMember);

				return FilterContent(evaluator.GetValue(itemSource), filterText);
			}

			return FilterContent(itemSource, filterText);
		}
	}
}