// <copyright file="UnicodeCategoryRanges.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Range = Zaaml.Core.Range<int>;

namespace Zaaml.Text.Unicode
{
	internal static partial class UnicodeCategoryRanges
	{
		private static readonly Dictionary<string, Range[]> ShortCategoryRangeDictionary;
		private static readonly Dictionary<string, Range[]> LongCategoryRangeDictionary;

		static UnicodeCategoryRanges()
		{
			ShortCategoryRangeDictionary = new Dictionary<string, Range[]>
			{
				// Letter
				{ "Lu", UppercaseLetter },
				{ "Ll", LowercaseLetter },
				{ "Lt", TitlecaseLetter },
				{ "Lm", ModifierLetter },
				{ "Lo", OtherLetter },
				{ "L", Letter },

				// Mark
				{ "Mn", NonSpacingMark },
				{ "Mc", SpacingCombiningMark },
				{ "Me", EnclosingMark },
				{ "M", Mark },

				// Number
				{ "Nd", DecimalDigitNumber },
				{ "Nl", LetterNumber },
				{ "No", OtherNumber },
				{ "N", Number },

				// Punctuation
				{ "Pc", ConnectorPunctuation },
				{ "Pd", DashPunctuation },
				{ "Ps", OpenPunctuation },
				{ "Pe", ClosePunctuation },
				{ "Pi", InitialQuotePunctuation },
				{ "Pf", FinalQuotePunctuation },
				{ "Po", OtherPunctuation },
				{ "P", Punctuation },

				// Separator
				{ "Zs", SpaceSeparator },
				{ "Zl", LineSeparator },
				{ "Zp", ParagraphSeparator },
				{ "Z", Separator },

				// Other
				{ "Cc", Control },
				{ "Cf", Format },
				{ "Cs", Surrogate },
				{ "Co", PrivateUse },
				{ "Cn", OtherNotAssigned },
				{ "C", Other }
			};

			LongCategoryRangeDictionary = new Dictionary<string, Range[]>
			{
				// Letter
				{ "UppercaseLetter", UppercaseLetter },
				{ "LowercaseLetter", LowercaseLetter },
				{ "TitlecaseLetter", TitlecaseLetter },
				{ "ModifierLetter", ModifierLetter },
				{ "OtherLetter", OtherLetter },
				{ "Letter", Letter },

				// Mark
				{ "NonSpacingMark", NonSpacingMark },
				{ "SpacingCombiningMark", SpacingCombiningMark },
				{ "EnclosingMark", EnclosingMark },
				{ "Mark", Mark },

				// Number
				{ "DecimalDigitNumber", DecimalDigitNumber },
				{ "LetterNumber", LetterNumber },
				{ "OtherNumber", OtherNumber },
				{ "Number", Number },

				// Punctuation
				{ "ConnectorPunctuation", ConnectorPunctuation },
				{ "DashPunctuation", DashPunctuation },
				{ "OpenPunctuation", OpenPunctuation },
				{ "ClosePunctuation", ClosePunctuation },
				{ "InitialQuotePunctuation", InitialQuotePunctuation },
				{ "FinalQuotePunctuation", FinalQuotePunctuation },
				{ "OtherPunctuation", OtherPunctuation },
				{ "Punctuation", Punctuation },

				// Separator
				{ "SpaceSeparator", SpaceSeparator },
				{ "LineSeparator", LineSeparator },
				{ "ParagraphSeparator", ParagraphSeparator },
				{ "Separator", Separator },

				// Other
				{ "Control", Control },
				{ "Format", Format },
				{ "Surrogate", Surrogate },
				{ "PrivateUse", PrivateUse },
				{ "OtherNotAssigned", OtherNotAssigned },
				{ "Other", Other }
			};
		}

		private static Range[] BuildRange(params int[] rangeArray)
		{
			var count = rangeArray.Length / 2;
			var ranges = new Range[count];

			for (var i = 0; i < count; i++)
				ranges[i] = new Range(rangeArray[i * 2], rangeArray[i * 2 + 1]);

			return ranges;
		}

		public static Range[] GetByLongName(string longName)
		{
			return LongCategoryRangeDictionary[longName];
		}

		public static Range[] GetByShortName(string shortName)
		{
			return ShortCategoryRangeDictionary[shortName];
		}
	}
}