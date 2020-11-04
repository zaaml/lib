// <copyright file="IParser.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core.Text
{
	public interface IParser<T>
	{
		#region  Methods

		T Parse(string value);

		bool TryParse(string value, out T result);

		#endregion
	}
}