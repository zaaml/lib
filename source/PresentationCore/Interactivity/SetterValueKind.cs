using System;

namespace Zaaml.PresentationCore.Interactivity
{
	[Flags]
	internal enum SetterValueKind
	{
		Unspecified = 31,
		TemplateExpandoPath = 0,
		SelfExpandoPath = 1,
		ThemeResourcePath = 2,
		SkinPath = 4,
		TemplateSkinPath = 8,
		ValuePath = 15,
		Explicit = 16,
		Resolved = 32,
		Inherited = Resolved | Unspecified
	}
}