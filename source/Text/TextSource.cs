// <copyright file="TextSource.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.IO;

namespace Zaaml.Text
{
	public abstract class TextSource
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

		public abstract TextSpan GetTextSpan();

		public abstract TextSpan GetTextSpan(int start);

		public abstract TextSpan GetTextSpan(int start, int length);

		public abstract void WriteTo(TextWriter textWriter);

		public abstract int IndexOf(char c);

		public abstract int IndexOf(char c, int startIndex);
		
		public abstract int IndexOf(char c, int startIndex, int count);
	}
}