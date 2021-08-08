// <copyright file="Lexeme.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal struct Lexeme<TToken> where TToken : unmanaged, Enum
	{
		internal int EndField;
		internal int StartField;
		internal TToken TokenField;

		#region Fields

		#endregion

		#region Properties

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

		public TToken Token
		{
			get => TokenField;
			internal set => TokenField = value;
		}

		#endregion

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