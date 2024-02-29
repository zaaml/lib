// <copyright file="ReadOnlyClassList.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.PresentationCore
{
	public sealed class ReadOnlyClassList
	{
		private readonly ClassList _classList;

		public ReadOnlyClassList(ClassList classList)
		{
			_classList = classList;
		}

		public IEnumerable<string> Classes => _classList.Classes;

		public string ClassListString => _classList.ClassListString;

		public bool HasClass(string @class)
		{
			return _classList.HasClass(@class);
		}
	}
}