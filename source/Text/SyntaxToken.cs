// <copyright file="SyntaxToken.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	public readonly struct SyntaxToken<TSyntaxKind> where TSyntaxKind : unmanaged, Enum
	{
		private readonly Lexeme<TSyntaxKind> _lexeme;
		private readonly TextSpan _textSource;

		public TSyntaxKind Kind => _lexeme.Token;

		public TextSpan Span => _textSource.Slice(_lexeme.Start, _lexeme.Length);

		public SyntaxToken(Lexeme<TSyntaxKind> lexeme, TextSpan textSource)
		{
			_lexeme = lexeme;
			_textSource = textSource;
		}

		public SyntaxToken(TSyntaxKind token, TextSpan tokenText)
		{
			_lexeme = new Lexeme<TSyntaxKind>
			{
				Token = token,
				Start = 0,
				End = tokenText.End
			};

			_textSource = tokenText;
		}

		public string Text => Span.GetText();

		public static implicit operator string(SyntaxToken<TSyntaxKind> lexeme)
		{
			return lexeme.Text;
		}
	}

	public static class SyntaxToken
	{
		public static SyntaxToken<TSyntaxKind> Create<TSyntaxKind>(TSyntaxKind token, string tokenText) where TSyntaxKind : unmanaged, Enum
		{
			return new SyntaxToken<TSyntaxKind>(token, tokenText);
		}

		public static SyntaxToken<TSyntaxKind>? CreateNullable<TSyntaxKind>(TSyntaxKind token, string tokenText) where TSyntaxKind : unmanaged, Enum
		{
			return tokenText == null ? null : new SyntaxToken<TSyntaxKind>(token, tokenText);
		}
	}
}