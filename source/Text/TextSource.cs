// <copyright file="TextSource.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.IO;

namespace Zaaml.Text
{
	internal abstract class TextSource
	{
		public abstract int Length { get; }

		public abstract TextReader CreateReader();

		public abstract char GetChar(int offset);

		public abstract string GetText();

		public abstract string GetText(int start);

		public abstract string GetText(int start, int length);

		public abstract ReadOnlyMemory<char> GetTextMemory();

		public abstract ReadOnlyMemory<char> GetTextMemory(int start);

		public abstract ReadOnlyMemory<char> GetTextMemory(int start, int length);

		public abstract TextSourceSpan GetTextSpan();

		public abstract TextSourceSpan GetTextSpan(int start);

		public abstract TextSourceSpan GetTextSpan(int start, int length);

		public abstract void WriteTo(TextWriter textWriter);
	}
}