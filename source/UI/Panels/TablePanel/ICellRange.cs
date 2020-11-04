namespace Zaaml.UI.Panels.TablePanel
{
	public interface ICellRange
	{
		int Column { get; set; }
		int ColumnSpan { get; set; }
		int Row { get; set; }
		int RowSpan { get; set; }
	}
}