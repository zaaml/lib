namespace Zaaml.PresentationCore.Interactivity
{
	internal interface IValueSetter
	{
		SetterValueKind ValueKind { get; set; }

		object ValueStore { get; set; }
	}
}