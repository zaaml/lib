// <copyright file="TextSource.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.IO;

namespace Zaaml.Text
{
	internal abstract class TextSource : IDisposable
	{
		#region Properties

		public abstract int TextLength { get; }

		#endregion

		#region Methods

		public abstract TextReader CreateReader();

		public abstract char GetChar(int textPointer);

		public abstract ReadOnlySpan<char> GetSpan(int start, int end);

		public abstract string GetText(int start, int end);

		public abstract TextSource Slice(int offset);

		#endregion

		#region Interface Implementations

		#region IDisposable

		public abstract void Dispose();

		#endregion

		#endregion
	}
}