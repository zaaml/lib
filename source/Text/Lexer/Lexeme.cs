// <copyright file="Lexeme.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	public struct Lexeme<TToken> where TToken : unmanaged, Enum
	{
		internal int EndField;
		internal int StartField;
		internal int DataField;
		internal TToken TokenField;

		public int End
		{
			get => EndField;
			internal set => EndField = value;
		}

		public int Start
		{
			get => StartField;
			internal set => StartField = value;
		}

		public int Length => EndField - StartField;

		public TToken Token
		{
			get => TokenField;
			internal set => TokenField = value;
		}

		public int Data
		{
			get => DataField;
			internal set => DataField = value;
		}

		public override string ToString()
		{
			return Token.ToString();
		}

		public string GetText(TextSource textSource)
		{
			return textSource.GetText(StartField, EndField - StartField);
		}
	}
}