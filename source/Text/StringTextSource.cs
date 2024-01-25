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
			return _string.Substring(start);
#endif
		}

		public override string GetText(int start, int length)
		{
#if NETCOREAPP
			return new string(_string.AsSpan(start, length));
#else
			return _string.Substring(start, length);
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

		public override TextSpan GetTextSpan()
		{
			return new TextSpan(this);
		}

		public override TextSpan GetTextSpan(int start)
		{
			return new TextSpan(this, start);
		}

		public override TextSpan GetTextSpan(int start, int length)
		{
			return new TextSpan(this, start, length);
		}

		public override void WriteTo(TextWriter textWriter)
		{
			textWriter.Write(_string);
		}

		public override int IndexOf(char c)
		{
			return _string.IndexOf(c);
		}

		public override int IndexOf(char c, int startIndex)
		{
			return _string.IndexOf(c, startIndex);
		}

		public override int IndexOf(char c, int startIndex, int count)
		{
			return _string.IndexOf(c, startIndex, count);
		}
	}
}