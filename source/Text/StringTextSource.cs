// <copyright file="StringTextSource.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.IO;

namespace Zaaml.Text
{
	internal sealed class StringTextSource : TextSource
	{
		private readonly string _string;

		public StringTextSource(string value)
		{
			_string = value;
		}

		public override int Length => _string.Length;

		public override TextReader CreateReader()
		{
			return new StringReader(_string);
		}

		public override char GetChar(int offset)
		{
			if (offset < 0 || offset >= _string.Length)
				throw new ArgumentOutOfRangeException();

			return _string[offset];
		}

		public override string GetText()
		{
			return _string;
		}

		public override string GetText(int start)
		{
#if NETCOREAPP
			return new string(_string.AsSpan(start));
#else
			return String.Substring(start);
#endif
		}

		public override string GetText(int start, int length)
		{
#if NETCOREAPP
			return new string(_string.AsSpan(start, length));
#else
			return String.Substring(start, length);
#endif
		}

		public override ReadOnlyMemory<char> GetTextMemory()
		{
			return _string.AsMemory();
		}

		public override ReadOnlyMemory<char> GetTextMemory(int start)
		{
			return _string.AsMemory(start);
		}

		public override ReadOnlyMemory<char> GetTextMemory(int start, int length)
		{
			return _string.AsMemory(start, length);
		}

		public override TextSourceSpan GetTextSpan()
		{
			return new TextSourceSpan(this);
		}

		public override TextSourceSpan GetTextSpan(int start)
		{
			return new TextSourceSpan(this, start);
		}

		public override TextSourceSpan GetTextSpan(int start, int length)
		{
			return new TextSourceSpan(this, start, length);
		}

		public override void WriteTo(TextWriter textWriter)
		{
			textWriter.Write(_string);
		}
	}
}