// <copyright file="StringTextSource.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.IO;

namespace Zaaml.Text
{
	internal sealed class StringTextSource : TextSource
	{
		#region Fields

		public readonly int Offset;

		public readonly string String;

		#endregion

		#region Ctors

		public StringTextSource(string @string)
		{
			String = @string;
		}

		public StringTextSource(string @string, int offset)
		{
			String = @string;
			Offset = offset;
		}

		#endregion

		#region Properties

		public override int TextLength => String.Length - Offset;

		#endregion

		#region Methods

		public override TextReader CreateReader()
		{
			if (Offset == 0)
				return new StringReader(String);

			var stringReader = new StringReader(String);

			for (var i = 0; i < Offset; i++)
				stringReader.Read();

			return stringReader;
		}

		public override void Dispose()
		{
		}

		public override char GetChar(int textPointer)
		{
			textPointer += Offset;

			return textPointer < String.Length ? String[textPointer] : (char) 0;
		}

		public override ReadOnlySpan<char> GetSpan(int start, int end)
		{
			start += Offset;
			end += Offset;

			return String.AsSpan(start, end - start);
		}

		public override string GetText(int start, int end)
		{
			start += Offset;
			end += Offset;

#if NETCOREAPP3_1
			return new string(String.AsSpan(start, end - start));
#else
			return String.Substring(start, end - start);
#endif
		}

		public override TextSource Slice(int offset)
		{
			return new StringTextSource(String, Offset + offset);
		}

		#endregion
	}
}